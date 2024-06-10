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
    public float troopStoppingDistance = 1f; // distance which the enemy stops moving towards the target 
    public List<Transform> potentialTargets; // list of potential targets (players, killdozer)
    private Transform closestTarget;

    //Attack troops
    private bool isAttacking;

    public Transform killdozer;
    public float killdozerStoppingDistance = 3.0f;

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
            if (!target.gameObject.activeSelf) continue; // Skip inactive (dead) targets

            float distanceSqr = (target.position - transform.position).sqrMagnitude; // Calculate distance between enemy position and target position
            if (distanceSqr < closestDistanceSqr && distanceSqr <= detectionRange * detectionRange) // If current target is closer than other targets and if target is within enemy detection range
            {
                // Update closest target to current target
                closestDistanceSqr = distanceSqr;
                closestTarget = target;
                Debug.Log("Found " + closestTarget.name);
            }
        }
    }

    void MoveTowardsTarget()
    {
        if (closestTarget != null && closestTarget.gameObject.activeInHierarchy) // If have a closest target
        {
            float stoppingDistanceToUse = closestTarget == killdozer ? killdozerStoppingDistance : troopStoppingDistance; // Choose stopping distance based on if the target is the killdozer
            float distanceToTarget = Vector3.Distance(transform.position, closestTarget.position);

            if (distanceToTarget > stoppingDistanceToUse) // Move if distance to the target is greater than stopping distance
            {
                Vector3 direction = (closestTarget.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
            else if (!isAttacking) // otherwise stop and attack
            {
                Debug.Log("Attacking target");
                isAttacking = true;
                StartCoroutine(AttackTarget());
            }
        }
    }

    IEnumerator AttackTarget()
    {
        while (closestTarget != null && Vector3.Distance(transform.position, closestTarget.position) <= (closestTarget == killdozer ? killdozerStoppingDistance : troopStoppingDistance))
        {
            if (closestTarget == killdozer)
            {
                Killdozer killdozerScript = closestTarget.GetComponent<Killdozer>();
                if (killdozerScript != null)
                {
                    // Check if killdozer is dead
                    if (killdozerScript.currentHealth <= 0)
                    {
                        break;
                    }

                    killdozerScript.TakeDamage(attack); // Killdozer takes damage equal to attack
                    Debug.Log("Attacking killdozer");
                }
            }
            else
            {
                Troop troopScript = closestTarget.GetComponent<Troop>();
                if (troopScript != null)
                {
                    // Check if troop is dead
                    if (troopScript.currentHealth <= 0)
                    {
                        break;
                    }

                    troopScript.TakeDamage(attack); // Troop takes damage equal to attack
                    Debug.Log("Attacking troop: " + closestTarget.name);
                }
            }

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
