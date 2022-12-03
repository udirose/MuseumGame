using System;
using UnityEngine;


    public class BuildController : MonoBehaviour
    {
        public static BuildController Instance;
        //outlets
        public GameObject desk;
        
        //grid snap
        public float gridSizeX = 1.0f;
        public float gridSizeY = .5f;
        
        //state tracking
        public bool deleteMode = false;

        private void Awake()
        {
            if(Instance != null && Instance != this) {
                DestroyImmediate(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            if (deleteMode && Input.GetMouseButtonDown(1))
            {
                var hits = MouseController.Instance.GetFocusedTile();
                if (hits.HasValue)
                {
                    GameObject objectToDelete = hits.Value.collider.gameObject;
                    if (objectToDelete.CompareTag("Building")) 
                    { 
                        Destroy(objectToDelete);
                    }
                }
                
            }
        }

        public void BuildDesk()
        {
            Instantiate(desk,SnapToGrid(desk.transform.position),Quaternion.identity);
        }
        
        Vector3 SnapToGrid(Vector3 position)
        {
            // Round the x and y coordinates to the nearest multiple of the grid size
            int x = Mathf.RoundToInt(position.x / gridSizeX) * Mathf.RoundToInt(gridSizeX);
            int y = Mathf.RoundToInt(position.y / gridSizeY) * Mathf.RoundToInt(gridSizeY);

            // Return a new Vector3 with the rounded x and y coordinates
            return new Vector3(x, y, position.z);
        }
        
    }
