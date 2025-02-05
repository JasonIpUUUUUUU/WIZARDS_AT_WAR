using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterSpawner : MonoBehaviour
{
    [SerializeField]
    private int playerIndex;

    [SerializeField]
    private GameObject character;

    [SerializeField]
    private Transform characterPos1, characterPos2;

    private PhotonView view;

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        playerIndex = 2;
        if (view.IsMine)
        {
            playerIndex = 1;
        }
        int hatIndex = 0, wandIndex = 0, faceIndex = 0, robeIndex = 0;
        if (PlayerPrefs.HasKey("HAT"))
        {
            hatIndex = PlayerPrefs.GetInt("HAT");
        }
        if (PlayerPrefs.HasKey("WAND"))
        {
            wandIndex = PlayerPrefs.GetInt("WAND");
        }
        if (PlayerPrefs.HasKey("FACE"))
        {
            faceIndex = PlayerPrefs.GetInt("FACE");
        }
        if (PlayerPrefs.HasKey("ROBE"))
        {
            robeIndex = PlayerPrefs.GetInt("ROBE");
        }
        view.RPC("createCharacters", RpcTarget.AllBuffered, hatIndex, robeIndex, faceIndex, wandIndex, playerIndex);
    }

    [PunRPC]
    public void createCharacters(int hat, int robe, int face, int wand, int playerIndexParam)
    {
        if (playerIndex != playerIndexParam)
        {
            GameObject char1 = Instantiate(character);
            char1.GetComponent<Character>().setUpChara(hat, wand, face, robe);
            GameObject char2 = Instantiate(character);
            if (playerIndexParam == 1)
            {
                char1.transform.position = characterPos2.position;
                char2.transform.position = characterPos1.position;
            }
            else
            {
                char1.transform.position = characterPos1.position;
                char2.transform.position = characterPos2.position;
            }
        }
    }
}
