using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class emptySpace : MonoBehaviour
{
    //this object is attached to the background so the gameManager and player scripts know when the background is clicked (signifying to cancel an action)
    private Player player;

    private MovingCam camMove;

    private Vector3 initialPos, offset;

    // Start is called before the first frame update
    void Start()
    {
        camMove = GameObject.FindGameObjectWithTag("CAMHOLDER").GetComponent<MovingCam>();
        player = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Player>();
    }

    private void OnMouseDown()
    {
        initialPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        offset = camMove.transform.position;
        player.emptySpace();
    }

    private void OnMouseDrag()
    {
        camMove.dragMove(initialPos, Camera.main.ScreenToViewportPoint(Input.mousePosition), offset);
    }
}
