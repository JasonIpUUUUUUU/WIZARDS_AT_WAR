using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MovingCam : MonoBehaviour
{
    //forcedMoving is true when the player clicks onto a node and the camera is moved towards it
    [SerializeField]
    private bool forcedMoving, canMove, gameStopped, waitCam, waitZoom;

    private int orientation = 1;

    [SerializeField]
    private float Ylimits, maxSize, minSize, moveSpeed, dragSpeed;

    public List<Vector3> movePositions;

    [SerializeField]
    private Camera cam;

    private Player player;

    [SerializeField]
    private Tutorial tutorial;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Player>();
        canMove = true;
        if (player.getTeam() == false)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
            orientation = -1;
        }
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

    public void waitForCamMove()
    {
        waitCam = true;
    }

    public void waitForCamScroll()
    {
        waitZoom = true;
    }

    public void returnToDefault()
    {
        gameStopped = true;
        transform.position = Vector3.zero;
        cam.orthographicSize = 10;
        canMove = false;
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
            transform.position += new Vector3(Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime, Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime) * orientation;
            // when inputs are detected
            if (!checkInputs())
            {
                if (waitCam)
                {
                    tutorial.camMovedFunc();
                }
                interruptMove();
            }
        }
        else if(checkInputs() && !gameStopped)
        {
            canMove = true;
        }
    }

    //this manages zooming in and out for the player
    void cameraScroll()
    {
        float oldOrthographicSize = Camera.main.orthographicSize;
        if (Input.mouseScrollDelta.y != 0 && !gameStopped)
        {
            if (waitZoom)
            {
                tutorial.camScrolledFunc();
            }
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Mathf.Clamp(target, min, max) essentially clamps the target variable so it is always between min and max
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - Input.mouseScrollDelta.y, minSize, maxSize);
            //this ensures when it detects the camera zooming in, it moves the camera towards the mouse and the opposite as well
            transform.position = (Vector2) (mousePosition + (transform.position - mousePosition) * (Camera.main.orthographicSize / oldOrthographicSize));
        }
    }

    public void dragMove(Vector3 initialMousePos, Vector3 currentMousePos, Vector3 offset)
    {
        transform.position = offset - (currentMousePos - initialMousePos) * dragSpeed * orientation;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    public void freezeCam(bool freeze)
    {
        gameStopped = freeze;
        canMove = !freeze;
    }
}
