using UnityEngine;
using System.Collections.Generic;

public class CharacterModel : MonoBehaviour
{

    private List<ICharacterObserver> observers = new();//Listeners

    public void Show()
    {
        gameObject.SetActive(true);
        NotifyShown();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        NotifyHidden();
    }

    public void AddObserver(ICharacterObserver observer)
    {
        if (!observers.Contains(observer))
            observers.Add(observer);
    }

    public void RemoveObserver(ICharacterObserver observer)
    {
        observers.Remove(observer);
    }

    private void NotifyShown()
    {
        foreach (var observer in observers)
            observer.OnCharacterShown(this);
    }

    private void NotifyHidden()
    {
        foreach (var observer in observers)
            observer.OnCharacterHidden(this);
    }
}
