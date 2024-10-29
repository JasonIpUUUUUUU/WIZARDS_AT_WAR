using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class Node : MonoBehaviour
{
    [SerializeField]
    private int manPower, productionLevel = 1, levelUpCost, initialCost, knightStrength;

    private float potionCounter, potionTime, knightMin, knightMax;

    private BossBehaviour bossScript;

    [SerializeField]
    private bool isRoot, productionNode, redTeam, neutral, producing, selecting, hasPotion, makingPotion, knight, phase2, isBoss;

    private string potion;

    [SerializeField]
    Sprite redSprite, blueSprite, neutralSprite;

    [SerializeField]
    Canvas nodeCanvas;

    [SerializeField]
    private SpriteRenderer renderer;

    private TextMeshProUGUI manPowerText;

    private Manager manager;

    private Player player;

    public List<int> distances;

    //lists have to be kept public to be modified for some reason
    public List<GameObject> neighbours, paths;

    [SerializeField]
    private Image potionBar;

    [SerializeField]
    private Color rootColor, productionColor, baseColor;

    //transparent dark sprites to signify different states
    [SerializeField]
    private GameObject clickSprite, selectSprite, spinner, potionBarObject, potionVisual;

    private PhotonView playerView;

    // Start is called before the first frame update
    void Start()
    {
        nodeCanvas.worldCamera = Camera.main;
        player = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Player>();
        playerView = player.GetComponent<PhotonView>();
        manager = GameObject.FindGameObjectWithTag("MANAGER").GetComponent<Manager>();
        manPowerText = GetComponentInChildren<TextMeshProUGUI>();
        if (!player.getTeam())
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
        if(PlayerPrefs.GetInt("SINGLE") == 1)
        {
            bossScript = GameObject.FindGameObjectWithTag("BOSS").GetComponent<BossBehaviour>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!producing && productionNode)
        {
            StartCoroutine(productionCoroutine());
        }

        if (potionCounter > 0)
        {
            potionBar.fillAmount = returnPotionFill();
            potionCounter -= Time.deltaTime;
        }
        else if (makingPotion)
        {
            potionBarObject.SetActive(false);
            makingPotion = false;
            hasPotion = true;
            potionVisual.SetActive(true);
        }
    }

    //this is an infinite loop where the manpower on the node increments every second
    IEnumerator productionCoroutine()
    {
        if (productionNode)
        {
            producing = true;
            if (player.isSinglePlayer())
            {
                modifyManPower(1, true, redTeam);
            }
            else if (playerView.IsMine)
            {
                playerView.RPC("modifyManpower", RpcTarget.AllBuffered, name, 1, true, redTeam);
            }
            yield return new WaitForSeconds(1f / productionLevel);
            StartCoroutine(productionCoroutine());
        }
    }

    //this is to reset the node back to default
    public void resetNode()
    {
        GetComponent<SpriteRenderer>().color = baseColor;
        knight = false;
        hasPotion = false;
        neutral = true;
        productionNode = false;
        producing = false;
        productionLevel = 1;
        levelUpCost = initialCost;
        if (manPower < 0)
        {
            manPower *= -1;
        }
        if (isRoot)
        {
            // the only scenario at which is root node will be resetted is if it is taken over of. So this can act as a criteria for victory
            // !redTeam is necessary as it's the opposite team who won
            manager.win(!redTeam);
        }
    }

    //this manages what counts as a neighbour to this node
    public void addNeighbour(GameObject neighbour, GameObject edge, int distance, bool addMore)
    {
        paths.Add(edge);
        neighbours.Add(neighbour);
        distances.Add(distance);
        if (addMore)
        {
            //it then adds itself as a node to it's neighbour if it hasn't already
            neighbour.GetComponent<Node>().addNeighbour(gameObject, edge, distance, false);
        }
    }

    //this returns true if the target gameobject is a neighbour
    public bool findNeighbour(GameObject target)
    {
        return neighbours.Contains(target);
    }

    public List<GameObject> returnAllNeigbours(List<GameObject> initialList, bool playerTeam)
    {
        List<GameObject> currentObjects = new List<GameObject> { gameObject };
        if (!neutral && playerTeam == redTeam)
        {
            foreach (GameObject neighbour in neighbours)
            {
                if (!initialList.Contains(neighbour))
                {
                    List<GameObject> everything = currentObjects;
                    everything.AddRange(initialList);
                    currentObjects.AddRange(neighbour.GetComponent<Node>().returnAllNeigbours(everything, playerTeam));
                }
            }
        }
        return currentObjects;
    }

    public void showShadow(bool doShow)
    {
        selecting = doShow;
        selectSprite.SetActive(doShow);
    }

    //a built in unity function which runs when the object is clicked
    private void OnMouseDown()
    {
        player.onNodeClicked(gameObject);
        clickSprite.SetActive(true);
        // if the node is being selected for sending an army to it, call a function in the player script to do so
        if (selecting)
        {
            player.chooseNode(name);
        }
    }

    //a built in unity function which runs when the object is let go of
    private void OnMouseUp()
    {
        //transform.localScale /= 0.9f;
        clickSprite.SetActive(false);
    }

    public bool upgradeManpower()
    {
        if (levelUpCost <= manPower && redTeam == player.getTeam())
        {
            if (productionNode)
            {
                if (player.isSinglePlayer())
                {
                    productionLevel++;
                }
                else
                {
                    playerView.RPC("increaseProductionLevel", RpcTarget.AllBuffered, name);
                }
            }
            else
            {
                if (player.isSinglePlayer())
                {
                    changeState("production");
                }
                else
                {
                    playerView.RPC("changeNodeState", RpcTarget.AllBuffered, name, "production");
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public void increaseProductionLevel()
    {
        productionLevel++;
    }

    public int moreExpensiveUpgrades()
    {
        int originalCost = levelUpCost;
        levelUpCost *= 5;
        return originalCost;
    }

    public int returnManpower()
    {
        return manPower;
    }

    public int getLevelUpCost()
    {
        return levelUpCost;
    }

    public bool isNeutral()
    {
        return neutral;
    }

    public string getType()
    {
        if (isBoss)
        {
            return "boss";
        }
        else if (isRoot)
        {
            return "root node";
        }
        else if (knight)
        {
            return "knight";
        }
        else if (producing)
        {
            return "production node";
        }
        return "node";
    }

    public float returnPotionFill()
    {
        if (makingPotion)
        {
            return potionCounter / potionTime;
        }
        else
        {
            return 0;
        }
    }

    public void addPotion(string potionArg, float potionTimeArg)
    {
        if (!hasPotion && !makingPotion)
        {
            potionBarObject.SetActive(true);
            potion = potionArg;
            potionTime = potionTimeArg;
            potionCounter = potionTime;
            makingPotion = true;
        }
    }

    public string usePotion()
    {
        if (hasPotion)
        {
            Debug.Log("USING " + potion);
            potionVisual.SetActive(false);
            hasPotion = false;
            return potion;
        }
        else
        {
            return "";
        }
    }

    public bool sameTeam(bool team)
    {
        if (neutral)
        {
            return false;
        }
        else
        {
            return redTeam == team;
        }
    }

    public void spinShow(bool willShow)
    {
        spinner.SetActive(willShow);
    }

    public edges returnPath(GameObject targetParam)
    {
        for(int i = 0; i < neighbours.Count; i++)
        {
            if(neighbours[i] == targetParam)
            {
                return paths[i].GetComponent<edges>();
            }
        }
        return null;
    }

    //this is to increase or decrease manpower on the node
    public void modifyManPower(int amount, bool add, bool red, bool transformed = false)
    {
        if (neutral || (redTeam == red && add))
        {
            manPower += amount;
            if (neutral)
            {
                player.reselect(neighbours);
                if (red)
                {
                    if (player.isSinglePlayer())
                    {
                        changeState("red");
                    }
                    else if(playerView.IsMine)
                    {
                        playerView.RPC("changeNodeState", RpcTarget.AllBuffered, name, "red");
                    }
                }
                else if (!red && (redTeam || neutral))
                {
                    if (player.isSinglePlayer())
                    {
                        changeState("blue");
                    }
                    else if (playerView.IsMine)
                    {
                        playerView.RPC("changeNodeState", RpcTarget.AllBuffered, name, "blue");
                    }
                }
            }
        }
        else
        {
            manPower -= amount;
            if (manPower < 0)
            {
                if(isBoss && !phase2)
                {
                    phase2 = true;
                    changeState("boss2");
                }
                else
                {
                    manPower *= -1;
                    if (redTeam)
                    {
                        if (player.isSinglePlayer())
                        {
                            changeState("blue");
                            if (transformed)
                            {
                                knightMin = 6;
                                knightMax = 20;
                                knightStrength = 10;
                                changeState("knight");
                            }
                        }
                        else if (playerView.IsMine)
                        {
                            playerView.RPC("changeNodeState", RpcTarget.AllBuffered, name, "blue");
                        }
                    }
                    else
                    {
                        if (player.isSinglePlayer())
                        {
                            changeState("red");
                        }
                        else if (playerView.IsMine)
                        {
                            playerView.RPC("changeNodeState", RpcTarget.AllBuffered, name, "red");
                        }
                    }
                }
            }
        }
        // in case manPowerText hasn't been obtained yet
        if (!manPowerText)
        {
            manPowerText = GetComponentInChildren<TextMeshProUGUI>();
        }
        manPowerText.text = manPower.ToString();
    }

    // for when the production potion is used
    public IEnumerator tempIncreaseProduction(float duration)
    {
        productionLevel++;
        yield return new WaitForSeconds(duration);
        Debug.Log("revert");
        productionLevel--;
    }

    //this is to chance the state of the node
    public void changeState(string changeThing)
    {
        SpriteRenderer selfRenderer = GetComponent<SpriteRenderer>();
        switch (changeThing)
        {
            case "neutral":
                // neutral nodes are not occupied by either players
                selfRenderer.color = baseColor;
                resetNode();
                break;
            case "production":
                // production nodes have a blue background and produce manpower
                productionNode = true;
                selfRenderer.color = productionColor;
                break;
            case "red":
                // red nodes are nodes occupied by the red team
                resetNode();
                neutral = false;
                redTeam = true;
                renderer.sprite = redSprite;
                break;
            case "blue":
                // blue nodes are nodes occupied by the blue team
                resetNode();
                neutral = false;
                redTeam = false;
                renderer.sprite = blueSprite;
                break;
            case "root":
                // root nodes produce manpower and are the most important nodes which determine victory
                selfRenderer.color = rootColor;
                levelUpCost = initialCost * 2;
                isRoot = true;
                productionNode = true;
                break;
            case "boss":
                bossScript = GameObject.FindGameObjectWithTag("BOSS").GetComponent<BossBehaviour>();
                if(bossScript.returnBoss() != "TUTORIAL")
                {
                    isBoss = true;
                }
                // boss nodes start with a set amount of health and have to be defeated by the player in singleplayer
                selfRenderer.color = rootColor;
                isRoot = true;
                // increases manpower by the health of the boss and gives a reference of this script to the boss
                modifyManPower(bossScript.linkNode(this), true, redTeam);
                break;
            case "boss2":
                // phase 2 of each boss
                manPower = bossScript.returnMaxHealth();
                bossScript.startPhase2();
                break;
            case "knight":
                knight = true;
                StartCoroutine(knightLoop());
                break;
        }
    }

    // a loop for if the node is a knight node (limited to royal wizard fight)
    IEnumerator knightLoop()
    {
        float randoTime = Random.Range(knightMin, knightMax);
        yield return new WaitForSeconds(randoTime);
        if (knight)
        {
            createKnight();
            StartCoroutine(knightLoop());
        }
    }

    public void setKnightStrength(int strength, float knightMinP, float knightMaxP)
    {
        // so the boss itself produces manpower differently to normal nodes
        knightStrength = strength;
        knightMin = knightMinP;
        knightMax = knightMaxP;
    }

    // returns a singular node which it can conquer, the node is randomly selected
    public GameObject returnRandomNeigbour(List<GameObject> initialList, bool playerTeam)
    {
        // define initial lists such as a list of of the nodes that have been explored, a list of the nodes which haven't been explored and a list of every node
        List<GameObject> currentObjects = new List<GameObject> { gameObject }, checkObjects = new List<GameObject>(), everything = currentObjects;
        everything.AddRange(initialList);

        // define the object to return
        GameObject returnObject = gameObject;

        if (!neutral && playerTeam == redTeam)
        {
            // loop through each neighbouring node and puts them into the checkObjects script so a random one can be selected
            foreach (GameObject neighbour in neighbours)
            {
                // if it hasn't been explored previously, add it into the list of possible nodes the explore and isn't a root of the same team as the the current node
                if (!initialList.Contains(neighbour) && !(neighbour.GetComponent<Node>().getType() == "root node" && neighbour.GetComponent<Node>().sameTeam(redTeam)))
                {
                    checkObjects.Add(neighbour);
                }
            }
            // once a list of possible nodes is created, select a random one and explore it
            if (checkObjects.Count > 0)
            {
                Node nextCheck = checkObjects[Random.Range(0, checkObjects.Count)].GetComponent<Node>();
                everything.Add(nextCheck.gameObject);
                returnObject = nextCheck.returnRandomNeigbour(everything, playerTeam);
            }
        }
        return returnObject;
    }

    public void createKnight()
    {
        int tempStrength = knightStrength;
        if (PlayerPrefs.GetInt("DIFFICULTY") == 3)
        {
            tempStrength += Mathf.RoundToInt(manPower * 0.1f);
        }
        player.sendArmy(name, returnRandomNeigbour(new List<GameObject>(), false).name, tempStrength, false, true, true);
    }
}
