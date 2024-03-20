using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //this script with be local to each player

    //showing refers to when the UI is shown, hiding is true when the UI is in the process of hiding and redTeam shows what team the player is in via a single variable
    [SerializeField]
    private bool showing, hiding, redTeam;

    [SerializeField]
    private GameObject node, UI_Prefab, current_UI;

    [SerializeField]
    private Canvas UI_Canvas;

    private MovingCam cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("CAMHOLDER").GetComponent<MovingCam>();
    }

    //this is to show the UI on node properties for one player
    public IEnumerator showUI()
    {
        if (!hiding)
        {
            if (showing)
            {
                StartCoroutine(hideUI());
                yield return new WaitForSeconds(0.25f);
            }
            showing = true;
            current_UI = Instantiate(UI_Prefab, UI_Canvas.transform);
            current_UI.transform.localPosition = new Vector3(0, -Screen.height * 1.5f);
            LeanTween.cancelAll();
            current_UI.LeanMoveLocalY(-Screen.height * 0.35f, 1).setEaseOutExpo();
            yield return new WaitForSeconds(0.5f);
        }
    }

    //this is to hide the UI
    public IEnumerator hideUI()
    {
        LeanTween.cancelAll();
        hiding = true;
        current_UI.LeanMoveLocalY(-Screen.height * 2, 0.25f);
        yield return new WaitForSeconds(0.25f);
        hiding = false;
        showing = false;
        node = null;
        Destroy(current_UI, 0.5f);
    }

    //this is run when the background is clicked
    public void emptySpace()
    {
        if (showing)
        {
            StartCoroutine(hideUI());
        }
        node = null;
    }

    //this is run when a node is clicked
    public void onNodeClicked(GameObject nodeArg)
    {
        if (node == nodeArg)
        {
            StartCoroutine(hideUI());
        }
        else
        {
            StartCoroutine(showUI());
            node = nodeArg;
            cam.addPosition(node.transform.position);
        }
    }
}
