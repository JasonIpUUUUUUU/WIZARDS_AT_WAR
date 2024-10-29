using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelect : MonoBehaviour
{
    private int stageIndex = 0, stageLen;

    [SerializeField]
    private GameObject currentScreen;

    [SerializeField]
    private GameObject[] stageScreens;

    private void Start()
    {
        stageLen = stageScreens.Length - 1;
        currentScreen = stageScreens[stageIndex];
    }

    // changes between stage screens
    public void changeStage(bool next)
    {
        currentScreen.SetActive(false);
        if (next)
        {
            stageIndex++;
        }
        else
        {
            stageIndex--;
        }
        if(stageIndex > stageLen)
        {
            stageIndex = 0;
        }
        else if(stageIndex < 0)
        {
            stageIndex = stageLen;
        }
        currentScreen = stageScreens[stageIndex];
        currentScreen.SetActive(true);
    }
}
