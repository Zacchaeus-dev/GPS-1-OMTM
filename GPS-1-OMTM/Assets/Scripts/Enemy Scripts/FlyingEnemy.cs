using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private Vector3 rightOffset = new Vector3(32f, 0, 0);
    private Vector3 leftOffset = new Vector3(-30f, 0, 0);
    private bool attackingKilldozer;
    private float lastAttackTime = 0f;
    private bool shouldMove = true; // Flag to control movement
    public event Action onDeath;
    private bool lineRendererDestroyed;

    LineRenderer lineRenderer;

    public GameObject damageIndicator;

    public SpriteRenderer[] enemySprite;
    public Color DamagedColor;
    public Color NormalColor;
    float timer;
    private bool tookdamage;
    Vector3 normalScale;

    public bool markedForDeath = false;
    public bool slowed = false;
    public GameObject markForDeathIcon;
    public GameObject slowedIcon;
    public float slowEffectRadius = 5f;
    private Rigidbody2D rb;
    private bool facingRight = false;

    public bool slowArea = false;
    private bool isKnockedBack = false;
    private bool isStunned = false;

    public DropEnergyOrbOnDeath energyOrb;
    public int energyOrbDropNum = 1;
    private int i = 0;

    [Header("Art / Animations")]
    public GameObject EnemyModel;
    TroopAnimationsManager Animator;

    void Start()
    {
        Animator = EnemyModel.GetComponent<TroopAnimationsManager>();

        // Initialize health
        currentHealth = maxHealth;
        lastAttackTime = Time.time;

        normalScale = transform.localScale;

        // Find the Killdozer in the scene
        killdozer = GameObject.FindWithTag("Killdozer");
        killdozerLeftTarget = killdozer.GetComponent<Killdozer>().leftTarget;
        killdozerRightTarget = killdozer.GetComponent<Killdozer>().rightTarget;

        if (lineRenderer != null)
        {
            Destroy(lineRenderer.gameObject);
            lineRenderer = null;
        }
    }

    void FixedUpdate()
    {
        if (shouldMove && killdozerRightTarget.transform.position.x + rightOffset.x < transform.position.x && !isStunned) //move depending on killdozer's location
        {
            MoveLeft();
            Animator.TroopAttackOff();
            facingRight = false;
        }
        else if (shouldMove && killdozerLeftTarget.transform.position.x + leftOffset.x > transform.position.x && !isStunned)
        {
            MoveRight();
            Animator.TroopAttackOff();
            facingRight = true;
        }

        if (facingRight)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        DetectTargets();
        MoveTowardsTarget();
        HandleAttack();

        if (tookdamage)
        {
            foreach (SpriteRenderer sprite in enemySprite)
            {
                sprite.color = DamagedColor;
            }

            timer += Time.deltaTime;
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

        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            Debug.Log("Rotation: " + transform.eulerAngles.y);
        }
    }

    void MoveLeft()
    {
        // Move towards the negative X-axis
        transform.position += Vector3.left * speed * Time.deltaTime;
    }

    void MoveRight()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
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

    public void AddTargetToList(Transform objectTransform)
    {
        targetTroop = objectTransform.gameObject;
    }

    void MoveTowardsTarget()
    {
        if (killdozer != null && !isStunned)
        {
            float distanceToKilldozer = 0;
            Vector3 direction = (killdozer.transform.position - transform.position).normalized;

            if (killdozer.transform.position.x < transform.position.x)
            {
                // Calculate distance to the Killdozer
                distanceToKilldozer = Vector2.Distance(transform.position, killdozerRightTarget.transform.position);
                direction = (killdozerRightTarget.transform.position - transform.position + rightOffset).normalized;
            }
            else if (killdozer.transform.position.x > transform.position.x)
            {
                // Calculate distance to the Killdozer
                distanceToKilldozer = Vector2.Distance(transform.position, killdozerLeftTarget.transform.position);
                direction = (killdozerLeftTarget.transform.position - transform.position + leftOffset).normalized;
            }

            // Move towards the Killdozer if it's within detection range
            if (distanceToKilldozer > attackRange)
            {
                direction.y = 0; // Ensure no movement in Y-axis
                transform.position += direction * speed * Time.deltaTime;
            }
            else
            {
                shouldMove = false; // Stop moving if within attack range of Killdozer
            }
        }

        if (targetTroop != null && !isStunned)
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
        if (!isStunned && killdozer != null && (Vector2.Distance(transform.position, killdozerRightTarget.transform.position) <= attackRange || Vector2.Distance(transform.position, killdozerLeftTarget.transform.position) <= attackRange))
        {
            Animator.TroopAttackOn();
            if (Time.time >= lastAttackTime + attackInterval)
            {
                lastAttackTime = Time.time;
                AttackKilldozer();
                attackingKilldozer = true;
            }
        }
        else if (!isStunned && targetTroop != null && Vector2.Distance(transform.position, targetTroop.transform.position) <= attackRange)
        {
            Animator.TroopAttackOn();
            if (Time.time >= lastAttackTime + attackInterval)
            {
                lastAttackTime = Time.time;
                AttackTroop();
            }
        }
    }

    void AttackTroop()
    {
        if (targetTroop != null && targetTroop.activeInHierarchy)
        {
            Troop troop = targetTroop.GetComponent<Troop>();
            if (troop != null)
            {
                troop.TakeDamage(attackDamage);
                //Debug.Log("Attacked troop: " + troop.name + " for " + attackDamage + " damage.");

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
                //Debug.Log("Attacked Killdozer for " + attackDamage + " damage.");

                if (killdozer.transform.position.x < transform.position.x)
                {
                    StartCoroutine(DrawBulletTracer(killdozerLeftTarget.transform.position));
                }
                else if (killdozer.transform.position.x > transform.position.x)
                {
                    StartCoroutine(DrawBulletTracer(killdozerRightTarget.transform.position));
                }
            }
        }
    }

    IEnumerator DrawBulletTracer(Vector3 targetPosition)
    {
        lineRenderer = Instantiate(lineRendererPrefab).GetComponent<LineRenderer>(); 
        lineRenderer.SetPosition(0, transform.position + startOffset);
        lineRenderer.SetPosition(1, targetPosition);

        Color startColor = lineRenderer.startColor;
        Color endColor = lineRenderer.endColor;
        float fadeSpeed = 1f / tracerFadeDuration;

        float progress = 0f;
        while (progress < 1f)
        {
            if (currentHealth <= 0) // Exit coroutine if enemy is dead
            {
                Destroy(lineRenderer.gameObject);
                yield break;
            }

            progress += Time.deltaTime * fadeSpeed;
            Color color = Color.Lerp(startColor, Color.clear, progress);
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            yield return null;
        }

        Destroy(lineRenderer.gameObject);
        lineRenderer = null; 
    }

    public void TakeDamage(int damage)
    {
        tookdamage = true;
        currentHealth -= damage;

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

        Animator.TroopDies();

        while (i < energyOrbDropNum)
        {
            energyOrb.DropEnergyOrb();
            i++;
        }

        if (lineRenderer != null)
        {
            Destroy(lineRenderer.gameObject);
            lineRenderer = null;
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
            else if (enemy == null)
            {
                FlyingEnemy flyingEnemy = hitCollider.GetComponent<FlyingEnemy>();
                if (flyingEnemy != null && flyingEnemy != this && !flyingEnemy.slowed)
                {
                    flyingEnemy.SlowedSpeedStart();
                }
            }
        }
    }

    public void SlowedSpeedStart()
    {
        if (slowed) //no effect if drone is already slowed
        {
            return;
        }

        slowed = true;
        slowedIcon.SetActive(true);
        speed = speed / 2;
        StartCoroutine((SlowedSpeedEnd()));
    }

    IEnumerator SlowedSpeedEnd()
    {
        yield return new WaitForSeconds(10000f);

        if (gameObject != null)
        {
            speed = speed * 2;
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
