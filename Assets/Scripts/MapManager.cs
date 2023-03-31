using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance => _instance;
    
    //outlets
    public OverlayTile overlayTilePrefab;
    public GameObject overlayContainer;
    public GameObject NPC;
    
    //tracks overlay tiles
    public Dictionary<Vector2Int, OverlayTile> overlayMap;

    //singleton
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        overlayMap = new Dictionary<Vector2Int, OverlayTile>();
        var tileMap = gameObject.GetComponentInChildren<Tilemap>();
        var bounds = tileMap.cellBounds;
        
        //generate map: loops through all drawn tiles on tilemap
        for (var z = bounds.max.z; z >= bounds.min.z; z--)
        {
            for (var y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (var x = bounds.min.x; x < bounds.max.x; x++)
                {
                    var tileLocation = new Vector3Int(x, y, z);
                    var tileKey = new Vector2Int(x, y);
                    if (!tileMap.HasTile(tileLocation) || overlayMap.ContainsKey(tileKey)) continue;
                    //create overlay and but it one z above
                    var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                    //to put position of grid in overlay object
                    overlayTile.gridLocation = tileLocation;
                    var cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);
                    overlayTile.transform.position = new Vector3(cellWorldPosition.x,cellWorldPosition.y,cellWorldPosition.z+1);
                    overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder;
                    overlayMap.Add(tileKey,overlayTile);
                }
            }
        }

        //places npc at specific starting tile. end tile is mouse click
        GameObject newNPC = Instantiate(NPC);
        newNPC.GetComponent<NPC>().activeTile = overlayMap[new Vector2Int(-12,12)];
        //newNPC.activeTile = -12 12 0
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
