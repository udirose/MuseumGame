using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private float startPosX;
    private float startPosY;
    private bool isSelected = false;
    private bool selectingClick = false;
    private Vector2 offset;
    
    //grid snapping
    public float gridSizeX = 1.0f;
    public float gridSizeY = .5f;
    //outlets
    public Canvas options;

    void Start()
    {
        options.enabled = false;
    }
    void Update()
    {
        //left click
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("selected = "+isSelected+", selectingClick = "+selectingClick);
            var pos = transform.localPosition;
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            //if hit this specific object
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isSelected = true;
                //alternates
                selectingClick = !selectingClick;
                CameraController.Instance.notClickingMap = true;
                startPosX = mousePos.x - pos.x;
                startPosY = mousePos.y - pos.y;
            }
        }

        if (isSelected)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (selectingClick)
            {
                gameObject.transform.localPosition = SnapToGrid(new Vector3(mousePos.x-startPosX,mousePos.y-startPosY,1));
            }
            options.enabled = true;
            GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        else
        {
            options.enabled = false;
            GetComponent<SpriteRenderer>().color = Color.white;
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
    

    private void OnMouseUp()
    {
        CameraController.Instance.notClickingMap = false;
    }
    
    
    //UI for object
    public void Delete()
    {
        Destroy(gameObject);
    }

    public void Place()
    {
        isSelected = false;
        selectingClick = false;
        options.enabled = false;
    }
}
