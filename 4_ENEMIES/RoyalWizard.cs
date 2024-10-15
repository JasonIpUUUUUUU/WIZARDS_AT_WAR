using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoyalWizard : MonoBehaviour
{
    [SerializeField]
    private string[] knightNodeNames;

    private Manager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("MANAGER").GetComponent<Manager>();
        StartCoroutine(lateStart());
    }

    IEnumerator lateStart()
    {
        yield return new WaitForSeconds(0.1f);
        foreach (string nodeName in knightNodeNames)
        {
            Node knightNode = GameObject.Find(nodeName).GetComponent<Node>();
            if(knightNode.getType() != "root node")
            {
                knightNode.changeState("blue");
                knightNode.changeState("knight");
                knightNode.modifyManPower(50, true, false);
                knightNode.setKnightStrength(10, 6, 20);
            }
            else
            {
                knightNode.changeState("knight");
                knightNode.setKnightStrength(20, 10, 30);
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
}
