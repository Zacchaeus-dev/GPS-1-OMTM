using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopController2D : MonoBehaviour
{
    private Camera mainCamera;
    public GameObject selectedTroop;
    private Vector2 targetPosition;
    private bool canMoveY;
    public float moveSpeed = 5f; // Speed of the troop movement
    public float ladderDetectionRange = 1f; // Range to detect ladders
    public float attackRange = 1.5f; // Range within which the troop can attack enemies

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleMouseInput();
        HandleEnemySelection();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Troop"))
            {
                if (selectedTroop != null) //if already have a troop selected
                {
                    DeselectTroop(); //deselect old troop
                }
                SelectTroop(hit.collider.gameObject);
            }
        }
        else if (Input.GetMouseButtonDown(1) && selectedTroop != null) // Right click
        {
            SetTroopTargetPosition();
        }
    }

    void SelectTroop(GameObject troop)
    {
        selectedTroop = troop;
        selectedTroop.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent rotation
        selectedTroop.GetComponent<Troop>().selected = true;
        Debug.Log("Troop selected: " + selectedTroop.name);
    }

    void DeselectTroop() //necessary for platform drop off to work as intended
    {
        selectedTroop.GetComponent<Troop>().selected = false;
    }


    void SetTroopTargetPosition()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        targetPosition = new Vector2(mousePosition.x, selectedTroop.transform.position.y); // Default to only X movement

        // Check if the target position is on a ladder
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit.collider != null && hit.collider.CompareTag("Ladder"))
        {
            targetPosition = mousePosition; // Allow Y movement
            canMoveY = true;
        }
        else
        {
            // Check for nearby ladders if target is not on a ladder
            Collider2D[] colliders = Physics2D.OverlapCircleAll(mousePosition, ladderDetectionRange);
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Ladder"))
                {
                    targetPosition = new Vector2(collider.transform.position.x, mousePosition.y); // Move to ladder X position first
                    canMoveY = true;
                    break;
                }
            }
        }

        if (selectedTroop != null)
        {
            selectedTroop.GetComponent<TroopClass>().SetTargetPosition(targetPosition, canMoveY);
        }
    }

    void HandleEnemySelection()
    {
        if (selectedTroop != null && Input.GetKeyDown(KeyCode.A))
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                GameObject enemy = hit.collider.gameObject;
                selectedTroop.GetComponent<Troop>().SetTargetEnemy(enemy, attackRange);
                Debug.Log("Enemy targeted: " + enemy.name);
            }
        }
    }
}
