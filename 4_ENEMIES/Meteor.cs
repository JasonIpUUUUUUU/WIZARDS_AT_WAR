using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    private bool started = false;
    private float speed;
    private GameObject target;

    public void setMeteor(GameObject targetP)
    {
        target = targetP;
        speed = Vector2.Distance(transform.position, target.transform.position);
        started = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (started)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            if(Vector2.Distance(transform.position, target.transform.position) < 0.1f)
            {
                Destroy(gameObject);
            }
        }
    }
}
