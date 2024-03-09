using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField]
    private int[] neighbourCount, neigbours;

    [SerializeField]
    private GameObject[] nodes;

    [SerializeField]
    private GameObject edges, node;

    // Start is called before the first frame update
    void Start()
    {
        spawnNodes();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void spawnNodes()
    {

    }
}
