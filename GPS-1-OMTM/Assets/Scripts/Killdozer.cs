using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killdozer : MonoBehaviour
{
    public float speed = 2f;
    public Transform destination; // Destination point

    private Rigidbody2D rb;
    private bool isMoving = true; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            // Move the Killdozer towards the destination
            Vector2 position = Vector2.MoveTowards(transform.position, destination.position, speed * Time.fixedDeltaTime);
            rb.MovePosition(position);

            // Check if the Killdozer has reached the destination
            if (Vector2.Distance(transform.position, destination.position) < 0.1f)
            {
                Debug.Log("Killdozer has reached the destination.");
                isMoving = false; 
            }
        }
    }

    /* // Use this when a stop point is made
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Stop"))
        {
            // Stop the Killdozer when it reaches a stop point
            isMoving = false;
            Debug.Log("Killdozer reached a stop point.");
        }
    }
    */
}
