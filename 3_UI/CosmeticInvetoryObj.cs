using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CosmeticInvetoryObj : MonoBehaviour
{
    private string cosmeticType;

    private int cosmeticIndex;

    private CharacterCustomisor cosmeticMenu;

    [SerializeField]
    private Image icon;

    public void setItem(int cosmeticIndexParam, string cosmeticTypeParam, CharacterCustomisor cosmeticMenuParam, Sprite spriteParam)
    {
        cosmeticIndex = cosmeticIndexParam;
        cosmeticType = cosmeticTypeParam;
        cosmeticMenu = cosmeticMenuParam;
        icon.sprite = spriteParam;
    }

    public void equipPotion()
    {
        cosmeticMenu.chooseCosmetic(cosmeticType, cosmeticIndex);
    }
}
