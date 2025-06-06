using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private CustomUIController UIController;
    void Awake()
    {
        UIController= FindAnyObjectByType<CustomUIController>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Chuột trái
        {
             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var customTarget = hit.collider.GetComponent<CombinedCustomizer>();
                if (customTarget != null)
                {
                    Debug.Log($"Clicked on: {customTarget.gameObject.name}");
                    if(UIController!=null)
                        UIController.ShowUIForModel(customTarget.gameObject);
                }
            }
        }
    }
}
