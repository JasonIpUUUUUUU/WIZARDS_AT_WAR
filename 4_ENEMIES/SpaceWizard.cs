using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceWizard : MonoBehaviour
{
    private bool phase2Started = false, won = false, raging = false;

    [SerializeField]
    private MovingCam camMove;

    [SerializeField]
    private AudioSource audio;

    [SerializeField]
    private SpriteRenderer bg;

    [SerializeField]
    private GameObject blackHoleFront;

    [SerializeField]
    private Animator blackHoleAnim;

    [SerializeField]
    private AudioClip meteorSound;

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
            StartCoroutine(turnBlackHole());
            phase2Started = true;
        }
        if (manager.returnRage() && !raging)
        {
            StartCoroutine(rage());
            raging = true;
        }
    }

    IEnumerator rage()
    {
        for(int i = 0; i < 3; i++)
        {
            StartCoroutine(meteorLoop());
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator lateStart()
    {
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(meteorLoop());
        if(PlayerPrefs.GetInt("DIFFICULTY") == 3)
        {
            yield return new WaitForSeconds(4);
            StartCoroutine(meteorLoop());
        }
    }

    IEnumerator meteorLoop()
    {
        yield return new WaitForSeconds(8);
        audio.PlayOneShot(meteorSound);
        manager.summonMeteor(phase2Started);
        StartCoroutine(meteorLoop());
    }

    public IEnumerator turnBlackHole()
    {
        camMove.addPosition(Vector2.zero);
        blackHoleAnim.SetBool("Black", true);
        yield return new WaitForSeconds(0.01f);
        blackHoleAnim.SetBool("Black", false);
        blackHoleFront.SetActive(true);
        float elapsedTime = 0;
        SpriteRenderer blackRenderer = blackHoleFront.GetComponent<SpriteRenderer>();
        while(elapsedTime < 1)
        {
            blackRenderer.color = new Color32(255, 255, 255, (byte)(255 * elapsedTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        blackRenderer.color = new Color32(255, 255, 255, 255);
    }

    public void gameEnded()
    {
        won = true;
    }
}
