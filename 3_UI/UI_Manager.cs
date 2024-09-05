using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    [SerializeField]
    private GameObject currentScreen;

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

    public void enterStage(string stage)
    {
        PlayerPrefs.SetInt("SINGLE", 1);
        SceneManager.LoadScene(stage);
    }
}
