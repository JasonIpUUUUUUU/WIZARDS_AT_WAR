using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class edges : MonoBehaviour
{
    [SerializeField]
    private int distance;

    [PunRPC]
    public void setDistance(int distanceP)
    {
        distance = distanceP;
    }

    [PunRPC]
    public void setLine(Vector2 startPoint, Vector2 endPoint)
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, startPoint);
        lr.SetPosition(1, endPoint);
    }
}
