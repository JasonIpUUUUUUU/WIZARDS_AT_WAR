using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion_UI : MonoBehaviour
{
    private Node node;

    [SerializeField]
    private GameObject potion_UI;

    public void setNode(Node nodeArg)
    {
        node = nodeArg;
    }

    public void attachPotion(int slot)
    {
        string slotName = "SLOT" + slot.ToString();
        node.addPotion(slotName, 10);
        closeUI();
    }

    public void closeUI()
    {
        gameObject.SetActive(false);
        node = null;
    }
}
