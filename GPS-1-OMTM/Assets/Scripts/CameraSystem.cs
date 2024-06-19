using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    public bool cameraMovement = true;
    public GameObject killdozer; // Reference to the Killdozer
    public float maxDistanceFromKilldozer = 50f; // Maximum allowed distance from the Killdozer

    void Update()
    {
        if (cameraMovement)
        {
            Vector3 inputDir = new Vector3(0, 0, 0);
            int edgeScrollSize = 20;

            if (Input.mousePosition.x < edgeScrollSize) // Move left
            {
                inputDir.x = -1f;
            }
            if (Input.mousePosition.x > Screen.width - edgeScrollSize) // Move right
            {
                inputDir.x = 1f;
            }

            Vector3 moveDir = transform.right * inputDir.x;

            float moveSpeed = 10f; // Camera movement speed
            Vector3 newPosition = transform.position + moveDir * moveSpeed * Time.deltaTime;

            // Calculate the horizontal distance between the new camera position and the Killdozer
            float horizontalDistanceFromKilldozer = Mathf.Abs(newPosition.x - killdozer.transform.position.x);

            // Check if the new position is within the range
            if (horizontalDistanceFromKilldozer <= maxDistanceFromKilldozer)
            {
                transform.position = newPosition;
            }
            else
            {
                // bring the camera back within range
                float clampedX = Mathf.Clamp(newPosition.x, killdozer.transform.position.x - maxDistanceFromKilldozer, killdozer.transform.position.x + maxDistanceFromKilldozer);
                transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
            }
        }
    }
}
