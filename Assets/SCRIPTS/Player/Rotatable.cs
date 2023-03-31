using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NOX
{
    public class Rotatable : MonoBehaviour
    {
        // /////////////////////////// STORED VARIABLES
        [SerializeField] bool isRotating;

        float previousMousePosX;
        [SerializeField] float turnSpeed;

        // /////////////////////////// STORED VARIABLES

        // /////////////////////////// EVENTS

        void Update()
        {
            if(isRotating)
            {
                var xDelta = Input.mousePosition.x - previousMousePosX;
                transform.Rotate(0f, -xDelta * turnSpeed, 0f);
            }
            previousMousePosX = Input.mousePosition.x;

            if (Input.GetMouseButtonUp(0))
            {
                isRotating = false;
            }
        }



        private void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isRotating = true;
            }
        }

        // /////////////////////////// EVENTS
    }


}

