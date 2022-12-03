using System;
using UnityEngine;

namespace Controllers
{
    public class BuildController : MonoBehaviour
    {
        //outlets
        public GameObject desk;
        private void Start()
        {
            
        }

        private void Update()
        {
            
        }

        public void BuildDesk()
        {
            Instantiate(desk);
        }
    }
}