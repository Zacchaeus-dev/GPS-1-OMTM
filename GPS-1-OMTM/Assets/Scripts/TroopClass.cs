using UnityEngine;

public class TroopClass : MonoBehaviour
{
    private Vector2 targetPosition;
    private bool isMoving;
    private bool canMoveY;
    private bool onLadder;

    public float moveSpeed = 5f; // Speed of the troop movement

    public void SetTargetPosition(Vector2 targetPosition, bool canMoveY)
    {
        this.targetPosition = targetPosition;
        this.canMoveY = canMoveY;
        isMoving = true;
    }

    public void UpdateMovement()
    {
        if (isMoving)
        {
            Move();
        }
    }

    void Move()
    {
        if (canMoveY && onLadder)
        {
            // Move in both X and Y directions
            GetComponent<Rigidbody2D>().gravityScale = 0; // Disable gravity
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
        else
        {
            // Move only in X direction
            Vector2 restrictedTargetPosition = new Vector2(targetPosition.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, restrictedTargetPosition, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, restrictedTargetPosition) < 0.1f && canMoveY)
            {
                onLadder = true;
            }
        }

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            transform.position = targetPosition;
            isMoving = false;
            onLadder = false;
            GetComponent<Rigidbody2D>().gravityScale = 1; // Re-enable gravity
            Debug.Log("Troop arrived at target position: " + targetPosition);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            onLadder = true;
            GetComponent<Rigidbody2D>().gravityScale = 0; // Disable gravity
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            onLadder = false;
            GetComponent<Rigidbody2D>().gravityScale = 1; // Re-enable gravity
        }
    }
}
