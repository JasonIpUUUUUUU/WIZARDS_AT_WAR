using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class potionInventoryObj : MonoBehaviour
{
    private string potionType;

    [SerializeField]
    private Inventory inventory;

    [SerializeField]
    private TextMeshProUGUI potionDesc;

    public void setPotion(string potionTypeParam, Inventory inventoryParam, string desc)
    {
        inventory = inventoryParam;
        potionType = potionTypeParam;
        potionDesc.text = desc;
    }

    public void equipPotion()
    {
        Debug.Log("equpping " + potionType);
        inventory.equipPotion(potionType);
    }
}
