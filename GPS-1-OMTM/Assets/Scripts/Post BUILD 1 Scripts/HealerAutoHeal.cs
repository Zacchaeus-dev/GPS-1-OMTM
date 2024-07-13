using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerAutoHeal : MonoBehaviour
{
    public bool autoHealEnabled = false;
    public int healAmount = 10; // Amount of health to heal per heal
    public float detectionRange = 3f; // Range within which the healer can detect allies
    public float healRange = 1.5f; // Range within which the healer can heal allies
    public float healCooldown = 1f; // Time between heals
    public float moveSpeed = 2f; // Speed at which the healer moves towards the ally

    private float lastHealTime = 0f;
    private GameObject targetAlly;
    private Rigidbody2D rb;

    private TroopEnergy troopEnergy;
    public TroopWeapon troopWeapon;

    // Public references for LineRenderer and start position offset
    public LineRenderer lineRenderer;
    public Vector3 startPositionOffset;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        troopEnergy = GetComponent<TroopEnergy>();
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false; // Initially disable the line renderer
        }
    }

    void Update()
    {
        if (autoHealEnabled)
        {
            if (targetAlly == null)
            {
                FindTarget();
            }
            else
            {
                //MoveTowardsTarget();
                HealTarget();
            }
        }
    }

    void FindTarget()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        float closestDistance = Mathf.Infinity;
        GameObject closestAlly = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Troop") && hitCollider.gameObject != this.gameObject)
            {
                float distanceToAlly = Vector2.Distance(transform.position, hitCollider.transform.position);
                if (distanceToAlly < closestDistance)
                {
                    closestDistance = distanceToAlly;
                    closestAlly = hitCollider.gameObject;
                }
            }
        }

        targetAlly = closestAlly;
    }

    void MoveTowardsTarget()
    {
        if (targetAlly != null)
        {
            float distanceToAlly = Vector2.Distance(transform.position, targetAlly.transform.position);
            if (distanceToAlly > healRange)
            {
                Vector2 direction = (targetAlly.transform.position - transform.position).normalized;
                direction.y = 0;
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
            }
        }
    }

    void HealTarget()
    {
        if (targetAlly != null)
        {
            float distanceToAlly = Vector2.Distance(transform.position, targetAlly.transform.position);
            if (distanceToAlly <= healRange)
            {
                if (Time.time >= lastHealTime + healCooldown)
                {
                    Troop allyTroop = targetAlly.GetComponent<Troop>();
                    if (allyTroop != null)
                    {
                        allyTroop.currentHealth = Mathf.Min(allyTroop.currentHealth + healAmount, allyTroop.maxHealth);
                        lastHealTime = Time.time;
                        //Debug.Log(targetAlly.name + " healed by " + healAmount + " to " + allyTroop.currentHealth + " health.");
                        troopEnergy.GainPower();
                        StartCoroutine(ShowHealTracer(targetAlly.transform));
                    }
                }
            }
            else
            {
                targetAlly = null; // Lost range, find another target
            }
        }
    }

    IEnumerator ShowHealTracer(Transform target)
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position + startPositionOffset);
            lineRenderer.SetPosition(1, target.position);

            float elapsedTime = 0f;
            float tracerDuration = 0.5f; // Duration for which the tracer is visible

            while (elapsedTime < tracerDuration)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            lineRenderer.enabled = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw heal range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, healRange);

        // Draw start position offset
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + startPositionOffset, 0.1f);
    }
}
