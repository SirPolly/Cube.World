using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Core/Detail Layer")]
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
