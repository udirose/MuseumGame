using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance => _instance;
    
    //private
    private Vector3 lastCameraPosition;


    // Outlets
    public OverlayTile overlayTilePrefab;
    public GameObject overlayContainer;
    public GameObject NPC;

    // Tracks overlay tiles
    public Dictionary<Vector2Int, OverlayTile> overlayMap;
    private Queue<OverlayTile> inactiveTiles;
    private Tilemap tileMap;
    private TilemapRenderer tileMapRenderer;
    private BoundsInt bounds;
    private Coroutine poolMapTilesCoroutine;
    //npc
    private NPC npc;
    private bool npcNotSet = false;


    // Singleton
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
        inactiveTiles = new Queue<OverlayTile>();

        tileMap = gameObject.GetComponentInChildren<Tilemap>();
        //bounds = tileMap.cellBounds;
        tileMapRenderer = tileMap.GetComponent<TilemapRenderer>();
        npc = Instantiate(NPC).GetComponent<NPC>();
        npcNotSet = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Get the current mouse position
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int mouseGridPosition = tileMap.WorldToCell(mouseWorldPosition);
        Vector2Int mouseTileKey = new Vector2Int(mouseGridPosition.x, mouseGridPosition.y);
        HandleTileHovers(mouseTileKey);
        Camera mainCamera = Camera.main;

        if (mainCamera.transform.position != lastCameraPosition)
        {
            StopCoroutine(PoolMapTiles());
            StartCoroutine(PoolMapTiles());
        }

        lastCameraPosition = mainCamera.transform.position;
        
        //fix character error
        if (npcNotSet)
        {
            Vector2Int tilePosition = new Vector2Int(0, 0);
    
            if (overlayMap.ContainsKey(tilePosition))
            {
                if (npc.activeTile == null)
                {
                    npc.activeTile = overlayMap[tilePosition];
                    npc.PositionCharacterOnTile(npc.activeTile);
                    npcNotSet = false;
                }
            }
            else
            {
                npcNotSet = true;
            }
        }
    }

    private IEnumerator PoolMapTiles()
    {
        Camera mainCamera = Camera.main;

        Bounds cameraBounds = IsometricBounds(mainCamera, 1f, 0.5f);
        HashSet<Vector2Int> tilesInView = new HashSet<Vector2Int>();

        var batchSize = 100;
        var batchCounter = 0;

        var z = 0;

        foreach (Vector2Int tilePos in GetDiamondTiles(cameraBounds))
        {
            Vector2Int tileKey = tilePos;
            tilesInView.Add(tileKey);

            if (!overlayMap.ContainsKey(tileKey))
            {
                GenerateTileAt(new Vector3Int(tilePos.x, tilePos.y, z), tileKey, tileMap, tileMapRenderer);
            }
            batchCounter++;

            if (batchCounter >= batchSize)
            {
                batchCounter = 0;
                yield return null;
            }
        }

        foreach (var kvp in overlayMap)
        {
            Vector2Int tileKey = kvp.Key;
            OverlayTile tile = kvp.Value;

            if (tilesInView.Contains(tileKey))
            {
                if (!tile.gameObject.activeSelf)
                {
                    tile.gameObject.SetActive(true);
                }
            }
            else
            {
                if (tile.gameObject.activeSelf)
                {
                    tile.gameObject.SetActive(false);
                    inactiveTiles.Enqueue(tile);
                }
            }
        }
        yield return null;
    }

    private OverlayTile GenerateTileAt(Vector3Int tileLocation, Vector2Int tileKey, Tilemap tileMap, TilemapRenderer tileMapRenderer)
    {
        if (!tileMap.HasTile(tileLocation)) return null;

        var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
        overlayTile.gridLocation = tileLocation;
        var cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);
        overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z + 1);
        overlayTile.spriteRenderer.sortingOrder = tileMapRenderer.sortingOrder;
        overlayTile.gameObject.SetActive(false);

        overlayMap.Add(tileKey, overlayTile);
        return overlayTile;
    }
    private OverlayTile lastMouseOverTile = null;
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
    
    private IEnumerable<Vector2Int> GetDiamondTiles(Bounds cameraBounds)
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        float orthographicSize = Camera.main.orthographicSize;
        float aspectRatio = Camera.main.aspect;
    
        Vector3Int topLeftCell = tileMap.WorldToCell(new Vector3(cameraPosition.x - orthographicSize * aspectRatio, cameraPosition.y + orthographicSize, cameraPosition.z));
        Vector3Int topRightCell = tileMap.WorldToCell(new Vector3(cameraPosition.x + orthographicSize * aspectRatio, cameraPosition.y + orthographicSize, cameraPosition.z));
        Vector3Int bottomLeftCell = tileMap.WorldToCell(new Vector3(cameraPosition.x - orthographicSize * aspectRatio, cameraPosition.y - orthographicSize, cameraPosition.z));
        Vector3Int bottomRightCell = tileMap.WorldToCell(new Vector3(cameraPosition.x + orthographicSize * aspectRatio, cameraPosition.y - orthographicSize, cameraPosition.z));

        int minX = Mathf.Min(topLeftCell.x, bottomLeftCell.x) - 2;
        int maxX = Mathf.Max(topRightCell.x, bottomRightCell.x) + 2;
        int minY = Mathf.Min(bottomLeftCell.y, bottomRightCell.y) - 2;
        int maxY = Mathf.Max(topLeftCell.y, topRightCell.y) + 2;

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                float dx = Mathf.Abs(x - topLeftCell.x);
                float dy = Mathf.Abs(y - topLeftCell.y);
                float dMaxX = Mathf.Abs(x - bottomRightCell.x);
                float dMaxY = Mathf.Abs(y - bottomRightCell.y);

                if (dx + dy <= maxY - minY && dMaxX + dMaxY <= maxY - minY)
                {
                    yield return new Vector2Int(x, y);
                }
            }
        }
    }

    public static Bounds IsometricBounds(Camera camera, float isoXScale, float isoYScale)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        float cameraWidth = cameraHeight * screenAspect;

        Vector3 cameraPosition = camera.transform.position;
        Vector3 min = new Vector3(cameraPosition.x - cameraWidth / 2, cameraPosition.y - cameraHeight / 2, cameraPosition.z);
        Vector3 max = new Vector3(cameraPosition.x + cameraWidth / 2, cameraPosition.y + cameraHeight / 2, cameraPosition.z);

        Bounds isoBounds = new Bounds(cameraPosition, new Vector3(cameraWidth, cameraHeight, 0f));
        return isoBounds;
    }
    void OnDestroy()
    {
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