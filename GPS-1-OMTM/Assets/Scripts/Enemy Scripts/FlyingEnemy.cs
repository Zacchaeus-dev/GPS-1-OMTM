using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public int maxHealth; // Maximum health of the enemy
    public int currentHealth;

    public float speed = 5f; // Speed of the enemy
    public float detectionRange = 5f; // Range to detect troops
    public float attackRange = 1.5f; // Range within which the enemy can attack troops
    public int attackDamage = 10; // Damage dealt per attack
    public float attackInterval = 1.0f; // Time between attacks
    public Vector3 startOffset; // Offset for the start point of the bullet tracer
    public LineRenderer lineRendererPrefab; // Prefab for the line renderer
    public float tracerFadeDuration = 0.5f; // Duration of the fade-out

    private GameObject target;
    private float lastAttackTime = 0f;
    private bool shouldMove = true; // Flag to control movement

    // Reference to the Killdozer
    public GameObject killdozer;

    void Start()
    {
        currentHealth = maxHealth;
        lastAttackTime = Time.time;

        if (killdozer == null)
        {
            Debug.LogError("Killdozer reference is not set!");
        }
    }

    void Update()
    {
        if (shouldMove)
        {
            MoveLeft();
        }
        DetectTargets();
        MoveTowardsTarget();
        HandleAttack();
    }

    void MoveLeft()
    {
        // Move towards the negative X-axis
        transform.position -= Vector3.right * speed * Time.deltaTime;
    }

    void DetectTargets()
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
            target = closestTroop;
            Debug.Log("Targeting troop: " + closestTroop.name);
        }
        else
        {
            target = killdozer; // Default to targeting the Killdozer if no troops are found
            if (killdozer != null)
            {
                Debug.Log("No troops found. Targeting Killdozer: " + killdozer.name);
            }
            else
            {
                Debug.LogError("Killdozer is not assigned in the Inspector!");
            }
        }
    }

    void MoveTowardsTarget()
    {
        if (target != null)
        {
            // Calculate distance to the target
            float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);

            // Check if within attack range
            if (distanceToTarget > attackRange)
            {
                // Move towards the target if outside attack range, only in X-axis
                Vector3 direction = (target.transform.position - transform.position).normalized;
                direction.y = 0; // Ensure no movement in Y-axis
                transform.position += direction * speed * Time.deltaTime;
            }
            else
            {
                // Stay at the edge of attack range
                shouldMove = false; // Stop moving
            }
        }
        else
        {
            shouldMove = true; // Resume moving if no target in attack range
        }
    }

    void HandleAttack()
    {
        if (target != null && Vector2.Distance(transform.position, target.transform.position) <= attackRange)
        {
            if (Time.time >= lastAttackTime + attackInterval)
            {
                lastAttackTime = Time.time;
                AttackTarget();
            }
        }
    }

    void AttackTarget()
    {
        if (target != null)
        {
            if (target.CompareTag("Troop"))
            {
                Troop troop = target.GetComponent<Troop>();
                if (troop != null)
                {
                    troop.TakeDamage(attackDamage);
                    Debug.Log("Attacked troop: " + troop.name + " for " + attackDamage + " damage.");

                    // Draw bullet tracer
                    StartCoroutine(DrawBulletTracer(target.transform.position));
                }
            }
            else if (target.CompareTag("Killdozer"))
            {
                Killdozer killdozerScript = target.GetComponent<Killdozer>();
                if (killdozerScript != null)
                {
                    killdozerScript.TakeDamage(attackDamage);
                    Debug.Log("Attacked Killdozer for " + attackDamage + " damage.");

                    // Draw bullet tracer
                    StartCoroutine(DrawBulletTracer(target.transform.position));
                }
                else
                {
                    Debug.LogError("Killdozer script not found on target!");
                }
            }
        }
    }

    IEnumerator DrawBulletTracer(Vector3 targetPosition)
    {
        LineRenderer lineRenderer = Instantiate(lineRendererPrefab);
        lineRenderer.SetPosition(0, transform.position + startOffset);
        lineRenderer.SetPosition(1, targetPosition);

        Color startColor = lineRenderer.startColor;
        Color endColor = lineRenderer.endColor;
        float fadeSpeed = 1f / tracerFadeDuration;

        float progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime * fadeSpeed;
            Color color = Color.Lerp(startColor, Color.clear, progress);
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            yield return null;
        }

        Destroy(lineRenderer.gameObject);
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
