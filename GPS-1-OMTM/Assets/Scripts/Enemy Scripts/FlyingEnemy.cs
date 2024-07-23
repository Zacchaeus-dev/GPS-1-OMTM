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

    private GameObject targetTroop;
    private GameObject killdozer;
    private GameObject killdozerLeftTarget;
    private GameObject killdozerRightTarget;
    private Vector3 rightOffset = new Vector3 (30f,0,0);
    private Vector3 leftOffset = new Vector3 (-30f,0,0);
    private bool attackingKilldozer;
    private float lastAttackTime = 0f;
    private bool shouldMove = true; // Flag to control movement

    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;
        lastAttackTime = Time.time;

        // Find the Killdozer in the scene
        killdozer = GameObject.FindWithTag("Killdozer");
        killdozerLeftTarget = killdozer.GetComponent<Killdozer>().leftTarget;
        killdozerRightTarget = killdozer.GetComponent<Killdozer>().rightTarget;
    }

    void Update()
    {
        if (shouldMove && killdozerRightTarget.transform.position.x + rightOffset.x  < transform.position.x) //move depending on killdozer's location
        {
            MoveLeft();
        }
        else if (shouldMove && killdozerLeftTarget.transform.position.x + leftOffset.x > transform.position.x)
        {
            MoveRight();
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

    void MoveRight()
    {
        transform.position -= Vector3.left * speed * Time.deltaTime;
    }

    void DetectTargets()
    {
        if (attackingKilldozer)
        {
            return;
        }

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

    void MoveTowardsTarget()
    {
        if (killdozer != null)
        {
            float distanceToKilldozer = 0;
            Vector3 direction = (killdozer.transform.position - transform.position).normalized;

            if (killdozer.transform.position.x < transform.position.x)
            {
                // Calculate distance to the Killdozer
                distanceToKilldozer = Vector2.Distance(transform.position, killdozerRightTarget.transform.position);
                direction = (killdozerRightTarget.transform.position - transform.position + rightOffset).normalized;
            }
            else if (killdozer.transform.position.x> transform.position.x)
            {
                // Calculate distance to the Killdozer
                distanceToKilldozer = Vector2.Distance(transform.position, killdozerLeftTarget.transform.position);
                direction = (killdozerLeftTarget.transform.position - transform.position + leftOffset).normalized;
            }

            // Move towards the Killdozer if it's within detection range
            if (distanceToKilldozer > attackRange)
            {
                //Vector3 direction = (killdozer.transform.position - transform.position).normalized;
                direction.y = 0; // Ensure no movement in Y-axis
                transform.position += direction * speed * Time.deltaTime;
            }
            else
            {
                shouldMove = false; // Stop moving if within attack range of Killdozer
            }
        }

        if (targetTroop != null)
        {
            // Calculate distance to the target troop
            float distanceToTroop = Vector2.Distance(transform.position, targetTroop.transform.position);

            // Move towards the troop if outside attack range, only in X-axis
            if (distanceToTroop > attackRange)
            {
                Vector3 direction = (targetTroop.transform.position - transform.position).normalized;
                direction.y = 0; // Ensure no movement in Y-axis
                transform.position += direction * speed * Time.deltaTime;
            }
        }
    }

    void HandleAttack()
    {
        if (killdozer != null && (Vector2.Distance(transform.position, killdozerRightTarget.transform.position) <= attackRange) || Vector2.Distance(transform.position, killdozerLeftTarget.transform.position) <= attackRange)
        {
            if (Time.time >= lastAttackTime + attackInterval)
            {
                lastAttackTime = Time.time;
                AttackKilldozer();
                attackingKilldozer = true;
            }
        }
        else if (targetTroop != null && Vector2.Distance(transform.position, targetTroop.transform.position) <= attackRange)
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

                // Draw bullet tracer
                StartCoroutine(DrawBulletTracer(targetTroop.transform.position));
            }
        }
    }

    void AttackKilldozer()
    {
        if (killdozer != null)
        {
            Killdozer kd = killdozer.GetComponent<Killdozer>();
            if (kd != null)
            {
                kd.TakeDamage(attackDamage);
                Debug.Log("Attacked Killdozer for " + attackDamage + " damage.");

                if (killdozer.transform.position.x < transform.position.x)
                {
                    StartCoroutine(DrawBulletTracer(killdozerLeftTarget.transform.position));
                }
                else if (killdozer.transform.position.x > transform.position.x)
                {
                    StartCoroutine(DrawBulletTracer(killdozerRightTarget.transform.position));
                }

                //StartCoroutine(DrawBulletTracer(killdozer.transform.position));
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
        //Debug.Log(name + " took " + damage + " damage, remaining health: " + currentHealth);

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