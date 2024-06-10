using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killdozer : MonoBehaviour
{
    public float speed = 2f;
    public Transform destination; // Destination point

    private Rigidbody2D rb;
    public bool isMoving = true;

    public int maxHealth;
    public int currentHealth;
    public bool invincible = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    void Update()
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

    public void TakeDamage(int damage)
    {
        if (invincible)
        {
            return;
        }

        currentHealth -= damage;
        Debug.Log("Killdozer took " + damage + " damage.");

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        // Put death animation or effects

        Debug.Log("Killdozer is dead");

        //lose screen
    }
}
