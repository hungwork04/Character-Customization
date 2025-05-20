using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CustomUIController : MonoBehaviour
{
    public GameObject UICustomCharacterE;
    public RectTransform parentUI;
    public List<RectTransform> Positions = new List<RectTransform>();

    public GameObject currentModelTarget = null;
    public GameObject currentUIParent = null;

    // Lưu UI của từng nhân vật (key = nhân vật, value = parent chứa UI elements)
    private Dictionary<GameObject, GameObject> uiPerCharacter = new Dictionary<GameObject, GameObject>();

    public GameObject SpawnUIElement(RectTransform UIposition, Transform parent)
    {
        GameObject newElement = Instantiate(UICustomCharacterE, parent);
        RectTransform newRect = newElement.GetComponent<RectTransform>();
        newRect.anchorMin = UIposition.anchorMin;
        newRect.anchorMax = UIposition.anchorMax;
        newRect.pivot = UIposition.pivot;
        newRect.anchoredPosition = UIposition.anchoredPosition;
        newRect.sizeDelta = UIposition.sizeDelta;
        newRect.localRotation = UIposition.localRotation;
        newRect.localScale = UIposition.localScale;
        return newElement;
    }

    public GameObject CreateUIForModel(GameObject model)
    {
        PlayerCharacterCustomized custom = model.GetComponent<PlayerCharacterCustomized>();
        if (custom == null) return null;

        GameObject uiParent = new GameObject($"UI_{model.name}", typeof(RectTransform));
        uiParent.transform.SetParent(parentUI, false);
        RectTransform uiParentRect = uiParent.GetComponent<RectTransform>();
        uiParentRect.anchorMin = Vector2.zero;
        uiParentRect.anchorMax = Vector2.one;
        uiParentRect.offsetMin = Vector2.zero;
        uiParentRect.offsetMax = Vector2.zero;
        uiParentRect.localScale = Vector3.one;

        for (int i = 0; i < custom.bodyPartDataArray.Count; i++)
        {
            int index = i;
            GameObject elementGO = SpawnUIElement(Positions[i], uiParent.transform);
            UIElement element = elementGO.GetComponent<UIElement>();
            element.bodyPartName.text = custom.bodyPartDataArray[i].name;
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
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Chuột trái
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var customTarget = hit.collider.GetComponent<PlayerCharacterCustomized>();
                if (customTarget != null)
                {
                    Debug.Log($"Clicked on: {customTarget.gameObject.name}");
                    ShowUIForModel(customTarget.gameObject);
                }
            }
        }
    }
}
