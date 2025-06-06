using UnityEngine;
using System.Collections.Generic;

public class CombinedCustomizer : BaseCharacterCustomizer<CombinedPartData>
{
    protected override void ApplyVariant(CombinedPartData data, int variantIndex)
    {
        data.currentIndex = variantIndex;

        switch (data.partType)
        {
            case CombinedPartData.PartType.MeshMaterial:
                ApplyMeshMaterial(data, variantIndex);
                break;

            case CombinedPartData.PartType.PrefabPart:
                ApplyPrefabPart(data, variantIndex);
                break;
            case CombinedPartData.PartType.BodyPartDisable:
                BodyDisable(data, variantIndex);
                break;
        }
    }

    private void ApplyMeshMaterial(CombinedPartData data, int variantIndex)
    {
        if (variantIndex < 0 || variantIndex >= data.variants.Count) return;

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

    private void ApplyPrefabPart(CombinedPartData data, int variantIndex)
    {
        if (variantIndex < 0 || variantIndex >= data.partPrefabs.Count) return;

        // Lazy init bones/rootBone
        if (data.bones == null || data.bones.Length == 0)
        {
            //var smr = GetComponent<SkinnedMeshRenderer>();
            var smr = data.SkinMesh;

            if (smr != null)
            {
                data.bones = smr.bones;
                data.rootBone = smr.rootBone;
            }
        }

        // Clear old children
        if (data.parentOBJ != null)
        {
            foreach (Transform child in data.parentOBJ.transform)
                Destroy(child.gameObject);
        }

        // Instantiate and setup
        //var newPart = GameObject.Instantiate(data.partPrefabs[variantIndex]);
        GameObject newObj = GameObject.Instantiate(data.partPrefabs[variantIndex]);
        SkinnedMeshRenderer newPart = newObj.GetComponentInChildren<SkinnedMeshRenderer>();

        newPart.bones = data.bones;
        newPart.rootBone = data.rootBone;

        if (data.parentOBJ != null)
            newObj.transform.SetParent(data.parentOBJ.transform, false);
    }


    private void BodyDisable(CombinedPartData data, int variantIndex)
    {
        if (variantIndex < 0 || variantIndex >= data.partPrefabs.Count) return;
        if(data.partPrefabs[variantIndex]==null){
            if (data.parentOBJ != null)
            {
                foreach (Transform child in data.parentOBJ.transform)
                    Destroy(child.gameObject);
            }

            foreach(var item in data.disableObjects){
                item.SetActive(true);
            }
            return;
        }
        // Lazy init bones/rootBone
        if (data.bones == null || data.bones.Length == 0)
        {
            //var smr = GetComponent<SkinnedMeshRenderer>();
            var smr = data.SkinMesh;

            if (smr != null)
            {
                data.bones = smr.bones;
                data.rootBone = smr.rootBone;
            }
        }

        if (data.parentOBJ != null)
        {
            foreach (Transform child in data.parentOBJ.transform)
                Destroy(child.gameObject);
        }
        if(data.disableObjects.Count>0){
            foreach(var item in data.disableObjects){
                item.SetActive(false);
            }
        }
        // else if(data.partPrefabs[variantIndex]==null){
        //     foreach(var item in data.disableObjects){
        //         item.SetActive(true);
        //     }
        //     return;
        // }
        var newObj = Instantiate(data.partPrefabs[variantIndex]);
        SkinnedMeshRenderer newPart = newObj.GetComponentInChildren<SkinnedMeshRenderer>();
        newObj.gameObject.SetActive(true);
        // newPart.bones = data.bones;
        // newPart.rootBone = data.rootBone;

        if (data.parentOBJ != null)
            newObj.transform.SetParent(data.parentOBJ.transform, false);
    }
}
