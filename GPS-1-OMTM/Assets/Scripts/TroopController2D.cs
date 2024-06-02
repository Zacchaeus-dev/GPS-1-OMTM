using UnityEngine;

public class TroopController2D : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject selectedTroop;

    public float moveSpeed = 5f; // Speed of the troop movement
    public float ladderDetectionRange = 1f; // Range to detect ladders

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleMouseInput();

        // Update movement for each troop
        foreach (var troop in FindObjectsOfType<TroopClass>())
        {
            troop.UpdateMovement();
        }
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0) && selectedTroop == null) // Left click with no troop selected
        {
            SelectTroop();
        }
        else if (Input.GetMouseButtonDown(0) && selectedTroop != null) // Left click a new troop with a troop already selected
        {
            //Deselect the old troop and select the new troop
            DeselectTroop();
            SelectTroop();
        }
        else if (Input.GetMouseButtonDown(1) && selectedTroop != null) // Right click with a troop selected
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
            selectedTroop.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent rotation

            Troop troop = selectedTroop.GetComponent<Troop>(); //change selected bool in troop component to true
            troop.selected = true;

            Debug.Log("Troop selected: " + selectedTroop.name);
        }
    }

    void DeselectTroop()
    {
        Troop troop = selectedTroop.GetComponent<Troop>(); //change selected bool in troop component to false
        troop.selected = false;
        Debug.Log("Troop Deselected: " + selectedTroop.name);
        selectedTroop = null;
    }

    void SetTroopTargetPosition()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 targetPosition = new Vector2(mousePosition.x, selectedTroop.transform.position.y); // Default to only X movement
        bool canMoveY = false;

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

        selectedTroop.GetComponent<TroopClass>().SetTargetPosition(targetPosition, canMoveY);
        //DeselectTroop(); // Deselect troop after issuing move command?
        Debug.Log("Troop target position set to: " + targetPosition + ", Can move Y: " + canMoveY);
    }
}
