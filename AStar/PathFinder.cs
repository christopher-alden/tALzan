using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    private Grid grid;
    public Vector3[] daPath;
    List<Node> traversedNodes;
    List<Node> optimizedNodes;
    Node.Quadrant currQuadrant;
    public Dictionary<Node.Quadrant, Vector3[]> QuadrantWaypoints { get; private set; } =  new Dictionary<Node.Quadrant, Vector3[]>();
    public Dictionary<Node.Quadrant, List<Node>> QuadrantPaths { get; private set; } = new Dictionary<Node.Quadrant, List<Node>>();
    public Dictionary<Node.Quadrant, List<Node>> OldPath { get; private set; } = new Dictionary<Node.Quadrant, List<Node>>();
    public bool drawOld, drawOptimized;
    public delegate void OnPathsGenerated();
    public static event OnPathsGenerated PathsGenerated;


    #region Singleton
    private static PathFinder instance;
    public static PathFinder Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            grid = GetComponent<Grid>();
            
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    private void Start()
    {
        GeneratePathsForQuadrants();
    }

    public void GeneratePathsForQuadrants()
    {
        for (int i = 0; i < 4; i++)
        {
            traversedNodes = new List<Node>();
            optimizedNodes = new List<Node>();
            currQuadrant = (Node.Quadrant)i;
            List<Node> points = grid.randomPointsByQuadrant[currQuadrant];

            for (int j = 0; j < points.Count - 1; j++)
            {
                FindPath(points[j], points[j + 1]);
            }
            OldPath[currQuadrant] = traversedNodes;
            OptimizePath(traversedNodes);

    
            AddAll(currQuadrant, optimizedNodes);
        }
        PathsGenerated?.Invoke();
    }

    private void AddAll(Node.Quadrant quadrant, List<Node> path)
    {
        QuadrantPaths[quadrant] = path;
        QuadrantWaypoints[quadrant] = SimplePath(path);
    }


    void FindPath(Node startNode, Node targetNode)
    {
        Vector3[] wayPoints = new Vector3[0];
        bool pathSuccess = false;

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closeSet = new HashSet<Node>();

        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closeSet.Add(currentNode);
            

            if (currentNode == targetNode)
            {
                pathSuccess = true;
                break;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closeSet.Contains(neighbour)) continue;
                int newCostToNeighbour = currentNode.gCost + getDistance(currentNode, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = getDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                    else openSet.UpdateItem(neighbour);
                }
            }
        }
        //yield return null;
        if (pathSuccess)
        {
            Retrace(startNode, targetNode);
        }

    }

    void OptimizePath(List<Node> path)
    {

        Node start = path[0];
        Node last = path[^1];

        bool pathSuccess = false;

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closeSet = new HashSet<Node>();

        openSet.Add(start);
        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closeSet.Add(currentNode);


            if (currentNode == last)
            {
                pathSuccess = true;
                break;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closeSet.Contains(neighbour) || !path.Contains(neighbour)) continue;
                int newCostToNeighbour = currentNode.gCost + getDistance(currentNode, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = getDistance(neighbour, last);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour)) {
                        openSet.Add(neighbour);
                    }
                    else openSet.UpdateItem(neighbour);
                }
            }
        }
        if (pathSuccess)
        {
            optimizedRetrace(start, last);
        }

        
    }

    void Retrace(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);

            currentNode = currentNode.parent;
        }
        path.Reverse();
        traversedNodes.AddRange(path);
    }

    void optimizedRetrace(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
            

            if (currentNode == null)
            {
                Debug.Log("Path broke before reaching the start node");
                break;
            }
        }
        path.Reverse();
        optimizedNodes.AddRange(path);
        //Vector3[] waypoints = SimplePath(path);
        //Array.Reverse(waypoints);
        //Debug.Log(currQuadrant);
        //foreach(Vector3 way in waypoints){
        //    Debug.Log(way);
        //}
        //return waypoints;
    }

    Vector3[] SimplePath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            //Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            //if (directionNew != directionOld)
            //{
            //    waypoints.Add(path[i].worldPos);
            //}
            //directionOld = directionNew;
            waypoints.Add(path[i].worldPos);
        }
        return waypoints.ToArray();
    }

    int getDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY) return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    private void OnDrawGizmos()
    {
        if (daPath != null)
        {
            foreach (var path in daPath)
            {
                Gizmos.color = Color.grey;
                Gizmos.DrawCube(path, Vector3.one * (2f - .0f));
            }
        }

        if (drawOld)
        {
            foreach (var quadrant in OldPath.Keys)
            {
                List<Node> points = OldPath[quadrant];
                for (int i = 0; i < points.Count; i++)
                {
                    Gizmos.color = Color.grey;
                    Gizmos.DrawCube(points[i].worldPos, Vector3.one * (2.0f));
                }

            }
        }

        if (drawOptimized)
        {
            foreach (var quadrant in QuadrantPaths.Keys)
            {
                List<Node> points = QuadrantPaths[quadrant];
                for (int i = 0; i < points.Count; i++)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawCube(points[i].worldPos, Vector3.one * (2.0f));
                }

            }
        }
        
    }

}
