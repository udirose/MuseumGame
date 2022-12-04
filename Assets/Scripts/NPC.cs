using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    private Pathfinding pathfinder;
    private List<OverlayTile> path;
    private OverlayTile activeTile;

    public OverlayTile start;

    public OverlayTile end;
    // Start is called before the first frame update
    void Start()
    {
        path = new List<OverlayTile>();
        pathfinder = new Pathfinding();
    }

    // Update is called once per frame
    void Update()
    {
        var path = pathfinder.FindPath(start, end);

        if (path.Count > 0)
        {
            MoveAlongPath();
        }
    }

    private void MoveAlongPath()
    {
        //replace 1 with speed
        var step = 1 * Time.deltaTime;
        var zIndex = path[0].transform.position.z;

        transform.position = Vector2.MoveTowards(transform.position, path[0].transform.position, step);
        transform.position = new Vector3(transform.position.x, transform.position.y, zIndex);

        if (Vector2.Distance(transform.position, path[0].transform.position) < 0.0001f)
        {
            PositionCharacterOnTile(path[0]);
            path.RemoveAt(0);
        }

    }

    private void PositionCharacterOnTile(OverlayTile tile)
    {
        var tilePos = tile.transform.position;
        transform.position = new Vector3(tilePos.x, tilePos.y, tilePos.z);
        GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
        activeTile = tile;
    }
}
