using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PrefabPartData : BasePartData
{
    public List<SkinnedMeshRenderer> partPrefabs = new List<SkinnedMeshRenderer>();
    public GameObject parentOBJ;

    public override int GetVariantCount() => partPrefabs.Count;
}

public class PrefabSwapperCustomizer : BaseCharacterCustomizer<PrefabPartData>
{
    private Transform[] bones;
    public Transform rootBone;

    private void Start()
    {
        bones = GetComponent<SkinnedMeshRenderer>().bones;
    }

    protected override void ApplyVariant(PrefabPartData data, int variantIndex)
    {
        data.currentIndex = variantIndex;

        // Clear old children
        if (data.parentOBJ != null)
        {
            foreach (Transform child in data.parentOBJ.transform)
            {
                Destroy(child.gameObject);
            }
        }

        // Instantiate and set up new part
        SkinnedMeshRenderer newPart = Instantiate(data.partPrefabs[variantIndex]);
        newPart.bones = bones;
        newPart.rootBone = rootBone;

        if (data.parentOBJ != null)
            newPart.transform.SetParent(data.parentOBJ.transform, false);
    }
}
