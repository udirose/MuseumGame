using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private float startPosX;
    private float startPosY;
    private bool isSelected = false;
    private Vector2 offset;

    void Update()
    {
        if (isSelected)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gameObject.transform.localPosition = new Vector3(mousePos.x-startPosX,mousePos.y-startPosY,1);
        }
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
