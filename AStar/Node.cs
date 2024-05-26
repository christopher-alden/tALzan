using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPos;
    public int gCost;
    public int hCost;

    public int gridX;
    public int gridY;
    public Node parent;

    public enum Quadrant
    {
        top = 0,
        right = 1,
        bottom = 2,
        left = 3,
        idk = 4
    }

    public Quadrant currQuadrant;

    public Node(bool Walkable, Vector3 WorldPos, int GridX, int GridY, Quadrant myQuadrant)
    {
        walkable = Walkable;
        worldPos = WorldPos;
        gridX = GridX;
        gridY = GridY;
        currQuadrant = myQuadrant;
    }
    public int fCost
    {
        get { return gCost + hCost; }
    }

    private int heapIndex;
    public int HeapIndex
    {
        get { return heapIndex; }
        set { heapIndex = value; }
    }
    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0) compare = hCost.CompareTo(nodeToCompare.hCost);
        return -compare;
    }
}
