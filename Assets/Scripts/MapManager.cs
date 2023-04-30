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
    //private ObjectPool<OverlayTile> tilePool;
    private Coroutine generateMapCoroutine;

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
        //tilePool = new ObjectPool<OverlayTile>(overlayTilePrefab, 500);
        generateMapCoroutine = StartCoroutine(GenerateMap());
    }

    private IEnumerator GenerateMap()
    {
        var tileMap = gameObject.GetComponentInChildren<Tilemap>();
        var bounds = tileMap.cellBounds;
        var tileMapRenderer = tileMap.GetComponent<TilemapRenderer>();
        
        //batching
        int batchSize = 50;
        int batchCounter = 0;
        
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
                    overlayTile.spriteRenderer.sortingOrder = tileMapRenderer.sortingOrder;
                    overlayMap.Add(tileKey,overlayTile);
                    
                    //batch handling
                    batchCounter++;
                    if (batchCounter >= batchSize)
                    {
                        batchCounter = 0;
                        yield return null;
                    }
                }
            }
        }

        //places npc at specific starting tile. end tile is mouse click
        GameObject newNPC = Instantiate(NPC);
        newNPC.GetComponent<NPC>().SetActiveTile(overlayMap[new Vector2Int(-12,12)]);
    }

    // Update is called once per frame
    private OverlayTile lastMouseOverTile = null;
    void Update()
    {
        // Get the current mouse position
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int mouseGridPosition = gameObject.GetComponentInChildren<Tilemap>().WorldToCell(mouseWorldPosition);
        Vector2Int mouseTileKey = new Vector2Int(mouseGridPosition.x, mouseGridPosition.y);

        // Update only the tile under the mouse
        if (overlayMap.ContainsKey(mouseTileKey))
        {
            // Hide the previously hovered tile if the mouse moved to a different tile
            if (lastMouseOverTile != null && lastMouseOverTile != overlayMap[mouseTileKey])
            {
                lastMouseOverTile.HideTile();
            }

            // Handle mouse input for the current tile
            overlayMap[mouseTileKey].HandleMouseInput();

            // Update the last mouse over tile
            lastMouseOverTile = overlayMap[mouseTileKey];
        }
        else
        {
            // If the mouse is not over any tile, hide the last hovered tile
            if (lastMouseOverTile != null)
            {
                lastMouseOverTile.HideTile();
                lastMouseOverTile = null;
            }
        }
    }
    void OnDestroy()
    {
        if (generateMapCoroutine != null)
        {
            StopCoroutine(generateMapCoroutine);
        }

        if (overlayMap != null)
        {
            foreach (var overlayTile in overlayMap.Values)
            {
                if (overlayTile != null)
                {
                    DestroyImmediate(overlayTile.gameObject);
                }
            }
            overlayMap.Clear();
        }
    }
}
