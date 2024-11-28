using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoLight : MonoBehaviour
{
    [SerializeField]
    private float interval;

    private int prevNum;

    [SerializeField]
    private GameObject discoLight;

    private SpriteRenderer discoRenderer;

    [SerializeField]
    private Color[] possibleColors;

    // Start is called before the first frame update
    void Start()
    {
        discoRenderer = discoLight.GetComponent<SpriteRenderer>();
        StartCoroutine(discoBeat());
    }

    IEnumerator discoBeat()
    {
        int randoNum = Random.Range(0, possibleColors.Length);
        while(randoNum == prevNum)
        {
            randoNum = Random.Range(0, possibleColors.Length); 
        }
        prevNum = randoNum;
        discoRenderer.color = possibleColors[randoNum];
        yield return new WaitForSeconds(interval);
        StartCoroutine(discoBeat());
    }
}
