using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NPC : MonoBehaviour
{
    public float moveSpeed;

    public Tilemap tilemap;
    private Animator anim;
    private List<Vector3Int> path;
    public Vector3Int activeTile;
    public Pathfinding pathfinder;

    void Start()
    {
        path = new List<Vector3Int>();
        anim = GetComponent<Animator>();
        activeTile = tilemap.WorldToCell(transform.position);
        PositionCharacterOnTile(activeTile);
    }

    // Update is called once per frame
    void Update()
    {
        //select tile
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            Vector3Int gridPos = tilemap.WorldToCell(mousePos);
            if (tilemap.HasTile(gridPos))
            {
                path = pathfinder.FindPath(activeTile, gridPos);
                //pathfinder.VisualizePath(path);
            }
            Debug.Log("Path count: " + path.Count);
        }
        //path found
        if (path.Count > 0)
        {
            MoveAlongPath();
        }
    }

    private void MoveAlongPath()
    {
        var step = moveSpeed * Time.deltaTime;
        var newLoc = tilemap.GetCellCenterWorld(path[0]);

        transform.position = Vector2.MoveTowards(transform.position, newLoc, step);
        anim.SetTrigger("moving");
        WalkAnimation(newLoc);

        if (Vector2.Distance(transform.position, newLoc) < 0.0001f)
        {
            PositionCharacterOnTile(path[0]);
            path.RemoveAt(0);
        }
    }

    public void PositionCharacterOnTile(Vector3Int tile)
    {
        var tileWorldPos = tilemap.CellToWorld(tile);
        var tileHeight = tilemap.cellSize.y;
        transform.position = new Vector3(tileWorldPos.x, tileWorldPos.y + tileHeight / 2f, tileWorldPos.z);
        GetComponent<SpriteRenderer>().sortingOrder = tilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;
        activeTile = tile;
    }

    public void SetActiveTile(Vector3Int tile)
    {
        activeTile = tile;
    }

    void WalkAnimation(Vector3 newLoc)
    {
        Vector3 dir = newLoc - transform.position;
        dir.z = 0f; // Ignore the z axis
        int signX = Mathf.RoundToInt(Mathf.Sign(dir.x));
        int signY = Mathf.RoundToInt(Mathf.Sign(dir.y));
        anim.SetFloat("dirX", signX);
        anim.SetFloat("dirY", signY);
    }
}
