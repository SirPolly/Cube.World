using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainColliderEnableDisableTrigger : MonoBehaviour {
    public TerrainCollider terrainCollider;

    void OnTriggerEnter(Collider collider) {
        Physics.IgnoreCollision(collider, terrainCollider, true);
    }

    void OnTriggerExit(Collider collider) {
        Physics.IgnoreCollision(collider, terrainCollider, false);
    }
}
