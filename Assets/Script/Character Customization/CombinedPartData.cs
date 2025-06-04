using UnityEngine;
using System.Collections.Generic;


[Serializable]
public class CombinedPartData : BasePartData
{
    // public enum PartType
    // {
    //     MeshMaterial,
    //     PrefabPart
    // }

    // public PartType partType;

    // // MeshMaterial data
    // public Renderer render;
    //     [Serializable]
    // public class MeshMaterialSet
    // {
    //     [SerializeField]
    //     public Mesh mesh;
    //     [SerializeField]
    //     public Material[] materials;
    // }
    // public List<MeshMaterialSet> variants = new();

    // // Prefab data
    // public List<SkinnedMeshRenderer> partPrefabs = new();
    // public GameObject parentOBJ;
    // public Transform[] bones;     // 👈 moved here
    // public Transform rootBone;    // 👈 moved here

    // public override int GetVariantCount()
    // {
    //     return partType switch
    //     {
    //         PartType.MeshMaterial => variants.Count,
    //         PartType.PrefabPart => partPrefabs.Count,
    //         _ => 0
    //     };
    // }
}
