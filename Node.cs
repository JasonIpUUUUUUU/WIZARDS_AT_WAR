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

    private MovingCam movingCam;

    private Manager manager;

    //lists have to be kept public to be modified
    public List<GameObject> neighbours, paths;

    //a transparent dark sprite that appears over the object when clicked so the player knows which node is selected
    [SerializeField]
    private GameObject clickSprite;

    // Start is called before the first frame update
    void Start()
    {
        movingCam = GameObject.FindGameObjectWithTag("CAMHOLDER").GetComponent<MovingCam>();
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

    IEnumerator productionCoroutine()
    {
        producing = true;
        modifyManPower(1, true);
        yield return new WaitForSeconds(1);
        StartCoroutine(productionCoroutine());
    }

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

    public void addNeighbour(GameObject neighbour, GameObject edge, bool addMore)
    {
        paths.Add(edge);
        neighbours.Add(neighbour);
        if (addMore)
        {
            neighbour.GetComponent<Node>().addNeighbour(gameObject, edge, false);
        }
    }

    //a built in unity function which runs when the object is clicked
    private void OnMouseDown()
    {
        movingCam.addPosition(transform.position);
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
