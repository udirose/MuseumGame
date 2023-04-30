using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayTile : MonoBehaviour
{
    //pathfinding
    public int G;
    public int H;
    public int F => G + H;
    public bool isBlocked;
    public OverlayTile previous;
    //location
    public Vector3Int gridLocation;
    public SpriteRenderer spriteRenderer;

    // Update is called once per frame
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ShowTile()
    {
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
    public void HideTile()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0);
    }

    public void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HideTile();
        }
    }
}
