using System.Collections.Generic;
using UnityEngine;
using Core;

/// <summary>
/// Apply this to all GameObject with a Rigidbody and a MeshCollider you want to float in water.
/// </summary>
public class Buoyancy : MonoBehaviour
{
    public float density = 500;
    public int slicesPerAxis = 3;
    public bool isConcave = false;
    public int voxelsLimit = 16;

    const float DAMPFER = 0.2f;
    const float WATER_DENSITY = 1000;

    float _voxelHalfHeight;
    Vector3 _localArchimedesForce;
    List<Vector3> _voxels;
    bool _isMeshCollider;
    List<Vector3[]> _forces = new List<Vector3[]>(); // For drawing force gizmos
    
    Rigidbody _rigidBody;
    Collider _collider;
    MeshCollider _meshCollider;

    IWaterSystem _waterSystem;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _meshCollider = GetComponent<MeshCollider>();
        _waterSystem = SystemProvider.GetSystem<IWaterSystem>(gameObject);

        var originalRotation = transform.rotation;
        var originalPosition = transform.position;
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;

        // The object must have a collider
        if (_collider == null) {
            //            Debug.LogWarning(string.Format("[Buoyancy.cs] Object \"{0}\" had no collider.", name));
            enabled = false;
        }
        _isMeshCollider = _meshCollider != null;

        var bounds = _collider.bounds;
        if (bounds.size.x < bounds.size.y) {
            _voxelHalfHeight = bounds.size.x;
        }
        else {
            _voxelHalfHeight = bounds.size.y;
        }
        if (bounds.size.z < _voxelHalfHeight) {
            _voxelHalfHeight = bounds.size.z;
        }
        _voxelHalfHeight /= 2 * slicesPerAxis;

        // The object must have a RidigBody
        if (_rigidBody == null)
            enabled = false;

        _rigidBody.centerOfMass = new Vector3(0, -bounds.extents.y * 1.4f, 0) + transform.InverseTransformPoint(bounds.center);

        _voxels = SliceIntoVoxels(_isMeshCollider && isConcave);

        // Restore original rotation and position
        transform.rotation = originalRotation;
        transform.position = originalPosition;

        float volume = _rigidBody.mass / density;

        WeldPoints(_voxels, voxelsLimit);

        float archimedesForceMagnitude = WATER_DENSITY * Mathf.Abs(Physics.gravity.y) * volume;
        _localArchimedesForce = new Vector3(0, archimedesForceMagnitude, 0) / _voxels.Count;
    }

    List<Vector3> SliceIntoVoxels(bool concave)
    {
        var points = new List<Vector3>(slicesPerAxis * slicesPerAxis * slicesPerAxis);

        if (concave) {
            var meshC = _meshCollider;

            var convexValue = meshC.convex;
            meshC.convex = false;

            // Concave slicing
            var bounds = _collider.bounds;
            for (int ix = 0; ix < slicesPerAxis; ix++) {
                for (int iy = 0; iy < slicesPerAxis; iy++) {
                    for (int iz = 0; iz < slicesPerAxis; iz++) {
                        float x = bounds.min.x + bounds.size.x / slicesPerAxis * (0.5f + ix);
                        float y = bounds.min.y + bounds.size.y / slicesPerAxis * (0.5f + iy);
                        float z = bounds.min.z + bounds.size.z / slicesPerAxis * (0.5f + iz);

                        var p = transform.InverseTransformPoint(new Vector3(x, y, z));

                        if (PointIsInsideMeshCollider(meshC, p)) {
                            points.Add(p);
                        }
                    }
                }
            }
            if (points.Count == 0) {
                points.Add(bounds.center);
            }

            meshC.convex = convexValue;
        }
        else {
            // Convex slicing
            var bounds = _collider.bounds;
            for (int ix = 0; ix < slicesPerAxis; ix++) {
                for (int iy = 0; iy < slicesPerAxis; iy++) {
                    for (int iz = 0; iz < slicesPerAxis; iz++) {
                        float x = bounds.min.x + bounds.size.x / slicesPerAxis * (0.5f + ix);
                        float y = bounds.min.y + bounds.size.y / slicesPerAxis * (0.5f + iy);
                        float z = bounds.min.z + bounds.size.z / slicesPerAxis * (0.5f + iz);

                        var p = transform.InverseTransformPoint(new Vector3(x, y, z));

                        points.Add(p);
                    }
                }
            }
        }

        return points;
    }

    static bool PointIsInsideMeshCollider(Collider c, Vector3 p)
    {
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        foreach (var ray in directions) {
            RaycastHit hit;
            if (c.Raycast(new Ray(p - ray * 1000, ray), out hit, 1000f) == false) {
                return false;
            }
        }
        return true;
    }

    static void FindClosestPoints(IList<Vector3> list, out int firstIndex, out int secondIndex)
    {
        float minDistance = float.MaxValue, maxDistance = float.MinValue;
        firstIndex = 0;
        secondIndex = 1;

        for (int i = 0; i < list.Count - 1; i++) {
            for (int j = i + 1; j < list.Count; j++) {
                float distance = Vector3.Distance(list[i], list[j]);
                if (distance < minDistance) {
                    minDistance = distance;
                    firstIndex = i;
                    secondIndex = j;
                }
                if (distance > maxDistance) {
                    maxDistance = distance;
                }
            }
        }
    }

    static void WeldPoints(IList<Vector3> list, int targetCount)
    {
        if (list.Count <= 2 || targetCount < 2)
            return;

        while (list.Count > targetCount) {
            int first, second;
            FindClosestPoints(list, out first, out second);

            var mixed = (list[first] + list[second]) * 0.5f;
            list.RemoveAt(second); // the second index is always greater that the first => removing the second item first
            list.RemoveAt(first);
            list.Add(mixed);
        }
    }

    void FixedUpdate()
    {
        _forces.Clear();// For drawing force gizmos
        
        foreach (var point in _voxels) {
            var wp = transform.TransformPoint(point);
            float waterLevel = _waterSystem.GetHeight(wp.x, wp.z);

            if (wp.y - _voxelHalfHeight < waterLevel) {
                float k = (waterLevel - wp.y) / (2 * _voxelHalfHeight) + 0.5f;
                if (k > 1) {
                    k = 1f;
                }
                else if (k < 0) {
                    k = 0f;
                }

                var velocity = _rigidBody.GetPointVelocity(wp);
                var localDampingForce = -velocity * DAMPFER * _rigidBody.mass;
                var force = localDampingForce + Mathf.Sqrt(k) * _localArchimedesForce;
                _rigidBody.AddForceAtPosition(force, wp);

                _forces.Add(new[] { wp, force }); // For drawing force gizmos
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (_voxels == null || _forces == null)
            return;

        const float gizmoSize = 0.05f;
        Gizmos.color = Color.yellow;

        foreach (var p in _voxels) {
            Gizmos.DrawCube(transform.TransformPoint(p), new Vector3(gizmoSize, gizmoSize, gizmoSize));
        }

        Gizmos.color = Color.cyan;

        foreach (var force in _forces) {
            Gizmos.DrawCube(force[0], new Vector3(gizmoSize, gizmoSize, gizmoSize));
            Gizmos.DrawLine(force[0], force[0] + force[1] / _rigidBody.mass);
        }

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position + _rigidBody.centerOfMass, 0.2f);
    }
}