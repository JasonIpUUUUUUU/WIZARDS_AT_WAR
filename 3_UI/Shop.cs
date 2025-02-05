using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    [SerializeField]
    private int money, stardust;

    [SerializeField]
    private int[] allPotionPrices, prices;

    [SerializeField]
    private bool willResetShop;

    [SerializeField]
    private string[] potions, allPotionNames;

    [SerializeField]
    private Button[] sellButtons;

    [SerializeField]
    private TextMeshProUGUI[] itemTexts, priceTexts;

    [SerializeField]
    private Image[] itemImages;

    [SerializeField]
    private Sprite[] AllitemIcons;

    [SerializeField]
    private Sprite placeHolderSprite;

    Dictionary<string, int> pricesDict;

    // Start is called before the first frame update
    void Start()
    {
        pricesDict = new Dictionary<string, int>();

        // sets up the indexes of each potion into a dictionary
        for (int i = 0; i < allPotionPrices.Length; i++) 
        {
            pricesDict[allPotionNames[i]] = i;
        }
        // get current date in a string
        string currentDate = System.DateTime.Now.ToString("MM/dd/yyyy");
        // keep track of the previous time the shop was set up
        if (PlayerPrefs.HasKey("PREVDATE"))
        {
            string prevDate = PlayerPrefs.GetString("PREVDATE");
            System.DateTime currentDateTime = System.DateTime.ParseExact(currentDate, "MM/dd/yyyy", null);
            System.DateTime prevDateTime = System.DateTime.ParseExact(prevDate, "MM/dd/yyyy", null);
            if (currentDateTime > prevDateTime)
            {
                willResetShop = true;
                PlayerPrefs.SetString("PREVDATE", currentDate);
            }
        }
        else
        {
            PlayerPrefs.SetString("PREVDATE", currentDate);
            willResetShop = true; 
        }
        setupPotion();
        // when the scene is loaded, check if the player has already purchased the daily potions
        for (int i = 0; i < potions.Length; i++)
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
        if(money >= prices[slot] && potions[slot] != "placeholder")
        {
            PlayerPrefs.SetInt("MONEY", money -= prices[slot]);
            sellButtons[slot].interactable = false;
            PlayerPrefs.SetInt(potions[slot], 1);
        }
    }

    public void setupPotion()
    {
        if (willResetShop)
        {
            willResetShop = false;
            resetShop();
        }
        else
        {
            for(int i = 0; i < 3; i++)
            {
                string shopSlot = "SHOP" + i;
                int index = PlayerPrefs.GetInt(shopSlot);
                if(index == -1)
                {
                    potions[i] = "placeholder";
                    prices[i] = 0;
                    priceTexts[i].text = "$0";
                    itemTexts[i].text = "all sold out";
                    itemImages[i].sprite = placeHolderSprite;
                }
                else
                {
                    potions[i] = allPotionNames[index];
                    prices[i] = allPotionPrices[index];
                    priceTexts[i].text = '$' + allPotionPrices[index].ToString();
                    itemTexts[i].text = allPotionNames[index];
                    itemImages[i].sprite = AllitemIcons[index];
                }
            }
        }
    }

    public void hackerButton()
    {
        PlayerPrefs.SetInt("MONEY", 10000);
        resetShop();
    }

    public void resetShop()
    {
        Debug.Log("resetting shop");
        willResetShop = false;
        // set up an initial list of all possible items the shop can sell
        List<string> availableItems = new List<string>();
        foreach (string potionName in allPotionNames)
        {
            if (!PlayerPrefs.HasKey(potionName))
            {
                availableItems.Add(potionName);
            }
        }
        // set up the 3 potions that are being on sale
        for (int i = 0; i < 3; i++)
        {
            string shopSlot = "SHOP" + i;
            if (availableItems.Count > 0)
            {
                string item = availableItems[Random.Range(0, availableItems.Count)];
                // an index is used as multiple things need reference to the potion such as icon and price
                PlayerPrefs.SetInt(shopSlot, pricesDict[item]);
                availableItems.Remove(item);
            }
            else
            {
                PlayerPrefs.SetInt(shopSlot, -1);
            }
        }
        setupPotion();
    }
}
