using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviour
{
    //this script with be local to each player

    //showing refers to when the UI is shown, hiding is true when the UI is in the process of hiding and redTeam shows what team the player is in via a single variable
    [SerializeField]
    private bool showing, hiding, redTeam, sending, isSingle, canInteract = true, tutorialNodeRoot, canPotion, waitPotionUI, waitPotionMake, waitPotionCreated;

    private int sendManPower;

    [SerializeField]
    private GameObject node, UI_Prefab, potion_UI, current_UI, selectedNode, army;

    public List<GameObject> validNodes;

    private Manager manager;

    [SerializeField]
    private Canvas UI_Canvas;

    private MovingCam cam;

    private PhotonView view;

    [SerializeField]
    private Tutorial tutorial;

    // Start is called before the first frame update
    void Start()
    {
        // checks if the player is playing singleplayer move
        isSingle = PlayerPrefs.GetInt("SINGLE") == 1;
        view = GetComponent<PhotonView>();
        if (view)
        {
            if (!isSingle && view.IsMine)
            {
                redTeam = false;
            }
            else
            {
                redTeam = true;
            }
        }
        manager = GameObject.FindGameObjectWithTag("MANAGER").GetComponent<Manager>();
        cam = GameObject.FindGameObjectWithTag("CAMHOLDER").GetComponent<MovingCam>();
        UI_Canvas = GameObject.FindGameObjectWithTag("PLAYERUI").GetComponent<Canvas>();
    }

    //this is to show the UI on node properties for one player
    public IEnumerator showUI()
    {
        if (!hiding)
        {
            if (showing)
            {
                StartCoroutine(hideUI());
                yield return new WaitForSeconds(0.3f);
            }
            showing = true;
            current_UI = Instantiate(UI_Prefab, UI_Canvas.transform);
            RectTransform canvasRect = UI_Canvas.GetComponent<RectTransform>();
            float canvasHeight = canvasRect.rect.height;
            Debug.Log(node);
            RectTransform rect = current_UI.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector3(0, -canvasHeight * 1.5f);
            current_UI.GetComponent<NodeInfoUI>().instantiateValues(node.GetComponent<Node>(), this, redTeam, potion_UI);
            LeanTween.cancelAll();
            rect.LeanMoveLocalY(-canvasHeight * 0.325f, 1).setEaseOutExpo();
            yield return new WaitForSeconds(0.5f);
        }
    }

    public IEnumerator hideUI()
    {
        LeanTween.cancelAll();
        hiding = true;
        RectTransform rect = current_UI.GetComponent<RectTransform>();
        RectTransform canvasRect = UI_Canvas.GetComponent<RectTransform>();
        float canvasHeight = canvasRect.rect.height;
        rect.LeanMoveLocalY(-canvasHeight * 2, 0.25f);
        yield return new WaitForSeconds(0.25f);
        hiding = false;
        showing = false;
        Destroy(current_UI, 0.5f);
    }

    // this selects and displays all valid nodes
    public void selectNodesToSend(int manPower)
    {
        sendManPower = manPower;
        sending = true;
        selectedNode = node;
        validNodes = selectedNode.GetComponent<Node>().returnAllNeigbours(new List<GameObject>(), redTeam);
        validNodes.RemoveAll(obj => obj == selectedNode);
        foreach (GameObject nodeArg in validNodes)
        {
            nodeArg.GetComponent<Node>().showShadow(true);
        }
        StartCoroutine(hideUI());
    }

    // this is to cancel sending
    public void cancelSend()
    {
        node.GetComponent<Node>().spinShow(false);
        selectedNode.GetComponent<Node>().spinShow(false);
        foreach (GameObject nodeArg in validNodes)
        {
            nodeArg.GetComponent<Node>().showShadow(false);
        }
        sending = false;
        sendManPower = 0;
        selectedNode = null;
        node = null;
        validNodes.Clear();
    }

    public void reselect(List<GameObject> neighbours)
    {
        if (sending)
        {
            foreach (GameObject neighbour in neighbours)
            {
                if (!validNodes.Contains(neighbour) && neighbour != selectedNode)
                {
                    validNodes.Add(neighbour);
                    neighbour.GetComponent<Node>().showShadow(true);
                }
            }
        }
    }

    public bool isSinglePlayer()
    {
        return isSingle;
    }

    public bool returnPlayerInteract()
    {
        return canInteract;
    }

    public void setPotionInteract(bool potionParam)
    {
        canPotion = potionParam;
    }

    public bool canPotionInteract()
    {
        return canPotion;
    }

    public void startWaitPotion()
    {
        waitPotionUI = true;
    }

    public void startWaitPotionMake()
    {
        waitPotionMake = true;
    }

    public bool returnPotionMake()
    {
        return waitPotionMake;
    }

    public void madePotion()
    {
        tutorial.potionMake();
    }

    public void finalPotion()
    {
        tutorial.potionMade();
    }

    public void UI_interacted()
    {
        if (waitPotionUI)
        {
            Debug.Log("interact");
            tutorial.potionUI();
        }
    }

    //this is run when the background is clicked
    public void emptySpace()
    {
        if (canInteract)
        {
            if (node && !sending)
            {
                node.GetComponent<Node>().spinShow(false);
            }
            node = null;
            if (showing)
            {
                showing = false;
                StartCoroutine(hideUI());
            }
        }
    }

    //this is run when a node is clicked
    public void onNodeClicked(GameObject nodeArg)
    {
        if (canInteract)
        {
            //if the nodeArg is equal to the selected node
            if (node == nodeArg)
            {
                node.GetComponent<Node>().spinShow(false);
                showing = false;
                if (sending)
                {
                    cancelSend();
                }
                node = null;
                if (current_UI)
                {
                    StartCoroutine(hideUI());
                }
            }
            else
            {
                if (nodeArg.GetComponent<Node>().getType() == "root node" && nodeArg.GetComponent<Node>().sameTeam(redTeam) && tutorialNodeRoot)
                {
                    tutorial.rootClicked();
                }
                // if there is a node currently being selected
                if (node)
                {
                    node.GetComponent<Node>().spinShow(false);
                }
                Debug.Log("show");
                node = nodeArg;
                node.GetComponent<Node>().spinShow(true);
                cam.addPosition(node.transform.position);
                StartCoroutine(showUI());
            }
        }
    }

    public void setInteract(bool can)
    {
        canInteract = can;
    }

    public void gameEnded()
    {
        if (showing)
        {
            StartCoroutine(hideUI());
        }
    }

    public bool getTeam()
    {
        return redTeam;
    }

    public bool isTutorial()
    {
        return tutorial;
    }

    public void chooseNode(string nodeName)
    {
        GameObject nodeArg = GameObject.Find(nodeName);
        if (validNodes.Contains(nodeArg))
        {
            if (isSingle)
            {
                sendArmy(selectedNode.name, nodeName, sendManPower, redTeam, isSingle, false, selectedNode.GetComponent<Node>().usePotion());
            }
            else
            {
                view.RPC("sendArmy", RpcTarget.AllBuffered, selectedNode.name, nodeName, sendManPower, redTeam, isSingle, false, selectedNode.GetComponent<Node>().usePotion());
            }
        }
    }

    public void producePotionPre(string nodeName, float duration)
    {
        if (isSingle)
        {
            productionPotion(nodeName, duration);
        }
        else
        {
            view.RPC("productionPotion", RpcTarget.AllBuffered, nodeName, duration);
        }
    }

    public void poisonPotionPre(string nodeName, int duration, bool team)
    {
        if (isSingle)
        {
            poisonPotion(nodeName, duration, team);
        }
        else if(team = redTeam)
        {
            view.RPC("poisonPotion", RpcTarget.AllBuffered, nodeName, duration, team);
        }
    }


    [PunRPC]
    public void sendArmy(string selectedNodeParam, string nodeName, int sendManPowerParam, bool redTeamParam, bool singleParam, bool bossParam, string potionParam)
    {
        // prevents armies from being sent when the player wins
        if (!manager.hasWon())
        {
            GameObject sendArmy = Instantiate(army);
            GameObject nodeActual = GameObject.Find(selectedNodeParam);
            int tempSendManPower = sendManPowerParam;
            // ensures a singleplayer enemy doesn't follow this rule
            if(sendManPowerParam > nodeActual.GetComponent<Node>().returnManpower() && !(isSingle && nodeActual.GetComponent<Node>().sameTeam(false)))
            {
                tempSendManPower = nodeActual.GetComponent<Node>().returnManpower();
            }
            sendArmy.transform.position = nodeActual.transform.position;
            sendArmy.GetComponent<Army>().assignValues(selectedNodeParam, nodeName, tempSendManPower, redTeamParam, potionParam, singleParam);
            if (!bossParam)
            {
                if (isSingle)
                {
                    selectedNode.GetComponent<Node>().modifyManPower(tempSendManPower, false, redTeam);
                    cancelSend();
                }
                else if (redTeam == redTeamParam)
                {
                    view.RPC("modifyManpower", RpcTarget.AllBuffered, selectedNodeParam, tempSendManPower, false, redTeamParam);
                    cancelSend();
                }
            }
        }
    }

    public void readyClickRootNode()
    {
        tutorialNodeRoot = true;
    }

    [PunRPC]
    public void modifyManpower(string nodeName, int amount, bool add, bool red)
    {
        GameObject node = GameObject.Find(nodeName);
        node.GetComponent<Node>().modifyManPower(amount, add, red);
    }

    [PunRPC]
    public void changeNodeState(string nodeName, string state)
    {
        GameObject node = GameObject.Find(nodeName);
        node.GetComponent<Node>().changeState(state);
    }

    [PunRPC]
    public void increaseProductionLevel(string nodeName)
    {
        GameObject node = GameObject.Find(nodeName);
        node.GetComponent<Node>().increaseProductionLevel();
    }

    [PunRPC]
    public void productionPotion(string nodeName, float duration)
    {
        GameObject node = GameObject.Find(nodeName);
        StartCoroutine(node.GetComponent<Node>().tempIncreaseProduction(duration));
    }

    [PunRPC]
    public void poisonPotion(string nodeName, int duration, bool redTeam)
    {
        GameObject node = GameObject.Find(nodeName);
        if (!(node.GetComponent<Node>().sameTeam(redTeam) && node.GetComponent<Node>().isNeutral()))
        {
            StartCoroutine(node.GetComponent<Node>().poisonLoop(duration, redTeam));
        }
    }
}