using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviour
{
    //this script with be local to each player

    //showing refers to when the UI is shown, hiding is true when the UI is in the process of hiding and redTeam shows what team the player is in via a single variable
    [SerializeField]
    private bool showing, hiding, redTeam, sending, isSingle;

    private int sendManPower;

    [SerializeField]
    private GameObject node, UI_Prefab, potion_UI, current_UI, selectedNode, army;

    public List<GameObject> validNodes;

    private Manager manager;

    [SerializeField]
    private Canvas UI_Canvas;

    private MovingCam cam;

    private PhotonView view;

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
                yield return new WaitForSeconds(0.25f);
            }
            showing = true;
            current_UI = Instantiate(UI_Prefab, UI_Canvas.transform);
            current_UI.transform.localPosition = new Vector3(0, -Screen.height * 1.5f);
            current_UI.GetComponent<NodeInfoUI>().instantiateValues(node.GetComponent<Node>(), this, redTeam, potion_UI);
            LeanTween.cancelAll();
            current_UI.LeanMoveLocalY(-Screen.height * 0.35f, 1).setEaseOutExpo();
            yield return new WaitForSeconds(0.5f);
        }
    }

    //this is to hide the UI
    public IEnumerator hideUI()
    {
        LeanTween.cancelAll();
        hiding = true;
        current_UI.LeanMoveLocalY(-Screen.height * 2, 0.25f);
        yield return new WaitForSeconds(0.25f);
        hiding = false;
        if (showing)
        {
            showing = false;
        }
        else
        {
            node = null;
        }
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
        selectedNode.GetComponent<Node>().spinShow(false);
        foreach (GameObject nodeArg in validNodes)
        {
            nodeArg.GetComponent<Node>().showShadow(false);
        }
        sending = false;
        sendManPower = 0;
        selectedNode = null;
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

    //this is run when the background is clicked
    public void emptySpace()
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

    //this is run when a node is clicked
    public void onNodeClicked(GameObject nodeArg)
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
            StartCoroutine(hideUI());
        }
        else
        {
            // if there is a node currently being selected
            if (node)
            {
                node.GetComponent<Node>().spinShow(false);
            }
            node = nodeArg;
            node.GetComponent<Node>().spinShow(true);
            cam.addPosition(node.transform.position);
            StartCoroutine(showUI());
        }
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

    public void chooseNode(string nodeName)
    {
        GameObject nodeArg = GameObject.Find(nodeName);
        if (validNodes.Contains(nodeArg))
        {
            if (isSingle)
            {
                sendArmy(selectedNode.name, nodeName, sendManPower, redTeam, isSingle, false);
            }
            else
            {
                view.RPC("sendArmy", RpcTarget.AllBuffered, selectedNode.name, nodeName, sendManPower, redTeam, isSingle, false);
            }
        }
    }

    [PunRPC]
    public void sendArmy(string selectedNodeParam, string nodeName, int sendManPowerParam, bool redTeamParam, bool singleParam, bool bossParam)
    {
        // prevents armies from being sent when the player wins
        if (!manager.hasWon())
        {
            GameObject sendArmy = Instantiate(army);
            GameObject nodeActual = GameObject.Find(selectedNodeParam);
            sendArmy.transform.position = nodeActual.transform.position;
            sendArmy.GetComponent<Army>().assignValues(selectedNodeParam, nodeName, sendManPowerParam, redTeamParam, nodeActual.GetComponent<Node>().usePotion(), singleParam);
            if (!bossParam)
            {
                if (isSingle)
                {
                    selectedNode.GetComponent<Node>().modifyManPower(sendManPower, false, redTeam);
                    cancelSend();
                }
                else if (view.IsMine)
                {
                    view.RPC("modifyManpower", RpcTarget.AllBuffered, selectedNodeParam, sendManPowerParam, false, redTeamParam);
                    cancelSend();
                }
            }
        }
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
}