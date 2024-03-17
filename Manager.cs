using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField]
    private int[] neighbourCount, neigbours, distances;

    [SerializeField]
    private Vector2[] spawnPositions;

    public List<GameObject> nodes;

    [SerializeField]
    private GameObject edge, node;

    [SerializeField]
    private Transform map;

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
        //firstly it spawn the nodes onto the map at the specified positions and sets its parent to the map
        for(int i = 0; i < spawnPositions.Length; i++)
        {
            GameObject tempNode = Instantiate(node, map);
            tempNode.transform.position = spawnPositions[i];
            nodes.Add(tempNode);
            if(i == 0)
            {
                tempNode.GetComponent<Node>().changeState("blue");
                tempNode.GetComponent<Node>().changeState("root");
            }
            else if(i == spawnPositions.Length - 1)
            {
                tempNode.GetComponent<Node>().changeState("red");
                tempNode.GetComponent<Node>().changeState("root");
            }
        }
        //then it assigns the neighbours to each of the nodes and creates paths between them
        int currentNeighbour = 0;
        for(int i = 0; i < nodes.Count; i++)
        {
            for(int x = 1; x <= neighbourCount[i]; x++)
            {
                GameObject tempPath = Instantiate(edge);
                tempPath.transform.position = Vector3.zero;
                tempPath.GetComponent<LineRenderer>().SetPosition(0, nodes[i].transform.position);
                tempPath.GetComponent<LineRenderer>().SetPosition(1, nodes[neigbours[currentNeighbour]].transform.position);
                tempPath.GetComponent<edges>().setDistance(distances[currentNeighbour]);
                nodes[i].GetComponent<Node>().addNeighbour(nodes[neigbours[currentNeighbour]], tempPath, true);
                currentNeighbour++;
            }
        }
    }

    public void win()
    {
        Debug.Log("win");
    }
}
