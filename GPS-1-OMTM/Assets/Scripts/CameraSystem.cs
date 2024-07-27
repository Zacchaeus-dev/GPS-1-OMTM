using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

public class CameraSystem : MonoBehaviour
{
    public float cameraSpeed;

    public bool cameraMovement = true;
    public GameObject killdozer; // Reference to the Killdozer
    public bool limitCameraMovement = false;
    public float maxDistanceFromKilldozer = 35f; // Maximum allowed distance from the Killdozer

    // Zoom feature
    public bool isZoomedOut = false;
    private float originalCameraSize;
    public float zoomOutSize = 50f;
    private bool isZooming = false;
    private Vector3 zoomOutPosition;
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

    private bool focusOnKD;

    public TutorialPhase tutorialPhase;
    public WaveSystem waveSystem;

    // Borders
    public Vector3 leftBorder = new Vector3(-24.5f, 0, 0);
    public Vector3 rightBorder = new Vector3(220, 0, 0);

    private void Start()
    {
        var camera = Camera.main;
        var brain = (camera == null) ? null : camera.GetComponent<CinemachineBrain>();
        originalCameraSize = vcam.m_Lens.OrthographicSize;

        //UpdateZoomPosition();

        zoomOutPosition = new Vector3((leftBorder.x + rightBorder.x) / 2, transform.position.y, transform.position.z);

        Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
    }

    void Update()
    {
        if (Killdozer.gameOver)
        {
            /*
            if (isZoomedOut)
            {
                StartCoroutine(ZoomCamera(originalCameraSize, 1f, 0.3f)); // Zoom in
                zoomDim.SetActive(false);
                Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
                isZoomedOut = !isZoomedOut;
            }
            */

            return;
        }

        //HandleZoomInput();

        //if (cameraMovement && focusedTroop == null) // Disable manual movement when focusing on a troop
        if (cameraMovement)
        {
            HandleCameraMovement();
            LimitCameraMovement();
        }
    }

    void HandleCameraMovement()
    {
        if (tutorialPhase.tutorialOn || !cameraMovement)
        {
            //transform.position = new Vector3(2f, 0, 0);
            return;
        }

        Vector3 inputDir = new Vector3(0, 0, 0);
        int edgeScrollSize = 20; //cursor hover area

        if (Input.mousePosition.x < edgeScrollSize && transform.position.x > leftBorder.x) // Move left
        {
            inputDir.x = -1f;
            //Debug.Log(leftBorder.x);
        }
        if (Input.mousePosition.x > Screen.width - edgeScrollSize && transform.position.x < rightBorder.x) // Move right
        {
            inputDir.x = 1f;
            //Debug.Log(rightBorder.x);
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
        float moveSpeed = cameraSpeed; 
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float clampedX = Mathf.Clamp(transform.position.x, leftBorder.x, rightBorder.x);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
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

    public void UpdateZoomPosition()
    {
        float screenAspect = Screen.width / (float)Screen.height;
        float horizontalSize = (rightBorder.x - leftBorder.x) / 2;
        float offset = 20f;
        
        /*
        switch (waveSystem.currentWaveIndex)
        {
            case 0:
                offset = 40f;
                break;
            case 1:
                offset = 20f;
                break;
            default:
                offset = 40f;
                break;
        }
        */

        horizontalSize += offset / 2;
        zoomOutSize = horizontalSize / screenAspect;
        // Calculate the center position between left and right borders
        zoomOutPosition = new Vector3((leftBorder.x + rightBorder.x + offset) / 2, transform.position.y, transform.position.z);
    }

    public void ToggleZoom()
    {
        if (isZooming) return;

        if (tutorialPhase.tutorialOn) //does not zoom out in tutorial
        {
            //zoomDim.SetActive(true);
            isZoomedOut = true;
            //Cursor.SetCursor(zoomedOutCursor, Vector2.zero, CursorMode.Auto);
            return;
        }

        if (isZoomedOut)
        {
            StartCoroutine(ZoomCamera(originalCameraSize, 1f, 0.3f)); // Zoom in
            zoomDim.SetActive(false);
            Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
            cameraMovement = true;
        }
        else if (troopController2D.selectedTroop != null)
        {
            StartCoroutine(ZoomCamera(zoomOutSize, 0.25f, 0.3f, zoomOutPosition)); // Zoom out
            zoomDim.SetActive(true);
            Cursor.SetCursor(zoomedOutCursor, Vector2.zero, CursorMode.Auto);
            cameraMovement = false;
        }
        isZoomedOut = !isZoomedOut;
    }

    private IEnumerator ZoomCamera(float targetSize, float targetTimeScale, float duration, Vector3? targetPosition = null)
    {
        float startSize = vcam.m_Lens.OrthographicSize;
        float startTimeScale = Time.timeScale;
        float elapsedTime = 0f;
        isZooming = true;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = targetPosition ?? transform.position; // Use targetPosition if provided, else current position

        while (elapsedTime < duration)
        {
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, elapsedTime / duration);
            Time.timeScale = Mathf.Lerp(startTimeScale, targetTimeScale, elapsedTime / duration);
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration); // Lerp position
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        vcam.m_Lens.OrthographicSize = targetSize;
        Time.timeScale = targetTimeScale;
        transform.position = endPosition; // Set final position
        isZooming = false;
    }

    public void FocusOnTroop(GameObject troop)
    {
        if (tutorialPhase.tutorialOn)
        {
            return;
        }

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
        StartCoroutine(FocusCameraOnTroop());
    }

    public void DefocusTroop()
    {
        focusedTroop = null;
    }

    public void FocusOnKilldozer()
    {
        StopAllCoroutines();
        focusOnKD = true;
        StartCoroutine(FocusCameraOnKilldozer());
    }

    public void DefocusKilldozer()
    {
        focusOnKD = false;
    }

    private IEnumerator FocusCameraOnTroop()
    {
        /*
        while (focusedTroop != null)
        {
            Vector3 targetPosition = new Vector3(focusedTroop.transform.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, moveTime);
            yield return null;
        }
        */

        if (focusedTroop != null)
        {
            Vector3 targetPosition = new Vector3(focusedTroop.transform.position.x, transform.position.y, transform.position.z);
            Vector3 currentVelocity = Vector3.zero;
            float elapsedTime = 0f;

            while (elapsedTime < moveTime)
            {
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, moveTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the camera is exactly at the target position after the smooth movement
            transform.position = targetPosition;
        }
    }

    private IEnumerator FocusCameraOnKilldozer()
    {
        while (focusOnKD)
        {
            Vector3 targetPosition = new Vector3(killdozer.transform.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, moveTime);
            yield return null;
        }
    }
}
