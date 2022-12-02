using Controllers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class DraggableWindow : EventTrigger
    {
        private bool dragging;
        Vector2 Offset;

        void Start()
        {
            Hide();
        }

        // Update is called once per frame
        void Update()
        {
            if (dragging)
            {
                transform.position = new Vector2(Input.mousePosition.x+Offset.x,Input.mousePosition.y+Offset.y);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            CameraController.Instance.notClickingUI = false;
            Offset = new Vector2((transform.position.x - Input.mousePosition.x),(transform.position.y - Input.mousePosition.y));
            dragging = true;
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            CameraController.Instance.notClickingUI = true;
            dragging = false;
        }
    }
}
