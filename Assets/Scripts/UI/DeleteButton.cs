using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DeleteButton : MonoBehaviour
    {
        public Image delButton;
        // Start is called before the first frame update
        void Start()
        {
        
        }
        
        void ToggleDeleteMode()
        {
            if (BuildController.Instance.deleteMode)
            {
                delButton.color = Color.black;
                BuildController.Instance.deleteMode = true;
            }
            else
            {
                delButton.color = Color.red;
                BuildController.Instance.deleteMode = false;
            }
        }
    }
}
