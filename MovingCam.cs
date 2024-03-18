using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCam : MonoBehaviour
{
    //forcedMoving is true when the player clicks onto a node and the camera is moved towards it
    private bool forcedMoving, canMove;

    [SerializeField]
    private float Ylimits, maxSize, minSize, moveSpeed;

    public List<Vector3> movePositions;

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        cameraScroll();
        cameraMove();
        forcedMove();
    }

    //it adds a position into the queue of positions for the camera to move to
    public void addPosition(Vector3 position)
    {
        canMove = checkInputs();
        movePositions.Add(position);
        forcedMoving = true;
    }

    //this forces the camera to gradually move towards the positions within the movePositions queue
    void forcedMove()
    {
        if (forcedMoving)
        {
            //by timesing the movement speed by the distance between the object and the target position, it gives it a smoother movement
            transform.position = Vector2.MoveTowards(transform.position, movePositions[0], moveSpeed * (transform.position - movePositions[0]).magnitude * Time.deltaTime);
            //runs if the distance between targetposition and the current position is low
            if((transform.position - movePositions[0]).magnitude <= 0.2f)
            {
                movePositions.RemoveAt(0);
                if(movePositions.Count == 0)
                {
                    forcedMoving = false;
                    canMove = true;
                }
            }
        }
    }

    //it returns true if any directional key is pressed
    bool checkInputs()
    {
        return Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0;
    }

    //it stops the forcedmove from happening
    void interruptMove()
    {
        forcedMoving = false;
        movePositions.Clear();
    }

    //it respons to player inputs to move the camera
    void cameraMove()
    {
        if (canMove)
        {
            transform.position += new Vector3(Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime, Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime);
            if (!checkInputs())
            {
                interruptMove();
            }
        }
        else if(checkInputs())
        {
            canMove = true;
        }
    }

    //this manages zooming in and out for the player
    void cameraScroll()
    {
        float oldOrthographicSize = Camera.main.orthographicSize;
        if (Input.mouseScrollDelta.y != 0)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Mathf.Clamp(target, min, max) essentially clamps the target variable so it is always between min and max
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - Input.mouseScrollDelta.y, minSize, maxSize);
            //this ensures when it detects the camera zooming in, it moves the camera towards the mouse and the opposite as well
            transform.position = mousePosition + (transform.position - mousePosition) * (Camera.main.orthographicSize / oldOrthographicSize);
        }
    }
}
