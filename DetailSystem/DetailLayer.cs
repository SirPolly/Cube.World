using UnityEngine;
using UnityEngine.Serialization;

namespace Cube.World {
    [CreateAssetMenu(menuName = "Cube.World/Detail Layer")]
    public class DetailLayer : ScriptableObject {
        [FormerlySerializedAs("mesh")]
        public Mesh Mesh;
        [FormerlySerializedAs("material")]
        public Material Material;
        public PhysicMaterial[] Mask;
        [FormerlySerializedAs("cameraDistance")]
        public float CameraDistance;
        [FormerlySerializedAs("density")]
        [Range(0.01f, 4f)]
        public float Density;
        [FormerlySerializedAs("amount")]
        [Range(1, 1023)]
        public int Amount;
    }
}