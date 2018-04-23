using UnityEngine;

namespace Cube.World {
    public class TerrainColliderEnableDisableTrigger : MonoBehaviour {
        public TerrainCollider terrainCollider;

        void OnTriggerEnter(Collider collider) {
            Physics.IgnoreCollision(collider, terrainCollider, true);
        }

        void OnTriggerExit(Collider collider) {
            Physics.IgnoreCollision(collider, terrainCollider, false);
        }
    }
}