using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    [SerializeField]
    private string boss;

    [SerializeField]
    private int maxHealth, currentHealth, tutorialIndex;

    private bool phase2 = false;

    [SerializeField]
    private Manager manager;

    private Node rootNode;

    private void Start()
    {
        if(boss == "TUTORIAL")
        {
            manager.tutorialSequence(tutorialIndex);
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentHealth = rootNode.returnManpower();
        if (manager.hasWon())
        {
            win();
        }
    }

    public int linkNode(Node nodeParam)
    {
        if(boss != "TUTORIAL")
        {
            maxHealth += PlayerPrefs.GetInt("DIFFICULTY") * 50;
        }
        currentHealth = maxHealth;
        rootNode = nodeParam;
        return currentHealth;
    }

    public int returnMaxHealth()
    {
        return maxHealth;
    }

    public void startPhase2()
    {
        phase2 = true;
    }

    public bool returnPhase()
    {
        return phase2;
    }

    public string returnBoss()
    {
        return boss;
    }

    public void win()
    {
        if(boss == "royal")
        {
            GetComponent<RoyalWizard>().gameEnded();
        }
    }
}
