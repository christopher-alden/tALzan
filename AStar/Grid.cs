using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool displayGizmos;
    public Transform player;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;
    private Terrain terrain;

    public Dictionary<Node.Quadrant, List<Node>> randomPointsByQuadrant;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        randomPointsByQuadrant = new Dictionary<Node.Quadrant, List<Node>>();
        terrain = Terrain.activeTerrain;
        CreateGrid();
        GenereateRandomPointsInQuadrants();
    }

    public int MaxSize
    {
        get { return gridSizeX * gridSizeY; }
    }

    Vector3 AlignWithTerrain(Vector3 position)
    {
        float height = terrain.SampleHeight(position);
        position.y = height;
        return position;
    }
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

                worldPoint = AlignWithTerrain(worldPoint);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                Node.Quadrant quadrant = CheckQuadrant(x,y);


                Node nn = new Node(walkable, worldPoint, x, y, quadrant);


                grid[x, y] = nn;
            }
        }
    }

    void GenereateRandomPointsInQuadrants()
    {
        randomPointsByQuadrant.Clear();
        for (int i = 0; i < 4; i++)
        {
            Node.Quadrant quadrant = (Node.Quadrant)i;

            var quadrantPoints = new List<Node> {
                GenerateStartPoint(quadrant)
            };

            for (int j = 0; j < 4; j++)
            {
                quadrantPoints.Add(GenerateRandomPoint(quadrant));
            }
            quadrantPoints.Add(GenerateEndPoint());
            randomPointsByQuadrant[quadrant] = quadrantPoints;
        }

        Debug.Log("done generating");
    }


    Node GenerateStartPoint(Node.Quadrant quadrant)
    {
        int halfX = gridSizeX / 2;
        int halfY = gridSizeY / 2;

        if (quadrant == Node.Quadrant.top)
            return grid[0, halfY];
        else if (quadrant == Node.Quadrant.right)
            return grid[halfX, gridSizeY - 1];
        else if (quadrant == Node.Quadrant.bottom)
            return grid[gridSizeX - 1, halfY];
        else if (quadrant == Node.Quadrant.left)
            return grid[halfX, 0];
        else
            return null;
    }

    Node GenerateEndPoint()
    {
        int halfX = Mathf.FloorToInt(gridSizeX / 2);
        int halfY = Mathf.FloorToInt(gridSizeY / 2);
        return grid[halfX, halfY];
    }

    public bool isStartPoint(Node.Quadrant quadrant, Node node)
    {
        int halfX = gridSizeX / 2;
        int halfY = gridSizeY / 2;

        if (quadrant == Node.Quadrant.top)
        {
            if (node.gridX == 0 && node.gridY == halfY) return true;
        }
        else if (quadrant == Node.Quadrant.right)
        {
            if (node.gridX == halfX && node.gridY == gridSizeY - 1) return true;
        }
        else if (quadrant == Node.Quadrant.bottom)
        {
            if (node.gridX == gridSizeX - 1 && node.gridY == halfY) return true;
        }
        else if (quadrant == Node.Quadrant.left)
        {
            if (node.gridX == halfX && node.gridY == 0) return true;
        }
        return false;
    }

    public bool isEndPoint(Node node)
    {
        int halfX = Mathf.FloorToInt(gridSizeX / 2);
        int halfY = Mathf.FloorToInt(gridSizeY / 2);

        if (node.gridX == halfX && node.gridY == halfY) return true;
        return false;
    }

    Node GenerateRandomPoint(Node.Quadrant _quadrant)
    {
        int pointX;
        int pointY;
        Node.Quadrant quadrant;
        int maxAttempts = 1000;
        int currentAttempt = 0;
        do
        {
            pointX = Random.Range(0, gridSizeX);
            pointY = Random.Range(0, gridSizeY);
            quadrant = CheckQuadrant(pointX, pointY);
            currentAttempt++;
            if (currentAttempt > maxAttempts) break;
        } while (quadrant != _quadrant);


        return grid[pointX, pointY];
    }



    public Node.Quadrant CheckQuadrant(int x, int y)
    {
        if (y == x || y == gridSizeY - x - 1)
            return Node.Quadrant.idk;

        bool aboveMainDiagonal = y > x;
        bool aboveAntiDiagonal = x + y < gridSizeY;

        if (aboveMainDiagonal)
        {
            if (aboveAntiDiagonal)
                return Node.Quadrant.top;
            return Node.Quadrant.right;
        }
        else
        {
            if (aboveAntiDiagonal)
                return Node.Quadrant.left;
            return Node.Quadrant.bottom;
        }
    }




    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = Mathf.Clamp01((worldPosition.x - (transform.position.x - gridWorldSize.x / 2)) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPosition.z - (transform.position.z - gridWorldSize.y / 2)) / gridWorldSize.y);

        int x = Mathf.RoundToInt(percentX * (gridSizeX - 1));
        int y = Mathf.RoundToInt(percentY * (gridSizeY - 1));

        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null && displayGizmos)
        {
            foreach (Node n in grid)
            {
               
                if (n.currQuadrant == Node.Quadrant.top)
                    Gizmos.color = Color.green;
                else if (n.currQuadrant == Node.Quadrant.right)
                    Gizmos.color = Color.blue;
                else if (n.currQuadrant == Node.Quadrant.bottom)
                    Gizmos.color = Color.yellow;
                else if (n.currQuadrant == Node.Quadrant.left)
                    Gizmos.color = Color.magenta;
                else
                    Gizmos.color = Color.black;

                Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeDiameter - .0f));
            }

            foreach (var quadrant in randomPointsByQuadrant.Keys)
            {
                List<Node> points = randomPointsByQuadrant[quadrant];
                for (int i = 0; i < points.Count; i++)
                {
                    
                    Gizmos.color = Color.cyan;
                    if (isStartPoint(quadrant, points[i])) Gizmos.color = Color.red;
                    if (isEndPoint(points[i])) Gizmos.color = Color.gray;
                    Gizmos.DrawCube(points[i].worldPos, Vector3.one * (nodeDiameter - .0f));
                   
                }

            }

        }
    }
}
