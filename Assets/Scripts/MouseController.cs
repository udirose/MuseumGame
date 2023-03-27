using System;
using System.Linq;
using UnityEngine;


    //Followed this tutorial: https://www.youtube.com/watch?v=riLtglHwoYw
    public class MouseController : MonoBehaviour
    {
        public static MouseController Instance;

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
        
        }

        // Update is called once per frame
        void LateUpdate()
        {
            var focusedTileHit = GetFocusedTile();
            if (focusedTileHit.HasValue)
            {
                GameObject overlayTile = focusedTileHit.Value.collider.gameObject;
                transform.position = overlayTile.transform.position;
                gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder+1;
            
                //clicking logic
                if (Input.GetMouseButtonDown(0) && !CameraController.Instance.notClickingMap)
                {
                    overlayTile.GetComponent<OverlayTile>().ShowTile();
                }
            }
        }

        public RaycastHit2D? GetFocusedTile()
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePosition2d = new Vector2(mousePosition.x, mousePosition.y);

            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition2d, Vector2.zero);
            if (hits.Length > 0)
            {
                return hits.OrderByDescending(i => i.collider.transform.position.z).First();
            }

            return null;
        }
    }

