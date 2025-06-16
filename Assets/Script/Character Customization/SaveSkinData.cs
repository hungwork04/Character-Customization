using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;

public class SaveSkinData : MonoBehaviour
{
    public CharacterCustomizationManager characterCustomizationManager;
    public static SaveSkinData instance;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); 
        }
        else
        {
            instance = this;
        }
        //string path = Application.persistentDataPath + "/characterData.json";
        string path = Application.dataPath + "/Script/Data/playerdata.json";
        //LoadFromJson(path);
        SaveToJson(path);
        LoadFromJson(path);
    }
    public void SaveToJson(string filePath)
    {
        if(characterCustomizationManager==null){
            Debug.LogWarning("k có characterCustomizationManager");
            return;
        }
        characterCustomizationManager.customizationData = new CharacterCustomizationData
        {
            Models = new List<ModelData>(),
            BodyParts = new List<BodyPartData>()
        };

        foreach (var modelObj in characterCustomizationManager.modelGameObjects)
        {
            if (modelObj == null)
            {
                Debug.LogWarning("Model GameObject là null trong danh sách modelGameObjects!");
                continue;
            }
            ModelIdentifier modelIdentifier = modelObj.GetComponent<ModelIdentifier>();
            if (modelIdentifier != null && !string.IsNullOrEmpty(modelIdentifier.modelID))
            {
                characterCustomizationManager.customizationData.Models.Add(new ModelData
                {
                    ID = modelIdentifier.modelID,
                    isStartCustom = modelIdentifier.isStartCustom,
                    isUnlocked = modelIdentifier.isUnlocked
                });
            }
            else
            {
                Debug.LogWarning($"Model {modelObj.name} thiếu ModelIdentifier hoặc modelID!");
            }
        }

        foreach (var part in characterCustomizationManager.combinedPartDatas)
        {
            if (part == null || string.IsNullOrEmpty(part.groupID))
            {
                Debug.LogWarning($"CombinedPartData {(part != null ? part.groupID : "null")} thiếu groupID!");
                continue;
            }
            var bodyPart = new BodyPartData
            {
                ID = part.groupID,
                SkinParts = new List<SkinPartData>()
            };
            if (part.partType == CombinedPartData.PartType.MeshMaterial)
            {
                foreach (var variant in part.variants)
                {
                    if (string.IsNullOrEmpty(variant.ID)) continue;
                    bodyPart.SkinParts.Add(new SkinPartData
                    {
                        ID = variant.ID,
                        skinRefType = part.partType,
                        skinModelIDs = variant.skinModelIDs != null ? new List<string>(variant.skinModelIDs) : new List<string>(),
                        isStartCustom = variant.isStartCustom,
                        isUnlocked = variant.isUnlocked
                    });
                }
            }
            else
            {
                foreach (var prefab in part.partPrefabs)
                {
                    if (string.IsNullOrEmpty(prefab.ID)) continue;
                    bodyPart.SkinParts.Add(new SkinPartData
                    {
                        ID = prefab.ID,
                        skinRefType = part.partType,
                        skinModelIDs = prefab.skinModelIDs != null ? new List<string>(prefab.skinModelIDs) : new List<string>(),
                        isStartCustom = prefab.isStartCustom,
                        isUnlocked = prefab.isUnlocked
                    });
                }
            }
            if (bodyPart.SkinParts.Count > 0)
            {
                characterCustomizationManager.customizationData.BodyParts.Add(bodyPart);
            }
        }

        string json = JsonUtility.ToJson(characterCustomizationManager.customizationData, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"Đã lưu JSON vào: {filePath}");
    }
    public void LoadFromJson(string customizationPath)
    {
        if (characterCustomizationManager == null)
        {
            Debug.LogError("CharacterCustomizationManager chưa được gán trong Inspector!");
            return;
        }

        //string customizationPath = Application.persistentDataPath + "/characterData.json";

        // Tải customizationData
        if (File.Exists(customizationPath))
        {
            string json = File.ReadAllText(customizationPath);
            characterCustomizationManager.customizationData = JsonUtility.FromJson<CharacterCustomizationData>(json);
            Debug.Log($"Đã tải customizationData từ: {customizationPath}");
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy file: {customizationPath}. Giữ customizationData hiện tại.");
            return;
        }

        // Đồng bộ isStartCustom và isUnlocked từ customizationData sang combinedPartDatas
        if (characterCustomizationManager.customizationData != null && characterCustomizationManager.customizationData.BodyParts != null)
        {
            if (characterCustomizationManager.combinedPartDatas == null)
            {
                Debug.LogWarning("combinedPartDatas là null. Khởi tạo danh sách rỗng.");
                characterCustomizationManager.combinedPartDatas = new List<CombinedPartData>();
            }

            foreach (var partData in characterCustomizationManager.combinedPartDatas)
            {
                if (partData == null || string.IsNullOrEmpty(partData.groupID)) continue;
                var bodyPart = characterCustomizationManager.customizationData.BodyParts.Find(bp => bp.ID == partData.groupID);
                if (bodyPart == null)
                {
                    Debug.LogWarning($"Không tìm thấy BodyPartData cho {partData.groupID}");
                    continue;
                }

                // Chỉ đồng bộ isStartCustom và isUnlocked
                foreach (var skinPart in bodyPart.SkinParts)
                {
                    if (partData.partType == CombinedPartData.PartType.MeshMaterial && partData.variants != null)
                    {
                        var variant = partData.variants.Find(v => v.ID == skinPart.ID);
                        if (variant != null)
                        {
                            variant.isStartCustom = skinPart.isStartCustom;
                            variant.isUnlocked = skinPart.isUnlocked;
                        }
                    }
                    else if (partData.partType == CombinedPartData.PartType.PrefabPart && partData.partPrefabs != null)
                    {
                        var prefab = partData.partPrefabs.Find(p => p.ID == skinPart.ID);
                        if (prefab != null)
                        {
                            prefab.isStartCustom = skinPart.isStartCustom;
                            prefab.isUnlocked = skinPart.isUnlocked;
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("customizationData hoặc BodyParts là null, không thể đồng bộ CombinedPartData.");
        }

        Debug.Log("Đã đồng bộ isStartCustom và isUnlocked cho CombinedPartData.");
    }
}

