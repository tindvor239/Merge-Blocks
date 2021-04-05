using UnityEngine;
using UnityEngine.UI;
using UnityEngine.CustomComponents;

public class LevelUpUI : CustomUIComponent
{
    #region Properties
    public Image Cat { get => (Image)GetDisplay("Cat"); }
    #endregion
    public void Initialized(Sprite sprite)
    {
        Cat.sprite = sprite;
    }
}
