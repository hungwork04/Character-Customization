using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;

// Các class bạn cung cấp
[Serializable]
public class ModelData
{
    public string ID;
    public bool isStartCustom;
    public bool isUnlocked;
}

[Serializable]
public class SkinPartData
{
    public string ID; 
    public CombinedPartData.PartType skinRefType;
    public List<string> skinModelIDs;
    public bool isStartCustom;
    public bool isUnlocked;
}

[Serializable]
public class BodyPartData
{
    public string ID;
    public List<SkinPartData> SkinParts;
}

[Serializable]
public class CharacterCustomizationData
{
    public List<ModelData> Models;
    public List<BodyPartData> BodyParts;
}

public class CharacterCustomizationManager : MonoBehaviour
{

public CharacterCustomizationData customizationData;
public List<CombinedPartData> combinedPartDatas;
public List<GameObject> modelGameObjects;
public ModeChangingEven modeEven;
// [SerializeField] private ModeChangingEven.CustomizationMode currentMode = ModeChangingEven.CustomizationMode.CreateCharacter;
[SerializeField] private string currentModelID;

void Start()
{
    //Debug.Log($"BodyParts count: {customizationData.BodyParts.Count}");
    var availableModels = GetAvailableModels();
    if (availableModels.Count <= 0)
    {
        Debug.LogWarning("Không có Model nào khả dụng trong chế độ này!");
        return;
    }
    currentModelID = availableModels[0].ID;
    Debug.Log($"Khởi tạo currentModelID thành {currentModelID}");
}
//GetAvailableBodyParts trọc vào để tìm current ID từng bodypart 
public void RefreshCreate()
{
    if (modeEven.currentMode != ModeChangingEven.CustomizationMode.CreateCharacter)
    {
        Debug.LogWarning("Refresh Create chỉ khả dụng trong CreateCharacter mode!");
        return;
    }

    // Đặt isUnlocked = false cho tất cả SkinPartData trong BodyPartData
    foreach (var bodyPart in customizationData.BodyParts)
    {
        foreach (var skinPart in bodyPart.SkinParts)
        {
            skinPart.isUnlocked = false;
        }
    }

    // Đặt isUnlocked = false cho tất cả variants/partPrefabs trong CombinedPartData
    foreach (var partData in combinedPartDatas)
    {
        if (partData == null) continue;
        if (partData.variants != null)
        {
            foreach (var variant in partData.variants)
            {
                variant.isUnlocked = false;
            }
        }
        if (partData.partPrefabs != null)
        {
            foreach (var prefab in partData.partPrefabs)
            {
                prefab.isUnlocked = false;
            }
        }
    }

    // Lưu dữ liệu vào JSON
    string path =  Application.dataPath + "/Script/Data/playerdata.json";
    SaveSkinData.instance.SaveToJson(path);

    Debug.Log("Đã đặt lại isUnlocked = false và lưu vào JSON.");
}

public void SaveCreate()
{
    if (modeEven.currentMode != ModeChangingEven.CustomizationMode.CreateCharacter)
    {
        Debug.LogWarning("Save Create chỉ khả dụng trong CreateCharacter mode!");
        return;
    }

    if (string.IsNullOrEmpty(currentModelID))
    {
        Debug.LogWarning("Không có model nào được chọn để lưu!");
        return;
    }

    // Đặt isUnlocked = true cho model hiện tại
    var model = customizationData.Models.Find(m => m.ID == currentModelID);
    if (model != null)
    {
        model.isUnlocked = true;
        Debug.Log($"Đã mở khóa model: {currentModelID}");
    }

    // Đặt isUnlocked = true cho SkinPartData và variants/partPrefabs đã chọn
    foreach (var partData in combinedPartDatas)
    {
        if (partData == null || string.IsNullOrEmpty(partData.groupID)) continue;
        var bodyPart = customizationData.BodyParts.Find(bp => bp.ID == partData.groupID);
        if (bodyPart == null) continue;

        var validSkinParts = bodyPart.SkinParts
            .Where(sp => modeEven.IsSkinPartValidForMode(sp, currentModelID))
            .ToList();
        if (validSkinParts.Count == 0 || partData.currentIndex < 0 || partData.currentIndex >= validSkinParts.Count) continue;

        // Mở khóa SkinPartData
        var selectedSkinPart = validSkinParts[partData.currentIndex];
        selectedSkinPart.isUnlocked = true;

        // Mở khóa variant hoặc prefab tương ứng
        if (partData.partType ==CombinedPartData.PartType.MeshMaterial)
        {
            Debug.Log("hh");
            var variant = partData.variants.Find(v => v.ID == selectedSkinPart.ID);
            if (variant != null)
            {
                variant.isUnlocked = true;
                 Debug.Log("hh");
            }
        }
        else if (partData.partType ==CombinedPartData.PartType.PrefabPart||partData.partType ==CombinedPartData.PartType.BodyPartDisable)
        {
            var prefab = partData.partPrefabs.Find(p => p.ID == selectedSkinPart.ID);
            if (prefab != null)
            {
                prefab.isUnlocked = true;
                Debug.Log("hh");
            }
        }
        Debug.Log(partData.partPrefabs.Count);
        Debug.Log($"Đã mở khóa SkinPart {selectedSkinPart.ID} cho bộ phận {partData.groupID}");
    }

    // Lưu dữ liệu vào JSON
    string path =  Application.dataPath + "/Script/Data/playerdata.json";
    SaveSkinData.instance.SaveToJson(path);
}
public void RightButtonChange(int index) 
{
    if (string.IsNullOrEmpty(currentModelID))
    {
        Debug.LogWarning("Hiện tại không có Model nào được chọn!");
        return;
    }
    var availableBodyParts = GetAvailableBodyParts(currentModelID);
    Debug.Log($"Số BodyParts khả dụng: {availableBodyParts.Count}");
    if (index < 0 || index >= availableBodyParts.Count)
    {
        Debug.LogWarning($"Chỉ số không hợp lệ {index} cho availableBodyParts!");
        return;
    }
    var bodyPart = availableBodyParts[index];
    if (bodyPart == null)
    {
        Debug.LogWarning($"BodyPart tại chỉ số {index} là null!");
        return;
    }
    Debug.Log($"BodyPart: {bodyPart.ID}");
    // Tìm CombinedPartData tương ứng
    var partData = combinedPartDatas.Find(p => p != null && p.groupID == bodyPart.ID);
    if (partData == null)
    {
        Debug.LogWarning($"CombinedPartData cho BodyPart {bodyPart.ID} không tìm thấy!");
        return;
    }
    // Lấy danh sách SkinPartData hợp lệ
    var validSkinParts = bodyPart.SkinParts
        .Where(sp => modeEven.IsSkinPartValidForMode(sp, currentModelID))
        .ToList();
    // Debug.Log($"Valid SkinParts cho {bodyPart.ID}: {string.Join(", ", validSkinParts.Select(sp => sp.ID))}");
    if (validSkinParts.Count == 0)
    {
        Debug.LogWarning($"Không có SkinPart hợp lệ trong BodyPart {bodyPart.ID}!");
        return;
    }
    // Kiểm tra và điều chỉnh currentIndex
    if (partData.currentIndex < 0 || partData.currentIndex >= validSkinParts.Count)
    {
        Debug.LogWarning($"currentIndex {partData.currentIndex} không hợp lệ, đặt về 0!");
        partData.currentIndex = 0;
    }
    // Tăng currentIndex
    partData.currentIndex = (partData.currentIndex + 1) % validSkinParts.Count;
    string nextRefID = validSkinParts[partData.currentIndex].ID;
    // Áp dụng SkinPart
    ApplySkinPart(nextRefID, currentModelID);
    Debug.Log($"Chuyển sang SkinPart {nextRefID} trong BodyPart {bodyPart.ID}, currentIndex: {partData.currentIndex}");
}

public void LeftButtonChange(int index)
{
    if (string.IsNullOrEmpty(currentModelID))
    {
        Debug.LogWarning("Hiện tại không có Model nào được chọn!");
        return;
    }
    var availableBodyParts = GetAvailableBodyParts(currentModelID);
    Debug.Log($"Số BodyParts khả dụng: {availableBodyParts.Count}");
    if (index < 0 || index >= availableBodyParts.Count)
    {
        Debug.LogWarning($"Chỉ số không hợp lệ {index} cho availableBodyParts!");
        return;
    }
    var bodyPart = availableBodyParts[index];
    if (bodyPart == null)
    {
        Debug.LogWarning($"BodyPart tại chỉ số {index} là null!");
        return;
    }
    //Debug.Log($"BodyPart: {bodyPart.ID}");
    // Tìm CombinedPartData tương ứng
    var partData = combinedPartDatas.Find(p => p != null && p.groupID == bodyPart.ID);
    if (partData == null)
    {
        Debug.LogWarning($"CombinedPartData cho BodyPart {bodyPart.ID} không tìm thấy!");
        return;
    }
    // Lấy danh sách SkinPartData hợp lệ
    var validSkinParts = bodyPart.SkinParts
        .Where(sp => modeEven.IsSkinPartValidForMode(sp, currentModelID))
        .ToList();
    Debug.Log($"Valid SkinParts cho {bodyPart.ID}: {string.Join(", ", validSkinParts.Select(sp => sp.ID))}");
    if (validSkinParts.Count == 0)
    {
        Debug.LogWarning($"Không có SkinPart hợp lệ trong BodyPart {bodyPart.ID}!");
        return;
    }
    // Kiểm tra và điều chỉnh currentIndex
    if (partData.currentIndex < 0 || partData.currentIndex >= validSkinParts.Count)
    {
        Debug.LogWarning($"currentIndex {partData.currentIndex} không hợp lệ, đặt về 0!");
        partData.currentIndex = 0;
    }
    // Giảm currentIndex
    partData.currentIndex = (partData.currentIndex - 1 + validSkinParts.Count) % validSkinParts.Count;
    string nextRefID = validSkinParts[partData.currentIndex].ID;
    // Áp dụng SkinPart
    ApplySkinPart(nextRefID, currentModelID);
    //Debug.Log($"Chuyển sang SkinPart {nextRefID} trong BodyPart {bodyPart.ID}, currentIndex: {partData.currentIndex}");
}

public List<BodyPartData> GetAvailableBodyParts(string modelID)
{
    List<BodyPartData> availableBodyParts = new();
    foreach (var bodyPart in customizationData.BodyParts)
    {
        if (bodyPart.SkinParts == null || bodyPart.SkinParts.Count == 0) continue;
        
        bool hasValidSkinPart = false;
        foreach (var skinPart in bodyPart.SkinParts)
        {
            if (modeEven.IsSkinPartValidForMode(skinPart, modelID))
            {
                foreach (var partData in combinedPartDatas)
                {
                    if (partData == null || partData.groupID != bodyPart.ID) continue;
                    if (partData.partType == CombinedPartData.PartType.MeshMaterial)
                    {
                        var variant = partData.variants.Find(v => v.ID == skinPart.ID  && v.skinModelIDs.Contains(modelID));
                        if (variant != null)
                        {
                            hasValidSkinPart = true;
                            break;
                        }
                    }
                    var prefab = partData.partPrefabs.Find(p => p.ID == skinPart.ID && p.skinModelIDs.Contains(modelID));
                    if (prefab != null)
                    {
                        hasValidSkinPart = true;
                        break;
                    }
                }
                if (hasValidSkinPart) break;
            }
        }
        
        if (hasValidSkinPart && !availableBodyParts.Contains(bodyPart))
        {
            availableBodyParts.Add(bodyPart);
        }
    }
    return availableBodyParts;
}

public void SetCurrentModel(string modelID)//đặt Model hiện tại
{
    if (GetAvailableModels().Any(x => x.ID == modelID))
    {
        currentModelID = modelID;
        Debug.Log($"Đặt currentModelID thành {modelID}");
    }
    else
    {
        Debug.LogWarning($"Model ID {modelID} không khả dụng!");
    }
}


public List<ModelData> GetAvailableModels()
{
    List<ModelData> availableModels = new();
    foreach (var model in customizationData.Models)
    {
        if (modeEven.IsModelValidForMode(model))
            availableModels.Add(model);
    }
    return availableModels;
}


public void ApplySkinPart(string refID, string modelID)
{
    SkinPartData skinPart = null;
    BodyPartData bodyPart = null;
    foreach (var bp in customizationData.BodyParts)
    {
        skinPart = bp.SkinParts.Find(sp => sp.ID == refID);
        if (skinPart != null)
        {
            bodyPart = bp;
            break;
        }
    }
    if (skinPart == null || !skinPart.skinModelIDs.Contains(modelID))
    {
        Debug.LogWarning($"SkinPart với refID {refID} không tìm thấy hoặc không tương thích với model {modelID}!");
        return;
    }
    CombinedPartData partData = combinedPartDatas.Find(p => p != null && p.groupID == bodyPart.ID);
    if (partData == null)
    {
        Debug.LogWarning($"CombinedPartData cho BodyPart {bodyPart.ID} không tìm thấy trong Inspector!");
        return;
    }
    if (skinPart.skinRefType != partData.partType)
    {
        Debug.LogWarning($"SkinRefType {skinPart.skinRefType} không khớp với PartType {partData.partType}!");
        return;
    }
    switch (partData.partType)
    {
        case CombinedPartData.PartType.MeshMaterial:
            ApplyMeshMaterial(partData, refID);
            break;
        case CombinedPartData.PartType.PrefabPart:
            ApplyPrefabPart(partData, refID);
            break;
        case CombinedPartData.PartType.BodyPartDisable:
            ApplyBodyPartDisable(partData, refID);
            break;
    }
}

private void ApplyMeshMaterial(CombinedPartData partData, string refID)
{
    var variant = partData.variants.Find(v => v.ID == refID);
    if (variant == null)
    {
        Debug.LogError($"Variant với ID {refID} không tìm thấy trong CombinedPartData {partData.groupID}!");
        return;
    }
    if (partData.render is SkinnedMeshRenderer skinned)
    {
        skinned.sharedMesh = variant.mesh;
        if (variant.materials != null && variant.materials.Length > 0)
            skinned.sharedMaterials = variant.materials;
        Debug.Log($"Áp dụng MeshMaterial (SkinnedMeshRenderer): {variant.mesh.name} cho {refID}");
    }
    else if (partData.render is MeshRenderer meshRenderer)
    {
        var filter = partData.render.GetComponent<MeshFilter>();
        if (filter != null)
            filter.sharedMesh = variant.mesh;
        else
            Debug.LogError($"Thiếu MeshFilter cho MeshRenderer trong {refID}!");
        if (variant.materials != null && variant.materials.Length > 0)
            meshRenderer.sharedMaterials = variant.materials;
        Debug.Log($"Áp dụng MeshMaterial (MeshRenderer): {variant.mesh.name} cho {refID}");
    }
    else
    {
        Debug.LogError($"Renderer cho {refID} không phải là SkinnedMeshRenderer hoặc MeshRenderer!");
    }
}

private void ApplyPrefabPart(CombinedPartData partData, string refID)
{
    var prefabSet = partData.partPrefabs.Find(p => p.ID == refID);
    if (prefabSet == null || prefabSet.prefab == null)
    {
        Debug.LogError($"Prefab với ID {refID} không tìm thấy hoặc null trong CombinedPartData {partData.groupID}!");
        return;
    }
    if (partData.bones == null || partData.bones.Length == 0)
    {
        var smr = partData.SkinMesh;
        if (smr != null)
        {
            partData.bones = smr.bones;
            partData.rootBone = smr.rootBone;
            Debug.Log($"Khởi tạo bones và rootBone từ SkinMesh cho {refID}");
        }
    }
    if (partData.parentOBJ != null)
    {
        foreach (Transform child in partData.parentOBJ.transform)
            Destroy(child.gameObject);
    }
    else
    {
        Debug.LogError($"parentOBJ là null cho {refID}!");
        return;
    }
    GameObject newObj = Instantiate(prefabSet.prefab);
    SkinnedMeshRenderer newPart = newObj.GetComponentInChildren<SkinnedMeshRenderer>();
    if (newPart != null)
    {
        newPart.bones = partData.bones;
        newPart.rootBone = partData.rootBone;
        //Debug.Log($"Áp dụng PrefabPart: {newObj.name} với bones cho {refID}");
    }
    else
    {
        Debug.LogWarning($"Không tìm thấy SkinnedMeshRenderer trong prefab cho {refID}");
    }
    newObj.transform.SetParent(partData.parentOBJ.transform, false);
}

private void ApplyBodyPartDisable(CombinedPartData partData, string refID)
{
    var prefabSet = partData.partPrefabs.Find(p => p.ID == refID);
    if (prefabSet == null || prefabSet.prefab == null)
    {
        if (partData.parentOBJ != null)
        {
            foreach (Transform child in partData.parentOBJ.transform)
                Destroy(child.gameObject);
        }
        if (partData.disableObjects != null)
        {
            foreach (var item in partData.disableObjects)
            {
                if (item != null) item.SetActive(true);
            }
        }
        Debug.Log($"Xóa prefab và kích hoạt disableObjects cho {refID}");
        return;
    }
    if (partData.bones == null || partData.bones.Length == 0)
    {
        var smr = partData.SkinMesh;
        if (smr != null)
        {
            partData.bones = smr.bones;
            partData.rootBone = smr.rootBone;
            Debug.Log($"Khởi tạo bones và rootBone từ SkinMesh cho {refID}");
        }
    }
    if (partData.parentOBJ != null)
    {
        foreach (Transform child in partData.parentOBJ.transform)
            Destroy(child.gameObject);
    }
    else
    {
        Debug.LogError($"parentOBJ là null cho {refID}!");
        return;
    }
    if (partData.disableObjects != null && partData.disableObjects.Count > 0)
    {
        foreach (var item in partData.disableObjects)
        {
            if (item != null) item.SetActive(false);
        }
    }
    GameObject newObj = Instantiate(prefabSet.prefab);
    SkinnedMeshRenderer newPart = newObj.GetComponentInChildren<SkinnedMeshRenderer>();
    if (newPart != null)
    {
        newPart.bones = partData.bones;
        newPart.rootBone = partData.rootBone;
        Debug.Log($"Áp dụng BodyPartDisable: {newObj.name} với bones cho {refID}");
    }
    else
    {
        Debug.LogWarning($"Không tìm thấy SkinnedMeshRenderer trong prefab cho {refID}");
    }
        newObj.SetActive(true);
        newObj.transform.SetParent(partData.parentOBJ.transform, false);
    }

}