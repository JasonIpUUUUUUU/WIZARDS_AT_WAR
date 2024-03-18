using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class emptySpace : MonoBehaviour
{
    //this object is attached to the background so the gameManager and player scripts know when the background is clicked (signifying to cancel an action)
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Player>();
    }

    private void OnMouseDown()
    {
        player.emptySpace();
    }
}
