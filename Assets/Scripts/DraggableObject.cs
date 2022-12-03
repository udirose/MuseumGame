using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private float startPosX;
    private float startPosY;
    private bool isSelected = false;
    private Vector2 offset;
    
    //grid snapping
    public float gridSizeX = 1.0f;
    public float gridSizeY = .5f;

    void Update()
    {
        if (isSelected)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gameObject.transform.localPosition = SnapToGrid(new Vector3(mousePos.x-startPosX,mousePos.y-startPosY,1));
        }
    }

    Vector3 SnapToGrid(Vector3 position)
    {
        // Round the x and y coordinates to the nearest multiple of the grid size
        int x = Mathf.RoundToInt(position.x / gridSizeX) * Mathf.RoundToInt(gridSizeX);
        int y = Mathf.RoundToInt(position.y / gridSizeY) * Mathf.RoundToInt(gridSizeY);

        // Return a new Vector3 with the rounded x and y coordinates
        return new Vector3(x, y, position.z);
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var pos = transform.localPosition;
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);;
            CameraController.Instance.notClickingMap = true;
            isSelected = true;
            startPosX = mousePos.x - pos.x;
            startPosY = mousePos.y - pos.y;
        }

        if (Input.GetMouseButtonDown(1))
        {
            CameraController.Instance.notClickingMap = true;
            Destroy(gameObject);
        }
        
    }

    private void OnMouseUp()
    {
        CameraController.Instance.notClickingMap = false;
        isSelected = false;
    }
}
