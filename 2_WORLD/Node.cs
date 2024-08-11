using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    [SerializeField]
    private int manPower, productionLevel = 1, levelUpCost;

    private float potionCounter, potionTime;

    [SerializeField]
    private bool isRoot, productionNode, redTeam, neutral, producing, selecting, hasPotion, makingPotion;

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

    // Start is called before the first frame update
    void Start()
    {
        nodeCanvas.worldCamera = Camera.main;
        player = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Player>();
        manager = GameObject.FindGameObjectWithTag("MANAGER").GetComponent<Manager>();
        manPowerText = GetComponentInChildren<TextMeshProUGUI>();
        if (!player.getTeam())
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
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
        else if(makingPotion)
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
            modifyManPower(1, true, redTeam);
            yield return new WaitForSeconds(1f / productionLevel);
            StartCoroutine(productionCoroutine());
        }
    }

    //this is to reset the node back to default
    public void resetNode()
    {
        GetComponent<SpriteRenderer>().color = baseColor;
        hasPotion = false;
        neutral = true;
        productionNode = false;
        productionLevel = 1;
        if(manPower < 0)
        {
            manPower *= -1;
        }
        if (isRoot)
        {
            //the only scenario at which is root node will be resetted is if it is taken over of. So this can act as a criteria for victory
            manager.win();
        }
    }

    //this is to chance the state of the node
    public void changeState(string changeThing)
    {
        SpriteRenderer selfRenderer = GetComponent<SpriteRenderer>();
        switch (changeThing)
        {
            case "neutral":
                //neutral nodes are not occupied by either players
                selfRenderer.color = baseColor;
                resetNode();
                break;
            case "production":
                //production nodes have a blue background and produce manpower
                productionNode = true;
                selfRenderer.color = productionColor;
                break;
            case "red":
                //red nodes are nodes occupied by the red team
                resetNode();
                neutral = false;
                redTeam = true;
                renderer.sprite = redSprite;
                break;
            case "blue":
                //blue nodes are nodes occupied by the blue team
                resetNode();
                neutral = false;
                redTeam = false;
                renderer.sprite = blueSprite;
                break;
            case "root":
                //root nodes produce manpower and are the most important nodes which determine victory
                selfRenderer.color = rootColor;
                levelUpCost *= 2;
                isRoot = true;
                productionNode = true;
                break;
        }
    }

    //this is to increase or decrease manpower on the node
    public void modifyManPower(int amount, bool add, bool red)
    {
        if (neutral || (redTeam == red && add))
        {
            manPower += amount;
            if (neutral)
            {
                player.reselect(neighbours);
                if (red)
                {
                    changeState("red");
                }
                else if (!red && (redTeam || neutral))
                {
                    changeState("blue");
                }
            }
        }
        else
        {
            manPower -= amount;
            if (manPower < 0)
            {
                if (redTeam)
                {
                    changeState("blue");
                }
                else
                {
                    changeState("red");
                }
            }
        }
        manPowerText.text = manPower.ToString();
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
            foreach(GameObject neighbour in neighbours)
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
        //transform.localScale *= 0.9f;
        clickSprite.SetActive(true);

        // if the node is being selected for sending an army to it, call a function in the player script to do so
        if (selecting)
        {
            player.chooseNode(gameObject);
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
                productionLevel++;
            }
            else
            {
                changeState("production");
            }
            return true;
        }
        else
        {
            return false;
        }
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
        if (isRoot)
        {
            return "root node";
        }
        if (producing)
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

    public bool sameTeam()
    {
        if (neutral)
        {
            return false;
        }
        else
        {
            return redTeam == player.getTeam();
        }
    }

    public void spinShow(bool willShow)
    {
        spinner.SetActive(willShow);
    }
}
