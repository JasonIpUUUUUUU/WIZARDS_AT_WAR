using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    private bool difficultySet = false;

    [SerializeField]
    private GameObject currentScreen, blackThing;

    [SerializeField]
    private StageSelect stageSelect;

    [SerializeField]
    private TextMeshProUGUI moneyText, stardustText;

    private void Update()
    {
        moneyText.text = "Money: " + PlayerPrefs.GetInt("MONEY").ToString();
        stardustText.text = "Stardust: " + PlayerPrefs.GetInt("STARDUST").ToString();
    }

    public void enterScreen(GameObject screen)
    {
        currentScreen.SetActive(false);
        screen.SetActive(true);
        currentScreen = screen;
    }

    // sets the difficutly for the player
    public void setDifficulty(int difficulty)
    {
        PlayerPrefs.SetInt("DIFFICULTY", difficulty);
        difficultySet = true;
    }

    // calls the function to enter the stage
    public void enterStage(string stage)
    {
        StartCoroutine(waitForDifficulty(stage));
    }

    // waits until the difficulty has been modified to switch scenes
    private IEnumerator waitForDifficulty(string stage)
    {
        while (!difficultySet)
        {
            yield return null;
        }
        PlayerPrefs.SetInt("SINGLE", 1);
        blackThing.LeanMoveY(0, 1);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(stage);
    }
}
