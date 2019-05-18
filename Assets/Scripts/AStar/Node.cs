using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public int gridZ;

    public float gCost;
    public float hCost;
    public Node parent;

    public float fCost { get => gCost + hCost; }
    public int HeapIndex { get { return heapIndex; } set { heapIndex = value; } }

    private int heapIndex;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _gridZ)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        gridZ = _gridZ;
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
