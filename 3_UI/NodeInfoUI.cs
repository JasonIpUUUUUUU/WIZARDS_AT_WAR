using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NodeInfoUI : MonoBehaviour
{
    private bool hasNode, redTeam, neutral, makingPotion;
    private Player player;
    private Node node;

    private int manpower, sendAmount, upgradeCost;

    private float potionCounter, potionTime;

    [SerializeField]
    private GameObject potionScreen;

    [SerializeField]
    private TextMeshProUGUI manPowerText, potionLevelText, productionLevelText, sendText, nodeType, upgradeText;

    [SerializeField]
    private Image potionCover;

    [SerializeField]
    private Slider slider;

    // Update is called once per frame
    void Update()
    {
        if (hasNode)
        {
            potionCover.fillAmount = node.returnPotionFill();
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

    public void interactPotion()
    {
        if (!makingPotion)
        {
            potionScreen.GetComponent<Potion_UI>().setNode(node);
            potionScreen.SetActive(true);
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

    public void instantiateValues(Node nodeArg, Player playerArg, bool redArg, GameObject potionArg)
    {
        potionScreen = potionArg;
        redTeam = redArg;
        node = nodeArg;
        player = playerArg;
        hasNode = true;
    }
}
