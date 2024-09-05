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

    // Start is called before the first frame update
    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("INVENTORY").GetComponent<Inventory>();
    }

    public void setPotion(string potionTypeParam)
    {
        potionType = potionTypeParam;
        switch (potionType)
        {
            case "SPEED":
                potionDesc.text = "Speed potion: doubles movement speed of army when used until the army reaches its destination";
                break;
        }
    }

    public void equipPotion()
    {
        Debug.Log("equpping " + potionType);
        inventory.equipPotion(potionType);
    }
}
