using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Node : MonoBehaviour
{
    [SerializeField]
    private int manPower, potionLevel, productionLevel = 1, levelUpCost;

    [SerializeField]
    private bool isRoot, productionNode, potionNode, redTeam, neutral, producing, selecting;

    [SerializeField]
    Sprite redSprite, blueSprite, neutralSprite;

    [SerializeField]
    private SpriteRenderer renderer;

    private TextMeshProUGUI manPowerText;

    private Manager manager;

    private Player player;

    public List<int> distances;

    //lists have to be kept public to be modified
    public List<GameObject> neighbours, paths;

    //transparent dark sprites to signify different states
    [SerializeField]
    private GameObject clickSprite, selectSprite;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Player>();
        manager = GameObject.FindGameObjectWithTag("MANAGER").GetComponent<Manager>();
        manPowerText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!producing && productionNode)
        {
            StartCoroutine(productionCoroutine());
        }
    }

    //this is an infinite loop where the manpower on the node increments every second
    IEnumerator productionCoroutine()
    {
        producing = true;
        modifyManPower(1, true, redTeam);
        yield return new WaitForSeconds(1f/productionLevel);
        StartCoroutine(productionCoroutine());
    }

    //this is to reset the node back to default
    public void resetNode()
    {
        neutral = true;
        productionNode = false;
        potionNode = false;
        potionLevel = 0;
        productionLevel = 1;
        if (isRoot)
        {
            manager.win();
        }
    }

    //this is to chance the state of the node
    public void changeState(string changeThing)
    {
        switch (changeThing)
        {
            case "neutral":
                resetNode();
                break;
            case "production":
                productionNode = true;
                break;
            case "red":
                resetNode();
                neutral = false;
                redTeam = true;
                renderer.sprite = redSprite;
                break;
            case "blue":
                resetNode();
                neutral = false;
                redTeam = false;
                renderer.sprite = blueSprite;
                break;
            case "potion":
                potionNode = true;
                potionLevel += 1;
                break;
            case "root":
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
                if (red)
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

    public int returnManpower()
    {
        return manPower;
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

    public int getLevelUpCost()
    {
        return levelUpCost;
    }

    public int moreExpensiveUpgrades()
    {
        int originalCost = levelUpCost;
        levelUpCost *= 5;
        return originalCost;
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
        if (potionNode)
        {
            return "potion node";
        }
        if (producing)
        {
            return "production node";
        }
        return "node";
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
}
