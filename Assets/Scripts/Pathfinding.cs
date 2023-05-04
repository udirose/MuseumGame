using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour
{
    public Tilemap Tilemap;

    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int destination)
    {
        SortedSet<Node> openSet = new SortedSet<Node>(new Node(default, null, 0, 0));
        HashSet<Node> closedSet = new HashSet<Node>();

        Node startNode = new Node(start, null, 0, Vector3Int.Distance(start, destination));
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.Min; // Get the node with the lowest FCost
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode.GridPosition == destination)
            {
                return RetracePath(startNode, currentNode);
            }

            List<Vector3Int> neighbors = GetNeighbors(currentNode.GridPosition);
            foreach (Vector3Int neighborPos in neighbors)
            {
                Node neighbor = new Node(neighborPos, currentNode, currentNode.GCost + 1, Vector3Int.Distance(neighborPos, destination));

                if (closedSet.Contains(neighbor)) continue;

                if (!openSet.Contains(neighbor) || currentNode.GCost + 1 < neighbor.GCost)
                {
                    neighbor.GCost = currentNode.GCost + 1;
                    neighbor.Parent = currentNode;

                    if (openSet.Contains(neighbor))
                    {
                        openSet.Remove(neighbor);
                    }

                    openSet.Add(neighbor);
                }
            }
        }

        return null;
    }

    private List<Vector3Int> GetNeighbors(Vector3Int nodePosition)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        // Define the four possible orthogonal movements (up, down, left, right)
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(0, 1, 0), // up
            new Vector3Int(0, -1, 0), // down
            new Vector3Int(1, 0, 0), // right
            new Vector3Int(-1, 0, 0) // left
        };

        // Check for valid neighbors in each direction
        foreach (Vector3Int direction in directions)
        {
            Vector3Int neighborPosition = nodePosition + direction;
            if (Tilemap.HasTile(neighborPosition))
            {
                neighbors.Add(neighborPosition);
            }
        }

        return neighbors;
    }

    private List<Vector3Int> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.GridPosition);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }
    public void VisualizePath(List<Vector3Int> path)
    {
        if (path == null) return;

        Color pathColor = new Color(0, 255, 0, 1f);
        foreach (Vector3Int position in path)
        {
            Tilemap.SetTileFlags(position, TileFlags.None);
            Tilemap.SetColor(position, pathColor);
        }
    }
}