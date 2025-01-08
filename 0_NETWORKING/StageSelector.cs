using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class StageSelector : MonoBehaviour
{
    private int playerIndex;

    [SerializeField]
    private string[] stageNames;

    [SerializeField]
    private Sprite[] stageImages;

    [SerializeField]
    private GameObject blackThing;

    [SerializeField]
    private TextMeshProUGUI mapText, usernameText;

    [SerializeField]
    private Image stageImage;

    private PhotonView view;

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        playerIndex = 2;
        if (view.IsMine)
        {
            int mapIndex = Random.Range(0, stageNames.Length);
            // have 1 of the players choose a map and call the function for both players
            view.RPC("setMap", RpcTarget.AllBuffered, mapIndex);
            playerIndex = 1;
        }
        view.RPC("setUsernames", RpcTarget.AllBuffered, PlayerPrefs.GetString("USERNAME"), playerIndex);
    }

    // a method to choose a map for all players to enter after waiting for a bit and sets up the UI
    [PunRPC]
    public void setMap(int mapIndex)
    {
        string map = stageNames[mapIndex];
        mapText.text = map;
        stageImage.sprite = stageImages[mapIndex];
        StartCoroutine(waitToEnterStage(map));
    }

    [PunRPC]
    public void setUsernames(string username, int playerIndexParam)
    {
        if(playerIndex != playerIndexParam)
        {
            usernameText.text = username + " vs " + PlayerPrefs.GetString("USERNAME");
        }
    }

    IEnumerator waitToEnterStage(string map)
    {
        yield return new WaitForSeconds(4);
        blackThing.LeanMoveY(0, 1);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(map);
    }
}
