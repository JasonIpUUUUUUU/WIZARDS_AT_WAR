using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NodeInfoUI : MonoBehaviour
{
    private bool hasNode;
    private Node node;

    private int manpower, sendAmount, upgradeCost, productionLevel, potionLevel;

    [SerializeField]
    TextMeshProUGUI manPowerText, potionLevelText, productionLevelText, sendText1, sendText2, nodeType, upgradeText;

    // Update is called once per frame
    void Update()
    {
        if (hasNode)
        {
            manpower = node.returnManpower();
            manPowerText.text = manpower.ToString();
        }
    }

    public void instantiateValues(Node nodeArg)
    {
        node = nodeArg;
        hasNode = true;
    }
}
