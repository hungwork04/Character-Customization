using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CustomUIController : MonoBehaviour
{
    public GameObject UICustomCharacterE;
    public RectTransform parentUI;
    // public List<RectTransform> Positions = new List<RectTransform>();

    public GameObject currentModelTarget = null;
    public GameObject currentUIParent = null;

    // Lưu UI của từng nhân vật (key = nhân vật, value = parent chứa UI elements)
    private Dictionary<GameObject, GameObject> uiPerCharacter = new Dictionary<GameObject, GameObject>();

    public ScrollRect scrollView;
    CharacterCustomizationManager custom;

    void Awake()
    {
        custom=FindAnyObjectByType<CharacterCustomizationManager>();
    }
    public GameObject SpawnUIElement(Transform parent)
    {
        GameObject newElement = Instantiate(UICustomCharacterE, parent);
        //RectTransform newRect = newElement.GetComponent<RectTransform>();
        return newElement;
    }

    public GameObject ContentGO;
    public GameObject CreateUIForModel(GameObject model)
    {
        //Debug.Log("!");
        if (custom == null) return null;
        GameObject uiParent= Instantiate(ContentGO);
        // GameObject uiParent = new GameObject($"UI_{model.name}", typeof(RectTransform));
        uiParent.gameObject.name=$"UI_{model.name}";
        uiParent.transform.SetParent(parentUI, false);


        string id=model.GetComponent<ModelIdentifier>().modelID;
        //Debug.Log(custom.GetAvailableSkinParts(id).Count);
        for (int i = 0; i < custom.GetAvailableBodyParts(id).Count; i++)   
        {
            int index = i;
            GameObject elementGO = SpawnUIElement(uiParent.transform);
            UIElement element = elementGO.GetComponent<UIElement>();
            element.bodyPartName.text = custom.GetAvailableBodyParts(id)[i].ID;
            element.left.onClick.AddListener(() => custom.LeftButtonChange(index));//đổi
            element.right.onClick.AddListener(() => custom.RightButtonChange(index));//đổi
           // Debug.Log(custom.GetAvailableBodyParts(id)[i].ID);
        }

        return uiParent;
    }
    public void ShowUIForModel(GameObject model)
    {
    //    Debug.Log("1");
        // Ẩn UI hiện tại nếu có
        if (currentUIParent != null)
        {
            currentUIParent.SetActive(false);
        }

        currentModelTarget = model;
        // Nếu đã có UI rồi thì chỉ bật lại
        if (uiPerCharacter.ContainsKey(model))
        {
            currentUIParent = uiPerCharacter[model];
            scrollView.content = currentUIParent.GetComponent<RectTransform>();
            currentUIParent.SetActive(true);
        }
        else
        {
            // Chưa có UI thì tạo mới
            GameObject newUI = CreateUIForModel(model);
            if (newUI != null)
            {
                uiPerCharacter.Add(model, newUI);
                currentUIParent = newUI;
                scrollView.content = currentUIParent.GetComponent<RectTransform>();
//            Debug.Log("1");
            }
        }
    }
    

}
