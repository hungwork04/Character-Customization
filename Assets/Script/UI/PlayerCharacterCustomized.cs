using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCharacterCustomized : MonoBehaviour
{
    [SerializeField]
    public List<BodyPartData> bodyPartDataArray = new List<BodyPartData>();

    [Serializable]
    public class MeshMaterialSet
    {
        public Mesh mesh;
        public Material[] materials;
    }

    [Serializable]
    public class BodyPartData
    {
        public string name;
        public List<MeshMaterialSet> variants = new List<MeshMaterialSet>();

        // Dùng chung cho cả SkinnedMeshRenderer và MeshRenderer
        public Renderer renderer;
    }

    public void RightButtonChange(int index)
    {
        if (index < 0 || index >= bodyPartDataArray.Count) return;

        var data = bodyPartDataArray[index];
        int meshIndex = GetCurrentMeshIndex(data);

        if (meshIndex + 1 >= data.variants.Count)
            meshIndex = 0;
        else
            meshIndex += 1;

        ApplyVariant(data, data.variants[meshIndex]);

    }

    public void LeftButtonChange(int index)
    {
        if (index < 0 || index >= bodyPartDataArray.Count) return;

        var data = bodyPartDataArray[index];
        int meshIndex = GetCurrentMeshIndex(data);

        if (meshIndex - 1 < 0)
            meshIndex = data.variants.Count - 1;
        else
            meshIndex -= 1;

        ApplyVariant(data, data.variants[meshIndex]);
    }

    private int GetCurrentMeshIndex(BodyPartData data)
    {
        Mesh currentMesh = null;

        if (data.renderer is SkinnedMeshRenderer skinned)
        {
            currentMesh = skinned.sharedMesh;
        }
        else if (data.renderer is MeshRenderer)
        {
            var filter = data.renderer.GetComponent<MeshFilter>();
            if (filter != null)
            {
                currentMesh = filter.sharedMesh;
            }
        }

        for (int i = 0; i < data.variants.Count; i++)
        {
            if (data.variants[i].mesh == currentMesh)
                return i;
        }

        return -1;
    }

    private void ApplyVariant(BodyPartData data, MeshMaterialSet set)
    {
        if (data.renderer is SkinnedMeshRenderer skinned)
        {
            skinned.sharedMesh = set.mesh;

            // Nếu có materials thì mới đổi, còn không thì giữ nguyên
            if (set.materials != null && set.materials.Length > 0)
            {
                skinned.sharedMaterials = set.materials;
            }
        }
        else if (data.renderer is MeshRenderer meshRenderer)
        {
            var filter = data.renderer.GetComponent<MeshFilter>();
            if (filter != null)
            {
                filter.sharedMesh = set.mesh;
            }

            // Nếu có materials thì mới đổi, còn không thì giữ nguyên
            if (set.materials != null && set.materials.Length > 0)
            {
                meshRenderer.sharedMaterials = set.materials;
            }
        }
    }
}
