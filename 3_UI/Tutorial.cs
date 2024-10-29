using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
    private int index;

    private bool textFinished = false, camMoved = false;

    [SerializeField]
    private string[] texts;

    [SerializeField]
    private MovingCam cam;

    [SerializeField]
    private Player player;

    [SerializeField]
    private GameObject UI_Screen;

    [SerializeField]
    private List<GameObject> specialScreens;

    [SerializeField]
    private TextMeshProUGUI tutorialText, objectiveText;

    public void startTutorial(int index)
    {
        switch (index)
        {
            case 1:
                StartCoroutine(tutorial1());
                break;
        }
    }

    IEnumerator tutorial1()
    {
        yield return new WaitForSeconds(0.1f);
        cam.freezeCam(true);
        player.setInteract(false);
        yield return new WaitForSeconds(1);
        UI_Screen.SetActive(true);
        yield return new WaitForSeconds(1);
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
        cam.freezeCam(false);
        objectiveText.text = "Move the camera with WASD";
        while (!camMoved)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1);
        cam.freezeCam(true);
        UI_Screen.SetActive(true);
        StartCoroutine(setText());
    }

    IEnumerator setText()
    {
        textFinished = false;
        tutorialText.text = "";
        foreach(char c in texts[index])
        {
            tutorialText.text += c;
            yield return new WaitForSeconds(0.05f);
            if (!textFinished && Input.GetKeyDown(KeyCode.Space))
            {
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
}
