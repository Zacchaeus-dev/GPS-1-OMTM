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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
                MoveTowardsTarget();
                HealTarget();
            }
        }
    }

    void FindTarget()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Troop") && hitCollider.gameObject != this.gameObject)
            {
                targetAlly = hitCollider.gameObject;
                break;
            }
        }
    }

    void MoveTowardsTarget()
    {
        if (targetAlly != null)
        {
            float distanceToAlly = Vector2.Distance(transform.position, targetAlly.transform.position);
            if (distanceToAlly > healRange)
            {
                Vector2 direction = (targetAlly.transform.position - transform.position).normalized;
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
                        Debug.Log(targetAlly.name + " healed by " + healAmount + " to " + allyTroop.currentHealth + " health.");
                    }
                }
            }
            else
            {
                targetAlly = null; // Lost range, find another target
            }
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
    }
}
