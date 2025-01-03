using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Army : MonoBehaviour
{
    [SerializeField]
    private bool moving, disappearing, redteam, singlePlayer, transformed, stun, astroPhase2, blackHoled;

    [SerializeField]
    private int manpower, index = -1, shieldAmount, poisonDamage;

    [SerializeField]
    private float speed, counter, fireDamageCounter, speedMultiplierElectro;

    private string potion;

    [SerializeField]
    private float potionDuration;

    [SerializeField]
    private SpriteRenderer renderer;

    [SerializeField]
    private Sprite redSprite, blueSprite;

    [SerializeField]
    private Image damageImage;

    private Player player;

    [SerializeField]
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
            if(fireDamageCounter > 0)
            {
                fireDamageCounter -= Time.deltaTime;
            }

            float tempSpeed = speed;

            if (blackHoled && PlayerPrefs.GetInt("DIFFICULTY") >= 2)
            {
                tempSpeed *= 1.5f;
            }

            if (currentEdge.returnElectro())
            {
                if (redteam)
                {
                    tempSpeed *= 0.5f;
                }
                else
                {
                    tempSpeed *= 1.5f;
                }
            }
            if (stun)
            {
                tempSpeed = 0;
            }
            //move towards the next node
            transform.position = Vector3.MoveTowards(transform.position, next.transform.position, tempSpeed * Time.deltaTime);

            //calculating time before reaching target (distance/speed)
            if ((!next.GetComponent<Node>().sameTeam(redteam) || next == target) && (next.transform.position - transform.position).magnitude / speed < 0.4f && !disappearing)
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

            if(potion == "FIRE")
            {
                currentEdge.setFire(redteam, 10);
            }

            if (currentEdge.returnGold())
            {
                turnIntoEnemy();
            }

            if (currentEdge.hasFire(redteam) && fireDamageCounter <= 0) 
            {
                adjustManpower(1);
                fireDamageCounter = 1;
            }
        }
    }

    public void nextNode()
    {
        if (!disappearing)
        {
            //determines the next node and the speed required to reach it within the specific time limit
            counter = 0;
            index++;
            if (index < path.Count)
            {
                //this shows the distance between the node and the army in a vector format
                Vector2 direction = path[index].Item1.transform.position - transform.position;

                //speed = distance/time, the path[index].Item2 stores time needed to reach from the starting node so it is necessary to deduct it by the cumulation of time taken to reach previous nodes.
                float distance = direction.magnitude;
                float time = path[index].Item2;
                if (potion == "HASTE")
                {
                    time /= 2;
                }
                if(potion == "ELECTROSPEED")
                {
                    time /= 1 + PlayerPrefs.GetInt("DIFFICULTY") * 0.5f;
                }
                speed = distance / time;
                //conversion of the angle from radians to degrees
                float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, -angle);
                moving = true;
                next = path[index].Item1;
                if (index == 0)
                {
                    prev = start;
                }
                else
                {
                    prev = path[index - 1].Item1;
                }
                if(astroPhase2 && prev.GetComponent<Node>().returnBlackHole())
                {
                    redteam = false;
                    renderer.sprite = blueSprite;
                    int blackHolePower = -(10 + (PlayerPrefs.GetInt("DIFFICUTLY") - 1) * 5);
                    adjustManpower(blackHolePower);
                    blackHoled = true;
                }
                currentEdge = prev.GetComponent<Node>().returnPath(next);
            }
        }
    }

    public bool getTeam()
    {
        return redteam;
    }

    public void adjustManpower(int amount)
    {
        StartCoroutine(damageEffect());
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

    IEnumerator damageEffect()
    {
        damageImage.color = new Color32(255, 255, 255, 0);
        float flashDuration = 0.25f, flashCounter = 0f;
        float shorterFlash = flashDuration / 4;
        while(flashCounter < shorterFlash)
        {
            flashCounter += Time.deltaTime;
            damageImage.color = new Color32(255, 255, 255, (byte) (255 * flashCounter / shorterFlash));
            yield return null;
        }
        flashCounter = 0;
        while (flashCounter < flashDuration)
        {
            flashCounter += Time.deltaTime;
            damageImage.color = new Color32(255, 255, 255, (byte)(255 * (1 - flashCounter / flashDuration)));
            yield return null;
        }
        damageImage.color = new Color32(255, 255, 255, 0);
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
            speed = distance / time;
            if (float.IsNaN(speed))
            {
                speed = 0.1f;
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

    public IEnumerator stunCoroutine(int duration)
    {
        Debug.Log("STUN");
        stun = true;
        yield return new WaitForSeconds(duration);
        stun = false;
    }

    public bool returnStun()
    {
        return potion == "STUN";
    }


    [PunRPC]
    public void assignValues(string startParam, string targetParam, int manpowerParam, bool redParam, string potionArg, bool isSingle, bool phase2)
    {
        //called when created so values can be passed into the object
        astroPhase2 = phase2;
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

    IEnumerator appear()
    {
        //animation when the gameobject is created
        transform.localScale = Vector3.zero;
        gameObject.LeanScale(Vector2.one, 0.5f).setEaseInExpo();
        yield return new WaitForSeconds(0.5f);
        transform.localScale = Vector3.one;
    }

    IEnumerator disappear()
    {
        //animation before it is destroyed
        gameObject.LeanScale(Vector3.zero, 0.5f).setEaseInElastic();
        yield return new WaitForSeconds(0.5f);
        if (potion == "PRODUCE")
        {
            player.producePotionPre(next.name, potionDuration);
        }
        if (potion == "POISON")
        {
            player.poisonPotionPre(next.name, poisonDamage, redteam);
        }
        if (singlePlayer)
        {
            next.GetComponent<Node>().modifyManPower(manpower, true, redteam, transformed);
        }
        else
        {
            PhotonView playerView = player.GetComponent<PhotonView>();
            if (playerView.IsMine)
            {
                playerView.RPC("modifyManpower", RpcTarget.AllBuffered, target.name, manpower, true, redteam);
            }
        }
        Destroy(gameObject);
    }

    public void stunMethod()
    {
        if (!redteam)
        {
            Debug.Log("blueStun");
        }
        StartCoroutine(stunCoroutine(3));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check collisions for a single team only (as collisions only occur when opposing teams collide, to ensure the values are correct only have this function called once)
        if (collision.CompareTag("ARMY") && redteam)
        {
            if(redteam != collision.GetComponent<Army>().redteam)
            {
                int enemyManpower = collision.GetComponent<Army>().getManpower();
                collision.GetComponent<Army>().adjustManpower(manpower);
                adjustManpower(enemyManpower);
                if (potion == "STUN")
                {
                    collision.GetComponent<Army>().stunMethod();
                }
                if (collision.GetComponent<Army>().returnStun())
                {
                    stunMethod();
                }
            }
        }
    }
}
