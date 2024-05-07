using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //this script with be local to each player

    //showing refers to when the UI is shown, hiding is true when the UI is in the process of hiding and redTeam shows what team the player is in via a single variable
    [SerializeField]
    private bool showing, hiding, redTeam, sending;

    private int sendManPower;

    [SerializeField]
    private GameObject node, UI_Prefab, current_UI, selectedNode, army;

    public List<GameObject> validNodes;

    private Manager manager;

    [SerializeField]
    private Canvas UI_Canvas;

    private MovingCam cam;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("MANAGER").GetComponent<Manager>();
        cam = GameObject.FindGameObjectWithTag("CAMHOLDER").GetComponent<MovingCam>();
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
            current_UI.GetComponent<NodeInfoUI>().instantiateValues(node.GetComponent<Node>(), this);
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

    public void selectNodesToSend(int manPower)
    {
        sendManPower = manPower;
        sending = true;
        selectedNode = node;
        validNodes = selectedNode.GetComponent<Node>().returnAllNeigbours(new List<GameObject>());
        validNodes.Remove(selectedNode);
        foreach(GameObject nodeArg in validNodes)
        {
            nodeArg.GetComponent<Node>().showShadow(true);
        }
        StartCoroutine(hideUI());
    }

    public void chooseNode(GameObject nodeArg)
    {
        if (validNodes.Contains(nodeArg))
        {
            selectedNode.GetComponent<Node>().modifyManPower(sendManPower, false, redTeam);
            GameObject sendArmy = Instantiate(army);
            sendArmy.transform.position = selectedNode.transform.position;
            sendArmy.GetComponent<Army>().assingValues(selectedNode, nodeArg, manager, sendManPower, redTeam);
            cancelSend();
        }
    }

    public void cancelSend()
    {
        foreach (GameObject nodeArg in validNodes)
        {
            nodeArg.GetComponent<Node>().showShadow(false);
        }
        sendManPower = 0;
        selectedNode = null;
        validNodes.Clear();
    }

    //this is run when the background is clicked
    public void emptySpace()
    {
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
        if (node == nodeArg)
        {
            showing = false;
            StartCoroutine(hideUI());
        }
        else
        {
            node = nodeArg;
            cam.addPosition(node.transform.position);
            StartCoroutine(showUI());
        }
    }
}
