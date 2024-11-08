using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;

public class LoadingScreen : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private string stage;

    // checks number of players every frame, when it reaches 2, enter the game.
    void Update()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerCount == 2)
        {
            enterGame();
        }
    }

    // currently only loads the scene but defined it in a function in case more things need to be done
    public void enterGame()
    {
        SceneManager.LoadScene(stage);
    }
}
