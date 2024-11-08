using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagePage : MonoBehaviour
{
    [SerializeField]
    private string stageName;

    [SerializeField]
    private GameObject[] difficultyButtons;

    // Start is called before the first frame update
    void Start()
    {
        int difficulty = PlayerPrefs.GetInt(stageName);
        for(int i = 0; i <= difficulty; i++)
        {
            if(i < 3)
            {
                difficultyButtons[i].SetActive(true);
            }
        }
    }
}
