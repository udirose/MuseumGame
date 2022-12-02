using System;
using UnityEngine;

namespace Controllers
{
    //zooming from this tut: https://gamedevbeginner.com/how-to-zoom-a-camera-in-unity-3-methods-with-examples/#two_d_zoom
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance;
        public Camera cam;
        //zoom settings
        public float maxZoom =5;
        public float minZoom =20;
        public float sensitivity = 1;
        public float speed =30;
        float _targetZoom;
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
            _targetZoom = cam.orthographicSize;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                _targetZoom -= Input.mouseScrollDelta.y * sensitivity;
                _targetZoom = Mathf.Clamp(_targetZoom, maxZoom, minZoom);
                var newSize = Mathf.MoveTowards(cam.orthographicSize, _targetZoom, speed * Time.deltaTime);
                cam.orthographicSize = newSize;
            }
            if (Input.GetKeyDown(KeyCode.G) && cam.orthographicSize > maxZoom)
            {
                cam.orthographicSize -= 1;
            }
            if (Input.GetKeyDown(KeyCode.B) && cam.orthographicSize < minZoom)
            {
                cam.orthographicSize += 1;
            }
        }
    }
}
