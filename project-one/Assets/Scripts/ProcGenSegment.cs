using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcGenSegment : MonoBehaviour
{
    [SerializeField] int test_node1, test_node2;

    [SerializeField] int length, height;
    [SerializeField] Dictionary<int, HashSet<int>> nodeAdjList;
    public int[] directionIDs;
        
    [SerializeField] GameObject edgeObj;
    [SerializeField] GameObject[] nodes;
    [SerializeField] List<GameObject> edges;

    [SerializeField] TileMapBuilder tileMapBuilder;
    // Start is called before the first frame update
    void Start()
    {
        currentSnake = new Stack<int>();
        snakeVisited = new HashSet<int>();

        edges = new List<GameObject>();
        nodeAdjList = new Dictionary<int, HashSet<int>>();
        directionIDs = new int[length * height];
        ClearEdges();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GenEdgesSnakePass(0);
            tileMapBuilder.Run();
        }
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            ClearEdges();
            tileMapBuilder.Clear();
        }
        if (Input.GetKeyDown(KeyCode.Slash)) {
            Debug.Log(IsConnected(test_node1, test_node2));
        }
        //if (Input.GetKeyDown(KeyCode.Space)) { DoSnakePassTick(); }
    }

    void ClearEdges()
    {
        nodeAdjList.Clear();
        for (int i = 0; i < length * height; i++)
        {
            nodeAdjList.Add(i, new HashSet<int>());
        }

        for (int i = 0; i < edges.Count; i++)
        {
            Destroy(edges[i]);
        }
        edges = new List<GameObject>();

        currentSnake.Clear();
        snakeVisited.Clear();
        loops = 0;
    }    

    void CreateEdge(int node1, int node2)
    {
        nodeAdjList[node1].Add(node2);
        nodeAdjList[node2].Add(node1);
        GameObject newEdge = Instantiate(edgeObj, (nodes[node1].transform.position + nodes[node2].transform.position) / 2, Quaternion.identity);
        newEdge.transform.right = nodes[node2].transform.position - newEdge.transform.position;
        edges.Add(newEdge);

        SetDirectionID(node1, node2);
    }

    void SetDirectionID(int node1, int node2)
    {
        int targetIndex = (height - 1 - node1 / length) * length + node1 % length;        
        directionIDs[targetIndex] += (node2 == node1 + 1) ? 8 : (node2 == node1 - 1) ? 4 : (node2 == node1 + length) ? 2 : (node2 == node1 - length) ? 1 : 0;

        targetIndex = (height - 1 - node2 / length) * length + node2 % length;
        directionIDs[targetIndex] += (node1 == node2 + 1) ? 8 : (node1 == node2 - 1) ? 4 : (node1 == node2 + length) ? 2 : (node1 == node2 - length) ? 1 : 0;
    }

    bool IsConnected(int node1, int node2)
    {
        if (node1 == node2) { return true; }
        if (nodeAdjList[node1].Contains(node2)) { return true; }
        foreach (int adjNode in nodeAdjList[node1])
        {
            nodeAdjList[adjNode].Remove(node1);
            if (IsConnected(adjNode, node2))
            {
                nodeAdjList[adjNode].Add(node1);
                return true;
            }
            nodeAdjList[adjNode].Add(node1);
        }
        return false;
    }

    #region SNAKE_PASS
    Stack<int> currentSnake;
    HashSet<int> snakeVisited;
    [SerializeField] int loops = 0;
    [SerializeField] int targetLoops = 1;

    void GenEdgesSnakePass(int startNode)
    {
        int snakeHead = startNode;
        currentSnake.Push(snakeHead);

        bool running = true;

        while (running)
        {
            if (snakeVisited.Contains(currentSnake.Peek()))
            {
                if (loops < targetLoops) { loops++; }
                while (!hasUnexploredAdjacent(currentSnake.Peek()))
                {
                    currentSnake.Pop();
                    if (currentSnake.Count == 0) { return; }
                }
            }
            else
            {
                snakeVisited.Add(currentSnake.Peek());
            }

            List<int> nodeOptions = getNodeOptions(currentSnake.Peek());
            int nextNode = nodeOptions[Random.Range(0, nodeOptions.Count)];

            if (snakeVisited.Contains(nextNode) && loops >= targetLoops) { continue; }

            CreateEdge(currentSnake.Peek(), nextNode);
            currentSnake.Push(nextNode);
        }        
    }

    List<int> getNodeOptions(int node)
    {
        List<int> result = new List<int>();
        if (node % length < length - 1 && !nodeAdjList[node].Contains(node + 1)) { result.Add(node + 1); }
        if (node % length > 0 && !nodeAdjList[node].Contains(node - 1)) { result.Add(node - 1); }
        if (node < length * height - length  && !nodeAdjList[node].Contains(node + length)) { result.Add(node + length); }
        if (node >= length && !nodeAdjList[node].Contains(node - length)) { result.Add(node - length); }
        return result;
    }
    
    bool hasUnexploredAdjacent(int node)
    {
        if (node % length < length - 1 && !snakeVisited.Contains(node + 1)) { return true; }
        if (node % length > 0 && !snakeVisited.Contains(node - 1)) { return true; }
        if (node < length * height - length && !snakeVisited.Contains(node + length)) { return true; }
        if (node >= length && !snakeVisited.Contains(node - length)) { return true; }
        return false;
    }
    #endregion
}
