using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Potion_UI : MonoBehaviour
{
    private Node node;

    [SerializeField]
    private GameObject potion_UI;

    [SerializeField]
    private string[] potions;

    [SerializeField]
    private Sprite[] potionSprites;

    [SerializeField]
    private Image[] slotImages;

    [SerializeField]
    private TextMeshProUGUI[] texts;

    public void Start()
    {
        setScreen();
    }

    public void setNode(Node nodeArg)
    {
        node = nodeArg;
    }

    public void setScreen()
    {
        for (int i = 0; i < potions.Length; i++)
        {
            // 3 if statements instead of if-else statements in case they have the same sprite (e.g. if it is the empty sprite)
            if (PlayerPrefs.GetString("SLOT0") == potions[i])
            {
                slotImages[0].sprite = potionSprites[i];
                texts[0].text = potions[i];
            }
            if (PlayerPrefs.GetString("SLOT1") == potions[i])
            {
                slotImages[1].sprite = potionSprites[i];
                texts[1].text = potions[i];
            }
            if (PlayerPrefs.GetString("SLOT2") == potions[i])
            {
                slotImages[2].sprite = potionSprites[i];
                texts[2].text = potions[i];
            }
        }
    }

    public void attachPotion(int slot)
    {
        string slotName = "SLOT" + slot.ToString();
        string potion = PlayerPrefs.GetString(slotName);
        float cooldown = 0;
        switch (potion)
        {
            case "HASTE":
                cooldown = 15f;
                break;
            case "PRODUCE":
                cooldown = 20f;
                break;
            case "SHIELD":
                cooldown = 15f;
                break;
        }
        node.addPotion(potion, cooldown);
        closeUI();
    }

    public void closeUI()
    {
        gameObject.SetActive(false);
        node = null;
    }
}
