using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NOX
{
    public class EndPoint : MonoBehaviour
    {
        public bool winState;

        private void OnTriggerEnter(Collider other)
        {
            winState = true;
        }
    }
}

