using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class NodeInfoUI : MonoBehaviour
{
    private bool hasNode, redTeam, neutral, makingPotion;
    private Player player;
    private Node node;

    private int manpower, sendAmount, upgradeCost;

    private float potionCounter, potionTime;

    [SerializeField]
    private GameObject potionScreen, playerUI;

    [SerializeField]
    private TextMeshProUGUI manPowerText, potionLevelText, productionLevelText, sendText, nodeType, upgradeText;

    [SerializeField]
    private Image potionCover;

    [SerializeField]
    private Slider slider;

    private PhotonView playerView;

    // Update is called once per frame
    void Update()
    {
        if (hasNode)
        {
            // displays how expensive the next upgrade should be
            upgradeCost = node.getLevelUpCost();

            // detemines if the node has a team or not
            if (node.isNeutral())
            {
                neutral = true;
            }
            else
            {
                neutral = false;
                if (node.sameTeam(player.getTeam()))
                {
                    redTeam = player.getTeam();
                }
                else
                {
                    redTeam = !player.getTeam();
                }
            }

            // display for the manpower
            manpower = node.returnManpower();
            manPowerText.text = sendAmount.ToString() + '/' + manpower.ToString();
            sendText.text = "Send amount: " + sendAmount.ToString();
            upgradeText.text = "UPGRADE: " + upgradeCost.ToString() + " MANPOWER";

            // display of the node type
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
            potionCover.fillAmount = node.returnPotionFill();

            if (node.sameTeam(redTeam))
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    interactPotion(!potionScreen.active);
                }
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    slider.value = 0.25f;
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    slider.value = 0.5f;
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    slider.value = 0.75f;
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    slider.value = 1;
                }
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    startSend();
                }
                if (Input.GetKeyDown(KeyCode.U))
                {
                    startUpgrade();
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log(manpower);
                    slider.value += 1f / manpower;
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    slider.value -= 1f / manpower;
                }
            }
        }

        // adjust the manpower to be controlled by a built in slider object
        sendAmount = (int)(slider.value * manpower);

        // determines whether the player should be shown the UI based on the team
        if (neutral || (redTeam != player.getTeam()))
        {
            playerUI.SetActive(false);
        }
        else
        {
            playerUI.SetActive(true);
        }
    }

    // show the potion UI screen
    public void interactPotion(bool potionParam)
    {
        if (!makingPotion)
        {
            if (potionParam)
            {
                potionScreen.GetComponent<Potion_UI>().setNode(node);
            }
            potionScreen.SetActive(potionParam);
        }
    }

    // start sending manpower
    public void startSend()
    {
        if (sendAmount > 0)
        {
            player.selectNodesToSend(sendAmount);
        }
        else
        {
            player.cancelSend();
        }
    }

    public void startUpgrade()
    {
        if (node.upgradeManpower())
        {
            if (player.isSinglePlayer())
            {
                node.modifyManPower(node.moreExpensiveUpgrades(), false, redTeam);
            }
            else
            {
                playerView.RPC("modifyManpower", RpcTarget.AllBuffered, node.name, node.moreExpensiveUpgrades(), false, redTeam);
            }
            upgradeCost = node.getLevelUpCost();
        }
        else
        {
            Debug.Log("purchase failed");
        }
    }

    public void instantiateValues(Node nodeArg, Player playerArg, bool redArg, GameObject potionArg)
    {
        potionScreen = potionArg;
        redTeam = redArg;
        node = nodeArg;
        player = playerArg;
        playerView = player.GetComponent<PhotonView>();
        hasNode = true;
    }
}
