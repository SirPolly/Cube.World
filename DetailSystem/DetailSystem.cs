using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cube.Gameplay;

namespace Cube.World
{
    public class DetailSystem : MonoBehaviour
    {
        class DetailPool
        {
            public Matrix4x4[] positions;
            public HashSet<IntVector2> positionSet;
        }

        Dictionary<DetailLayer, DetailPool> _pools = new Dictionary<DetailLayer, DetailPool>();
        Dictionary<DetailCamera, Vector3> _lastCameraPositions = new Dictionary<DetailCamera, Vector3>();

        void Start()
        {
            StartCoroutine(UpdateDetailCameras());
        }

        void Update()
        {
            foreach (var detailCamera in DetailCamera.all) {
                foreach (var detailLayer in detailCamera.detailLayers) {
                    DetailPool pool;
                    if (!_pools.TryGetValue(detailLayer, out pool))
                        continue;

                    Graphics.DrawMeshInstanced(detailLayer.Mesh, 0, detailLayer.Material, pool.positions);
                }
            }
        }

        IEnumerator UpdateDetailCameras()
        {
            yield return new WaitForSeconds(1); // Wait for world to init? Raycasts not working

            int iteration = 0;
            while (true) {
                foreach (var detailCamera in DetailCamera.all) {
                    UpdateDetailCamera(detailCamera, iteration);
                    yield return new WaitForSeconds(1f / 60f);
                }
                ++iteration;
            }
        }

        void UpdateDetailCamera(DetailCamera detailCamera, int iteration)
        {
            var updateDistanceThreshold = 0.1f;
            var updateInitialThreshold = 10;

            bool initialUpdate;
            var moveDirection = Vector3.zero;
            Vector3 lastPosition;
            if (_lastCameraPositions.TryGetValue(detailCamera, out lastPosition)) {
                moveDirection = lastPosition - detailCamera.transform.position;
                if (moveDirection.sqrMagnitude < updateDistanceThreshold)
                    return;

                initialUpdate = moveDirection.sqrMagnitude > updateInitialThreshold;
            }
            else {
                initialUpdate = true;
            }

            _lastCameraPositions[detailCamera] = detailCamera.transform.position;

            foreach (var detailLayer in detailCamera.detailLayers) {
                UpdateDetailLayer(detailCamera, detailLayer, moveDirection, iteration, initialUpdate);
            }
        }

        void UpdateDetailLayer(DetailCamera detailCamera, DetailLayer detailLayer, Vector3 moveDirection, int iteration, bool initial)
        {
            var updateBatchSize = 20;

            var offsetDistance = detailLayer.CameraDistance * 0.95f;
            var sqrCameraDistance = Mathf.Pow(detailLayer.CameraDistance, 2);

            var pool = GetOrCreateDetailLayerPool(detailLayer);
            var poolPositions = pool.positions;
            for (int jj = 0; jj < (!initial ? updateBatchSize : poolPositions.Length); ++jj) {
                var j = (iteration * updateBatchSize + jj) % poolPositions.Length;
                var poolPosition = (Vector3)poolPositions[j].GetColumn(3);

                var wasAlive = poolPosition != Vector3.zero;
                var detailCameraDistanceSqr = Mathf.Pow(poolPosition.x - detailCamera.transform.position.x, 2) + Mathf.Pow(poolPosition.z - detailCamera.transform.position.z, 2);
                if (wasAlive && detailCameraDistanceSqr < sqrCameraDistance)
                    continue;

                var oldHashPosition = GetHashPositionForPosition(poolPosition, detailLayer.Density);
                if (wasAlive) {
                    pool.positionSet.Remove(oldHashPosition);
                    poolPositions[j] = Matrix4x4.zero;
                }

                var heightRayOffsetUp = 30f;
                var heightRayOffsetDown = 100f;
                var offsetY = detailCamera.transform.position.y + heightRayOffsetUp;

                Vector3 offset;
                if (!initial) {
                    // Only spawn new details in the direction we are moving +- 90 degrees
                    var a = Math3d.SignedVectorAngle(Vector3.back, moveDirection, Vector3.up);
                    var a2 = Random.Range(a - 90, a + 90) * Mathf.Deg2Rad;
                    offset = new Vector3(Mathf.Sin(a2) * offsetDistance, offsetY, Mathf.Cos(a2) * offsetDistance);
                }
                else {
                    offset = new Vector3(Random.Range(-offsetDistance, offsetDistance), offsetY, Random.Range(-offsetDistance, offsetDistance));
                }
                var newPos = detailCamera.transform.position + offset;

                var newHashPosition = GetHashPositionForPosition(newPos, detailLayer.Density);
                if (pool.positionSet.Contains(newHashPosition))
                    continue;

                Debug.DrawLine(newPos, newPos + Vector3.down * heightRayOffsetDown, Color.yellow);

                RaycastHit hitInfo;
                if (!Physics.Raycast(newPos, Vector3.down, out hitInfo, heightRayOffsetDown, Layers.DefaultMask))
                    continue;

                PhysicMaterial material;

                var terrainSurfaces = hitInfo.collider.GetComponent<TerrainSurfaces>();
                if (terrainSurfaces != null) {
                    material = terrainSurfaces.GetPhysicMaterialForPosition(hitInfo.point);
                }
                else {
                    material = hitInfo.collider.sharedMaterial;
                }

                if (!detailLayer.Mask.Contains(material))
                    continue;

                newPos.y = hitInfo.point.y;

                var rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal) * Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
                var scale = Vector3.one * Random.Range(0.8f, 1.2f);
                poolPositions[j] = Matrix4x4.TRS(newPos, rotation, scale);
                pool.positionSet.Add(newHashPosition);
            }
        }

        static IntVector2 GetHashPositionForPosition(Vector3 position, float density)
        {
            return new IntVector2((int)(position.x * density), (int)(position.z * density));
        }

        DetailPool GetOrCreateDetailLayerPool(DetailLayer detailLayer)
        {
            DetailPool pool;
            if (_pools.TryGetValue(detailLayer, out pool))
                return pool;

            pool = new DetailPool {
                positions = new Matrix4x4[detailLayer.Amount],
                positionSet = new HashSet<IntVector2>()
            };

            _pools.Add(detailLayer, pool);

            return pool;
        }
    }
}