using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour, ICharacterObserver
{
    CharacterSelector characterSelector;
    CustomUIController UIController;
    public TextMeshProUGUI CharacterName;
    void Awake()
    {
        characterSelector=FindAnyObjectByType<CharacterSelector>();
        UIController= FindAnyObjectByType<CustomUIController>();
    }

    //public TextMeshProUGUI nameText;

    public void OnCharacterShown(CharacterModel character)
    {
        int thisInt=characterSelector.characters.IndexOf(character);
        CharacterName.text = character.gameObject.name;
        var customTarget =characterSelector.characters[thisInt].GetComponent<CombinedCustomizer>();
        if (customTarget != null)
        {
            Debug.Log($" On: {customTarget.gameObject.name}");
            if(UIController!=null)
                UIController.ShowUIForModel(customTarget.gameObject);
        }
    }

    public void OnCharacterHidden(CharacterModel character)
    {
        // Có thể dùng để fade-out UI hoặc clear text nếu muốn
    }
}
