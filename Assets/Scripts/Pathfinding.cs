using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding
{
    public Tilemap tilemap;


    public Pathfinding(Tilemap tilemap)
    {
        this.tilemap = tilemap;

    }

    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int end)
    {
        List<Vector3Int> openList = new List<Vector3Int>();
        List<Vector3Int> closedList = new List<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> previousNodes = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, int> gValues = new Dictionary<Vector3Int, int>();
        Dictionary<Vector3Int, int> hValues = new Dictionary<Vector3Int, int>();

        openList.Add(start);
        gValues[start] = 0;
        hValues[start] = GetManhattanDistance(start, end);

        while (openList.Count > 0)
        {
            Vector3Int currentCell = openList.OrderBy(c => gValues[c] + hValues[c]).FirstOrDefault();

            if (currentCell == end)
            {
                return GetFinishedList(start, end, previousNodes);
            }

            openList.Remove(currentCell);
            closedList.Add(currentCell);

            var neighborCells = GetNeighbourCells(currentCell);

            foreach (var neighbour in neighborCells)
            {
                if (closedList.Contains(neighbour) || tilemap.HasTile(neighbour))
                {
                    continue;
                }

                int tentativeG = gValues[currentCell] + GetManhattanDistance(currentCell, neighbour);

                if (!openList.Contains(neighbour))
                {
                    openList.Add(neighbour);
                }
                else if (tentativeG >= gValues[neighbour])
                {
                    continue;
                }

                previousNodes[neighbour] = currentCell;
                gValues[neighbour] = tentativeG;
                hValues[neighbour] = GetManhattanDistance(neighbour, end);
            }
        }

        return new List<Vector3Int>();
    }

    private List<Vector3Int> GetFinishedList(Vector3Int start, Vector3Int end, Dictionary<Vector3Int, Vector3Int> previousNodes)
    {
        List<Vector3Int> finishedList = new List<Vector3Int>();

        Vector3Int currentCell = end;

        while (currentCell != start)
        {
            finishedList.Add(currentCell);
            currentCell = previousNodes[currentCell];
        }

        finishedList.Reverse();
        return finishedList;
    }

    private int GetManhattanDistance(Vector3Int start, Vector3Int neighbour)
    {
        return Mathf.Abs(start.x - neighbour.x) + Mathf.Abs(start.y - neighbour.y);
    }

    private List<Vector3Int> GetNeighbourCells(Vector3Int currentCell)
    {
        List<Vector3Int> neighbours = new List<Vector3Int>();

        Vector3Int[] directions = {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right
        };

        foreach (var direction in directions)
        {
            Vector3Int neighbour = currentCell + direction;

            if (tilemap.cellBounds.Contains(neighbour))
            {
                neighbours.Add(neighbour);
            }
        }

        return neighbours;
    }
}