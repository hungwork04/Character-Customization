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

    public string groupID; // ID nhóm, ví dụ: "head", "body"

    public PartType partType;

    [Header("▶ MeshMaterial Data")]
    public Renderer render;

    [Serializable]
    public class MeshMaterialSet
    {
        public string ID; // ID của variant, ví dụ: "variant_head_001"
        public bool isStartCustom;
        public bool isUnlocked;
        public List<string> skinModelIDs;
        public Mesh mesh;
        public Material[] materials;
    }

    public List<MeshMaterialSet> variants = new();

    [Header("▶ PrefabPart & BodyPartDisable Data")]
    public List<PrefabSet> partPrefabs = new();

    [Serializable]
    public class PrefabSet
    {
        public string ID; // ID của prefab, ví dụ: "prefab_body_001"
        public bool isStartCustom;
        public bool isUnlocked;
        public List<string> skinModelIDs;
        public GameObject prefab;
    }

    public GameObject parentOBJ;
    public Transform[] bones;
    public Transform rootBone;
    public SkinnedMeshRenderer SkinMesh;

    [Header("▶ Chỉ dùng cho BodyPartDisable")]
    public List<GameObject> disableObjects = new();

    public override int GetVariantCount()
    {
        return partType switch
        {
            PartType.MeshMaterial => variants.Count,
            PartType.PrefabPart => partPrefabs.Count,
            PartType.BodyPartDisable => partPrefabs.Count,
            _ => 0
        };
    }
}
