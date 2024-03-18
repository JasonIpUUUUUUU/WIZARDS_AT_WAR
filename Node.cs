using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Node : MonoBehaviour
{
    private int manPower, level, levelUpCost;

    [SerializeField]
    private bool isRoot, productionNode, potionNode, redTeam, neutral, producing;

    [SerializeField]
    Sprite redSprite, blueSprite, neutralSprite;

    [SerializeField]
    private SpriteRenderer renderer;

    private TextMeshProUGUI manPowerText;

    private Manager manager;

    private Player player;

    //lists have to be kept public to be modified
    public List<GameObject> neighbours, paths;

    //a transparent dark sprite that appears over the object when clicked so the player knows which node is selected
    [SerializeField]
    private GameObject clickSprite;

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
        modifyManPower(1, true);
        yield return new WaitForSeconds(1);
        StartCoroutine(productionCoroutine());
    }

    //this is to reset the node back to default
    public void resetNode()
    {
        productionNode = false;
        potionNode = false;
        level = 0;
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
                level = 1;
                break;
            case "red":
                resetNode();
                redTeam = true;
                renderer.sprite = redSprite;
                break;
            case "blue":
                resetNode();
                redTeam = false;
                renderer.sprite = blueSprite;
                break;
            case "potion":
                potionNode = true;
                level = 1;
                break;
            case "root":
                isRoot = true;
                productionNode = true;
                break;
        }
    }

    //this is to increase or decrease manpower on the node
    public void modifyManPower(int amount, bool add)
    {
        if (add)
        {
            manPower += amount;
        }
        else
        {
            manPower -= amount;
        }
        manPowerText.text = manPower.ToString();
    }

    //this manages what counts as a neighbour to this node
    public void addNeighbour(GameObject neighbour, GameObject edge, bool addMore)
    {
        paths.Add(edge);
        neighbours.Add(neighbour);
        if (addMore)
        {
            neighbour.GetComponent<Node>().addNeighbour(gameObject, edge, false);
        }
    }

    //this returns true if the target gameobject is a neighbour
    public bool findNeighbour(GameObject target)
    {
        return neighbours.Contains(target);
    }

    //a built in unity function which runs when the object is clicked
    private void OnMouseDown()
    {
        player.onNodeClicked(gameObject);
        //transform.localScale *= 0.9f;
        clickSprite.SetActive(true);
    }

    //a built in unity function which runs when the object is let go of
    private void OnMouseUp()
    {
        //transform.localScale /= 0.9f;
        clickSprite.SetActive(false);
    }
}
