using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Army : MonoBehaviour
{
    private bool moving, disappearing, redTeam, singlePlayer;

    private int manpower, index = -1;

    private float speed, timeCumulation = 0;

    private string potion;

    [SerializeField]
    private SpriteRenderer renderer;

    [SerializeField]
    private Sprite redSprite, blueSprite;

    private Player player;

    private GameObject start, target, next;

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
            //move towards the next node
            transform.position = Vector3.MoveTowards(transform.position, next.transform.position, speed * Time.deltaTime);

            //calculating time before reaching target (distance/speed)
            if (path[path.Count - 1].Item1 == next && (next.transform.position - transform.position).magnitude / speed < 0.4f && !disappearing)
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
        }
    }

    public void nextNode()
    {
        //determines the next node and the speed required to reach it within the specific time limit
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
        }
    }

    public bool getTeam()
    {
        return player.getTeam();
    }

    public void adjustManpower(int amount)
    {
        manpower -= amount;
        if(manpower <= 0)
        {
            Destroy(gameObject, 0.01f);
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
        singlePlayer = isSingle;
        manpower = manpowerParam;
        path = manager.d_Algorithm(start, target);
        transform.position = start.transform.position;
        redTeam = redParam;
        potion = potionArg;
        if (redTeam)
        {
            renderer.sprite = redSprite;
        }
        else
        {
            renderer.sprite = blueSprite;
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
            target.GetComponent<Node>().modifyManPower(manpower, true, redTeam);
        }
        else
        {
            PhotonView playerView = player.GetComponent<PhotonView>();
            playerView.RPC("modifyManpower", RpcTarget.AllBuffered, target.name, manpower, true, redTeam);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ARMY"))
        {
            if(collision.GetComponent<Army>().getTeam() == player.getTeam())
            {
                adjustManpower(collision.GetComponent<Army>().getManpower());
            }
        }
    }
}
