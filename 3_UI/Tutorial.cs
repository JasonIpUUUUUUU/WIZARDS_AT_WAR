using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
    private int index;

    [SerializeField]
    private bool textFinished = false, camMoved = false, skip, camScrolled = false, nodeClicked, pUIClicked = false, pMake = false, pMade = false;

    [SerializeField]
    private string[] texts;

    [SerializeField]
    private MovingCam cam;

    [SerializeField]
    private Manager manager;

    [SerializeField]
    private Player player;

    [SerializeField]
    private GameObject UI_Screen;

    [SerializeField]
    private List<GameObject> specialScreens;

    [SerializeField]
    private TextMeshProUGUI tutorialText, objectiveText;

    private void Update()
    {
        if (!textFinished && Input.GetKeyDown(KeyCode.Space))
        {
            skip = true;
        }
    }

    public void startTutorial(int index)
    {
        switch (index)
        {
            case 1:
                StartCoroutine(tutorial1());
                break;
            case 2:
                StartCoroutine(tutorial2());
                break;
            case 3:
                StartCoroutine(tutorial3());
                break;
        }
    }

    IEnumerator tutorial3()
    {
        // initially freeze the camera and prevent player interactions with nodes
        yield return new WaitForSeconds(0.1f);
        cam.freezeCam(true);
        player.setInteract(false);
        yield return new WaitForSeconds(1);
        UI_Screen.SetActive(true);
        yield return new WaitForSeconds(1);
        // display UI screen and text after short delay
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        objectiveText.gameObject.SetActive(true);
        cam.freezeCam(false);
        player.setInteract(true);
        player.startWaitPotionMake();
        player.setPotionInteract(true);
        UI_Screen.SetActive(false);
        manager.startTutorialBattle();
    }

    IEnumerator tutorial2()
    {
        if (PlayerPrefs.GetInt("TUTORIAL") != 1)
        {
            PlayerPrefs.SetInt("HASTE", 1);
            PlayerPrefs.SetString("SLOT0", "HASTE");
        }
        // initially freeze the camera and prevent player interactions with nodes
        yield return new WaitForSeconds(0.1f);
        cam.freezeCam(true);
        player.setInteract(false);
        yield return new WaitForSeconds(1);
        UI_Screen.SetActive(true);
        yield return new WaitForSeconds(1);
        // display UI screen and text after short delay
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        objectiveText.text = "Click on the red root node";
        UI_Screen.SetActive(false);
        cam.freezeCam(false);
        player.setInteract(true);
        player.readyClickRootNode();
        while (!nodeClicked)
        {
            yield return null;
        }
        player.setInteract(false);
        yield return new WaitForSeconds(2);
        objectiveText.text = "";
        cam.freezeCam(true);
        UI_Screen.SetActive(true);
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        UI_Screen.SetActive(false);
        cam.freezeCam(false);
        player.setPotionInteract(true);
        player.startWaitPotion();
        objectiveText.text = "Press P to open up node UI";
        while (!pUIClicked)
        {
            yield return null;
        }
        player.setPotionInteract(false);
        yield return new WaitForSeconds(2);
        objectiveText.text = "";
        UI_Screen.SetActive(true);
        cam.freezeCam(true);
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        UI_Screen.SetActive(false);
        player.setPotionInteract(true);
        player.startWaitPotionMake();
        objectiveText.text = "Click on the haste potion option";
        while (!pMake)
        {
            yield return null;
        }
        player.setPotionInteract(false);
        yield return new WaitForSeconds(2);
        UI_Screen.SetActive(true);
        objectiveText.text = "";
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space) && pMade))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        UI_Screen.SetActive(false);
        player.setPotionInteract(true);
        cam.freezeCam(false);
        player.setInteract(true);
    }

    // sequence of actions for the first tutorial page
    IEnumerator tutorial1()
    {
        // initially freeze the camera and prevent player interactions with nodes
        yield return new WaitForSeconds(0.1f);
        cam.freezeCam(true);
        player.setInteract(false);
        yield return new WaitForSeconds(1);
        UI_Screen.SetActive(true);
        yield return new WaitForSeconds(1);
        // display UI screen and text after short delay, wait for player input to continue
        StartCoroutine(setText());
        while(!(textFinished && Input.GetKeyDown(KeyCode.Space))){
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        UI_Screen.SetActive(false);
        objectiveText.text = "Move the camera with WASD";
        cam.freezeCam(false);
        cam.waitForCamMove();
        while (!camMoved)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2);
        objectiveText.text = "";
        cam.freezeCam(true);
        UI_Screen.SetActive(true);
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        cam.freezeCam(false);
        cam.waitForCamScroll();
        UI_Screen.SetActive(false);
        objectiveText.text = "Zoom in and out using the scrollwheel";
        while (!camScrolled)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2);
        objectiveText.text = "";
        cam.freezeCam(true);
        UI_Screen.SetActive(true);
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        objectiveText.text = "Click on the red root node";
        cam.freezeCam(false);
        player.setInteract(true);
        player.readyClickRootNode();
        UI_Screen.SetActive(false);
        while (!nodeClicked)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2);
        objectiveText.text = "";
        cam.freezeCam(true);
        player.setInteract(false);
        UI_Screen.SetActive(true);
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        StartCoroutine(setText());
        while (!(textFinished && Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        objectiveText.text = "Press 4 to select all manpower, press enter to prepare send and send army to blue node";
        cam.freezeCam(false);
        player.setInteract(true);
        UI_Screen.SetActive(false);
    }

    IEnumerator setText()
    {
        textFinished = false;
        skip = false;
        tutorialText.text = "";
        foreach(char c in texts[index])
        {
            tutorialText.text += c;
            yield return new WaitForSeconds(0.05f);
            if (skip)
            {
                skip = false;
                break;
            }
        }
        tutorialText.text = texts[index];
        textFinished = true;
        index++;
    }

    public void camMovedFunc()
    {
        camMoved = true;
    }

    public void camScrolledFunc()
    {
        camScrolled = true;
    }

    public void rootClicked()
    {
        nodeClicked = true;
    }

    public void potionUI()
    {
        pUIClicked = true;
    }

    public void potionMake()
    {
        pMake = true;
    }

    public void potionMade()
    {
        pMade = true;
    }
}

