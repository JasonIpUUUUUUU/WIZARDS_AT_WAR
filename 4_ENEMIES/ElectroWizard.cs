using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectroWizard : MonoBehaviour
{
    private bool phase2Started = false, won = false;

    [SerializeField]
    private Transform camHolderPos;

    [SerializeField]
    private AudioSource audio;

    [SerializeField]
    private SpriteRenderer bg;

    [SerializeField]
    private GameObject blackBlock;

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
        yield return new WaitForSeconds(3f);
        manager.turnPathElectro(false);
        StartCoroutine(conveyerPath());
    }

    public IEnumerator blackOut()
    {
        if (!won)
        {
            audio.Stop();
            blackBlock.SetActive(true);
            manager.blackOut(2);
            camHolderPos.position = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
            if(PlayerPrefs.GetInt("DIFFICULTY") == 3)
            {
                for(int i = 0; i < 2; i++)
                {
                    boss.returnRootNode().createElectro(30, true);
                    yield return new WaitForSeconds(1);
                }
            }
            else
            {
                yield return new WaitForSeconds(2);
            }
            blackBlock.SetActive(false);
            audio.Play();
            yield return new WaitForSeconds(Random.Range(6f, 18f));
            StartCoroutine(blackOut());
        }
    }

    public void gameEnded()
    {
        won = true;
    }
}
