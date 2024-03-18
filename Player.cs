using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //this script with be local to each player
    private bool showing;

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
    public void showUI()
    {
        if (showing)
        {
            hideUI();
        }
        showing = true;
        current_UI = Instantiate(UI_Prefab, UI_Canvas.transform);
        current_UI.transform.localPosition = new Vector3(0, -Screen.height * 1.5f);
        current_UI.LeanMoveLocalY(-Screen.height * 0.35f, 1).setEaseOutExpo();
    }

    //this is to hide the UI
    public void hideUI()
    {
        showing = false;
        current_UI.LeanMoveLocalY(-Screen.height * 2, 0.5f);
        Destroy(current_UI, 2);
    }

    //this is run when the background is clicked
    public void emptySpace()
    {
        if (showing)
        {
            hideUI();
        }
        node = null;
    }

    //this is run when a node is clicked
    public void onNodeClicked(GameObject nodeArg)
    {
        showUI();
        node = nodeArg;
        cam.addPosition(node.transform.position);
    }
}
