using System;
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

    //public float detectionRange = 10f; // enemy detection range

    public float detectionRangeX1 = 10f;
    public float detectionRangeY1 = 10f;
    //public float detectionRangeX2 = 5f;
    //public float detectionRangeY2 = 5f;

    public float troopStoppingDistance = 1f; // distance which the enemy stops moving towards the target 
    public List<Transform> potentialTargets; // list of potential targets (players, killdozer)
    private Transform closestTarget;
    private bool isAttacking;
    private bool attackingKilldozer;

    public Transform killdozerTransform; //killdozer position
    public Collider2D killdozerCollider;
    public float killdozerStoppingDistance = 5.0f;

    //visual effect from damaged
    public SpriteRenderer[] enemySprite;
    public Color DamagedColor;
    public Color NormalColor;
    float timer;
    bool tookdamage;
    Vector3 normalScale;

    public GameObject markForDeathIcon;
    public GameObject slowedIcon;
    public float slowEffectRadius = 5f;
    private Rigidbody2D rb;

    public bool slowArea = false;
    private bool isKnockedBack = false;
    private bool isStunned = false;
    private bool facingRight = false;
    public event Action onDeath;

    public GameObject damageIndicator;

    public DropEnergyOrbOnDeath energyOrb;
    public int energyOrbDropNum = 1;
    private int i = 0;

    private bool moveRight; //initial move direction
    //float radius = 0.1f;  //check targets inside the Killdozer

    public bool isDummy;
    public bool dummyDead;

    [Header(" Art / Animations ")]

    public GameObject EnemyModel;
    TroopAnimationsManager Animator;

    private void Start()
    {
        if(isDummy)
        {
            damageIndicator.SetActive(false);
        }

        if (EnemyModel != null)
        {
            Animator = EnemyModel.GetComponent<TroopAnimationsManager>();
        }

        rb = GetComponent<Rigidbody2D>();
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
            //killdozerCollider = killdozer.GetComponent<BoxCollider2D>();
        }

        normalScale = transform.localScale;

        if (killdozerTransform != null) //determine move direction
        {
            moveRight = killdozerTransform.position.x > transform.position.x; //if kd is on the right
        }
    }

    void Update()
    {
        if (tookdamage == true)
        {
            foreach (SpriteRenderer sprite in enemySprite)
            {
                sprite.color = DamagedColor;
            }

            timer = timer + Time.deltaTime;
            transform.localScale += transform.localScale / 1000;

            if (timer >= 0.3)
            {
                foreach (SpriteRenderer sprite in enemySprite)
                {
                    sprite.color = NormalColor;
                }

                transform.localScale = normalScale;
                timer = 0;
                tookdamage = false;
            }
        }

        if (isDummy)
        {
            return;
        }

        FindClosestTarget();
        MoveTowardsTarget();

            if (moveRight == true)
            {
                EnemyModel.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (moveRight == false)
            {
                EnemyModel.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
    }

    void FindClosestTarget()
    {
        potentialTargets.RemoveAll(target => target == null); // Removes null transforms in potential target list, so that enemy can always find a real transform

        closestTarget = null; 
        float closestDistanceSqr = Mathf.Infinity;

        foreach (Transform target in potentialTargets) // For each target in the list
        {
            if (!target.gameObject.activeSelf) continue; // Skip inactive (dead) targets

            Vector3 offset = target.position - transform.position;
            float distanceSqr = offset.sqrMagnitude; // Calculate distance between enemy position and target position

            // Check if the target is in the initial move direction
            bool isTargetInInitialDirection = (moveRight && offset.x > 0) || (!moveRight && offset.x < 0);

            // Check if the target is within the horizontal detection range
            bool isTargetInHorizontalRange = Mathf.Abs(offset.x) <= detectionRangeX1 && Mathf.Abs(offset.y) <= detectionRangeY1;

            if (distanceSqr < closestDistanceSqr && isTargetInHorizontalRange && isTargetInInitialDirection) // If current target is closer than other targets and within detection range and in the initial direction
            {
                if (target == killdozerTransform || (target != killdozerTransform && (IsInsideKilldozer(target) || attackingKilldozer)))
                {
                    // Prioritize the Killdozer if the target is the Killdozer itself, or if it is inside it, or if enemy is already attacking the Killdozer
                    closestTarget = killdozerTransform;
                    closestDistanceSqr = distanceSqr;
                }
                else
                {
                    // Update closest target to current target
                    closestDistanceSqr = distanceSqr;
                    closestTarget = target;
                }
            }
        }
    }

    void MoveTowardsTarget()
    {
        if (closestTarget != null && closestTarget.gameObject.activeInHierarchy && !isStunned) // If have a closest target
        {
            float stoppingDistanceToUse = closestTarget == killdozerTransform ? killdozerStoppingDistance : troopStoppingDistance; // Choose stopping distance based on if the target is the killdozer
            float distanceToTarget = Vector3.Distance(transform.position, closestTarget.position);

            facingRight = closestTarget.position.x > transform.position.x; //check facing direction (for knock back)

            // Ensure enemy only moves in the initial direction
            if ((moveRight && closestTarget.position.x > transform.position.x) || (!moveRight && closestTarget.position.x < transform.position.x))
            {
                if (distanceToTarget > stoppingDistanceToUse) // Move if distance to the target is greater than stopping distance
                {
                    Vector3 direction = (closestTarget.position - transform.position).normalized;
                    if (Animator != null)
                    {
                        Animator.TroopWalkOn();
                    }
                    direction.y = 0; //only move horizontally
                    transform.position += direction * moveSpeed * Time.deltaTime;
                }
                else if (!isAttacking) // otherwise stop and attack
                {
                    if (Animator != null)
                    {
                        Animator.TroopWalkOff();
                    }
                    isAttacking = true;
                    StartCoroutine(AttackTarget());
                }
            }
        }
    }

    public void AddTargetToList(Transform objectTransform)
    {
        potentialTargets.RemoveAll(target => target == null); //removes null transforms in potential target list, so that enemy can always find a real transform
        potentialTargets.Add(objectTransform);

        FindClosestTarget();
    }

    bool IsInsideKilldozer(Transform target)
    {
        /*
        radius = 0.1f;

        // Check if any point within the radius around the target's position is inside the Killdozer's collider bounds
        Vector2 targetPosition = target.position;
        return killdozerCollider.OverlapPoint(targetPosition) ||
               killdozerCollider.OverlapPoint(targetPosition + new Vector2(radius, 0)) ||
               killdozerCollider.OverlapPoint(targetPosition + new Vector2(-radius, 0)) ||
               killdozerCollider.OverlapPoint(targetPosition + new Vector2(0, radius)) ||
               killdozerCollider.OverlapPoint(targetPosition + new Vector2(0, -radius));

        */
        //return killdozerCollider.bounds.Contains(target.position);
        //Debug.Log(target);

        if (target == null)
        {
            return false;
        }

        //return target.GetComponent<Troop>().troopOnKilldozer;

        Troop troopComponent = target.GetComponent<Troop>();
        if (troopComponent == null)
        {
            Debug.Log("Target does not have a Troop component");
            return false;
        }

        //if (killdozerCollider == null)
        //{
        //Debug.Log("Killdozer Collider is null in IsInsideKilldozer");
        //return false;
        // }

        //Debug.Log(troopComponent.troopOnKilldozer);
        return troopComponent.troopOnKilldozer;
    }

    IEnumerator AttackTarget()
    {
        while (closestTarget != null && Vector3.Distance(transform.position, closestTarget.position) <= (closestTarget == killdozerTransform ? killdozerStoppingDistance : troopStoppingDistance) && !isStunned)
        {
            Animator.TroopAttackOn();

            yield return new WaitForSeconds(attackSpeed);

            if (closestTarget == killdozerTransform)
            {
                Killdozer killdozerScript = closestTarget.GetComponent<Killdozer>();
                if (killdozerScript != null)
                {
                    // Check if killdozer is dead
                    if (killdozerScript.currentHealth <= 0)
                    {
                        attackingKilldozer = false;
                        break;
                    }

                    killdozerScript.TakeDamage(attack); // Killdozer takes damage equal to attack
                    attackingKilldozer = true;
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
        }
        closestTarget = null; // deselect target
        isAttacking = false;
        Animator.TroopAttackOff();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; //enemy detection range
        //Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.white; //enemy slowed affected range
        Gizmos.DrawWireSphere(transform.position, slowEffectRadius);

        Gizmos.color = Color.blue; //targets in kd range
        //Gizmos.DrawWireSphere(killdozerTransform.position, radius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector2(detectionRangeX1, detectionRangeY1));
    }

    public void TakeDamage(int damage)
    {
        FindObjectOfType<AudioManager>().Play("MetalHit2");

        if (invincible)
        {
            return;
        }

        if (markedForDeath)
        {
            damage = damage * 2;
        }

        tookdamage = true;
        currentHealth -= damage;
        //Debug.Log("Enemy took " + damage + " damage.");

        damageIndicator.SetActive(true);
        StartCoroutine(DisableDamageIndicator());

        if (currentHealth <= 0)
        {
            tookdamage = false;
            StartCoroutine(Death());
        }
    }

    IEnumerator DisableDamageIndicator()
    {
        yield return new WaitForSeconds(0.2f);

        damageIndicator.SetActive(false);
    }

    IEnumerator Death()
    {
        FindObjectOfType<AudioManager>().Play("MetalHit");

        if (!isDummy)
        {
            Animator.TroopDies();
        }

        while (i < energyOrbDropNum)
        {
            energyOrb.DropEnergyOrb();
            i++;
        }

        if (isDummy)
        {
            dummyDead = true;
            i = 0; //reset energy dropped amount
            damageIndicator.SetActive(false);
            gameObject.SetActive(false);
            yield break;
        }

        onDeath.Invoke();

        yield return new WaitForSeconds(0.75f);
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

        if (slowArea)
        {
            SlowNearbyEnemies();
        }

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
        slowedIcon.SetActive(true);
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
            slowedIcon.SetActive(false);
        }
    }

    public void ApplyKnockback(Vector3 attackerPosition)
    {
        if (!isKnockedBack)
        {
            StartCoroutine(KnockbackCoroutine(attackerPosition));
        }
    }

    [Header(" knockback ")]
    public float knockbackForce;
    private IEnumerator KnockbackCoroutine(Vector3 attackerPosition)
    {
        isKnockedBack = true;

        //float knockbackForce = 10f;

        if (facingRight)
        {
            rb.AddForce(Vector2.left * knockbackForce, ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(Vector2.right * knockbackForce, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(0.3f); // Knockback effect duration

        // Stop the knockback by setting velocity to zero
        rb.velocity = new Vector2(0, rb.velocity.y);

        isKnockedBack = false;
    }

    public void Stun(bool stun)
    {
        isStunned = stun;
    }
}

