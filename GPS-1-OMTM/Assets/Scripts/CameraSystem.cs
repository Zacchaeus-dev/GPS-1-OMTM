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
    private bool isZooming = false;
    public GameObject zoomDim;
    public CinemachineVirtualCamera vcam;
    public TroopController2D troopController2D;
    public EnergySystem energySystem;
    public Texture2D zoomedOutCursor; 
    public Texture2D normalCursor;

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

        Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
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
        if (Input.GetKey(KeyCode.A)) // Move left
        {
            inputDir.x = -1f;
        }
        if (Input.GetKey(KeyCode.D)) // Move right
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
        //can only zoom out if enough energy
        if(energySystem.currentEnergy < 50f)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleZoom();
        }
    }

    public void ToggleZoom()
    {
        if (isZooming) return;

        if (isZoomedOut)
        {
            StartCoroutine(ZoomCamera(originalCameraSize, 1f, 0.3f)); // Zoom in
            zoomDim.SetActive(false);
            Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
        }
        else if (troopController2D.selectedTroop != null)
        {
            StartCoroutine(ZoomCamera(zoomOutSize, 0.25f, 0.3f)); // Zoom out
            zoomDim.SetActive(true);
            Cursor.SetCursor(zoomedOutCursor, Vector2.zero, CursorMode.Auto);
        }
        isZoomedOut = !isZoomedOut;
    }

    private IEnumerator ZoomCamera(float targetSize, float targetTimeScale, float duration)
    {
        float startSize = vcam.m_Lens.OrthographicSize;
        float startTimeScale = Time.timeScale;
        float elapsedTime = 0f;
        isZooming = true;

        while (elapsedTime < duration)
        {
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, elapsedTime / duration);
            Time.timeScale = Mathf.Lerp(startTimeScale, targetTimeScale, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        vcam.m_Lens.OrthographicSize = targetSize;
        Time.timeScale = targetTimeScale;
        isZooming = false;
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
