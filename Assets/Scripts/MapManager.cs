using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private ObjectPool<OverlayTile> tilePool;
    private Coroutine generateMapCoroutine;
    private Dictionary<Vector2Int, OverlayTile> activeTiles;
    private Queue<OverlayTile> inactiveTiles;
    private Tilemap tileMap;
    private TilemapRenderer tileMapRenderer;
    private BoundsInt bounds;

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
        activeTiles = new Dictionary<Vector2Int, OverlayTile>();
        inactiveTiles = new Queue<OverlayTile>();
        tilePool = new ObjectPool<OverlayTile>(overlayTilePrefab, 500);
        generateMapCoroutine = StartCoroutine(GenerateMap());

        tileMap = gameObject.GetComponentInChildren<Tilemap>();
        bounds = tileMap.cellBounds;
        tileMapRenderer = tileMap.GetComponent<TilemapRenderer>();
    }

    private IEnumerator GenerateMap()
    {
        //TO UNDO POOLING... change loops from minTile to bounds.min and comment out poolmap tiles
        var tileMap = gameObject.GetComponentInChildren<Tilemap>();
        var bounds = tileMap.cellBounds;
        var tileMapRenderer = tileMap.GetComponent<TilemapRenderer>();

        // Batching
        int batchSize = 50;
        int batchCounter = 0;

        Bounds cameraBounds = OrthographicBounds(Camera.main);
        Vector3Int minTile = tileMap.WorldToCell(cameraBounds.min);
        Vector3Int maxTile = tileMap.WorldToCell(cameraBounds.max);

        // Generate tiles in view
        for (var z = bounds.max.z; z >= bounds.min.z; z--)
        {
            for (var y = minTile.y; y <= maxTile.y; y++)
            {
                for (var x = minTile.x; x <= maxTile.x; x++)
                {
                    var tileLocation = new Vector3Int(x, y, z);
                    var tileKey = new Vector2Int(x, y);

                    GenerateTileAt(tileLocation, tileKey, tileMap, tileMapRenderer);

                    // Batch handling
                    batchCounter++;
                    if (batchCounter >= batchSize)
                    {
                        batchCounter = 0;
                        yield return null;
                    }
                }
            }
        }
        // Places NPC at specific starting tile. End tile is mouse click
        GameObject newNPC = Instantiate(NPC);
        newNPC.GetComponent<NPC>().SetActiveTile(overlayMap[new Vector2Int(0,0)]);
    }

    // Update is called once per frame
    private OverlayTile lastMouseOverTile = null;
    void Update()
    {
        // Get the current mouse position
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int mouseGridPosition = gameObject.GetComponentInChildren<Tilemap>().WorldToCell(mouseWorldPosition);
        Vector2Int mouseTileKey = new Vector2Int(mouseGridPosition.x, mouseGridPosition.y);
        HandleTileHovers(mouseTileKey);
        PoolMapTiles();
    }
    //helped by chatppp
    private void PoolMapTiles()
    {
        // Manage tile visibility and object pool
        Bounds cameraBounds = OrthographicBounds(Camera.main);
        Vector3Int minTile = tileMap.WorldToCell(cameraBounds.min);
        Vector3Int maxTile = tileMap.WorldToCell(cameraBounds.max);

        // Deactivate tiles that are out of view
        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var kvp in activeTiles)
        {
            Vector2Int tileKey = kvp.Key;
            OverlayTile tile = kvp.Value;
            if (!IsTileInCameraView(tile.transform.position))
            {
                tile.gameObject.SetActive(false);
                inactiveTiles.Enqueue(tile);
                toRemove.Add(tileKey);
            }
        }

        foreach (var tileKey in toRemove)
        {
            activeTiles.Remove(tileKey);
        }

        // Activate tiles that are in view
        for (var z = bounds.max.z; z >= bounds.min.z; z--)
        {
            for (var y = minTile.y; y <= maxTile.y; y++)
            {
                for (var x = minTile.x; x <= maxTile.x; x++)
                {
                    Vector2Int tileKey = new Vector2Int(x, y);
                    if (!overlayMap.ContainsKey(tileKey) || activeTiles.ContainsKey(tileKey))
                    {
                        continue;
                    }

                    // Generate a new tile if not in the object pool
                    if (inactiveTiles.All(t => t.gridLocation != new Vector3Int(x, y, (int)z)))
                    {
                        GenerateTileAt(new Vector3Int(x, y, (int)z), tileKey, tileMap, tileMapRenderer);
                    }

                    if (inactiveTiles.Count > 0)
                    {
                        OverlayTile tile = inactiveTiles.Dequeue();
                        tile.gameObject.SetActive(true);
                        activeTiles.Add(tileKey, tile);
                    }
                    
                }
            }
        }
    }
    
    private void GenerateTileAt(Vector3Int tileLocation, Vector2Int tileKey, Tilemap tileMap,
        TilemapRenderer tileMapRenderer)
    {
        if (!tileMap.HasTile(tileLocation) || overlayMap.ContainsKey(tileKey)) return;
        //create overlay and but it one z above
        var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
        //to put position of grid in overlay object
        overlayTile.gridLocation = tileLocation;
        var cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);
        overlayTile.transform.position = new Vector3(cellWorldPosition.x,cellWorldPosition.y,cellWorldPosition.z+1);
        overlayTile.spriteRenderer.sortingOrder = tileMapRenderer.sortingOrder;
        overlayMap.Add(tileKey,overlayTile);
    }
    private bool IsTileInCameraView(Vector3 position)
    {
        Bounds cameraBounds = OrthographicBounds(Camera.main);
        return cameraBounds.Contains(position);
    }
    public static Bounds OrthographicBounds(Camera camera)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new Bounds(
            camera.transform.position,
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }

    void HandleTileHovers(Vector2Int mouseTileKey)
    {
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
    //handle stopping game
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
