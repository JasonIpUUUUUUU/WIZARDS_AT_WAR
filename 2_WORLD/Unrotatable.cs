using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unrotatable : MonoBehaviour
{
    // Counter rotate the rotation
    void Start()
    {
        Player player = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Player>();
        if (player.getTeam() == false)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
    }
}
