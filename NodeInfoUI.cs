using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NodeInfoUI : MonoBehaviour
{
    private bool hasNode;
    private Player player;
    private Node node;

    private int manpower, sendAmount, upgradeCost, productionLevel, potionLevel;

    [SerializeField]
    TextMeshProUGUI manPowerText, potionLevelText, productionLevelText, sendText, nodeType, upgradeText;

    [SerializeField]
    private Slider slider;

    // Update is called once per frame
    void Update()
    {
        if (hasNode)
        {
            manpower = node.returnManpower();
            manPowerText.text = sendAmount.ToString() + '/' + manpower.ToString();
            sendText.text = "Send amount: " + sendAmount.ToString();
        }
    }

    public void startSend()
    {
        player.selectNodesToSend(sendAmount);
    }

    public void sliderManpower()
    {
        sendAmount = (int)(slider.value * manpower);
    }

    public void instantiateValues(Node nodeArg, Player playerArg)
    {
        node = nodeArg;
        player = playerArg;
        hasNode = true;
    }
}
