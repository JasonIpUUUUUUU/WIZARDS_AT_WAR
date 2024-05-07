using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Army : MonoBehaviour
{
    private bool moving, disappearing, redTeam;

    private int manpower, index = -1;

    private float speed;

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
            transform.position = Vector3.MoveTowards(transform.position, next.transform.position, speed);

            //calculating time before reaching target (distance/speed)
            if (path[path.Count - 1].Item1 == next && (next.transform.position - transform.position).magnitude * Time.deltaTime / speed < 0.4f && !disappearing)
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

    public void assingValues(GameObject startParam, GameObject targetParam, Manager managerParam, int manpowerParam, bool redParam)
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
        StartCoroutine(appear());
        nextNode();
    }

    public void nextNode()
    {
        //determines the next node and the speed required to reach it within the specific time limit
        index++;
        if(index < path.Count)
        {
            Vector2 direction = path[index].Item1.transform.position - transform.position;
            speed = direction.magnitude / path[index].Item2 * Time.deltaTime;
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
