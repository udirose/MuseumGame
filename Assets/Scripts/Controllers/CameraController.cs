using System;
using UnityEngine;

namespace Controllers
{
    //zooming from this tut: https://gamedevbeginner.com/how-to-zoom-a-camera-in-unity-3-methods-with-examples/#two_d_zoom
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance;
        //outlets
        public Camera cam;
        //zoom settings
        [Serializable] public class ZoomVariables
        {
            public float maxZoom =5;
            public float minZoom =20;
            public float zoomSensitivity = 1;
            public float zoomSpeed =30;
        } public ZoomVariables zoom = new ZoomVariables();
        float _targetZoom;
        //drag settings
        private Vector3 _resetCamera;
        private Vector3 _origin;
        private Vector3 _difference;
        private bool _drag;
        
        private void Awake()
        {
            if(Instance != null && Instance != this) {
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
                _targetZoom -= Input.mouseScrollDelta.y * zoom.zoomSensitivity;
                _targetZoom = Mathf.Clamp(_targetZoom, zoom.maxZoom, zoom.minZoom);
                var newSize = Mathf.MoveTowards(cam.orthographicSize, _targetZoom, zoom.zoomSpeed * Time.deltaTime);
                cam.orthographicSize = newSize;
            }
            if (Input.GetKeyDown(KeyCode.G) && cam.orthographicSize > zoom.maxZoom)
            {
                cam.orthographicSize -= 1;
            }
            if (Input.GetKeyDown(KeyCode.B) && cam.orthographicSize < zoom.minZoom)
            {
                cam.orthographicSize += 1;
            }
        }

        //taken from this page: https://forum.unity.com/threads/click-drag-camera-movement.39513/
        void ControlDrag()
        {
            if (Input.GetMouseButton (0)) {
                _difference=(cam.ScreenToWorldPoint (Input.mousePosition))- cam.transform.position;
                if (_drag==false){
                    _drag=true;
                    _origin= cam.ScreenToWorldPoint (Input.mousePosition);
                }
            } else {
                _drag=false;
            }
            if (_drag){
                cam.transform.position = _origin-_difference;
            }
            //RESET CAMERA TO STARTING POSITION WITH RIGHT CLICK
            if (Input.GetMouseButton (1)) {
                cam.transform.position=_resetCamera;
            }
        }
    }
}
