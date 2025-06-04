using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
[Serializable]
public class MeshPartData : BasePartData
{
    [Serializable]
    public class MeshMaterialSet
    {
        [SerializeField]
        public Mesh mesh;
        [SerializeField]
        public Material[] materials;
    }

    [SerializeField]
    public Renderer render;
    [SerializeField]
    public List<MeshMaterialSet> variants = new List<MeshMaterialSet>();

    public override int GetVariantCount() => variants.Count;
}
public class MeshMaterialCustomizer : BaseCharacterCustomizer<MeshPartData>
{

    protected override void ApplyVariant(MeshPartData data, int variantIndex)
    {
        if (variantIndex < 0 || variantIndex >= data.variants.Count) return;

        data.currentIndex = variantIndex;
        var set = data.variants[variantIndex];

        if (data.render is SkinnedMeshRenderer skinned)
        {
            skinned.sharedMesh = set.mesh;
            if (set.materials != null && set.materials.Length > 0)
                skinned.sharedMaterials = set.materials;
        }
        else if (data.render is MeshRenderer meshRenderer)
        {
            var filter = data.render.GetComponent<MeshFilter>();
            if (filter != null)
                filter.sharedMesh = set.mesh;
            if (set.materials != null && set.materials.Length > 0)
                meshRenderer.sharedMaterials = set.materials;
        }
    }
}
