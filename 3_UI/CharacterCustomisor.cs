using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCustomisor : MonoBehaviour
{
    private int itemNum;

    [SerializeField]
    private string[] hats, wands, faces, robes;

    [SerializeField]
    private RectTransform firstPos, secondPos;

    [SerializeField]
    private Image hatImage, wandImage, faceImage, robeImage;

    [SerializeField]
    private Sprite[] hatSprite, wandSprite, faceSprite, robeSprite;

    [SerializeField]
    private GameObject selectScreen, cosButton, tempHideChar;

    private List<GameObject> currentItems = new List<GameObject>();

    private void Start()
    {
        PlayerPrefs.SetInt("NORMALHAT", 1);
        PlayerPrefs.SetInt("NORMALFACE", 1);
        PlayerPrefs.SetInt("NORMALWAND", 1);
        PlayerPrefs.SetInt("NORMALROBE", 1);
        setDisplays();
    }

    public void customizeChara(string clothType)
    {
        tempHideChar.SetActive(false);
        selectScreen.SetActive(true);
        itemNum = 0;
        foreach (GameObject item in currentItems)
        {
            Destroy(item);
        }
        currentItems.Clear();
        switch (clothType){
            case "HAT":
                for(int i = 0; i < hats.Length; i++)
                {
                    if (PlayerPrefs.HasKey(hats[i]))
                    {
                        GameObject item = Instantiate(cosButton, firstPos);
                        currentItems.Add(item);
                        item.GetComponent<CosmeticInvetoryObj>().setItem(i, "HAT", this, hatSprite[i]);
                        // localPosition is the relative position to parent object
                        item.transform.localPosition = Vector3.zero;
                        item.transform.position += (secondPos.position - firstPos.position) * itemNum;
                        itemNum++;
                    }
                }
                break;
            case "WAND":
                for (int i = 0; i < wands.Length; i++)
                {
                    if (PlayerPrefs.HasKey(wands[i]))
                    {
                        GameObject item = Instantiate(cosButton, firstPos);
                        currentItems.Add(item);
                        item.GetComponent<CosmeticInvetoryObj>().setItem(i, "WAND", this, wandSprite[i]);
                        // localPosition is the relative position to parent object
                        item.transform.localPosition = Vector3.zero;
                        item.transform.position += (secondPos.position - firstPos.position) * itemNum;
                        itemNum++;
                    }
                }
                break;
            case "FACE":
                for (int i = 0; i < faces.Length; i++)
                {
                    if (PlayerPrefs.HasKey(faces[i]))
                    {
                        GameObject item = Instantiate(cosButton, firstPos);
                        currentItems.Add(item);
                        item.GetComponent<CosmeticInvetoryObj>().setItem(i, "FACE", this, faceSprite[i]);
                        // localPosition is the relative position to parent object
                        item.transform.localPosition = Vector3.zero;
                        item.transform.position += (secondPos.position - firstPos.position) * itemNum;
                        itemNum++;
                    }
                }
                break;
            case "ROBE":
                for (int i = 0; i < robes.Length; i++)
                {
                    if (PlayerPrefs.HasKey(robes[i]))
                    {
                        GameObject item = Instantiate(cosButton, firstPos);
                        currentItems.Add(item);
                        item.GetComponent<CosmeticInvetoryObj>().setItem(i, "ROBE", this, robeSprite[i]);
                        // localPosition is the relative position to parent object
                        item.transform.localPosition = Vector3.zero;
                        item.transform.position += (secondPos.position - firstPos.position) * itemNum;
                        itemNum++;
                    }
                }
                break;
        }
    }

    public void chooseCosmetic(string slot, int index)
    {
        Debug.Log("found cosmetic");
        closeSelect();
        PlayerPrefs.SetInt(slot, index);
        GameObject[] characters = GameObject.FindGameObjectsWithTag("CHARACTER");
        foreach(GameObject character in characters)
        {
            Debug.Log("found characters");
            Character characterScript = character.GetComponent<Character>();
            characterScript.updateChara();
        }
        setDisplays();
    }

    private void setDisplays()
    {
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
        hatImage.sprite = hatSprite[hatIndex];
        wandImage.sprite = wandSprite[wandIndex];
        faceImage.sprite = faceSprite[faceIndex];
        robeImage.sprite = robeSprite[robeIndex];
    }

    public void closeSelect()
    {
        tempHideChar.SetActive(true);
        selectScreen.SetActive(false);
    }
}
