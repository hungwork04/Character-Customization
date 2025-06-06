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


    public GameObject SpawnUIElement(Transform parent)
    {
        GameObject newElement = Instantiate(UICustomCharacterE, parent);
        RectTransform newRect = newElement.GetComponent<RectTransform>();
        return newElement;
    }

    // public GameObject CreateUIForModel(GameObject model)
    // {
    //     CombinedCustomizer custom = model.GetComponent<CombinedCustomizer>();
    //     if (custom == null) return null;

    //     GameObject uiParent = new GameObject($"UI_{model.name}", typeof(RectTransform));
    //     uiParent.transform.SetParent(parentUI, false);
    //     RectTransform uiParentRect = uiParent.GetComponent<RectTransform>();
    //     uiParentRect.anchorMin = Vector2.zero;
    //     uiParentRect.anchorMax = Vector2.one;
    //     uiParentRect.offsetMin = Vector2.zero;
    //     uiParentRect.offsetMax = Vector2.zero;
    //     uiParentRect.localScale = Vector3.one;

    //     for (int i = 0; i < custom.parts.Count; i++)
    //     {
    //         int index = i;
    //         GameObject elementGO = SpawnUIElement(Positions[i], uiParent.transform);
    //         UIElement element = elementGO.GetComponent<UIElement>();
    //         //element.bodyPartName.text = custom.parts[i].name;
    //         element.left.onClick.AddListener(() => custom.LeftButtonChange(index));
    //         element.right.onClick.AddListener(() => custom.RightButtonChange(index));
    //     }

    //     return uiParent;
    // }
    public GameObject ContentGO;
    public GameObject CreateUIForModel(GameObject model)
    {
        CombinedCustomizer custom = model.GetComponent<CombinedCustomizer>();
        if (custom == null) return null;
        GameObject uiParent= Instantiate(ContentGO);
        // GameObject uiParent = new GameObject($"UI_{model.name}", typeof(RectTransform));
        uiParent.gameObject.name=$"UI_{model.name}";
        uiParent.transform.SetParent(parentUI, false);



        for (int i = 0; i < custom.parts.Count; i++)   
        {
            int index = i;
            GameObject elementGO = SpawnUIElement(uiParent.transform);
            UIElement element = elementGO.GetComponent<UIElement>();
            element.bodyPartName.text = custom.parts[i].partName;
            element.left.onClick.AddListener(() => custom.LeftButtonChange(index));
            element.right.onClick.AddListener(() => custom.RightButtonChange(index));
        }

        return uiParent;
    }
    public void ShowUIForModel(GameObject model)
    {
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
            }
        }
    }
    

}
