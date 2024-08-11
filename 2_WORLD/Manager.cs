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
            //neighbourCount is the number of neighbours the node should have
            for(int x = 0 ; x < neighbourCount[i]; x++)
            {
                GameObject tempPath = Instantiate(edge);
                tempPath.transform.position = Vector3.zero;
                //set the starting and end point of the line so it connects the two ndoes
                tempPath.GetComponent<LineRenderer>().SetPosition(0, nodes[i].transform.position);
                tempPath.GetComponent<LineRenderer>().SetPosition(1, nodes[neigbours[currentNeighbour]].transform.position);
                //set the distance the line represents in the edges script
                tempPath.GetComponent<edges>().setDistance(distances[currentNeighbour]);
                //add the neighbour node into the neighbour list of the node it is referencing.
                nodes[i].GetComponent<Node>().addNeighbour(nodes[neigbours[currentNeighbour]], tempPath, distances[currentNeighbour], true);
                currentNeighbour++;
            }
        }
    }

    // This function returns a list of tuples with the first variable being the node and the second being the distance to said node, this selects the optimal path via dijkstra's algorithm
    // it takes the starting node and target node as parameters
    public List<(GameObject, int)> d_Algorithm(GameObject start, GameObject target)
    {
        // the list of nodes for the optimal path
        List<GameObject> path = new List<GameObject>();

        // a dictionary containing the node as the key and the distance as the value so the shortest distance to a specific node can be found
        Dictionary<GameObject, int> distances = new Dictionary<GameObject, int>();

        // a dictionary containing the current node as the key and the previous node to reaching that node as the value
        Dictionary<GameObject, GameObject> previous = new Dictionary<GameObject, GameObject>();

        // a HashSet to keep track of all visited nodes
        HashSet<GameObject> visited = new HashSet<GameObject>();

        // the final output which adds the information of distance to the path
        List<(GameObject, int)> output = new List<(GameObject, int)>();

        foreach (GameObject node in nodes)
        {
            distances[node] = int.MaxValue;
            previous[node] = null;
        }

        distances[start] = 0;

        // a priority queue which sorts nodes based on distances as they are appended and focuses on nodes with least distance
        SortedSet<GameObject> queue = new SortedSet<GameObject>(Comparer<GameObject>.Create((a, b) =>
        {
            //checks the path with a shorter distance
            int thing = distances[a].CompareTo(distances[b]);

            //checks if they are the same gameobject so even if they have the same key value, they are considered to be different
            return thing != 0 ? thing : a.GetHashCode().CompareTo(b.GetHashCode());
        }));

        queue.Add(start);

        // a while loop to find the optimal path by checking the shortest distance to the next node until the target is reached
        while (queue.Count > 0)
        {
            // Get the node with the smallest distance and mark it as visited
            GameObject currentNode = queue.Min;
            queue.Remove(currentNode);
            visited.Add(currentNode);

            // If the target node is reached, reconstruct the path and return it
            if (currentNode == target)
            {
                GameObject node = target;
                while (node != null)
                {
                    path.Insert(0, node);
                    node = previous[node];
                }
                int prevTime = 0;
                for(int i = 1; i < path.Count; i++)
                {
                    output.Add((path[i], distances[path[i]] - prevTime));
                    prevTime = distances[path[i]];
                }
                return output;
            }

            // iterate through each of the neighbours to find the next node
            Node nodeScript = currentNode.GetComponent<Node>();
            for(int i = 0; i < nodeScript.neighbours.Count; i++)
            {
                // Skip visited nodes and nodes not on the same team (except the target node)
                if (visited.Contains(nodeScript.neighbours[i]) || (!nodeScript.sameTeam() && nodeScript.gameObject != target))
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
