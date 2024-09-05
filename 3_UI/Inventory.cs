using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private RectTransform firstPos, secondPos;

    private int potionNum, currentSlot;

    [SerializeField]
    private string[] potions;

    [SerializeField]
    private GameObject potionObjects, selectPotionBG;

    [SerializeField]
    private Sprite[] potionSprites;

    [SerializeField]
    private Image[] slotImages;

    private List<GameObject> currentPotions = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // show the correct potions in the correct slots
        showPotions();
        addPotions();
    }

    // this displays the potions that should be shown in the potion select screen
    public void addPotions()
    {
        Debug.Log("adding potions");
        // clear out the current potions so they can be reset
        foreach(GameObject potion in currentPotions)
        {
            Destroy(potion);
        }
        currentPotions.Clear();
        // number of potions instantiated, by default it should be zero
        potionNum = 0;
        // iterate through a list of potions to see if the player has it
        for (int i = 0; i < potions.Length; i++)
        {
            if (PlayerPrefs.HasKey(potions[i]))
            {
                // create an instance of the potion if the player has unlocked it
                GameObject potion = Instantiate(potionObjects, firstPos);
                currentPotions.Add(potion);
                // localPosition is the relative position to parent object
                potion.transform.localPosition = Vector3.zero;
                potion.transform.position += (secondPos.position - firstPos.position) * potionNum;
                potionNum++;
                // set the details of the potion
                potion.GetComponent<potionInventoryObj>().setPotion(potions[i]);
            }
        }
    }

    // The function called by the potion button when it is clicked
    public void equipPotion(string potion)
    {
        if(potion != "EXIT")
        {
            string equippedSlot = hasEquipped(potion);
            if(equippedSlot != "")
            {
                PlayerPrefs.SetString(equippedSlot, "");
            }
            string slotName = "SLOT" + currentSlot.ToString();
            Debug.Log(slotName);
            PlayerPrefs.SetString(slotName, potion);
        }
        showPotions();
        selectPotionBG.SetActive(false);
    }


    // check if a potion has already been equipped, if it is, return the slot it has been equipped in
    public string hasEquipped(string potion)
    {
        if (PlayerPrefs.GetString("SLOT0") == potion)
        {
            return "SLOT0";
        }
        else if (PlayerPrefs.GetString("SLOT1") == potion)
        {
            return "SLOT1";
        }
        else if (PlayerPrefs.GetString("SLOT2") == potion)
        {
            return "SLOT2";
        }
        else
        {
            return "";
        }
    }

    // the function called to get into the potion select screen for the specific slot
    public void setPotion(int slot)
    {
        // reset the potions every time the potion select screen is open
        addPotions();
        currentSlot = slot;
        selectPotionBG.SetActive(true);
    }

    public void showPotions()
    {
        // currently a brute force method similar to a linear search is used.
        // This is justified as there aren't many potions so using a binary search would not give a significant boost in performance
        for(int i = 0; i < potions.Length; i++)
        {
            // 3 if statements instead of if-else statements in case they have the same sprite (e.g. if it is the empty sprite)
            if(PlayerPrefs.GetString("SLOT0") == potions[i])
            {
                slotImages[0].sprite = potionSprites[i];
            }
            if (PlayerPrefs.GetString("SLOT1") == potions[i])
            {
                slotImages[1].sprite = potionSprites[i];
            }
            if (PlayerPrefs.GetString("SLOT2") == potions[i])
            {
                slotImages[2].sprite = potionSprites[i];
            }
        }
    }

    public string getDescription(string potionName)
    {
        return potionName;
    }
}
