using UnityEngine;
using System.Collections.Generic;

public abstract class BaseCharacterCustomizer<T> : MonoBehaviour where T : BasePartData
{
    [SerializeField] public List<T> parts = new List<T>();

    public void RightButtonChange(int index)
    {
        if (index < 0 || index >= parts.Count) return;

        var data = parts[index];
        int variantIndex = (data.currentIndex + 1) % data.GetVariantCount();
        ApplyVariant(data, variantIndex);
    }

    public void LeftButtonChange(int index)
    {
        if (index < 0 || index >= parts.Count) return;

        var data = parts[index];
        int variantIndex = (data.currentIndex - 1 + data.GetVariantCount()) % data.GetVariantCount();
        ApplyVariant(data, variantIndex);
    }

    protected abstract void ApplyVariant(T data, int variantIndex);
}
