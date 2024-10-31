using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int rootStartIndex = 0, winAmount;

    [SerializeField]
    private int[] neighbourCount, neigbours, distances;

    [SerializeField]
    private float timer, moneySpeed;

    [SerializeField]
    private bool won, tutorialBoss, moneyFluctuate;

    [SerializeField]
    private Vector2[] spawnPositions;

    public List<GameObject> nodes, edgesList;

    [SerializeField]
    private GameObject edge, node, player, winScreen, whiteScreen;

    [SerializeField]
    private Transform map;

    [SerializeField]
    private BossBehaviour boss;

    [SerializeField]
    private CanvasGroup winAlpha;

    [SerializeField]
    private TextMeshProUGUI winText, winMoney;

    private PhotonView view;

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        player = GameObject.FindGameObjectWithTag("PLAYER");
        spawnNodes();
    }

    private void Update()
    {
        if (!won)
        {
            timer += Time.deltaTime;
        }
    }

    [PunRPC]
    public void spawnNodes()
    {
        bool isSingle = player.GetComponent<Player>().isSinglePlayer();
        PhotonView playerView = player.GetComponent<PhotonView>();
        // firstly it spawn the nodes onto the map at the specified positions and sets its parent to the map
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            GameObject tempNode = Instantiate(node, map);
            tempNode.transform.position = spawnPositions[i];
            tempNode.name = "node" + i.ToString();
            nodes.Add(tempNode);
            if (i == rootStartIndex)
            {
                if (isSingle)
                {
                    tempNode.GetComponent<Node>().changeState("blue");
                    tempNode.GetComponent<Node>().changeState("boss");
                }
                else if (view.IsMine)
                {
                    playerView.RPC("changeNodeState", RpcTarget.AllBuffered, tempNode.name, "blue");
                    playerView.RPC("changeNodeState", RpcTarget.AllBuffered, tempNode.name, "root");
                }
            }
            else if (i == spawnPositions.Length - 1)
            {
                if (isSingle)
                {
                    tempNode.GetComponent<Node>().changeState("red");
                    tempNode.GetComponent<Node>().changeState("root");
                }
                else if (view.IsMine)
                {
                    playerView.RPC("changeNodeState", RpcTarget.AllBuffered, tempNode.name, "red");
                    playerView.RPC("changeNodeState", RpcTarget.AllBuffered, tempNode.name, "root");
                }
            }
        }
        //then it assigns the neighbours to each of the nodes and creates paths between them
        int currentNeighbour = 0;
        for(int i = 0; i < nodes.Count; i++)
        {
            for(int x = 0 ; x < neighbourCount[i]; x++)
            {
                GameObject tempPath;
                Vector2 startPoint = nodes[i].transform.position;
                Vector2 endPoint = nodes[neigbours[currentNeighbour]].transform.position;
                tempPath = Instantiate(edge);
                tempPath.name = "path" + (x + i).ToString();
                tempPath.transform.position = Vector3.zero;
                //set the starting and end point of the line so it connects the two ndoes
                tempPath.GetComponent<edges>().setLine(startPoint, endPoint);
                //set the distance the line represents in the edges script
                tempPath.GetComponent<edges>().setDistance(distances[currentNeighbour]);
                //add the neighbour node into the neighbour list of the node it is referencing.
                nodes[i].GetComponent<Node>().addNeighbour(nodes[neigbours[currentNeighbour]], tempPath, distances[currentNeighbour], true);
                currentNeighbour++;
                edgesList.Add(tempPath);
            }
        }
    }

    // This function returns a list of tuples with the first variable being the node and the second being the distance to said node, this selects the optimal path via dijkstra's algorithm
    // it takes the starting node and target node as parameters
    public List<(GameObject, int)> d_Algorithm(GameObject start, GameObject target, bool redTeamParam)
    {
        // the list of nodes for the optimal path
        List<GameObject> path = new List<GameObject>();

        // a dictionary containing the node as the key and the distance as the value so the shortest distance to a specific node can be found
        Dictionary<GameObject, int> distances = new Dictionary<GameObject, int>();

        // a dictionary containing the current node as the key and the previous node to reaching that node as the value
        Dictionary<GameObject, GameObject> previous = new Dictionary<GameObject, GameObject>();

        // a HashSet to keep track of all visited nodes
        HashSet<GameObject> visited = new HashSet<GameObject>();

        // the final output which adds the information of distance to the path
        List<(GameObject, int)> output = new List<(GameObject, int)>();

        foreach (GameObject node in nodes)
        {
            distances[node] = int.MaxValue;
            previous[node] = null;
        }

        distances[start] = 0;

        // a priority queue which sorts nodes based on distances as they are appended and focuses on nodes with least distance
        SortedSet<GameObject> queue = new SortedSet<GameObject>(Comparer<GameObject>.Create((a, b) =>
        {
            //checks the path with a shorter distance
            int thing = distances[a].CompareTo(distances[b]);

            //checks if they are the same gameobject so even if they have the same key value, they are considered to be different
            return thing != 0 ? thing : a.GetHashCode().CompareTo(b.GetHashCode());
        }));

        queue.Add(start);

        // a while loop to find the optimal path by checking the shortest distance to the next node until the target is reached
        while (queue.Count > 0)
        {
            // Get the node with the smallest distance and mark it as visited
            GameObject currentNode = queue.Min;
            queue.Remove(currentNode);
            visited.Add(currentNode);

            // If the target node is reached, reconstruct the path and return it
            if (currentNode == target)
            {
                GameObject node = target;
                while (node != null)
                {
                    path.Insert(0, node);
                    node = previous[node];
                }
                int prevTime = 0;
                for(int i = 1; i < path.Count; i++)
                {
                    output.Add((path[i], distances[path[i]] - prevTime));
                    prevTime = distances[path[i]];
                }
                return output;
            }

            // iterate through each of the neighbours to find the next node
            Node nodeScript = currentNode.GetComponent<Node>();
            for(int i = 0; i < nodeScript.neighbours.Count; i++)
            {
                // Skip visited nodes and nodes not on the same team (except the target node)
                if (visited.Contains(nodeScript.neighbours[i]) || (!nodeScript.sameTeam(redTeamParam) && nodeScript.gameObject != target))
                {
                    continue;
                }

                int distanceToNeighbour = distances[currentNode] + nodeScript.distances[i];

                if (distanceToNeighbour < distances[nodeScript.neighbours[i]])
                {
                    distances[nodeScript.neighbours[i]] = distanceToNeighbour;
                    previous[nodeScript.neighbours[i]] = currentNode;

                    bool willAdd = true;
                    foreach(GameObject element in queue)
                    {
                        if(element == nodeScript.neighbours[i])
                        {
                            willAdd = false;
                            break;
                        }
                    }
                    if (willAdd)
                    {
                        queue.Add(nodeScript.neighbours[i]);
                    }
                }
            }
        }

        for (int i = 1; i < path.Count; i++)
        {
            output.Add((path[i], distances[path[i]]));
        }
        return output;
    }

    // method activates when one player wins
    public void win(bool redTeamParam)
    {
        player.GetComponent<Player>().gameEnded();
        won = true;
        StartCoroutine(winCoroutine(redTeamParam == player.GetComponent<Player>().getTeam()));
    }

    public bool hasWon()
    {
        return won;
    }

    IEnumerator winCoroutine(bool win)
    {
        int winAmountTemp = 0;
        if (tutorialBoss)
        {
            if(PlayerPrefs.GetInt("TUTORIAL") != 1)
            {
                PlayerPrefs.SetInt("TUTORIAL", 1);
                winAmountTemp = 100;
                moneyFluctuate = false;
            }
        }
        if (moneyFluctuate)
        {
            winAmountTemp = Mathf.RoundToInt(winAmount * Random.Range(0.75f, 1.5f));
        }
        won = true;
        // shows and hides a white screen for dramatic effect
        whiteScreen.SetActive(true);
        float duration = 2f, elapsedTime = 0f;
        while(elapsedTime < duration)
        {
            winAlpha.alpha = elapsedTime / duration;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // finds the camera and locks it
        GameObject.FindGameObjectWithTag("CAMHOLDER").GetComponent<MovingCam>().returnToDefault();
        winScreen.SetActive(true);
        // adjusts the text so it displays a message for the winning/losing player
        if (win)
        {
            PlayerPrefs.SetInt("MONEY", PlayerPrefs.GetInt("MONEY") + winAmount);
            winText.text = "YOU WIN";
        }
        else
        {
            // do not show gain in money for losing team as they won't gain money
            winText.text = "DEFEATED";
        }
        duration /= 2;
        elapsedTime = 0;
        while (elapsedTime < duration)
        {
            winAlpha.alpha = 1 - elapsedTime / duration;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        whiteScreen.SetActive(false);
        if (win && winAmountTemp > 0)
        {
            winMoney.gameObject.SetActive(true);
            float showAmount = 0;
            while(showAmount < winAmountTemp)
            {
                winMoney.text = "+" + Mathf.RoundToInt(showAmount) + "$";
                showAmount += Time.deltaTime * moneySpeed;
                yield return null;
            }
        }
    }

    public void turnPathGold()
    {
        int randoIndex = Random.Range(0, edgesList.Count);
        edgesList[randoIndex].GetComponent<edges>().turnGold();

        // if the difficulty is 2 or above, create 2 golden paths at once
        if (PlayerPrefs.GetInt("DIFFICULTY") >= 2)
        {
            int randoIndex2 = Random.Range(0, edgesList.Count);
            while (randoIndex == randoIndex2)
            {
                randoIndex2 = Random.Range(0, edgesList.Count);
            }
            edgesList[randoIndex2].GetComponent<edges>().turnGold();
        }
    }

    public Node returnRandomValidNode()
    {
        return null;
    }

    public void returnMenu()
    {
        if (player.GetComponent<Player>().isSinglePlayer())
        {
            SceneManager.LoadScene("Menu");
        }
        else
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public void enterStage(string stageName)
    {
        SceneManager.LoadScene(stageName);
    }

    public void tutorialSequence(int index)
    {
        GameObject.FindGameObjectWithTag("TUTORIAL").GetComponent<Tutorial>().startTutorial(index);
    }

    public void makeRoyalNodes()
    {
        foreach (GameObject node in nodes)
        {
            Node nodeScript = node.GetComponent<Node>();
            if (nodeScript.getType() == "boss")
            {
                nodeScript.changeState("production");
                nodeScript.setKnightStrength(20, 10, 25);
                nodeScript.createKnight();
            }
            else if (nodeScript.getType() == "knight")
            {
                nodeScript.setKnightStrength(10, 10, 15);
                nodeScript.createKnight();
            }
            else if (nodeScript.isNeutral() || nodeScript.sameTeam(false))
            {
                nodeScript.setKnightStrength(10, 10, 15);
                nodeScript.changeState("blue");
                nodeScript.changeState("knight");
                nodeScript.modifyManPower(50, true, false);
                nodeScript.createKnight();
            }
        }
    }

    public void startTutorialBattle()
    {
        Node bossNode = boss.returnRootNode();
        bossNode.startTutorialFight();
        tutorialBoss = true;
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Menu");
    }
}
