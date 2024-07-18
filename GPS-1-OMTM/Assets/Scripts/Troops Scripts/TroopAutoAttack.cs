using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public float lastAttackTime = 0f;
    public GameObject targetEnemy;
    private Rigidbody2D rb;


    public Troop troop;
    public TroopCharacter troopCharacter;
    public TroopClass troopClass;
    public TroopEnergy troopEnergy;
    public TroopWeapon troopWeapon;
    private int tankAttackCounter = 0;
    private int dpsWeapon2Range = 10;
    private int dpsWeapon2Width = 3;
    public enum TroopCharacter
    {
        DPS,
        Tank,
        CC,
    }

    public GameObject TroopModel;
    TroopAnimationsManager TroopAnimator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        troopEnergy = GetComponent<TroopEnergy>();
        DetermineAttack();

        TroopAnimator = TroopModel.GetComponent<TroopAnimationsManager>();
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

    void DetermineAttack()
    {
        switch (troopWeapon.selectedWeapon) //determine attack based on selected weapon
        {
            case TroopWeapon.Weapon.Weapon1_DPS:
                attackDamage = 20;
                attackCooldown = 0.25f;
                break;
            case TroopWeapon.Weapon.Weapon2_DPS:
                attackDamage = 30;
                attackCooldown = 1f;
                break;
            case TroopWeapon.Weapon.Weapon1_Tank:
                attackDamage = 10;
                attackCooldown = 1f;
                break;
            case TroopWeapon.Weapon.Weapon2_Tank:
                attackDamage = 75;
                attackCooldown = 1.5f;
                break;
            case TroopWeapon.Weapon.Weapon1_CC:
                attackDamage = 10;
                attackCooldown = 1f;
                break;
            case TroopWeapon.Weapon.Weapon2_CC:
                attackDamage = 15;
                attackCooldown = 2f;
                break;
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
                        Enemy enemy = targetEnemy.GetComponent<Enemy>();

                        if (enemy != null)
                        {
                            switch (troopCharacter) //do different attack based on the troop and weapon
                            {
                                case TroopCharacter.DPS:
                                    switch (troopWeapon.selectedWeapon)
                                    {
                                        case TroopWeapon.Weapon.Weapon1_DPS:
                                            DPS_Weapon1Attack(enemy);
                                            break;
                                        case TroopWeapon.Weapon.Weapon2_DPS:
                                            DPS_Weapon2Attack(enemy);
                                            break;
                                    }
                                    break;
                                case TroopCharacter.Tank:
                                    switch (troopWeapon.selectedWeapon)
                                    {
                                        case TroopWeapon.Weapon.Weapon1_Tank:
                                            Tank_Weapon1Attack();
                                            TroopAnimator.TroopAttack1();
                                            break;
                                        case TroopWeapon.Weapon.Weapon2_Tank:
                                            Tank_Weapon2Attack(enemy);
                                            TroopAnimator.TroopAttack1();
                                            break;
                                    }
                                    break;
                                case TroopCharacter.CC:
                                    switch (troopWeapon.selectedWeapon)
                                    {
                                        case TroopWeapon.Weapon.Weapon1_CC:
                                            CC_Weapon1Attack(enemy);
                                            break;
                                        case TroopWeapon.Weapon.Weapon2_CC:
                                            CC_Weapon2Attack(enemy);
                                            break;
                                    }
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
            
        }
    }

    void DPS_Weapon1Attack(Enemy enemy)
    {
        enemy.TakeDamage(attackDamage);
        DrawBulletTracer(transform.position + startOffset, targetEnemy.transform.position);
        troopEnergy.GainPower();
    }

    void DPS_Weapon2Attack(Enemy _enemy)
    {
        Vector3 targetPosition = _enemy.transform.position;
        Vector2 attackDirection = (targetPosition - transform.position).normalized;

        // Define the rectangle's size
        float halfWidth = dpsWeapon2Width / 2;
        float length = dpsWeapon2Range;

        // Find all enemies within a larger circle
        Collider2D[] allEnemies = Physics2D.OverlapCircleAll(transform.position, length, LayerMask.GetMask("Enemy"));

        foreach (Collider2D enemyCollider in allEnemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                Vector2 toEnemy = enemy.transform.position - transform.position;
                float distanceAlongDirection = Vector2.Dot(toEnemy, attackDirection);
                float distancePerpendicular = Mathf.Abs(Vector2.Dot(toEnemy, new Vector2(-attackDirection.y, attackDirection.x)));

                // Check if the enemy is within the length and width of the attack rectangle
                if (distanceAlongDirection > 0 && distanceAlongDirection <= length && distancePerpendicular <= halfWidth)
                {
                    enemy.TakeDamage(attackDamage);
                    DrawBulletTracer(transform.position + startOffset, targetEnemy.transform.position);
                }
            }
        }

        DrawAttackRectangle(transform.position, attackDirection, dpsWeapon2Width, dpsWeapon2Range);
        troopEnergy.GainPower();
    }

    void DrawAttackRectangle(Vector3 start, Vector3 direction, float width, float length)
    {
        // Calculate the four corners of the rectangle
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward).normalized * (width / 2);
        Vector3 topLeft = start + perpendicular;
        Vector3 topRight = start - perpendicular;
        Vector3 bottomLeft = topLeft + (direction.normalized * length);
        Vector3 bottomRight = topRight + (direction.normalized * length);

        LineRenderer line = Instantiate(lineRendererPrefab);
        line.positionCount = 5; // Four corners plus the start point to close the rectangle

        line.SetPosition(0, topLeft);
        line.SetPosition(1, bottomLeft);
        line.SetPosition(2, bottomRight);
        line.SetPosition(3, topRight);
        line.SetPosition(4, topLeft); // Closing the rectangle

        StartCoroutine(FadeOutRectangle(line));
    }

    IEnumerator FadeOutRectangle(LineRenderer line)
    {
        float elapsedTime = 0f;
        float fadeDuration = 0.2f; // Duration for which the rectangle is visible

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(line.gameObject);
    }

    void Tank_Weapon1Attack()
    {
        //aoe
        tankAttackCounter++;
        if (tankAttackCounter < 3)
        {
            // No damage for the first two attacks (only knockback)
            Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Enemy"));
            foreach (Collider2D enemyCollider in enemiesHit)
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.ApplyKnockback(transform.position);
                    //Debug.Log("Knockback");
                }
            }
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
                    //Debug.Log("Knockback and Damage");
                }
            }
            tankAttackCounter = 0; // Reset the counter
        }
    }

    void Tank_Weapon2Attack(Enemy enemy)
    {
        //single attack
        enemy.TakeDamage(attackDamage);
    }

    void CC_Weapon1Attack(Enemy enemy)
    {
        enemy.TakeDamage(attackDamage);
        enemy.slowArea = true;
        enemy.MarkForDeathStart();
        DrawBulletTracer(transform.position + startOffset, targetEnemy.transform.position);
        troopEnergy.GainPower();
    }

    void CC_Weapon2Attack(Enemy _enemy)
    {
        _enemy.MarkForDeathStart();

        Vector2 attackCenter = _enemy.transform.position; // Center of the attack
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(attackCenter, 4f, LayerMask.GetMask("Enemy")); // AOE detection

        foreach (Collider2D enemyCollider in enemiesHit)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
            }
        }

        DrawBulletTracer(transform.position + startOffset, attackCenter);
        troopEnergy.GainPower();
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
