using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;


public class QuickMatchExample : MonoBehaviourPunCallbacks
{
    void Start()
    {
        // function used to connect to the network
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN.");
        PhotonNetwork.JoinRandomRoom();
    }
}