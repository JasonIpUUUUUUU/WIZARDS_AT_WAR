using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinning : MonoBehaviour
{
    [SerializeField]
    private float spinspeed = 100;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, spinspeed * Time.deltaTime));
    }
}
