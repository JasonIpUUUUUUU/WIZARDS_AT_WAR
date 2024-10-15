using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    [SerializeField]
    private int maxHealth, currentHealth;

    private Node rootNode;

    private Manager manager;

    // Update is called once per frame
    void Update()
    {
        currentHealth = rootNode.returnManpower();
        manager = GameObject.FindGameObjectWithTag("MANAGER").GetComponent<Manager>();
    }

    public int linkNode(Node nodeParam)
    {
        currentHealth = maxHealth;
        rootNode = nodeParam;
        return currentHealth;
    }
}
