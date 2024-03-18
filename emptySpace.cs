using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class emptySpace : MonoBehaviour
{
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Player>();
    }

    private void OnMouseDown()
    {
        Debug.Log("EEEE3EEEEE");
        player.emptySpace();
    }
}
