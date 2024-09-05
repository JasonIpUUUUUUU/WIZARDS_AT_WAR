using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;


public class ConnectToServer : MonoBehaviourPunCallbacks
{
    // a gameVersion variable is necessary as only players with the same version should play with each other
    string gameVersion = "1";

    void Awake()
    {
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        // function used to connect to the network
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
    }

    public void joinRoom()
    {
        //Fails if there are no open games. Error callback: IMatchmakingCallbacks.OnJoinRandomFailed
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PlayerPrefs.SetInt("SINGLE", 0);
        Debug.Log("join room failed, creating new room");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        PlayerPrefs.SetInt("SINGLE", 0);
        SceneManager.LoadScene("Loading");
    }
}