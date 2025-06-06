using UnityEngine;
using System.Collections.Generic;

public class CharacterSelector : MonoBehaviour
{
    public List<CharacterModel> characters;
    public int curIndex = 0;
    public CharacterUI characterUI;

    private void Start()
    {
        // Gắn observer cho từng nhân vật
        foreach (var model in characters)
        {
            model.AddObserver(characterUI);
            model.Hide(); // Ẩn hết
        }

        characters[curIndex].Show(); // Hiện nhân vật đầu tiên
    }

    public void ShowNext()
    {
        characters[curIndex].Hide();
        curIndex = (curIndex + 1) % characters.Count;
        characters[curIndex].Show();
    }

    public void ShowPrevious()
    {
        characters[curIndex].Hide();
        curIndex = (curIndex - 1 + characters.Count) % characters.Count;
        characters[curIndex].Show();
    }
}
