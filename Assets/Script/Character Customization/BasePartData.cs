using System;
using UnityEngine;
[Serializable]
public abstract class BasePartData 
{
    public string partName;
    [HideInInspector] public int currentIndex = 0;

    public abstract int GetVariantCount();
}
