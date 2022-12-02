using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class GridGen : MonoBehaviour
{
    public static GridGen Instance;

    public GridLayout gridLayout;
    public Tilemap MainTilemap;
    public Tilemap ValidatorMap;

    private static Dictionary<TileTypes, TileBase> tileBases = new Dictionary<TileTypes, TileBase>();
    
    private Vector3 prevPos;
    private BoundsInt prevArea;
    
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        string tilePath = @"Tiles\";
        tileBases.Add(TileTypes.Empty, null);
        tileBases.Add(TileTypes.White, Resources.Load<TileBase>(tilePath));
    }


    #region TilemapManagement

    private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int index = 0;
        foreach (var v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[index] = tilemap.GetTile(pos);
            index++;
        }
        return array;
    }

    private static void SetTilesBlock(BoundsInt area, TileTypes type, Tilemap tilemap)
    {
        int size = area.size.x * area.size.y * area.size.z;
        TileBase[] tileArray = new TileBase[size];
        FillTiles(tileArray, type);
        tilemap.SetTilesBlock(area, tileArray);
    }

    private static void FillTiles(TileBase[] array, TileTypes type)
    {
        for(int i = 0; i < array.Length; i++)
        {
            array[i] = tileBases[type];
        }
    }

    #endregion

}

public enum TileTypes
{
    Empty,
    White,
    Red,
    Green
}
