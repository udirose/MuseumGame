using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public float moveSpeed;
    //outlets
    
    //internal
    private Animator anim;
    private Pathfinding pathfinder;
    private List<OverlayTile> path;
    private OverlayTile activeTile;
    

    void Start()
    {
        path = new List<OverlayTile>();
        pathfinder = new Pathfinding();
        anim = GetComponent<Animator>();
        activeTile=MapManager.Instance.overlayMap[new Vector2Int(0,0)];
        PositionCharacterOnTile(activeTile);
    }

    // Update is called once per frame
    void Update()
    {
        //select tile
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            //if hit this specific object
            if (hit.collider != null && hit.collider.gameObject.GetComponent<OverlayTile>())
            {
                path = pathfinder.FindPath(activeTile, hit.collider.gameObject.GetComponent<OverlayTile>());
            }
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
        var zIndex = path[0].transform.position.z;
        //fix not line up with tile (adds .5 to y pos)
        var newLoc = new Vector3(path[0].transform.position.x, path[0].transform.position.y + .5f,
            path[0].transform.position.z);

        transform.position = Vector2.MoveTowards(transform.position, newLoc, step);
        anim.SetTrigger("moving");
        WalkAnimation(newLoc);
        transform.position = new Vector3(transform.position.x, transform.position.y, zIndex);

        //not sure what value .0001f is
        if (Vector2.Distance(transform.position, newLoc) < 0.0001f)
        {
            PositionCharacterOnTile(path[0]);
            path.RemoveAt(0);
        }
        
    }

    private void PositionCharacterOnTile(OverlayTile tile)
    {
        var tilePos = tile.transform.position;
        transform.position = new Vector3(tilePos.x, tilePos.y+.5f, tilePos.z);
        GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder+1;
        activeTile = tile;
    }

    public void SetActiveTile(OverlayTile tile)
    {
        activeTile = tile;
    }

    void WalkAnimation(Vector3 newLoc)
    {
        Vector3 dir = newLoc - transform.position;
        dir.z = 0f; // Ignore the z axis
        // Get the sign of the x and y components of the direction
        int signX = Mathf.RoundToInt(Mathf.Sign(dir.x));
        int signY = Mathf.RoundToInt(Mathf.Sign(dir.y));
        // Set the dirX and dirY parameters in the Animator
        anim.SetFloat("dirX", signX);
        anim.SetFloat("dirY", signY);
    }
}
