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
    public Vector3 startOffset; // Offset for the start point of the bullet tracer
    public LineRenderer lineRendererPrefab; // Prefab for the line renderer
    public float tracerFadeDuration = 0.5f; // Duration of the fade-out

    private float lastAttackTime = 0f;
    private GameObject targetEnemy;
    private Rigidbody2D rb;
    public Animator attackAnimation;

    public Troop troop;
    public TroopCharacter troopCharacter;
    public TroopClass troopClass;

    public TroopEnergy troopEnergy;

    private int tankAttackCounter = 0;
    public enum TroopCharacter
    {
        DPS,
        Tank,
        CC,
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        troopEnergy = GetComponent<TroopEnergy>();
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
                AttackTarget();
            }
        }
    }

    void FindTarget()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hitCollider.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hitCollider.gameObject;
                }
            }
        }

        if (closestEnemy != null)
        {
            targetEnemy = closestEnemy;
        }
    }

    void AttackTarget()
    {
        if (troopClass.isMoving == false) //refering to the TroopClass on "(moving == false)" then this autoattack is activated......
        {
            if (targetEnemy != null)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, targetEnemy.transform.position);
                if (distanceToEnemy <= attackRange)
                {
                    if (Time.time >= lastAttackTime + attackCooldown)
                    {
                        // Implement the attack here
                        Enemy enemy = targetEnemy.GetComponent<Enemy>();

                        if (enemy != null)
                        {
                            switch (troopCharacter) //do different attack based on the troop
                            {
                                case TroopCharacter.DPS:
                                    enemy.TakeDamage(attackDamage);
                                    DrawBulletTracer(transform.position + startOffset, targetEnemy.transform.position);
                                    troopEnergy.GainPower();
                                    break;
                                case TroopCharacter.Tank:
                                    TankAttack();
                                    break;
                                case TroopCharacter.CC:
                                    enemy.TakeDamage(attackDamage);
                                    enemy.MarkForDeathStart();
                                    DrawBulletTracer(transform.position + startOffset, targetEnemy.transform.position);
                                    troopEnergy.GainPower();
                                    break;
                                default:
                                    enemy.TakeDamage(attackDamage);
                                    DrawBulletTracer(transform.position + startOffset, targetEnemy.transform.position);
                                    break;
                            }
                        }
                        else
                        {
                            targetEnemy.GetComponent<FlyingEnemy>().TakeDamage(attackDamage);
                            DrawBulletTracer(transform.position + startOffset, targetEnemy.transform.position);
                        }

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

    }

    void TankAttack()
    {
        /* //single attack
        tankAttackCounter++;
        if (tankAttackCounter < 3)
        {
            // No damage for the first two attacks
            Debug.Log("Attack: " +  tankAttackCounter);
        }
        else
        {
            // Third attack deals damage and applies knockback
            enemy.TakeDamage(attackDamage);
            enemy.ApplyKnockback(transform.position);
            tankAttackCounter = 0; // Reset the counter
        }
        */

        //aoe
        tankAttackCounter++;
        if (tankAttackCounter < 3)
        {
            // No damage for the first two attacks
            //Debug.Log("Attack: " + tankAttackCounter);
        }
        else
        {
            // Third attack deals AoE damage and applies knockback
            Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Enemy"));
            foreach (Collider2D enemyCollider in enemiesHit)
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(attackDamage);
                    enemy.ApplyKnockback(transform.position);
                }
            }
            tankAttackCounter = 0; // Reset the counter
        }
    }

    void DrawBulletTracer(Vector3 start, Vector3 end)
    {
        LineRenderer line = Instantiate(lineRendererPrefab);
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        StartCoroutine(FadeOutLine(line));
    }

    IEnumerator FadeOutLine(LineRenderer line)
    {
        float elapsedTime = 0f;
        Color startColor = line.startColor;
        Color endColor = line.endColor;

        while (elapsedTime < tracerFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / tracerFadeDuration);
            Color newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            line.startColor = newColor;
            line.endColor = newColor;
            yield return null;
        }

        Destroy(line.gameObject);
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