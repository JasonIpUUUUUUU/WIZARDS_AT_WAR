using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoyalWizard : MonoBehaviour
{
    private bool phase2Started = false, won = false;

    [SerializeField]
    private string[] knightNodeNames;

    [SerializeField]
    private Color goldColor;

    [SerializeField]
    private SpriteRenderer bg;

    [SerializeField]
    private GameObject goldCanvas, coolFog;

    [SerializeField]
    private CanvasGroup goldenAlpha;

    private Manager manager;

    private BossBehaviour boss;

    // Start is called before the first frame update
    void Start()
    {
        boss = GetComponent<BossBehaviour>();
        manager = GameObject.FindGameObjectWithTag("MANAGER").GetComponent<Manager>();
        StartCoroutine(lateStart());
    }

    private void Update()
    {
        if (boss.returnPhase() && !phase2Started)
        {
            phase2Started = true;
            StartCoroutine(goldenScreen());
        }
    }

    IEnumerator lateStart()
    {
        yield return new WaitForSeconds(0.1f);
        foreach (string nodeName in knightNodeNames)
        {
            Node knightNode = GameObject.Find(nodeName).GetComponent<Node>();
            if(knightNode.getType() != "boss")
            {
                knightNode.setKnightStrength(10, 6, 20);
                knightNode.changeState("blue");
                knightNode.changeState("knight");
                knightNode.modifyManPower(50, true, false);
            }
            else
            {
                knightNode.setKnightStrength(20, 10, 30);
                knightNode.changeState("knight");
            }
        }
        yield return new WaitForSeconds(2);
        StartCoroutine(goldenPath());
    }

    IEnumerator goldenPath()
    {
        manager.turnPathGold();
        yield return new WaitForSeconds(8);
        StartCoroutine(goldenPath());
    }

    public IEnumerator goldenScreen()
    {
        if (!won)
        {
            goldCanvas.SetActive(true);
            float duration = 1f, elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                goldenAlpha.alpha = elapsedTime / duration;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            bg.color = goldColor;
            coolFog.SetActive(true);
            manager.makeRoyalNodes();
            elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                goldenAlpha.alpha = 1 - elapsedTime / duration;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(2);
            yield return new WaitForSeconds(20);
            StartCoroutine(goldenScreen());
        }
    }

    public void gameEnded()
    {
        won = true;
    }
}
