using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NodeInfoUI : MonoBehaviour
{
    private bool hasNode, redTeam, neutral;
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
            upgradeCost = node.getLevelUpCost();
            if (node.isNeutral())
            {
                neutral = true;
            }
            else
            {
                neutral = false;
                redTeam = player.getTeam();
            }
            manpower = node.returnManpower();
            manPowerText.text = sendAmount.ToString() + '/' + manpower.ToString();
            sendText.text = "Send amount: " + sendAmount.ToString();
            upgradeText.text = "UPGRADE: " + upgradeCost.ToString() + " MANPOWER";
            if (neutral)
            {
                nodeType.text = "NEUTRAL: node";
            }
            else
            {
                if (redTeam)
                {
                    nodeType.text = "RED: " + node.getType();
                }
                else
                {
                    nodeType.text = "BLUE: " + node.getType();
                }
            }
        }
    }

    public void startSend()
    {
        player.selectNodesToSend(sendAmount);
    }

    public void startUpgrade()
    {
        if (node.upgradeManpower())
        {
            node.modifyManPower(node.moreExpensiveUpgrades(), false, redTeam);
            upgradeCost = node.getLevelUpCost();
        }
        else
        {
            Debug.Log("purchase failed");
        }
    }

    public void sliderManpower()
    {
        sendAmount = (int)(slider.value * manpower);
    }

    public void instantiateValues(Node nodeArg, Player playerArg, bool redArg)
    {
        redTeam = redArg;
        node = nodeArg;
        player = playerArg;
        hasNode = true;
    }
}
