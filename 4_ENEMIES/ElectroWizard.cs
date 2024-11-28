using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectroWizard : MonoBehaviour
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
            StartCoroutine(blackOut());
        }
    }

    IEnumerator lateStart()
    {
        yield return new WaitForSeconds(0.1f);
        if (PlayerPrefs.GetInt("DIFFICULTY") >= 2)
        {
            manager.turnPathElectro(true);
        }
        StartCoroutine(conveyerPath());
    }

    IEnumerator conveyerPath()
    {
        manager.turnPathElectro(false);
        yield return new WaitForSeconds(4f);
        StartCoroutine(conveyerPath());
    }

    public IEnumerator blackOut()
    {
        if (!won)
        {
            yield return new WaitForSeconds(2000);
        }
    }

    public void gameEnded()
    {
        won = true;
    }
}
