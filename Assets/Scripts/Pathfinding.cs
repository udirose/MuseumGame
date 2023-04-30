using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding
{
    //script based on https://www.youtube.com/watch?v=u3hfWOCiIPg
    public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end)
    {
        List<OverlayTile> openList = new List<OverlayTile>();
        List<OverlayTile> closedList = new List<OverlayTile>();
        
        openList.Add(start);

        while (openList.Count > 0)
        {
            //OverlayTile currentOverlayTile = openList.OrderBy(x => x.F).First();
            OverlayTile currentOverlayTile = openList.OrderBy(x => x.F).FirstOrDefault();
            if (currentOverlayTile == null)
            {
                break;
            }
            openList.Remove(currentOverlayTile);
            closedList.Add(currentOverlayTile);

            if (currentOverlayTile == end)
            {
                return GetFinishedList(start, end);
            }

            var neighborTiles = GetNeighbourTiles(currentOverlayTile);

            foreach (var neighbour in neighborTiles)
            {
                //1 is the jump height
                if (neighbour.isBlocked || closedList.Contains(neighbour) ||
                    Mathf.Abs(currentOverlayTile.gridLocation.z - neighbour.gridLocation.z) > 1)
                {
                    continue;
                }
                
                //calc g h and f values: manhattan distance
                neighbour.G = GetManhattanDistance(start, neighbour);
                neighbour.H = GetManhattanDistance(end, neighbour);

                neighbour.previous = currentOverlayTile;

                if (!openList.Contains(neighbour))
                {
                    openList.Add(neighbour);
                }
            }
        }

        return new List<OverlayTile>();

    }

    private List<OverlayTile> GetFinishedList(OverlayTile start, OverlayTile end)
    {
        List<OverlayTile> finishedList = new List<OverlayTile>();

        OverlayTile currentTile = end;

        while (currentTile != start)
        {
            finishedList.Add(currentTile);
            currentTile = currentTile.previous;
        }

        finishedList.Reverse();
        return finishedList;
    }

    private int GetManhattanDistance(OverlayTile start, OverlayTile neighbour)
    {
        return Mathf.Abs(start.gridLocation.x - neighbour.gridLocation.x) +
               Mathf.Abs(start.gridLocation.y - neighbour.gridLocation.y);
    }

    private List<OverlayTile> GetNeighbourTiles(OverlayTile currentOverlayTile)
    {
        var map = MapManager.Instance.overlayMap;
        List<OverlayTile> neighbours = new List<OverlayTile>();
        
        //top
        Vector2Int locationToCheck =
            new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y + 1);

        if (map.ContainsKey(locationToCheck))
        {
            neighbours.Add(map[locationToCheck]);
        }
        
        //bottom
        locationToCheck =
            new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y - 1);

        if (map.ContainsKey(locationToCheck))
        {
            neighbours.Add(map[locationToCheck]);
        }
        
        //left
        locationToCheck =
            new Vector2Int(currentOverlayTile.gridLocation.x-1, currentOverlayTile.gridLocation.y);

        if (map.ContainsKey(locationToCheck))
        {
            neighbours.Add(map[locationToCheck]);
        }
        
        //right
        locationToCheck =
            new Vector2Int(currentOverlayTile.gridLocation.x+1, currentOverlayTile.gridLocation.y);

        if (map.ContainsKey(locationToCheck))
        {
            neighbours.Add(map[locationToCheck]);
        }

        return neighbours;
    }
}
