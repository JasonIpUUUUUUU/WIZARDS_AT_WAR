using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class StageSelector : MonoBehaviour
{
    [SerializeField]
    private string[] stageNames;

    [SerializeField]
    private Sprite[] stageImages;

    [SerializeField]
    private GameObject blackThing;

    [SerializeField]
    private TextMeshProUGUI mapText;

    [SerializeField]
    private Image stageImage;

    private PhotonView view;

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            int mapIndex = Random.Range(0, stageNames.Length);
            // have 1 of the players choose a map and call the function for both players
            view.RPC("setMap", RpcTarget.AllBuffered, mapIndex);
        }
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

    IEnumerator waitToEnterStage(string map)
    {
        yield return new WaitForSeconds(4);
        blackThing.LeanMoveY(0, 1);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(map);
    }
}
