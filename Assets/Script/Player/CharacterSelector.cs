using UnityEngine;
using System.Collections.Generic;

public class CharacterSelector : MonoBehaviour
{
    public List<CharacterModel> characters;// kiểm tra điều kiện lại :))) 
    public int curIndex = 0;
    public CharacterUI characterUI;
    public CharacterCustomizationManager characterCustomizationManager;
    private void Start()
    {
        // Gắn observer cho từng nhân vật
        foreach (var model in characters)
        {
            model.AddObserver(characterUI);
            model.Hide(); // Ẩn hết
        }
        if(characterCustomizationManager==null){
            Debug.Log("Null characterCustomizationManager");
        }
        string id=characters[curIndex].GetComponent<ModelIdentifier>().modelID;
        characterCustomizationManager.SetCurrentModel(id);
        characters[curIndex].Show(); // Hiện nhân vật đầu tiên

    }

    public void ShowNext()
    {
        characters[curIndex].Hide();
        curIndex = (curIndex + 1) % characters.Count;
        characters[curIndex].Show();
        string id=characters[curIndex].GetComponent<ModelIdentifier>().modelID;
        characterCustomizationManager.SetCurrentModel(id);
    }

    public void ShowPrevious()
    {
        characters[curIndex].Hide();
        curIndex = (curIndex - 1 + characters.Count) % characters.Count;
        string id=characters[curIndex].GetComponent<ModelIdentifier>().modelID;
        characterCustomizationManager.SetCurrentModel(id);
        characters[curIndex].Show();
    }
}
