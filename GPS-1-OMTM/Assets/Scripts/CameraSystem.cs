using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSystem : MonoBehaviour
{
    public bool cameraMovement = true;
    public GameObject killdozer; // Reference to the Killdozer
    public float maxDistanceFromKilldozer = 50f; // Maximum allowed distance from the Killdozer

    // Zoom feature
    public bool isZoomedOut = false;
    private float originalCameraSize;
    public float zoomOutSize = 50f; 
    public CinemachineVirtualCamera vcam;
    public TroopController2D troopController2D;

    private void Start()
    {
        var camera = Camera.main;
        var brain = (camera == null) ? null : camera.GetComponent<CinemachineBrain>();
        vcam = (brain == null) ? null : brain.ActiveVirtualCamera as CinemachineVirtualCamera;
        originalCameraSize = vcam.m_Lens.OrthographicSize;
    }

    void Update()
    {
        HandleZoomInput();

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

    void HandleZoomInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleZoom();
        }
    }

    public void ToggleZoom()
    {
        if (isZoomedOut)
        {
            vcam.m_Lens.OrthographicSize = originalCameraSize; //zoom in
            Time.timeScale = 1f;
        }
        else if (troopController2D.selectedTroop != null)
        {
            vcam.m_Lens.OrthographicSize = zoomOutSize; //zoom out
            Time.timeScale = 0.75f; //affects animation speed, change their update mode in animators to unscaled time to prevent this code affecting them
        }
        isZoomedOut = !isZoomedOut;
    }
}
