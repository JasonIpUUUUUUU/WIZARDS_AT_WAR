using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using EZCameraShake;

public class Node : MonoBehaviour
{
    [SerializeField]
    private int manPower, productionLevel = 1, levelUpCost, initialCost, knightStrength;

    private float potionCounter, potionTime, knightMin, knightMax;

    [SerializeField]
    private BossBehaviour bossScript;

    [SerializeField]
    private bool isRoot, productionNode, redTeam, neutral, producing, selecting, hasPotion, makingPotion, knight, phase2, isBoss, blackout, blackHole, meteoring, astroNode;

    private string potion;

    [SerializeField]
    Sprite redSprite, blueSprite, neutralSprite;

    [SerializeField]
    Canvas nodeCanvas;

    [SerializeField]
    private SpriteRenderer renderer;

    private TextMeshProUGUI manPowerText;

    private Manager manager;

    [SerializeField]
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
    private GameObject clickSprite, selectSprite, spinner, potionBarObject, potionVisual, redVisual, meteor, meteorEffect;

    private PhotonView playerView;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(PlayerPrefs.GetInt("SINGLE"));
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
            if (!blackout)
            {
                potionBar.fillAmount = returnPotionFill();
                potionCounter -= Time.deltaTime;
            }
        }
        else if (makingPotion)
        {
            potionBarObject.SetActive(false);
            makingPotion = false;
            hasPotion = true;
            potionVisual.SetActive(true);
            if (player.isTutorial())
            {
                player.finalPotion();
            }
        }
    }

    public IEnumerator blackOutCoroutine(int duration)
    {
        blackout = true;
        yield return new WaitForSeconds(duration);
        blackout = false;
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
            while (blackout)
            {
                yield return null;
            }
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

    public bool returnAstro()
    {
        return astroNode;
    }

    public bool returnBlackHole()
    {
        return blackHole;
    }

    public void setManpower(int manpowerP)
    {
        manPower = manpowerP;
    }

    //this is to increase or decrease manpower on the node
    public void modifyManPower(int amount, bool add, bool red, bool transformed = false)
    {
        if (neutral || (redTeam == red && add))
        {
            manPower += amount;
            if (neutral)
            {
                player = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Player>();
                if (player.getTeam() == redTeam)
                {
                    player.reselect(neighbours);
                }
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
                if (bossScript.returnBoss() == "ELECTRO")
                {
                    // stats of faster enemies
                    StartCoroutine(electroArmies(10, true));
                    // stats of slower enemies
                    StartCoroutine(electroArmies(20, false));
                }
                // boss nodes start with a set amount of health and have to be defeated by the player in singleplayer
                selfRenderer.color = rootColor;
                // increases manpower by the health of the boss and gives a reference of this script to the boss
                modifyManPower(bossScript.linkNode(this), true, redTeam);
                isRoot = true;
                break;
            case "boss2":
                // phase 2 of each boss
                if (bossScript.returnBoss() == "ELECTRO")
                {
                    // stats of faster enemies
                    StartCoroutine(electroArmies(10, true));
                    // stats of slower enemies
                    StartCoroutine(electroArmies(20, false));
                }
                if (bossScript.returnBoss() == "SPACE")
                {
                    isRoot = false;
                    isBoss = false;
                    blackHole = true;
                    manPower = 0;
                    manager.setAstroBoss();
                }
                else
                {
                    manPower = bossScript.returnMaxHealth();
                }
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
            if (manager.returnRage())
            {
                yield return new WaitForSeconds(1.5f);
                createKnight();
            }
            StartCoroutine(knightLoop());
        }
    }

    IEnumerator electroArmies(int size, bool fast)
    {
        float randoTime = Random.Range(5f, 10f);
        yield return new WaitForSeconds(randoTime);
        createElectro(size, fast);
        StartCoroutine(electroArmies(size, fast));
    }

    public void createElectro(int size, bool fast)
    {
        int tempSize = size;
        string effect = "";
        if (fast)
        {
            effect = "ELECTROSPEED";
        }
        if (!fast)
        {
            tempSize += (PlayerPrefs.GetInt("DIFFICULTY") - 1) * 5;
        }
        player.sendArmy(name, returnRandomNeigbour(new List<GameObject>(), false).name, tempSize, false, true, true, effect);
    }

    public void createKnight()
    {
        int tempStrength = knightStrength;
        if (PlayerPrefs.GetInt("DIFFICULTY") == 3)
        {
            tempStrength += Mathf.RoundToInt(manPower * 0.1f);
        }
        player.sendArmy(name, returnRandomNeigbour(new List<GameObject>(), false).name, tempStrength, false, true, true, "");
    }

    public void setKnightStrength(int strength, float knightMinP, float knightMaxP)
    {
        // so the boss itself produces manpower differently to normal nodes
        knightStrength = strength;
        knightMin = knightMinP;
        knightMax = knightMaxP;
    }

    public GameObject returnRandomNeigbour(List<GameObject> initialList, bool playerTeam, GameObject startingNode = null)
    {
        Stack<GameObject> nodeStack = new Stack<GameObject>();
        nodeStack.Push(gameObject); 
        List<GameObject> visitedNodes = new List<GameObject>(initialList);
        visitedNodes.Add(gameObject); 
        GameObject returnObject = gameObject;

        while (nodeStack.Count > 0)
        {
            GameObject currentNode = nodeStack.Pop();
            Node currentNodeComponent = currentNode.GetComponent<Node>();

            // Check if the current node is valid for exploration
            if (!currentNodeComponent.neutral && (playerTeam == currentNodeComponent.redTeam || currentNode == startingNode))
            {
                List<GameObject> validNeighbours = new List<GameObject>();
                foreach (GameObject neighbour in currentNodeComponent.neighbours)
                {
                    Node neighbourNode = neighbour.GetComponent<Node>();
                    if (!visitedNodes.Contains(neighbour) && !(neighbourNode.getType() == "root node" && neighbourNode.sameTeam(currentNodeComponent.redTeam)) && neighbourNode.isActiveAndEnabled)
                    {
                        validNeighbours.Add(neighbour);
                    }
                }

                // If there are valid neighbours, pick one randomly
                if (validNeighbours.Count > 0)
                {
                    GameObject nextNode = validNeighbours[Random.Range(0, validNeighbours.Count)];
                    visitedNodes.Add(nextNode);
                    nodeStack.Push(nextNode);  
                    returnObject = nextNode;  
                }
                else
                {
                    nodeStack.Clear();
                }
            }
            else
            {
                visitedNodes.Add(currentNode);
            }
        }

        Debug.Log($"GOING FROM: {name}, TO: {returnObject.name}");
        return returnObject;
    }



    // a version exclusively for the tutorial to prevent the palyer from being beaten
    public GameObject returnRandomNeigbourTutorial(List<GameObject> initialList, bool playerTeam)
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
                // if it hasn't been explored previously, prevent armies from sending manpower to the root node
                if (!initialList.Contains(neighbour) && neighbour.GetComponent<Node>().getType() != "root node" )
                {
                    checkObjects.Add(neighbour);
                }
            }
            // once a list of possible nodes is created, select a random one and explore it
            if (checkObjects.Count > 0)
            {
                Node nextCheck = checkObjects[Random.Range(0, checkObjects.Count)].GetComponent<Node>();
                everything.Add(nextCheck.gameObject);
                returnObject = nextCheck.returnRandomNeigbourTutorial(everything, playerTeam);
            }
        }
        return returnObject;
    }

    public IEnumerator poisonLoop(int damage, bool team)
    {
        for (int i = 0; i < damage; i++)
        {
            if(redTeam != team)
            {
                modifyManPower(1, false, !redTeam);
            }
            yield return new WaitForSeconds(1);
        }
    }

    public void startTutorialFight()
    {
        StartCoroutine(tutorialLoop());
    }

    IEnumerator tutorialLoop()
    {
        player.sendArmy(name, returnRandomNeigbourTutorial(new List<GameObject>(), false).name, 1, false, true, true, "");
        yield return new WaitForSeconds(2);
        StartCoroutine(tutorialLoop());
    }

    public IEnumerator fireMeteor(bool phase2)
    {
        StartCoroutine(fadeMeteor());
        meteoring = true;
        yield return new WaitForSeconds(2);
        GameObject b = Instantiate(meteor);
        b.transform.position = new Vector3(100, 100);
        b.GetComponent<Meteor>().setMeteor(gameObject);
        yield return new WaitForSeconds(1);
        CameraShaker.Instance.ShakeOnce(10, 10, 0.1f, 1f);
        int meteorPower = 10 + (PlayerPrefs.GetInt("DIFFICULTY") - 1) * 5;
        modifyManPower(meteorPower, true, false);
        player.sendArmy(name, returnRandomNeigbour(new List<GameObject>(), false, gameObject).name, meteorPower, false, true, true, "", true, phase2);
        meteoring = true;
        GameObject effect = Instantiate(meteorEffect);
        effect.transform.position = transform.position;
        redVisual.SetActive(false);
    }

    IEnumerator fadeMeteor()
    {
        redVisual.SetActive(true);
        float elapsedTime = 0f;
        SpriteRenderer renderer = redVisual.GetComponent<SpriteRenderer>();
        Color color = renderer.color;
        color.a = 0;

        while (elapsedTime < 3)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / 3);
            renderer.color = color; 
            yield return null; 
        }

        color.a = 1f; 
        renderer.color = color;
    }

    public void hideNode()
    {
        astroNode = true;
        foreach (GameObject edge in paths)
        {
            edge.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    public IEnumerator showNode()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.color = rootColor;
        foreach (GameObject edge in paths)
        {
            edge.SetActive(true);
            edge.GetComponent<LineRenderer>().SetColors(new Color32(255, 255, 255, 0), new Color32(255, 255, 255, 0));
        }
        gameObject.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < 2)
        {
            elapsedTime += Time.deltaTime;
            byte alphaValue = (byte)(255 * elapsedTime / 2);
            Color currentColor = renderer.color;
            currentColor.a = alphaValue;
            renderer.color = currentColor;
            foreach(GameObject edge in paths)
            {
                edge.GetComponent<LineRenderer>().SetColors(new Color32(255, 255, 255, alphaValue), new Color32(255, 255, 255, alphaValue));
            }
            yield return null;
        }
        GameObject.FindGameObjectWithTag("CAMHOLDER").GetComponent<MovingCam>().addPosition(transform.position);
        renderer.color = rootColor;
    }

    public bool returnMeteor()
    {
        return meteoring;
    }
}
