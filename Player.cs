using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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

    public void hideUI()
    {
        showing = false;
        current_UI.LeanMoveLocalY(-Screen.height * 2, 1f).setEaseInExpo();
        Destroy(current_UI, 2);
    }

    public void emptySpace()
    {
        if (showing)
        {
            hideUI();
        }
        node = null;
    }

    public void onNodeClicked(GameObject nodeArg)
    {
        showUI();
        node = nodeArg;
        cam.addPosition(node.transform.position);
    }
}
