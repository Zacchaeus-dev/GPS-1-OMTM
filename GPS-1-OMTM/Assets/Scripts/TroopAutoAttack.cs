using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopAutoAttack : MonoBehaviour
{
    public bool autoAttackEnabled = false;
    public int attackDamage = 10; // Attack damage settable via Inspector
    public float detectionRange = 3f; // Range within which the troop can detect enemies
    public float attackRange = 1.5f; // Range within which the troop can attack enemies
    public float attackCooldown = 1f; // Time between attacks
    //public float moveSpeed = 2f; // Speed at which the troop moves towards the enemy

    private float lastAttackTime = 0f;
    private GameObject targetEnemy;
    private Rigidbody2D rb;
    public Animator attackAnimation;

    public Troop troop;
    public TroopCharacter troopCharacter;

    public enum TroopCharacter
    {
        DPS,
        Tank,
        CC,
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (autoAttackEnabled)
        {
            if (targetEnemy == null)
            {
                FindTarget();
            }
            else
            {
                //MoveTowardsTarget();
                AttackTarget();
            }
        }
    }

    void FindTarget()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                targetEnemy = hitCollider.gameObject;
                break;
            }
        }
    }

    /*void MoveTowardsTarget()
    {
        if (targetEnemy != null)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, targetEnemy.transform.position);
            if (distanceToEnemy > attackRange)
            {
                Vector2 direction = (targetEnemy.transform.position - transform.position).normalized;
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
            }
        }
    }*/

    void AttackTarget()
    {
        if (targetEnemy != null)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, targetEnemy.transform.position);
            if (distanceToEnemy <= attackRange)
            {
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    // Implement the damage dealing logic here
                    Enemy enemy = targetEnemy.GetComponent<Enemy>();

                    if (enemy != null)
                    {
                        switch (troopCharacter) //do different attack based on the troop
                        {
                            case TroopCharacter.DPS:
                                enemy.TakeDamage(attackDamage);
                                break;
                            case TroopCharacter.Tank:
                                enemy.TakeDamage(attackDamage);
                                break;
                            case TroopCharacter.CC:
                                enemy.TakeDamage(attackDamage);
                                enemy.MarkForDeathStart();
                                break;
                            default:
                                enemy.TakeDamage(attackDamage);
                                break;
                        }
                    }
                    else
                    {
                        targetEnemy.GetComponent<FlyingEnemy>().TakeDamage(attackDamage);
                    }

                    //targetEnemy.GetComponent<EnemyHealth>().TakeDamage(attackDamage);
                    //targetEnemy.GetComponent<FlyingEnemy>().TakeDamage(attackDamage);
                    lastAttackTime = Time.time;
                }
            }
            else
            {
                targetEnemy = null; // Lost range, find another target
            }
        }

        //chg gun up sprite
        attackAnimation.SetBool("Attack", true);
    }

    public void SetTargetEnemy(GameObject enemy)
    {
        targetEnemy = enemy;
    }

    void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}