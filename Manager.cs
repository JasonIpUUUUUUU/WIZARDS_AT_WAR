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
            tempNode.name = "node" + i.ToString();
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
                nodes[i].GetComponent<Node>().addNeighbour(nodes[neigbours[currentNeighbour]], tempPath, distances[currentNeighbour], true);
                currentNeighbour++;
            }
        }
    }

    public List<(GameObject, int)> d_Algorithm(GameObject start, GameObject target)
    {
        List<GameObject> path = new List<GameObject>();
        Dictionary<GameObject, int> distances = new Dictionary<GameObject, int>();
        Dictionary<GameObject, GameObject> previous = new Dictionary<GameObject, GameObject>();
        HashSet<GameObject> visited = new HashSet<GameObject>();

        List<(GameObject, int)> output = new List<(GameObject, int)>();

        foreach (GameObject node in nodes)
        {
            distances[node] = int.MaxValue;
            previous[node] = null;
        }

        distances[start] = 0;

        SortedSet<GameObject> queue = new SortedSet<GameObject>(Comparer<GameObject>.Create((a, b) =>
        {
            int cmp = distances[a].CompareTo(distances[b]);
            return cmp != 0 ? cmp : a.GetHashCode().CompareTo(b.GetHashCode());
        }));

        queue.Add(start);

        while (queue.Count > 0)
        { 
            GameObject currentNode = queue.Min;

            queue.Remove(currentNode);

            visited.Add(currentNode);

            if (currentNode == target)
            {
                GameObject node = target;
                while (node != null)
                {
                    path.Insert(0, node);
                    node = previous[node];
                }
                for(int i = 1; i < path.Count; i++)
                {
                    output.Add((path[i], distances[path[i]]));
                }
                return output;
            }

            Node nodeScript = currentNode.GetComponent<Node>();
            for(int i = 0; i < nodeScript.neighbours.Count; i++)
            {
                if (visited.Contains(nodeScript.neighbours[i]))
                {
                    continue;
                }

                int distanceToNeighbour = distances[currentNode] + nodeScript.distances[i];

                if (distanceToNeighbour < distances[nodeScript.neighbours[i]])
                {
                    distances[nodeScript.neighbours[i]] = distanceToNeighbour;
                    previous[nodeScript.neighbours[i]] = currentNode;

                    bool willAdd = true;
                    foreach(GameObject element in queue)
                    {
                        if(element == nodeScript.neighbours[i])
                        {
                            willAdd = false;
                            break;
                        }
                    }
                    if (willAdd)
                    {
                        queue.Add(nodeScript.neighbours[i]);
                    }
                }
            }
        }

        for (int i = 1; i < path.Count; i++)
        {
            output.Add((path[i], distances[path[i]]));
        }
        return output;
    }

    public void win()
    {
        Debug.Log("win");
    }
}
