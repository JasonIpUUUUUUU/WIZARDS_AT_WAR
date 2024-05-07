using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Node : MonoBehaviour
{
    private int manPower, potionLevel, productionLevel, levelUpCost;

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
        renderer = GetComponent<SpriteRenderer>();
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
        yield return new WaitForSeconds(1);
        StartCoroutine(productionCoroutine());
    }

    //this is to reset the node back to default
    public void resetNode()
    {
        neutral = true;
        productionNode = false;
        potionNode = false;
        potionLevel = 0;
        productionLevel = 0;
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
                productionLevel += 1;
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
                isRoot = true;
                productionNode = true;
                break;
        }
    }

    //this is to increase or decrease manpower on the node
    public void modifyManPower(int amount, bool add, bool red)
    {
        if ((neutral || (redTeam == red)) && add)
        {
            manPower += amount;
            if (red)
            {
                changeState("red");
            }
            else
            {
                changeState("blue");
            }
        }
        else 
        {
            manPower -= amount;
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

    public List<GameObject> returnAllNeigbours(List<GameObject> initialList)
    {
        List<GameObject> currentObjects = new List<GameObject> { gameObject };
        if (!neutral)
        {
            foreach(GameObject neighbour in neighbours)
            {
                if (!initialList.Contains(neighbour))
                {
                    currentObjects.AddRange(neighbour.GetComponent<Node>().returnAllNeigbours(currentObjects));
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
}
