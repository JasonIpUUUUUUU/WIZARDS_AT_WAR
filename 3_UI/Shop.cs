using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField]
    private int money, stardust;

    [SerializeField]
    private string[] potions;

    [SerializeField]
    private int[] prices;

    [SerializeField]
    private Button[] sellButtons;

    // Start is called before the first frame update
    void Start()
    {
        // when the scene is loaded, check if the player has already purchased the daily potions
        for(int i = 0; i < potions.Length; i++)
        {
            if(PlayerPrefs.GetInt(potions[i]) == 1)
            {
                sellButtons[i].interactable = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // load in the live values for money and stardust as they constantly change
        money = PlayerPrefs.GetInt("MONEY");
        stardust = PlayerPrefs.GetInt("STARDUST");
    }

    public void buyPotion(int slot)
    {
        // if the player has enough money, buy the potion
        if(money >= prices[slot])
        {
            PlayerPrefs.SetInt("MONEY", money -= prices[slot]);
            sellButtons[slot].interactable = false;
            PlayerPrefs.SetInt(potions[slot], 1);
        }
    }
}
