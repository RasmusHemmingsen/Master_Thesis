using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HemmingsenVR
{
    public class DragCamera : MonoBehaviour
    {
        //ignore this code when building 
        #if UNITY_EDITOR

        // flag to keep track whether we are dragging or not
        private bool isDragging = false;

        //starting point of a camera movement
        private float startMouseX;
        private float startMouseY;

        //Camera component
        private Camera cam;

        // Start is called before the first frame update
        void Start()
        {
            // Get our camera component
            cam = GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            // if we press the left button and we havn't started dragging
            if (Input.GetMouseButtonDown(1) && !isDragging)
            {
                //set the flag to true
                isDragging = true;

                // save the mouse starting position
                startMouseX = Input.mousePosition.x;
                startMouseY = Input.mousePosition.y;
            }
            // if we are not pressing the left button, and we are dragging 
            else if (Input.GetMouseButtonUp(1) && isDragging)
            {
                //set the flag to false
                isDragging = false;
            }
        }

        void LateUpdate()
        {
            // check if we are dragging
            if (isDragging)
            {
                // calculate the current mouse position
                float endMouseX = Input.mousePosition.x;
                float endMouseY = Input.mousePosition.y;

                //diffence (in screen coordinates)
                float diffX = endMouseX - startMouseX;
                float diffY = endMouseY - startMouseY;

                // new center of the screen
                float newCenterX = Screen.width / 2 + diffX;
                float newCenterY = Screen.height / 2 + diffY;

                //Get the world coordinate, this is where we should be looking at
                Vector3 LookHerePoint = cam.ScreenToWorldPoint(new Vector3(newCenterX, newCenterY, cam.nearClipPlane));

                // Make outr camera look at the "LookHerePoint"
                transform.LookAt(LookHerePoint);

                //Starting position for the next call
                startMouseX = endMouseX;
                startMouseY = endMouseY;
            }
        }

        #endif
    }
}
