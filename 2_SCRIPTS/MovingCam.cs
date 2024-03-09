using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCam : MonoBehaviour
{
    [SerializeField]
    private float Ylimits, maxSize, minSize, moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cameraScroll();
        cameraMove();
    }

    void cameraMove()
    {
        transform.position += new Vector3(Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime, Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime);
    }

    void cameraScroll()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            if (Camera.main.orthographicSize < maxSize)
            {
                Camera.main.orthographicSize += Input.mouseScrollDelta.y;
            }
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            if (Camera.main.orthographicSize > minSize)
            {
                Camera.main.orthographicSize += Input.mouseScrollDelta.y;
            }
        }
    }
}
