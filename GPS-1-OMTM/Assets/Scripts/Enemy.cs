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
    public bool markedForDeath = false;
    public bool slowed = false;

    public float detectionRange = 10f; // enemy detection range
    public float troopStoppingDistance = 1f; // distance which the enemy stops moving towards the target 
    public List<Transform> potentialTargets; // list of potential targets (players, killdozer)
    private Transform closestTarget;

    //Attack troops
    private bool isAttacking;

    public Transform killdozerTransform; //killdozer position
    public float killdozerStoppingDistance = 5.0f;

    //visual effect from damaged
    public SpriteRenderer enemy;
    public Color DamagedColor;
    public Color NormalColor;
    float timer;
    bool tookdamage;
    Vector3 normalScale;

    public GameObject markForDeathIcon;
    public float slowEffectRadius = 5f;

    private void Start()
    {
        currentHealth = maxHealth;

        // Find all troops and add their transforms to the potentialTargets list
        Troop[] allTroops = FindObjectsOfType<Troop>();
        foreach (Troop troop in allTroops)
        {
            potentialTargets.Add(troop.transform);
        }

        Killdozer killdozer = FindObjectOfType<Killdozer>();
        if (killdozer != null)
        {
            potentialTargets.Add(killdozer.transform);
            killdozerTransform = killdozer.transform;
        }

        normalScale = transform.localScale;
    }

    void Update()
    {
        FindClosestTarget();
        MoveTowardsTarget();

        if (tookdamage == true)
        {
            
            enemy.color = DamagedColor;
            timer = timer + Time.deltaTime;
            transform.localScale += transform.localScale / 1000;

            if (timer >= 0.3)
            {
                enemy.color = NormalColor;
                transform.localScale = normalScale;
                timer = 0;
                tookdamage = false;
            }
        }
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
                //Debug.Log("Found " + closestTarget.name);
            }
        }
    }

    void MoveTowardsTarget()
    {
        if (closestTarget != null && closestTarget.gameObject.activeInHierarchy) // If have a closest target
        {
            float stoppingDistanceToUse = closestTarget == killdozerTransform ? killdozerStoppingDistance : troopStoppingDistance; // Choose stopping distance based on if the target is the killdozer
            float distanceToTarget = Vector3.Distance(transform.position, closestTarget.position);

            if (distanceToTarget > stoppingDistanceToUse) // Move if distance to the target is greater than stopping distance
            {
                Vector3 direction = (closestTarget.position - transform.position).normalized;
                direction.y = 0; //only move horizontally
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
            else if (!isAttacking) // otherwise stop and attack
            {
                //Debug.Log("Attacking target");
                isAttacking = true;
                StartCoroutine(AttackTarget());
            }
        }
    }

    public void AddTargetToList(Transform objectTransform)
    {
        potentialTargets.RemoveAll(target => target == null); //removes null transforms in potential target list, so that enemy can always find a real transform
        potentialTargets.Add(objectTransform);

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
                //Debug.Log("Found " + closestTarget.name);
            }
        }
    }

    IEnumerator AttackTarget()
    {
        while (closestTarget != null && Vector3.Distance(transform.position, closestTarget.position) <= (closestTarget == killdozerTransform ? killdozerStoppingDistance : troopStoppingDistance))
        {
            if (closestTarget == killdozerTransform)
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
                    //Debug.Log("Attacking killdozer");
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
                    //Debug.Log("Attacking troop: " + closestTarget.name);
                }
            }

            yield return new WaitForSeconds(attackSpeed);
        }
        closestTarget = null; // deselect target
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; //enemy detection range
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.white; //enemy slowed affected range
        Gizmos.DrawWireSphere(transform.position, slowEffectRadius);
    }

    public void TakeDamage(int damage)
    {
        if (invincible)
        {
            return;
        }

        if (markedForDeath)
        {
            damage = damage * 2;
        }

        currentHealth -= damage;
        Debug.Log("Enemy took " + damage + " damage.");

        if (currentHealth <= 0)
        {
            Death();
        }

        tookdamage = true;
    }


    void Death()
    {
        // Put death animation or effects

        Debug.Log("Enemy is dead");
        Destroy(gameObject);
    }

    public void MarkForDeathStart()
    {
        if (markedForDeath) //no effect if enemy is already marked for death
        {
            return;
        }

        markedForDeath = true;
        markForDeathIcon.SetActive(true);
        SlowNearbyEnemies();
        StartCoroutine((MarkForDeathEnd()));
    }

    IEnumerator MarkForDeathEnd()
    {
        yield return new WaitForSeconds(10000f);

        if (gameObject != null)
        {
            markedForDeath = false;
            markForDeathIcon.SetActive(false);
        }
    }

    void SlowNearbyEnemies()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, slowEffectRadius);
        foreach (var hitCollider in hitColliders)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null && enemy != this && !enemy.slowed)
            {
                enemy.SlowedSpeedStart();
            }
        }
    }

    public void SlowedSpeedStart()
    {
        if (slowed) //no effect if enemy is already slowed
        {
            return;
        }

        slowed = true;
        moveSpeed = moveSpeed / 2;
        StartCoroutine((SlowedSpeedEnd()));
    }

    IEnumerator SlowedSpeedEnd()
    {
        yield return new WaitForSeconds(10000f);

        if (gameObject != null)
        {
            moveSpeed = moveSpeed * 2;
            slowed = false;
        }
    }
}
