using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


//Followed this tutorial: https://www.youtube.com/watch?v=riLtglHwoYw
public class MouseController : MonoBehaviour
{
    public static MouseController Instance;
    private Tilemap tilemap;
    private Vector3Int prevTilePos;
    private Vector3 cursorOffset = new Vector3(-.5f, .25f, 0);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        tilemap = MapManager.Instance.tilemap;
    }

    void LateUpdate()
    {
        var focusedTilePos = GetFocusedTile();
        if (focusedTilePos.HasValue)
        {
            Vector3Int currentTilePos = focusedTilePos.Value;
            if (currentTilePos != prevTilePos)
            {
                //HighlightTile(currentTilePos);
                prevTilePos = currentTilePos;
            }

            // Position cursor object
            Vector3 cursorWorldPos = tilemap.CellToWorld(currentTilePos) + new Vector3(tilemap.cellSize.x / 2, 0);
            cursorWorldPos.z = 0;
            transform.position = cursorWorldPos + cursorOffset;
        }
    }

    public Vector3Int? GetFocusedTile()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePosition = WorldToCell(mousePosition);
        
        if (tilemap.HasTile(tilePosition))
        {
            return tilePosition;
        }
        return null;
    }

    void HighlightTile(Vector3Int tilePosition)
    {
        // Clear previous highlight
        tilemap.SetTileFlags(prevTilePos, TileFlags.None);
        tilemap.SetColor(prevTilePos, Color.white);

        // Highlight current tile
        tilemap.SetTileFlags(tilePosition, TileFlags.None);
        tilemap.SetColor(tilePosition, Color.yellow);
    }

    public static Vector3Int WorldToCell(Vector3 worldPosition)
    {
        Tilemap tilemap = MapManager.Instance.tilemap;
        Vector3 cellPosition = tilemap.transform.InverseTransformPoint(worldPosition);
        Vector3 cellSize = tilemap.cellSize;
        float halfX = cellSize.x / 2;
        float halfY = cellSize.y / 2;
        int x = Mathf.FloorToInt((cellPosition.x / halfX + cellPosition.y / halfY) / 2);
        int y = Mathf.FloorToInt((cellPosition.y / halfY - (cellPosition.x / halfX)) / 2);
        return new Vector3Int(x, y, 0);
    }
}
