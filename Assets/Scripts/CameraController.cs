using System;
using UnityEngine;


//zooming from this tut: https://gamedevbeginner.com/how-to-zoom-a-camera-in-unity-3-methods-with-examples/#two_d_zoom
public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    //outlets
    public Camera cam;

    //zoom settings
    float _targetZoom;
    public float maxZoom = .4f;
    public float minZoom = 4;
    public float zoomSens = .5f;

    public float zoomSpeed = 1;

    //drag settings
    private Vector3 _resetCamera;
    private Vector3 _origin;
    private Vector3 _difference;
    private bool _drag;
    public bool notClickingMap;

    private void Awake()
    {
        //singleton
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _resetCamera = cam.transform.position;
        _targetZoom = cam.orthographicSize;
    }

    void LateUpdate()
    {
        ControlZoom();
        ControlDrag();
    }

    void ControlZoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            if (Input.mouseScrollDelta.y > 0f && cam.orthographicSize > maxZoom)
            {
                cam.orthographicSize -= zoomSens;
            }

            if (Input.mouseScrollDelta.y < 0f && cam.orthographicSize < minZoom)
            {
                cam.orthographicSize += zoomSens;
            }
        }

        if (Input.GetKeyDown(KeyCode.G) && cam.orthographicSize > maxZoom)
        {
            cam.orthographicSize -= zoomSens;
        }

        if (Input.GetKeyDown(KeyCode.B) && cam.orthographicSize < minZoom)
        {
            cam.orthographicSize += zoomSens;
        }
    }

    //taken from this page: https://forum.unity.com/threads/click-drag-camera-movement.39513/
    void ControlDrag()
    {
        if (Input.GetMouseButton(0) && !notClickingMap)
        {
            _difference = (cam.ScreenToWorldPoint(Input.mousePosition)) - cam.transform.position;
            if (_drag == false)
            {
                _drag = true;
                _origin = cam.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            _drag = false;
        }

        if (_drag)
        {
            cam.transform.position = _origin - _difference;
        }

        //RESET CAMERA TO STARTING POSITION WITH RIGHT CLICK
        if (Input.GetMouseButton(1) && !notClickingMap)
        {
            cam.transform.position = _resetCamera;
        }
    }
}