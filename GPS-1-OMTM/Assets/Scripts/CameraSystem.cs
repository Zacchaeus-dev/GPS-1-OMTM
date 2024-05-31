using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    public bool cameraMovement = true;
    void Update()
    {

        if (cameraMovement)
        {
            Vector3 inputDir = new Vector3(0, 0, 0);
            int edgeScrollSize = 20;

            if (Input.mousePosition.x < edgeScrollSize) //move left
            {
                inputDir.x = -1f;
            }
            if (Input.mousePosition.x > Screen.width - edgeScrollSize) //move right
            {
                inputDir.x = +1f;
            }

            Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

            float moveSpeed = 10f; //camera movement speed
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
    }
}
