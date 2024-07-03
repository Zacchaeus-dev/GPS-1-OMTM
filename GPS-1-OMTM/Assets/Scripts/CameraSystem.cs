using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSystem : MonoBehaviour
{
    public bool cameraMovement = true;
    public GameObject killdozer; // Reference to the Killdozer
    public bool limitCameraMovement = false;
    public float maxDistanceFromKilldozer = 50f; // Maximum allowed distance from the Killdozer

    // Zoom feature
    public bool isZoomedOut = false;
    private float originalCameraSize;
    public float zoomOutSize = 50f; 
    public CinemachineVirtualCamera vcam;
    public TroopController2D troopController2D;

    // Focus on Troops
    private GameObject focusedTroop;
    private Vector3 velocity = Vector3.zero;
    public float moveTime = 0.3f;

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

        if (cameraMovement && focusedTroop == null) // Disable manual movement when focusing on a troop
        {
            HandleCameraMovement();
            LimitCameraMovement();
        }
    }

    void HandleCameraMovement()
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
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    void LimitCameraMovement()
    {
        if (limitCameraMovement)
        {
            Vector3 newPosition = transform.position;

            // Calculate the horizontal distance between the new camera position and the Killdozer
            float horizontalDistanceFromKilldozer = Mathf.Abs(newPosition.x - killdozer.transform.position.x);

            // Check if the new position is within the range
            if (horizontalDistanceFromKilldozer > maxDistanceFromKilldozer)
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
            Time.timeScale = 0.75f; 
        }
        isZoomedOut = !isZoomedOut;
    }

    public void FocusOnTroop(GameObject troop)
    {
        if (focusedTroop == troop)
        {
            // Unfocus the troop if it's already focused
            focusedTroop = null;
        }
        else
        {
            focusedTroop = troop;
        }

        StopAllCoroutines(); // Stop any existing focus coroutine
        StartCoroutine(FocusCamera());
    }

    public void DefocusTroop()
    {
        focusedTroop = null;
    }

    private IEnumerator FocusCamera()
    {
        while (focusedTroop != null)
        {
            Vector3 targetPosition = new Vector3(focusedTroop.transform.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, moveTime);
            yield return null;
        }
    }
}
