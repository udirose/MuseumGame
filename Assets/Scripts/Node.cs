using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IComparer<Node>
{
    public Vector3Int GridPosition { get; set; }
    public Node Parent { get; set; }
    public float GCost { get; set; }
    public float HCost { get; set; }
    public float FCost { get { return GCost + HCost; } }

    public Node(Vector3Int gridPosition, Node parent, float gCost, float hCost)
    {
        GridPosition = gridPosition;
        Parent = parent;
        GCost = gCost;
        HCost = hCost;
    }

    public int Compare(Node nodeA, Node nodeB)
    {
        int compare = nodeA.FCost.CompareTo(nodeB.FCost);
        if (compare == 0)
        {
            compare = nodeA.HCost.CompareTo(nodeB.HCost);
        }
        return compare;
    }
}
