using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class TroopClass : MonoBehaviour
{
    private Vector2 targetPosition;
    public bool isMoving;
    private bool canMoveY;
    private bool canClimb;
    private bool onLadder;
    private Collider2D nearestVert;
    private Vector2 vertPosition;
    private Rigidbody2D rb;
    public float onPlatform;

    public float moveSpeed = 5f; // Speed of the troop movement

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*        if (collision.tag == "test Ground")
                {
                    onPlatform = 1;
                }
                else if (collision.tag == "test Terrain 1")
                {
                    onPlatform = 2;
                }*/


        if (onPlatform == 1 && collision.tag == "[PF] Upper-Ground")
        {
            Debug.Log("gihai");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = 2;
        }

        else if (onPlatform == 2 && collision.tag == "[PF] Ground Check")
        {
            Debug.Log("fak");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = 1;
        }
    }
    public void SetTargetPosition(Vector2 targetPosition, Vector2 vertPosition, Collider2D nearestVert)
    {
        this.targetPosition = targetPosition;
        this.vertPosition = vertPosition;
        //this.canMoveY = canMoveY;
        this.nearestVert = nearestVert;
        isMoving = true;
    }

    public void Update()
    {
        if (isMoving)
        {
            Move();
        }
        if (canClimb)
        {
            ClimbAndMove();
        }
    }

    void Move()
    {
        if (onPlatform == 1)
        {
            if (nearestVert != null)
            {
                if (canClimb == false)
                {
                    transform.position = Vector2.MoveTowards(transform.position, vertPosition, moveSpeed * Time.deltaTime); // move to nearest vert
                    if (transform.position.x == nearestVert.transform.position.x) // when reached vert, activate climb
                    {
                        canClimb = true;
                        Debug.Log("CLIMB");
                    }
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                targetPosition.y = transform.position.y;
                if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
                {
                    transform.position = targetPosition;
                    isMoving = false;

                    Debug.Log("Troop arrived at target position: " + targetPosition);
                }
            }
        }

        else if (onPlatform == 2)
        {
            if (nearestVert != null)
            {
                if (canClimb == false)
                {
                    transform.position = Vector2.MoveTowards(transform.position, vertPosition, moveSpeed * Time.deltaTime); // move to nearest vert
                    if (transform.position.x == nearestVert.transform.position.x) // when reached vert, activate climb
                    {
                        canClimb = true;
                        Debug.Log("CLIMB");
                    }
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                targetPosition.y = transform.position.y;
                if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
                {
                    transform.position = targetPosition;
                    isMoving = false;

                    Debug.Log("Troop arrived at target position: " + targetPosition);
                }
            }
        }
    }

    void ClimbAndMove()
    {
        isMoving = false;

        if (onPlatform == 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y + 20), moveSpeed * Time.deltaTime);
        }
        else if (onPlatform == 2)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y - 20), moveSpeed * Time.deltaTime);
        }



    }

}
