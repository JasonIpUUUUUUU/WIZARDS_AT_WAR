using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Pun;


public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private bool connectedMaster, connecting, hiding, tutorial;

    [SerializeField]
    private float baseLoadingTime, masterLoadingTime;

    private float counter, totalLoadingTime, acceleration = 0.5f;

    [SerializeField]
    private AudioSource audio;

    [SerializeField]
    private CanvasGroup loadingCanvas;

    [SerializeField]
    private Slider progressBar;

    [SerializeField]
    private TextMeshProUGUI progressText;

    // a gameVersion variable is necessary as only players with the same version should play with each other
    string gameVersion = "1";

    void Awake()
    {
        totalLoadingTime = masterLoadingTime + baseLoadingTime;

        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("try connect");
            connectedMaster = false;
            PhotonNetwork.GameVersion = gameVersion;
            // function used to connect to the network
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            loadingCanvas.gameObject.SetActive(false);
            audio.Play();
            if (PlayerPrefs.GetInt("TUTORIAL") != 1 && tutorial)
            {
                PlayerPrefs.SetInt("SINGLE", 1);
                SceneManager.LoadScene("TUTORIAL1");
            }
        }
        print(Time.timeScale);
    }

    // this is to make the illusion of a progress bar when connecting to the server so the player feels like something is happening
    private void Update()
    {
        // baseLoadingTime is the time it will wait until it gets stuck to wait for the master, once it is connected to master it completes the loading
        if((counter <= baseLoadingTime || connectedMaster) && counter <= totalLoadingTime)
        {
            // so the loading screen accelerates
            acceleration += Time.deltaTime/2;
            counter += Time.deltaTime * acceleration;
            // returns a whole number percentage to give the illusion or progress, clamps it between 0-1 to prevent out of range errors
            progressText.text = Mathf.RoundToInt(Mathf.Clamp(counter / totalLoadingTime, 0, 1) * 100).ToString() + "%";
        }
        else if(!connecting && !connectedMaster)
        {
            connecting = true;
            StartCoroutine(connectingLoop(0));
        }
        progressBar.value = counter / totalLoadingTime;
        if(counter >= totalLoadingTime && !hiding)
        {
            if (PlayerPrefs.GetInt("TUTORIAL") != 1 && tutorial)
            {
                PlayerPrefs.SetInt("SINGLE", 1);
                SceneManager.LoadScene("TUTORIAL1");
            }
            else
            {
                audio.Play();
                StartCoroutine(hideUI());
                hiding = true;
            }
        }
    }

    // quality of life change so there is visually something going on while waiting for the server
    IEnumerator connectingLoop(int index)
    {
        PlayerPrefs.SetInt("SINGLE", 0);
        Debug.Log("Set single to false 3");
        string connectingMessage = "connecting";
        for(int i = 0; i <= index % 3; i++)
        {
            connectingMessage += ".";
        }
        progressText.text = connectingMessage;
        yield return new WaitForSeconds(0.5f);
        if (connecting)
        {
            StartCoroutine(connectingLoop(index + 1));
        }
    }

    // hide the progressBarUI
    IEnumerator hideUI()
    {
        float duration = 0.5f, elapsedTime = 0f;
        while(elapsedTime < duration)
        {
            // gradually cause the UI to fade over the duration
            elapsedTime += Time.deltaTime;
            loadingCanvas.alpha = 1 - elapsedTime / duration;
            yield return null;
        }
        loadingCanvas.gameObject.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        connectedMaster = true;
        connecting = false;
    }

    public void joinRoom()
    {
        //Fails if there are no open games. Error callback: IMatchmakingCallbacks.OnJoinRandomFailed
        PhotonNetwork.JoinRandomRoom();
        PlayerPrefs.SetInt("SINGLE", 0);
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
        Debug.Log("joined");
        PlayerPrefs.SetInt("SINGLE", 0);
        SceneManager.LoadScene("Loading");
    }
}