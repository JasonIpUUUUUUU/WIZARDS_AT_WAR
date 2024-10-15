using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class edges : MonoBehaviour
{
    [SerializeField]
    private bool golden;

    [SerializeField]
    private int distance;

    [SerializeField]
    private Color goldColor, defaultColor;

    private LineRenderer lr;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        defaultColor = lr.endColor;
    }

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

    public void turnGold()
    {
        StartCoroutine(goldCoroutine());
    }

    private IEnumerator goldCoroutine()
    {
        golden = true;
        lr.SetColors(goldColor, goldColor);
        yield return new WaitForSeconds(6);
        golden = false;
        lr.SetColors(defaultColor, defaultColor);
    }

    public bool returnGold()
    {
        return golden;
    }
}
