using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TroopAutoAttack : MonoBehaviour
{
    public bool autoAttackEnabled = false;
    public int attackDamage = 10; // Attack damage settable via Inspector
    public float detectionRange = 3f; // Range within which the troop can detect enemies
    public float attackRange = 1.5f; // Range within which the troop can attack enemies
    public float attackCooldown = 1f; // Time between attacks

    public float lastAttackTime = 0f;
    public GameObject targetEnemy;
    private Rigidbody2D rb;

    public Troop troop;
    public TroopCharacter troopCharacter;
    public TroopClass troopClass;
    public TroopEnergy troopEnergy;
    public TroopWeapon troopWeapon;
    //private int tankAttackCounter = 0;
    private float dpsWeapon2Range = 10;
    private float dpsWeapon2Width = 2f;
    public Transform dpsWeapon2Transform;
    public Transform dpsWeapon2ShootPoint;
    public Transform screenLeftShootingPoint;
    public Transform screenRightShootingPoint;
    public Transform cannonLeftShootingPoint;
    public Transform cannonRightShootingPoint;
    public GameObject screenProjectilePrefab;
    public GameObject cannonProjectilePrefab;
    public GameObject rectanglePrefab;

    float AnimationDelay;
    
    float delay;

    [Header(" Art / Animations ")]
    public float ShootingDelay;
    public GameObject AttackModelHead;
    public GameObject AttackModelWeapon1;
    public GameObject AttackModelWeapon2;
    public GameObject shootingPoint1; // Shooting point of troop's Weapon 1
    public GameObject shootingPoint2; // Shooting point of troop's Weapon 2
    public GameObject shootingPointUlt;
    public Vector3 startOffset; // Offset for the start point of the bullet tracer
    public LineRenderer lineRendererPrefab; // Prefab for the line renderer
    public float tracerFadeDuration = 0.5f; // Duration of the fade-out


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

        TroopAnimator = TroopModel.GetComponent<TroopAnimationsManager>();
    }
    private void FixedUpdate()
    {
        DetermineAttack();
    }
    public void DetermineWeaponLoadout() // to change animation set when troop changes weapon
    {
        switch (troopWeapon.selectedWeapon)
        {
            case TroopWeapon.Weapon.Weapon1_DPS:
                TroopAnimator.TroopOnWeapon1();
                break;
            case TroopWeapon.Weapon.Weapon2_DPS:
                TroopAnimator.TroopOnWeapon2();
                break;
            case TroopWeapon.Weapon.Weapon1_Tank:
                TroopAnimator.TroopOnWeapon1();
                break;
            case TroopWeapon.Weapon.Weapon2_Tank:
                TroopAnimator.TroopOnWeapon2();
                break;
            case TroopWeapon.Weapon.Weapon1_CC:
                TroopAnimator.TroopOnWeapon1();
                break;
            case TroopWeapon.Weapon.Weapon2_CC:
                TroopAnimator.TroopOnWeapon2();
                break;
            case TroopWeapon.Weapon.Weapon1_Healer:
                TroopAnimator.TroopOnWeapon1();
                break;
            case TroopWeapon.Weapon.Weapon2_Healer:
                TroopAnimator.TroopOnWeapon2();
                break;
        }
    }
    public int DPSUltBuff;
    void DetermineAttack()
    {
        switch (troopWeapon.selectedWeapon) //determine attack based on selected weapon
        {
            case TroopWeapon.Weapon.Weapon1_DPS:
                attackDamage = 40 + DPSUltBuff;
                attackCooldown = 0.25f;
                AnimationDelay = 0.4f;
                ShootingDelay = 0.5f;
                break;
            case TroopWeapon.Weapon.Weapon2_DPS:
                attackDamage = 35 + DPSUltBuff;
                attackCooldown = 1f;
                AnimationDelay = 0.4f;
                ShootingDelay = 1f;
                break;
            case TroopWeapon.Weapon.Weapon1_Tank:
                attackDamage = 20;
                attackCooldown = 1f;
                AnimationDelay = 0f;
                ShootingDelay = 2f;
                break;
            case TroopWeapon.Weapon.Weapon2_Tank:
                attackDamage = 75;
                attackCooldown = 1.5f;
                AnimationDelay = 0f;
                ShootingDelay = 0.4f;
                gameObject.GetComponent<Troop>().maxHealth = 750;
                //gameObject.GetComponent<Troop>().currentHealth = 750;
                break;
            case TroopWeapon.Weapon.Weapon1_CC:
                attackDamage = 30;
                attackCooldown = 1f;
                AnimationDelay = 0f;
                ShootingDelay = 1f;
                break;
            case TroopWeapon.Weapon.Weapon2_CC:
                attackDamage = 30;
                attackCooldown = 2f;
                AnimationDelay = 0.8f;
                ShootingDelay = 1f;
                break;
        }
    }
    
    void Update()
    {
        if (Killdozer.gameOver)
        {
            return;
        }

        DetermineWeaponLoadout();

        if (autoAttackEnabled)
        {
            if (targetEnemy == null || targetEnemy.activeInHierarchy == false)
            {
                FindTarget();
                DeactivateAttackVisuals();
                //delay = 0;
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
            if (targetEnemy != null && targetEnemy.activeInHierarchy == true)
            {
                // to check whether Troop is attacking Left or Right
                if (targetEnemy.transform.position.x < gameObject.transform.position.x)
                {
                    gameObject.GetComponent<TroopClass>().GoingLeft = true;
                }
                else if (targetEnemy.transform.position.x > gameObject.transform.position.x)
                {
                    gameObject.GetComponent<TroopClass>().GoingLeft = false;
                }

                float distanceToEnemy = Vector2.Distance(transform.position, targetEnemy.transform.position);
                if (distanceToEnemy <= attackRange)
                {
                    
                    //delay bc troops need a window of time to get into attack stance when transitioning from walking to attacking
                    delay = delay + Time.deltaTime;
                    if (delay > AnimationDelay)
                    {
                        ActivateAttackVisuals();
                    }

                    if (delay >= ShootingDelay) //if no delay bfr shooting, troop will shoot bfr even setting their weapon in the right position
                    {
                        delay = 0;
                        
                        if (Time.time >= lastAttackTime + attackCooldown)
                        {
                            //Enemy enemy = targetEnemy.GetComponent<Enemy>();

                            if (targetEnemy != null)
                            {
                                switch (troopCharacter) //do different attack based on the troop and weapon
                                {
                                    case TroopCharacter.DPS:
                                        switch (troopWeapon.selectedWeapon)
                                        {
                                            case TroopWeapon.Weapon.Weapon1_DPS:
                                                DPS_Weapon1Attack(targetEnemy);
                                                break;
                                            case TroopWeapon.Weapon.Weapon2_DPS:
                                                DPS_Weapon2Attack(targetEnemy);
                                                break;
                                        }
                                        break;
                                    case TroopCharacter.Tank:
                                        switch (troopWeapon.selectedWeapon)
                                        {
                                            case TroopWeapon.Weapon.Weapon1_Tank:
                                                Tank_Weapon1Attack();
                                                break;
                                            case TroopWeapon.Weapon.Weapon2_Tank:
                                                Tank_Weapon2Attack(targetEnemy);
                                                break;
                                        }
                                        break;
                                    case TroopCharacter.CC:
                                        switch (troopWeapon.selectedWeapon)
                                        {
                                            case TroopWeapon.Weapon.Weapon1_CC:
                                                CC_Weapon1Attack(targetEnemy);
                                                break;
                                            case TroopWeapon.Weapon.Weapon2_CC:
                                                CC_Weapon2Attack(targetEnemy);
                                                break;
                                        }
                                        break;
                                    default:
                                        //enemy.TakeDamage(attackDamage);
                                        //DrawBulletTracer(shootingPoint.transform.position, targetEnemy.transform.position);
                                        break;
                                }
                            }
                            else
                            {
                                targetEnemy.GetComponent<FlyingEnemy>().TakeDamage(attackDamage);
                                
                                //DrawBulletTracer(transform.position + startOffset, targetEnemy.transform.position); // change to below after adding in troop assets, fits better rn
                                //DrawBulletTracer(shootingPoint.transform.position, targetEnemy.transform.position);

                                targetEnemy = null; // enemy ded, remove target
                            }

                            lastAttackTime = Time.time;
                        }
                    }
                    
                }
                else
                {
                    targetEnemy = null; // Lost range, find another target
                }
            }  
        }
        else
        {
            delay = 0;
        }

    }

    public void ActivateAttackVisuals()
    {
        TroopAnimator.TroopAttackOn(); //activate attack animation
        switch (troopCharacter) // activate corresponding pivot models
        {
            case TroopCharacter.DPS:
                switch (troopWeapon.selectedWeapon)
                {
                    case TroopWeapon.Weapon.Weapon1_DPS:
                        AttackModelHead.SetActive(true);
                        AttackModelWeapon1.SetActive(true);
                        break;
                    case TroopWeapon.Weapon.Weapon2_DPS:
                        AttackModelHead.SetActive(true);
                        AttackModelWeapon2.SetActive(true);
                        break;
                }
                break;
            case TroopCharacter.CC:
                switch (troopWeapon.selectedWeapon)
                {
                    case TroopWeapon.Weapon.Weapon2_CC:
                        AttackModelHead.SetActive(true);
                        AttackModelWeapon1.SetActive(true);
                        AttackModelWeapon2 = null;
                        break;
                }
                break;
            default:
                break;
        }
    }
    public void DeactivateAttackVisuals()
    {
        //Stops Attack Animation
        TroopAnimator.TroopAttackOff();
        switch (troopCharacter) // activate corresponding pivot models
        {
            case TroopCharacter.DPS:
                switch (troopWeapon.selectedWeapon)
                {
                    case TroopWeapon.Weapon.Weapon1_DPS:
                        AttackModelHead.SetActive(false);
                        AttackModelWeapon1.SetActive(false);
                        break;
                    case TroopWeapon.Weapon.Weapon2_DPS:
                        AttackModelHead.SetActive(false);
                        AttackModelWeapon2.SetActive(false);
                        break;
                }
                break;
            case TroopCharacter.CC:
                switch (troopWeapon.selectedWeapon)
                {
                    case TroopWeapon.Weapon.Weapon2_CC:
                        AttackModelHead.SetActive(false);
                        AttackModelWeapon1.SetActive(false);
                        break;
                }
                break;
            default:
                break;
        }

    }

    void DPS_Weapon1Attack(GameObject _enemy)
    {
        Enemy enemy = _enemy.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(attackDamage);
            DrawBulletTracer(shootingPoint1.transform.position, new Vector2(_enemy.transform.position.x, _enemy.transform.position.y + 0.8f)); //height offset so gun shoots straight

            if (troop.dpsUltiOn == true)
            {
                DrawBulletTracer(shootingPointUlt.transform.position, new Vector2(_enemy.transform.position.x, _enemy.transform.position.y + 0.8f));
            }
        }
        else
        {
            FlyingEnemy flyingEnemy = _enemy.GetComponent<FlyingEnemy>();
            flyingEnemy.TakeDamage(attackDamage);
            DrawBulletTracer(shootingPoint1.transform.position, new Vector2(_enemy.transform.position.x, _enemy.transform.position.y + 0.8f)); //height offset so gun shoots straight

            if (troop.dpsUltiOn == true)
            {
                DrawBulletTracer(shootingPointUlt.transform.position, new Vector2(_enemy.transform.position.x, _enemy.transform.position.y + 0.8f));
            }
            //troop.UpdateHUD();
        }

        troopEnergy.GainPower();
    }

    void DPS_Weapon2Attack(GameObject _enemy)
    {
        Vector3 targetPositionFORVISUAL = new Vector2(_enemy.transform.position.x, _enemy.transform.position.y + 0.65f);
        Vector3 targetPosition = _enemy.transform.position;
        Vector2 attackDirection = (targetPositionFORVISUAL - transform.position).normalized;
        Vector2 attackDirectionFORVISUALS = (targetPositionFORVISUAL - transform.position).normalized;


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
                }
            }
            else
            {
                FlyingEnemy flyingEnemy = enemyCollider.GetComponent<FlyingEnemy>();

                Vector2 toEnemy = flyingEnemy.transform.position - transform.position;
                float distanceAlongDirection = Vector2.Dot(toEnemy, attackDirection);
                float distancePerpendicular = Mathf.Abs(Vector2.Dot(toEnemy, new Vector2(-attackDirection.y, attackDirection.x)));

                // Check if the enemy is within the length and width of the attack rectangle
                if (distanceAlongDirection > 0 && distanceAlongDirection <= length && distancePerpendicular <= halfWidth)
                {
                    flyingEnemy.TakeDamage(attackDamage);
                }
            }

            DrawBulletTracer(shootingPoint2.transform.position, new Vector2(_enemy.transform.position.x, _enemy.transform.position.y + 0.8f));
        }

        //Vector3 offset = new Vector3(5, 0, 0);

        DrawAttackRectangle(dpsWeapon2Transform.position, attackDirectionFORVISUALS, dpsWeapon2Width - 1, dpsWeapon2Range - 3.5f);
        troopEnergy.GainPower();
    }

    /*
    void DrawRectangle(Vector3 start, Vector2 direction, float width, float length)
    {
        width = width / 2;
        length = length / 1.4f;
        GameObject rectangle = Instantiate(rectanglePrefab, start, Quaternion.identity);

        // Adjust the rectangle's pivot to the start position
        RectTransform rectTransform = rectangle.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.pivot = new Vector2(0, 0.5f); // Pivot at the left center
        }

        // Set the size of the rectangle
        rectangle.transform.localScale = new Vector3(length, width, 1);

        // Calculate the angle and apply the rotation
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rectangle.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        StartCoroutine(FadeRectangle(rectangle));
    }

    IEnumerator FadeRectangle(GameObject rectangle)
    {
        float elapsedTime = 0f;
        float fadeDuration = 0.2f; // Duration for which the rectangle is visible

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(rectangle);
    }

    */

    /*
    void DrawRectangle(Vector3 start, Vector2 direction, float width, float length, Transform target)
    {
        width = width / 2;
        length = length / 1.4f;
        Vector3 offset = new Vector3(7 ,0, 0);
        if(target.position.x > transform.position.x && target.position.y > transform.position.y)
        {
            offset = new Vector3(4, 5, 0);
            //Debug.Log("a");
        }
        else if (target.position.x < transform.position.x && target.position.y < transform.position.y)
        {
            offset = new Vector3(-6, 0.5f, 0);
            //Debug.Log("b");
        }
        else if (target.position.x > transform.position.x && target.position.y < transform.position.y)
        {
            offset = new Vector3(6, 0.5f, 0);
            //Debug.Log("c");
        }
        else if (target.position.x < transform.position.x && target.position.y > transform.position.y)
        {
            offset = new Vector3(-4, 5, 0);
            //Debug.Log("d");
        }

        GameObject rectangle = Instantiate(rectanglePrefab, start + offset, Quaternion.identity);
        rectangle.transform.localScale = new Vector3(length, width, 1);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rectangle.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        StartCoroutine(FadeRectangle(rectangle));
    }

    IEnumerator FadeRectangle(GameObject rectangle)
    {
        float elapsedTime = 0f;
        float fadeDuration = 0.2f; // Duration for which the rectangle is visible

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(rectangle);
    }
    */

    void DrawAttackRectangle(Vector3 start, Vector3 direction, float width, float length)
    {
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

        // Create multiple lines to fill the rectangle
        int segments = 25; 
        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 startFill = Vector3.Lerp(topLeft, topRight, t);
            Vector3 endFill = Vector3.Lerp(bottomLeft, bottomRight, t);

            LineRenderer fillLine = Instantiate(lineRendererPrefab);
            fillLine.positionCount = 2;
            fillLine.SetPosition(0, startFill);
            fillLine.SetPosition(1, endFill);

            StartCoroutine(FadeOutLine(fillLine));
        }

        StartCoroutine(FadeOutLine(line));
    }

    /*
    IEnumerator FadeOutRectangle(LineRenderer line, GameObject quad)
    {
        float elapsedTime = 0f;
        float fadeDuration = 0.2f; // Duration for which the rectangle is visible

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(line.gameObject);
        Destroy(quad);
    }
    */

    void Tank_Weapon1Attack()
    {
        //aoe
/*        tankAttackCounter++;
        if (tankAttackCounter < 3)
        {
            // No damage for the first two attacks (only knockback)
            Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Enemy"));
            foreach (Collider2D enemyCollider in enemiesHit)
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    //enemy.ApplyKnockback(transform.position);
                    //Debug.Log("Knockback");
                }
            }
        }
        else
        {*/
            // Third attack deals AoE damage and applies knockback --> chg to just once big attack with knockback
            Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, (attackRange - 1), LayerMask.GetMask("Enemy"));
            foreach (Collider2D enemyCollider in enemiesHit)
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(attackDamage);
                    //enemy.ApplyKnockback(transform.position);
                    //Debug.Log("Knockback and Damage");
                }
                else
                {
                    FlyingEnemy flyingEnemy = enemyCollider.GetComponent<FlyingEnemy>();
                    flyingEnemy.TakeDamage(attackDamage);
                }
            }
            //tankAttackCounter = 0; // Reset the counter
       // }
    }

    void Tank_Weapon2Attack(GameObject _enemy)
    {
        //single attack
        Enemy enemy = _enemy.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(attackDamage);
        }
        else
        {
            FlyingEnemy flyingEnemy = _enemy.GetComponent<FlyingEnemy>();
            flyingEnemy.TakeDamage(attackDamage);
        }
    }

    void CC_Weapon1Attack(GameObject _enemy)
    {
        StartCoroutine(ShowScreenProjectile(_enemy.transform));

        StartCoroutine(CC_Weapon1Damage(_enemy));

        troopEnergy.GainPower();
        //troop.UpdateHUD();
        //DrawBulletTracer(shootingPoint1.transform.position, _enemy.transform.position);
    }

    IEnumerator CC_Weapon1Damage(GameObject _enemy)
    {
        yield return new WaitForSeconds(0.3f);

        if (_enemy != null)
        {
            Enemy enemy = _enemy.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
                enemy.slowArea = true;
                enemy.MarkForDeathStart();
            }
            else
            {
                FlyingEnemy flyingEnemy = _enemy.GetComponent<FlyingEnemy>();
                flyingEnemy.TakeDamage(attackDamage);
                flyingEnemy.slowArea = true;
                flyingEnemy.MarkForDeathStart();
            }
        }
    }

    IEnumerator ShowScreenProjectile(Transform target)
    {
        // Add a delay before the first shot
        yield return new WaitForSeconds(0.2f);

        GameObject projectile;
        Vector3 startPosition;

        //if (target.position.x < gameObject.transform.position.x)
        //{
            //projectile = Instantiate(screenProjectilePrefab, screenLeftShootingPoint.position, Quaternion.identity);
            //startPosition = screenLeftShootingPoint.position;
        //}
        //else
        //{
            projectile = Instantiate(screenProjectilePrefab, screenRightShootingPoint.position, Quaternion.identity);
            startPosition = screenRightShootingPoint.position;
        //}
        
        if (target == null)
        {
            Destroy(projectile);
            yield break;
        }

        Vector3 targetPosition = new Vector2(target.position.x, target.position.y + 0.8f);

        // Calculate the direction and angle
        Vector3 direction = (targetPosition - startPosition).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate the sprite to face the target
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

        float elapsedTime = 0f;
        float tracerDuration = 0.1f; // Duration for which the tracer is visible

        while (elapsedTime < tracerDuration)
        {
            elapsedTime += Time.deltaTime;

            if (targetPosition == null || projectile == null)
            {
                Destroy(projectile);
                yield return null;
            }

            projectile.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / tracerDuration);

            if(target == null)
            {
                Destroy(projectile);
                yield return null;
            }

            yield return null;
        }

        Destroy(projectile);
    }


    void CC_Weapon2Attack(GameObject _enemy)
    {
        StartCoroutine(ShowCannonProjectile(_enemy.transform));

        StartCoroutine(CC_Weapon2Damage(_enemy));

        //DrawBulletTracer(shootingPoint2.transform.position, attackCenter);
        troopEnergy.GainPower();
        //troop.UpdateHUD();

        //visual effect here
        // 1. muzzle
        // 2. hit effect
    }

    public GameObject CC_AOE_ExplosionEffect;
    IEnumerator CC_Weapon2Damage(GameObject _enemy)
    {
        yield return new WaitForSeconds(0.3f);

        if (_enemy != null)
        {
            Enemy enemy = _enemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.MarkForDeathStart();
            }
            else
            {
                FlyingEnemy flyingEnemy = _enemy.GetComponent<FlyingEnemy>();
                flyingEnemy.MarkForDeathStart();
            }

            Vector2 attackCenter = _enemy.transform.position; // Center of the attack
            Instantiate(CC_AOE_ExplosionEffect, attackCenter, Quaternion.identity);
            Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(attackCenter, 4f, LayerMask.GetMask("Enemy")); // AOE detection

            foreach (Collider2D enemyCollider in enemiesHit)
            {
                enemy = enemyCollider.GetComponent<Enemy>();
                FlyingEnemy flyingEnemy = enemyCollider.GetComponent<FlyingEnemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(attackDamage - 10);
                }
                else
                {
                    //FlyingEnemy flyingEnemy = _enemy.GetComponent<FlyingEnemy>();
                    flyingEnemy.TakeDamage(attackDamage);
                }
            }
        }
    }
    

    IEnumerator ShowCannonProjectile(Transform target)
    {
        // Add a delay before the first shot
        yield return new WaitForSeconds(0.2f);

        GameObject projectile;
        Vector3 startPosition;

        if (target == null)
        {
            yield break;
        }

        //if (target.position.x < gameObject.transform.position.x)
        //{
            //projectile = Instantiate(cannonProjectilePrefab, cannonLeftShootingPoint.position, Quaternion.identity);
            //startPosition = cannonLeftShootingPoint.position;
        //}
        //else
        //{
            projectile = Instantiate(cannonProjectilePrefab, cannonRightShootingPoint.position, Quaternion.identity);
            startPosition = cannonRightShootingPoint.position;
        //}

        if (target == null)
        {
            Destroy(projectile);
            yield break;
        }

        Vector3 targetPosition = new Vector2(target.position.x, target.position.y + 0.8f);

        // Calculate the direction and angle
        Vector3 direction = (targetPosition - startPosition).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (targetPosition!= null && targetPosition.y < transform.position.y)
        {
            angle = Mathf.Atan2(direction.y - 5f, direction.x) * Mathf.Rad2Deg;
        }

        // Rotate the sprite to face the target
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

        float elapsedTime = 0f;
        float tracerDuration = 0.15f; // Duration for which the tracer is visible

        while (elapsedTime < tracerDuration)
        {
            elapsedTime += Time.deltaTime;

            if (targetPosition == null)
            {
                Destroy(projectile);
                yield return null;
            }

            if(targetPosition != null && target != null)
            {
                projectile.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / tracerDuration);
            }
            else
            {
                Destroy(projectile);
                yield return null;
            }

            if (target == null)
            {
                Destroy(projectile);
                yield return null;
            }

            yield return null;
        }

        Destroy(projectile);
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
