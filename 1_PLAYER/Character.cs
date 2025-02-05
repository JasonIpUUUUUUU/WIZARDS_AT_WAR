using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private bool isBoss, isMulti;

    [SerializeField]
    private Sprite[] hats, wands, faces, robes;

    [SerializeField]
    private SpriteRenderer hat, wand, face, robe;

    private void Start()
    {
        updateChara();
    }

    public void updateChara()
    {
        Debug.Log("UPDATING CHARACTER");
        if (!isBoss && !isMulti)
        {
            Debug.Log("UPDATING CHARACTER SUCCESFUL");
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
            StartCoroutine(setUpCharDelayed(hatIndex, wandIndex, faceIndex, robeIndex));
        }
    }

    IEnumerator setUpCharDelayed(int hatIndex, int wandIndex, int faceIndex, int robeIndex)
    {
        yield return new WaitForSeconds(0.05f);
        hat.sprite = hats[hatIndex];
        wand.sprite = wands[wandIndex];
        face.sprite = faces[faceIndex];
        robe.sprite = robes[robeIndex];
    }

    // set as its own method so other scripts can reference
    public void setUpChara(int hatIndex, int wandIndex, int faceIndex, int robeIndex)
    {
        isMulti = true;
        StartCoroutine(setUpCharDelayed(hatIndex, wandIndex, faceIndex, robeIndex));
    }
}
