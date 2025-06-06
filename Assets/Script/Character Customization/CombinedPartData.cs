using UnityEngine;
using System.Collections.Generic;
using System;


[Serializable]
public class CombinedPartData : BasePartData
{
    public enum PartType
    {
        MeshMaterial,
        PrefabPart,
        BodyPartDisable
    }

    public PartType partType;

  [Header("▶ MeshMaterial Data")]
    public Renderer render;

    [Serializable]
    public class MeshMaterialSet
    {
        public Mesh mesh;
        public Material[] materials;
    }

    public List<MeshMaterialSet> variants = new();

    [Space(10)]
    [Header("▶ PrefabPart & BodyPartDisable Data")]
    public List<GameObject> partPrefabs = new();
    public GameObject parentOBJ;
    public Transform[] bones;
    public Transform rootBone;
    public SkinnedMeshRenderer SkinMesh;

    // ▼ Chỉ dùng riêng cho BodyPartDisable
    [Space(10)]
    [Header("▶ Chỉ dùng cho BodyPartDisable")]
    public List<GameObject> disableObjects = new();
    public override int GetVariantCount()
    {
        return partType switch
        {
            PartType.MeshMaterial => variants.Count,
            PartType.PrefabPart => partPrefabs.Count,
            PartType.BodyPartDisable=>partPrefabs.Count,
            _ => 0
        };
    }
}
