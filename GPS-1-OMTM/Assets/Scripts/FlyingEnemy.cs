using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public int maxHealth = 100; // Maximum health of the enemy
    private int currentHealth;

    public float speed = 5f; // Speed of the enemy
    public float detectionRange = 5f; // Range to detect troops
    public float attackRange = 1.5f; // Range within which the enemy can attack troops
    public int attackDamage = 10; // Damage dealt per attack
    public float attackInterval = 1.0f; // Time between attacks

    private GameObject targetTroop;
    private float lastAttackTime;

    private bool shouldMove = true; // Flag to control movement

    void Start()
    {
        currentHealth = maxHealth;
        lastAttackTime = Time.time;
    }

    void Update()
    {
        if (shouldMove)
        {
            MoveLeft();
        }
        DetectTroops();
        MoveTowardsTroop();
        HandleAttack();
    }

    void MoveLeft()
    {
        // Move towards the negative X-axis
        transform.position -= Vector3.right * speed * Time.deltaTime;
    }

    void DetectTroops()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        float closestDistance = Mathf.Infinity;
        GameObject closestTroop = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Troop"))
            {
                float distance = Vector2.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTroop = hitCollider.gameObject;
                }
            }
        }

        if (closestTroop != null)
        {
            targetTroop = closestTroop;
        }
        else
        {
            targetTroop = null;
        }
    }

    void MoveTowardsTroop()
    {
        if (targetTroop != null)
        {
            // Calculate distance to the target troop
            float distanceToTroop = Vector2.Distance(transform.position, targetTroop.transform.position);

            // Check if within attack range
            if (distanceToTroop > attackRange)
            {
                // Move towards the troop if outside attack range
                Vector2 direction = (targetTroop.transform.position - transform.position).normalized;
                transform.position += (Vector3)direction * speed * Time.deltaTime;
            }
            else
            {
                // Stay at the edge of attack range
                shouldMove = false; // Stop moving
            }
        }
        else
        {
            shouldMove = true; // Resume moving if no troop in attack range
        }
    }

    void HandleAttack()
    {
        if (targetTroop != null && Vector2.Distance(transform.position, targetTroop.transform.position) <= attackRange)
        {
            if (Time.time >= lastAttackTime + attackInterval)
            {
                lastAttackTime = Time.time;
                AttackTroop();
            }
        }
    }

    void AttackTroop()
    {
        if (targetTroop != null)
        {
            Troop troop = targetTroop.GetComponent<Troop>();
            if (troop != null)
            {
                troop.TakeDamage(attackDamage);
                Debug.Log("Attacked troop: " + troop.name + " for " + attackDamage + " damage.");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(name + " took " + damage + " damage, remaining health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(name + " died.");
        // Add death logic here (e.g., destroy the GameObject, play an animation, etc.)
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
