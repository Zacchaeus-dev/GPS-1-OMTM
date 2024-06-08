using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy: MonoBehaviour
{
    public bool invincible = false;

    //Enemy stats
    public int maxHealth;
    public int currentHealth;
    public int attack;
    public float attackSpeed;
    public float attackRange;
    public float moveSpeed = 2f;

    public float detectionRange = 10f; // enemy detection range
    public float stoppingDistance = 1f; // distance which the enemy stops moving towards the target 
    public List<Transform> potentialTargets; // list of potential targets (players, killdozer)
    private Transform closestTarget;

    //Attack troops
    private bool isAttacking;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        FindClosestTarget();
        MoveTowardsTarget();
    }

    void FindClosestTarget()
    {
        potentialTargets.RemoveAll(target => target == null); //removes null transforms in potential target list, so that enemy can always find a real transform

        closestTarget = null; // Start with no target
        float closestDistanceSqr = Mathf.Infinity;

        foreach (Transform target in potentialTargets) // For each target in the list
        {
            float distanceSqr = (target.position - transform.position).sqrMagnitude; // Calculate distance between enemy position and target position
            if (distanceSqr < closestDistanceSqr && distanceSqr <= detectionRange * detectionRange) // If current target is closer than other targets and if target is within enemy detection range
            {
                // Update closest target to current target
                closestDistanceSqr = distanceSqr;
                closestTarget = target;
            }
        }
    }

    void MoveTowardsTarget()
    {
        if (closestTarget != null) // If have a closest target
        {
            float distanceToTarget = Vector3.Distance(transform.position, closestTarget.position);

            if (distanceToTarget > stoppingDistance) // Move if distance to the target is greater than stopping distance
            {
                Vector3 direction = (closestTarget.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
            else if(!isAttacking) // otherwise stop and attack
            {
                Debug.Log("Attacking target");
                isAttacking = true;
                StartCoroutine(AttackTarget());
            }
        }
    }

    IEnumerator AttackTarget()
    {
        while (closestTarget != null && Vector2.Distance(transform.position, closestTarget.position) <= attackRange)
        {
            closestTarget.GetComponent<Troop>().TakeDamage(attack); // troop takes damage equal to attack
            Debug.Log("Attacking target: " + closestTarget.name);
            yield return new WaitForSeconds(attackSpeed);
        }

        closestTarget = null; // deselect target
        isAttacking = false;
    }

    // visualize enemy detection range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    public void TakeDamage(int damage)
    {
        if (invincible)
        {
            return;
        }

        currentHealth -= damage;
        Debug.Log("Enemy took " + damage + " damage.");

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        // Put death animation or effects

        Debug.Log("Enemy is dead");
        Destroy(gameObject);
    }
}
