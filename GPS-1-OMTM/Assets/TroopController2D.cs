using UnityEngine;

public class TroopController2D : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject selectedTroop;
    private Vector2 targetPosition;
    private bool isMoving;

    public float moveSpeed = 5f; // Speed of the troop movement

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleMouseInput();

        if (isMoving && selectedTroop != null)
        {
            MoveSelectedTroop();
        }
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            SelectTroop();
        }
        else if (Input.GetMouseButtonDown(1)) // Right click
        {
            SetTroopTargetPosition();
        }
    }

    void SelectTroop()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Troop"))
        {
            selectedTroop = hit.collider.gameObject;
            Debug.Log("Troop selected: " + selectedTroop.name);
        }
    }

    void SetTroopTargetPosition()
    {
        if (selectedTroop == null) return;

        targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        isMoving = true;
        Debug.Log("Troop target position set to: " + targetPosition);
    }

    void MoveSelectedTroop()
    {
        if (Vector2.Distance(selectedTroop.transform.position, targetPosition) > 0.1f)
        {
            selectedTroop.transform.position = Vector2.MoveTowards(selectedTroop.transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
        else
        {
            selectedTroop.transform.position = targetPosition;
            isMoving = false;
            Debug.Log("Troop arrived at target position: " + targetPosition);
        }
    }
}
