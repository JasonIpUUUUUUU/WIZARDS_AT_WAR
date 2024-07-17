using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Army : MonoBehaviour
{
    private bool moving, disappearing, redTeam;

    private int manpower, index = -1;

    private float speed, timeCumulation = 0;

    private string potion;

    [SerializeField]
    private SpriteRenderer renderer;

    [SerializeField]
    private Sprite redSprite, blueSprite;

    private GameObject start, target, next;

    private List<(GameObject, int)> path;

    private Manager manager;

    [SerializeField]
    private TextMeshProUGUI armyText;

    private void Update()
    {
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

    public void assingValues(GameObject startParam, GameObject targetParam, Manager managerParam, int manpowerParam, bool redParam, string potionArg)
    {
        //called when created so values can be passed into the object
        manager = managerParam;
        start = startParam;
        target = targetParam;
        manpower = manpowerParam;
        armyText.text = manpower.ToString();
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
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, -angle);
            moving = true;
            next = path[index].Item1;
        }
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
        target.GetComponent<Node>().modifyManPower(manpower, true, redTeam);
        Destroy(gameObject);
    }
}
