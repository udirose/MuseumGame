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
        mousePosition.z = 0;
        Vector3Int tilePosition = tilemap.WorldToCell(mousePosition);
    
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
}
