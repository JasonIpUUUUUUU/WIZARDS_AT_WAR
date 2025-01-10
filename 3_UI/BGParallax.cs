using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGParallax : MonoBehaviour
{
    [SerializeField]
    private float parallax;

    private Vector2 initialPos, screenCenter;

    private void Start()
    {
        initialPos = transform.position;
        screenCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
    }

    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float distanceX = (mousePos.x - screenCenter.x) * parallax;
        float distanceY = (mousePos.y - screenCenter.y) * parallax;

        transform.position = new Vector2(initialPos.x + distanceX, initialPos.y + distanceY);
    }
}
