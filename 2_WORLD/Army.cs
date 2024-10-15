using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Army : MonoBehaviour
{
    private bool moving, disappearing, redteam, singlePlayer, transformed;

    [SerializeField]
    private int manpower, index = -1, shieldAmount;

    private float speed, counter;

    private string potion;

    [SerializeField]
    private float potionDuration;

    [SerializeField]
    private SpriteRenderer renderer;

    [SerializeField]
    private Sprite redSprite, blueSprite;

    private Player player;

    private GameObject start, target, next, prev;

    [SerializeField]
    private edges currentEdge;

    private List<(GameObject, int)> path;

    private Manager manager;

    [SerializeField]
    private TextMeshProUGUI armyText;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Player>();
    }

    private void Update()
    {
        armyText.text = manpower.ToString();
        if (moving)
        {
            counter += Time.deltaTime;

            //move towards the next node
            transform.position = Vector3.MoveTowards(transform.position, next.transform.position, speed * Time.deltaTime);

            //calculating time before reaching target (distance/speed)
            if (next == target && (next.transform.position - transform.position).magnitude / speed < 0.4f && !disappearing)
            {
                disappearing = true;
                StartCoroutine(disappear());
            }

            //if reaching target, assign the next node
            if (transform.position == next.transform.position)
            {
                moving = false;
                nextNode();
            }

            if (currentEdge.returnGold())
            {
                turnIntoEnemy();
            }
        }
    }

    public void nextNode()
    {
        //determines the next node and the speed required to reach it within the specific time limit
        counter = 0;
        index++;
        if(index < path.Count)
        {
            //this shows the distance between the node and the army in a vector format
            Vector2 direction = path[index].Item1.transform.position - transform.position;

            //speed = distance/time, the path[index].Item2 stores time needed to reach from the starting node so it is necessary to deduct it by the cumulation of time taken to reach previous nodes.
            float distance = direction.magnitude;
            float time = path[index].Item2;
            if(potion == "HASTE")
            {
                Debug.Log("fast");
                time /= 2;
            }
            speed = distance / time;
            //conversion of the angle from radians to degrees
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, -angle);
            moving = true;
            next = path[index].Item1;
            if(index == 0)
            {
                prev = start;
            }
            else
            {
                prev = path[index - 1].Item1;
            }
            currentEdge = prev.GetComponent<Node>().returnPath(next);
        }
    }

    public bool getTeam()
    {
        return redteam;
    }

    public void adjustManpower(int amount)
    {
        Debug.Log("adjusting");
        int actualAmount = amount;
        if (shieldAmount > 0)
        {
            if(shieldAmount > actualAmount)
            {
                shieldAmount -= actualAmount;
                actualAmount = 0;
            }
            else
            {
                actualAmount -= shieldAmount;
                shieldAmount = 0;
            }
        }
        manpower -= actualAmount;
        if(manpower <= 0)
        {
            Destroy(gameObject, 0.01f);
        }
    }

    public void turnIntoEnemy()
    {
        if (redteam)
        {
            transformed = true;
            redteam = false;
            renderer.sprite = blueSprite;
            next = prev;
            target = next;
            Vector2 direction = next.transform.position - transform.position;
            float distance = direction.magnitude;
            float time = counter;
            if (potion == "HASTE")
            {
                time /= 2;
            }
            speed = distance / time;
            if (speed < 0.5f)
            {
                speed = 0.5f;
            }
            //conversion of the angle from radians to degrees
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, -angle);
            moving = true;
        }
    }

    public int getManpower()
    {
        return manpower;
    }


    [PunRPC]
    public void assignValues(string startParam, string targetParam, int manpowerParam, bool redParam, string potionArg, bool isSingle)
    {
        //called when created so values can be passed into the object
        start = GameObject.Find(startParam);
        target = GameObject.Find(targetParam);
        manager = GameObject.FindGameObjectWithTag("MANAGER").GetComponent<Manager>();
        prev = start;
        singlePlayer = isSingle;
        manpower = manpowerParam;
        path = manager.d_Algorithm(start, target, redParam);
        transform.position = start.transform.position;
        redteam = redParam;
        potion = potionArg;
        if (redteam)
        {
            renderer.sprite = redSprite;
        }
        else
        {
            renderer.sprite = blueSprite;
        }
        if(potion == "SHIELD")
        {
            shieldAmount = 20;
        }
        StartCoroutine(appear());
        nextNode();
    }

    [PunRPC]
    IEnumerator appear()
    {
        //animation when the gameobject is created
        transform.localScale = Vector3.zero;
        gameObject.LeanScale(Vector2.one, 0.5f).setEaseInExpo();
        yield return new WaitForSeconds(0.5f);
        transform.localScale = Vector3.one;
    }

    [PunRPC]
    IEnumerator disappear()
    {
        //animation before it is destroyed
        gameObject.LeanScale(Vector3.zero, 0.5f).setEaseInElastic();
        yield return new WaitForSeconds(0.5f);
        if (singlePlayer)
        {
            target.GetComponent<Node>().modifyManPower(manpower, true, redteam, transformed);
            if (potion == "PRODUCE")
            {
                player.productionPotion(target.name, potionDuration);
            }
        }
        else
        {
            PhotonView playerView = player.GetComponent<PhotonView>();
            if (playerView.IsMine)
            {
                playerView.RPC("modifyManpower", RpcTarget.AllBuffered, target.name, manpower, true, redteam);
            }
            if(potion == "PRODUCE")
            {
                playerView.RPC("productionPotion", RpcTarget.AllBuffered, target.name, potionDuration);
            }
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check collisions for a single team only (as collisions only occur when opposing teams collide, to ensure the values are correct only have this function called once)
        if (collision.CompareTag("ARMY") && redteam)
        {
            Debug.Log("Collided");
            if(redteam != collision.GetComponent<Army>().redteam)
            {
                int enemyManpower = collision.GetComponent<Army>().getManpower();
                collision.GetComponent<Army>().adjustManpower(manpower);
                adjustManpower(enemyManpower);
            }
        }
    }
}
