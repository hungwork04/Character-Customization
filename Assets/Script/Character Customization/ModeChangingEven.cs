using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeChangingEven : MonoBehaviour
{
    public static Action OnModeChange;
    public Button modeBtn;
    public Sprite[] BtnImg;
    public CustomizationMode currentMode = CustomizationMode.CreateCharacter;
    void Start()
    {
        OnModeChange+=ImgMode;
    }
    public enum CustomizationMode
    {
        CreateCharacter,
        InventoryCustom
    }
    public void EvenAct(){
        OnModeChange?.Invoke();
    }
    public bool IsModelValidForMode(ModelData model)
    {
        return currentMode switch
        {
            CustomizationMode.CreateCharacter => model.isStartCustom,
            CustomizationMode.InventoryCustom => model.isUnlocked,
            _ => false
        };
    }
    public bool IsSkinPartValidForMode(SkinPartData skinPart, string modelID)
    {
        bool isValid = currentMode switch
        {
            CustomizationMode.CreateCharacter => skinPart.isStartCustom,
            CustomizationMode.InventoryCustom => skinPart.isUnlocked,
            _ => false
        };
        return isValid && skinPart.skinModelIDs.Contains(modelID);//Kiểm tra xem modelID có trong ds skinModelIDs hay k
    }
    public void ImgMode(){
        if(currentMode==CustomizationMode.InventoryCustom){
            modeBtn.GetComponent<Image>().sprite=BtnImg[1];
            SetCustomizationMode(CustomizationMode.CreateCharacter);
        }else if(currentMode==CustomizationMode.CreateCharacter){
            modeBtn.GetComponent<Image>().sprite=BtnImg[0];
            SetCustomizationMode(CustomizationMode.InventoryCustom);
        }
    }
    public void SetCustomizationMode(CustomizationMode mode)//chuyển chế độ
    {
        currentMode = mode;
        Debug.Log($"Chuyển sang chế độ {mode}");
    }
    void OnDisable()
    {
        OnModeChange-=ImgMode;
    }
}
