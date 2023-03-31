using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NOX
{
    public class YValue : MonoBehaviour
    {
        public float yValue;
        public static YValue ins;

        private void Awake()
        {
            ins = this;
        }
    }
}

