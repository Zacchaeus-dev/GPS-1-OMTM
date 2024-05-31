using UnityEngine;

public class TroopController2D : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject selectedTroop;
    private Vector2 targetPosition;
    private bool isMoving;
    private bool canMoveY;
    private bool onLadder;

    public float moveSpeed = 5f; // Speed of the troop movement
    public float ladderDetectionRange = 1f; // Range to detect ladders

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
        if (Input.GetMouseButtonDown(0) && !isMoving) // Left click
        {
            SelectTroop();
        }
        else if (Input.GetMouseButtonDown(1) && selectedTroop != null) // Right click
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
            Debug.Log("Troop selected: " + selectedTroop.name);
        }
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

        isMoving = true;
        Debug.Log("Troop target position set to: " + targetPosition + ", Can move Y: " + canMoveY);
    }

    void MoveSelectedTroop()
    {
        //get the troop attributes component and set the movespeed according to each troop's attribute
        Troop troop = selectedTroop.GetComponent<Troop>();
        moveSpeed = troop.moveSpeed;


        if (canMoveY && onLadder)
        {
            // Move in both X and Y directions
            selectedTroop.GetComponent<Rigidbody2D>().gravityScale = 0; // Disable gravity
            selectedTroop.transform.position = Vector2.MoveTowards(selectedTroop.transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
        else
        {
            // Move only in X direction
            Vector2 restrictedTargetPosition = new Vector2(targetPosition.x, selectedTroop.transform.position.y);
            selectedTroop.transform.position = Vector2.MoveTowards(selectedTroop.transform.position, restrictedTargetPosition, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(selectedTroop.transform.position, restrictedTargetPosition) < 0.1f && canMoveY)
            {
                onLadder = true;
            }
        }

        if (Vector2.Distance(selectedTroop.transform.position, targetPosition) < 0.1f)
        {
            selectedTroop.transform.position = targetPosition;
            isMoving = false;
            onLadder = false;
            selectedTroop.GetComponent<Rigidbody2D>().gravityScale = 1; // Re-enable gravity
            Debug.Log("Troop arrived at target position: " + targetPosition);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder") && selectedTroop != null)
        {
            onLadder = true;
            selectedTroop.GetComponent<Rigidbody2D>().gravityScale = 0; // Disable gravity
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder") && selectedTroop != null)
        {
            onLadder = false;
            selectedTroop.GetComponent<Rigidbody2D>().gravityScale = 1; // Re-enable gravity
        }
    }
}
