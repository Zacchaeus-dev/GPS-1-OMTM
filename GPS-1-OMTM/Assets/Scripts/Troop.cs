using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
//using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UI;

public class Troop : MonoBehaviour
{
    private Camera mainCamera;
    public TroopController2D troopController2D;
    public bool invincible = false;
    public bool selected = false;
    public bool stopAction = false;

    public TroopHUD troopHUD;

    //Troop stats
    public int maxHealth;
    public int currentHealth;
    public int attack;
    public float attackSpeed;
    public float attackRange;
    public float moveSpeed = 5f;

    //Drop off platforms
    private Collider2D boxCollider;
    private Collider2D capsuleCollider;
    private bool troopOnPlatform = false;
    private bool troopOnGround = false;
    private Rigidbody2D rb;

    //Attacks
    public Weapon selectedWeapon = Weapon.None;
    private GameObject targetEnemy;
    private bool isAttacking;
    public bool canAttack = true;
    
    /*
    //Fall damage
    private bool isFalling = false;
    private float fallStartHeight;
    public float fallDamageThreshold = 1f; // Height at which fall damage starts to apply
    public float fallDamageMultiplier = 5f;
    */

    // Ultimate
    public Ultimate ultimate = Ultimate.None;
    private bool ultimateOnCooldown = false;
    public float ultimateCooldown = 5f; 
    public float ultimateDuration = 0f;
    private bool clickingOnLocation = false; //CC's Ultimate

    // Ultimate Objects
    public GameObject tankShield;
    public GameObject tauntMine;
    //public int tauntMineDamage;
    //public float tauntMineRadius;

    // UI 
    public Image ultimateImage;
    public Image ultimateCooldownOverlay;
    public Image ultimateDurationOverlay;
    public GameObject ultimateReady;

    // Cooldown time
    private float ultimateCooldownTimeRemaining;

    // Duration time
    private float ultimateDurationTimeRemaining;

    // Animation
    public Animator attackAnimation;

    // Highlight
    public GameObject highlight;

    // Energy
    private TroopEnergy troopEnergy;

    // Shield
    public int maxShield;
    public int currentShield;
    public bool shieldOn = false;
    public bool reducingShield = false;

    public enum Weapon
    {
        None,
        Weapon1_DPS,
        Weapon2_DPS,
        Weapon1_Tank,
        Weapon2_Tank,
        Weapon1_CC,
        Weapon2_CC,
        Weapon1_Healer,
        Weapon2_Healer
    }

    public enum Ultimate
    {
        None,
        Ultimate_DPS,
        Ultimate_Tank,
        Ultimate_CC,
        Ultimate_Healer,
    }

    void Start()
    {
        mainCamera = Camera.main;
        currentHealth = maxHealth;
        boxCollider = GetComponent<BoxCollider2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        troopEnergy = GetComponent<TroopEnergy>();

        UpdateHUD();
    }

    void Update()
    {
        /*
        // Check if the stop key is pressed and troop is selected
        if (selected && Input.GetKeyDown(KeyCode.S))
        {
            stopAction = true;
            StopAllCoroutines(); // Stop attacking coroutine
        }
        */

        //HandleDropOffInput();
        HandleUltimateInput();

        if (targetEnemy != null)
        {
            MoveTowardsEnemy();
        }

        //CheckFalling();
        CheckGround();
        UpdateUltimateUI();

        if (shieldOn && reducingShield && currentShield > 0)
        {
            currentShield = currentShield - 50;
            Debug.Log(gameObject.name + "'s Current Shield: " + currentShield);
            reducingShield = false;
            StartCoroutine(ReduceShieldOverTime());
        }
    }

    IEnumerator ReduceShieldOverTime()
    {
        /*
        reducingShield = true;
        while (currentShield > 0)
        {
            currentShield = Mathf.Max(0, currentShield - 10);
            Debug.Log(gameObject.name + "'s Current Shield: " + currentShield);
            yield return new WaitForSeconds(10f);
        }
        reducingShield = false;
        shieldOn = false;
        */

        yield return new WaitForSeconds(1f);

        if (currentShield <= 0)
        {
            shieldOn = false;
        }

        reducingShield = true;
    }

    void HandleUltimateInput()
    {
        if (selected && Input.GetKeyDown(KeyCode.R) && clickingOnLocation)
        {
            // Cancel clicking on location if it's already active
            clickingOnLocation = false;
            Debug.Log("Ultimate targeting cancelled.");
        }
        else if (selected && Input.GetKeyDown(KeyCode.R) && !ultimateOnCooldown)
        {
            // Start the ultimate if not on cooldown
            StartCoroutine(UseUltimate(ultimate));
        }
        if (clickingOnLocation && Input.GetMouseButtonDown(0)) //CC's ultimate
        {
            Vector3 newPosition = transform.position;
            Vector2 MousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(MousePosition, Vector2.zero);

            foreach (var Hit in hits)
            {
                if (Hit.collider != null && Hit.collider.CompareTag("[TP] Ground") || Hit.collider.CompareTag("[TP] Platform"))
                {
                    if (Hit.collider != null)
                    {
                        if (Hit.collider.CompareTag("[TP] Ground"))
                        {
                            newPosition.x = MousePosition.x;
                            newPosition.y = -3; //Y value for ground
                        }
                        else if (Hit.collider.CompareTag("[TP] Platform"))
                        {
                            newPosition.x = MousePosition.x;
                            newPosition.y = 3; //Y value for platform
                        }
                        else
                        {
                            newPosition.x = MousePosition.x;
                        }
                    }
                    break;
                }
            }

            Instantiate(tauntMine, newPosition, Quaternion.Euler(0f, 0f, 0f), null);
            clickingOnLocation = false;

            troopEnergy.UseAllPower();

            ultimateOnCooldown = true;
            ultimateCooldownTimeRemaining = ultimateCooldown;
            ultimateDurationTimeRemaining = ultimateDuration;
            Debug.Log("CC Ultimate Activated");

            StartCoroutine(Ultimate_CC_End());
        }
    }

    public void UpdateHUD()
    {
        if (troopHUD != null)
        {
            troopHUD.SetHUD(this);
        }

    }

    IEnumerator UseUltimate(Ultimate _ultimate)
    {
        if (troopEnergy.currentPower < troopEnergy.maxPower)
        {
            Debug.Log("Not enough energy");
            yield break;
        }

        switch (_ultimate)
        {
            case Ultimate.Ultimate_DPS:
                troopEnergy.UseAllPower();
                yield return StartCoroutine(Ultimate_DPS());
                break;
            case Ultimate.Ultimate_Tank:
                troopEnergy.UseAllPower();
                yield return StartCoroutine(Ultimate_Tank());
                break;
            case Ultimate.Ultimate_CC:
                Ultimate_CC();
                break;
            case Ultimate.Ultimate_Healer:
                troopEnergy.UseAllPower();
                yield return StartCoroutine(Ultimate_Healer());
                break;
        }
    }

    void UpdateUltimateUI()
    {
        if (ultimateOnCooldown)
        {
            if (ultimateDurationTimeRemaining > 0)
            {
                ultimateReady.SetActive(false);
                ultimateDurationTimeRemaining -= Time.deltaTime;
                ultimateDurationOverlay.fillAmount = 1 - (ultimateDurationTimeRemaining / ultimateDuration);
                ultimateDurationOverlay.enabled = true;
            }
            else
            {
                ultimateDurationOverlay.fillAmount = 0;
                ultimateDurationOverlay.enabled = false;

                ultimateCooldownTimeRemaining -= Time.deltaTime;
                ultimateCooldownOverlay.fillAmount = 1 - (ultimateCooldownTimeRemaining / ultimateCooldown);
                ultimateCooldownOverlay.enabled = true;
            }
        }
        else
        {
            ultimateCooldownOverlay.fillAmount = 0;
            ultimateCooldownOverlay.enabled = false;
            ultimateDurationOverlay.fillAmount = 0;
            ultimateDurationOverlay.enabled = false;
            ultimateReady.SetActive(true);
        }
    }

    IEnumerator Ultimate_DPS()
    {
        ultimateOnCooldown = true;
        ultimateCooldownTimeRemaining = ultimateCooldown;
        ultimateDurationTimeRemaining = ultimateDuration;
        Debug.Log("DPS Ultimate Activated");

        //berserk
        //add attack and attack speed 
        attack += 25;
        attackSpeed -= 0.5f;
        attackAnimation.SetBool("Berserk", true);
        yield return new WaitForSeconds(ultimateDuration); //add this for all abilities that have a duration
        attackAnimation.SetBool("Berserk", false);
        attack -= 25;
        attackSpeed += 0.5f;

        yield return new WaitForSeconds(ultimateCooldown);
        ultimateOnCooldown = false;    
    }

    IEnumerator Ultimate_Tank()
    {
        ultimateOnCooldown = true;
        ultimateCooldownTimeRemaining = ultimateCooldown;
        ultimateDurationTimeRemaining = ultimateDuration;
        Debug.Log("Tank Ultimate Activated");

        /*
        // Determine the facing direction of the tank
        Vector3 facingDirection = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        Vector3 offset = facingDirection * 10f;
        */

        //delay for animations
        yield return new WaitForSeconds(0.2f);

        Vector3 offset = new Vector3(10f, 0f, 0);

        // Spawn the shield at a distance apart from the tank
        GameObject TankShield = Instantiate(tankShield, transform.position + offset, Quaternion.identity);

        // Find all enemies between the tank and the shield
        Vector3 shieldPosition = TankShield.transform.position;
        Vector3 tankPosition = transform.position;
        Collider2D[] enemiesInRange = Physics2D.OverlapAreaAll(tankPosition, shieldPosition);

        yield return new WaitForSeconds(0.2f);

        foreach (var enemyCollider in enemiesInRange)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(50); 
                StartCoroutine(StunEnemy(enemy, 2f)); //stun duration
            }
        }

        yield return new WaitForSeconds(ultimateDuration);

        // Destroy the shield
        Destroy(TankShield);

        yield return new WaitForSeconds(ultimateCooldown);
        ultimateOnCooldown = false;
    }

    private IEnumerator StunEnemy(Enemy enemy, float duration)
    {
        enemy.Stun(true); 
        yield return new WaitForSeconds(duration);
        enemy.Stun(false);
    }

    void Ultimate_CC()
    {
        clickingOnLocation = true;
    }

    IEnumerator Ultimate_CC_End()
    {
        yield return new WaitForSeconds(ultimateDuration);

        //explosion is in taunt mine script

        yield return new WaitForSeconds(ultimateCooldown);
        ultimateOnCooldown = false;
    }

    IEnumerator Ultimate_Healer()
    {
        ultimateOnCooldown = true;
        ultimateCooldownTimeRemaining = ultimateCooldown;
        ultimateDurationTimeRemaining = ultimateDuration;
        Debug.Log("Healer Ultimate Activated");

        //golden fleece
        GainShield(troopController2D.troop1);
        GainShield(troopController2D.troop2);
        GainShield(troopController2D.troop3);
        GainShield(gameObject);

        yield return new WaitForSeconds(ultimateDuration);
        yield return new WaitForSeconds(ultimateCooldown);
        ultimateOnCooldown = false;
    }

    void GainShield(GameObject go)
    {
        Troop troop = go.GetComponent<Troop>();
        troop.maxShield = 500;
        troop.currentShield = 500;
        troop.shieldOn = true;
        troop.reducingShield = true;
    }

    /*
    void CheckFalling()
    {
        // Check if the troop is in the air
        if (!isFalling && !IsGrounded())
        {
            isFalling = true;
            fallStartHeight = transform.position.y;
            //rb.velocity = new Vector2(0, rb.velocity.y);
        }
        // Check if the troop has hit the ground
        else if (isFalling && IsGrounded())
        {
            isFalling = false;
            float fallDistance = fallStartHeight - transform.position.y;

            if (fallDistance > fallDamageThreshold)
            {
                float damage = (fallDistance - fallDamageThreshold) * fallDamageMultiplier;
                TakeDamage((int)damage); //change fall damage to int
            }
        }
    }
    

    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
        return hit.collider != null;
    }
    */

    public void TakeDamage(int damage)
    {
        if (invincible)
        {
            return;
        }

        if (currentShield > 0)
        {
            currentShield = currentShield - damage;
        }
        else
        {
            currentHealth -= damage;
        }

        troopHUD.SetHUD(this);

        if (currentHealth <= 0)
        {
            Death();
        }

        if (troopEnergy != null)
        {
            if (troopEnergy.powerMethod == TroopEnergy.PowerMethod.Tank)
            {
                troopEnergy.GainPower();
            }
        }
    }

    void Death()
    {
        // Put death animation or effects

        Debug.Log(gameObject.name + " is dead");

        // Notify troopController2D to respawn this troop
        troopController2D.HandleRespawn(this);

        // Deactivate the troop
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            troopOnPlatform = true;
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            troopOnGround = true;
        }
        /*if (collision.gameObject.CompareTag("Killdozer"))
        {
            transform.SetParent(collision.transform); //set troop as kd's child
        }*/
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            troopOnPlatform = false;
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            troopOnGround = false;
        }
        /*if (collision.gameObject.CompareTag("Killdozer"))
        {
            transform.SetParent(null); //remove troop from kd's child
        }*/
    }

    /*
    void HandleDropOffInput()
    {
        if (Input.GetKeyDown(KeyCode.F) && selected && troopOnPlatform)
        {
            Debug.Log("Drop Off");
            StartCoroutine(DisableAndEnableColliders());
        }
    }

    IEnumerator DisableAndEnableColliders()
    {
        boxCollider.enabled = false;
        capsuleCollider.enabled = false;

        rb.velocity = new Vector2(0, -30);

        yield return new WaitForSeconds(0.05f);

        boxCollider.enabled = true;
        capsuleCollider.enabled = true;
    }
    */

    void CheckGround()
    {
        if (troopOnGround || troopOnPlatform) //if troop is on a floor, stops troop going down
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void SetTargetEnemy(GameObject enemy, float attackRange)
    {
        this.targetEnemy = enemy;
        this.attackRange = attackRange;
        this.isAttacking = false;
        this.stopAction = false;
    }

    public void DeselectTargetEnemy()
    {
        this.targetEnemy = null;
    }

    void MoveTowardsEnemy()
    {
        if (stopAction || targetEnemy == null)
        {
            return;
        }

        Vector2 direction = (targetEnemy.transform.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, targetEnemy.transform.position);

        if (distance > attackRange)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetEnemy.transform.position, moveSpeed * Time.deltaTime);
            Debug.Log("Moving");
        }
        else
        {
            if (!isAttacking)
            {
                Debug.Log("Attacking");
                isAttacking = true;

                /*
                switch (selectedWeapon)
                {
                    case (Weapon)1: //dps weapon 1
                     
                        break;

                    case (Weapon)2: //dps weapon 2

                        break;

                    case (Weapon)3: //tank weapon 1
                        StartCoroutine(TankWeapon1());
                        break;

                    case (Weapon)4: //tank weapon 2

                        break;

                    case (Weapon)5: //cc weapon 1
                        StartCoroutine(CCWeapon1());
                        break;

                    case (Weapon)6: //cc weapon 2

                        break;

                    default:
                        StartCoroutine(AttackEnemy()); //can be removed once all the attacks are finished
                        break;
                }
                */
            }
        }
    }

    /*
    IEnumerator AttackEnemy()
    {
        while (targetEnemy != null && Vector2.Distance(transform.position, targetEnemy.transform.position) <= attackRange && !stopAction) 
        {
            if (targetEnemy.GetComponent<Enemy>().currentHealth <= 0)
            {
                break;
            }

            targetEnemy.GetComponent<Enemy>().TakeDamage(attack); // enemy take damage equal to troop's attack
            Debug.Log("Attacking enemy: " + targetEnemy.name);
            yield return new WaitForSeconds(attackSpeed); 
        }

        targetEnemy = null; //deselect target enemy
        isAttacking = false;
    }

    IEnumerator TankWeapon1() 
    {
        while (targetEnemy != null && Vector2.Distance(transform.position, targetEnemy.transform.position) <= attackRange && !stopAction)
        {
            if (targetEnemy.GetComponent<Enemy>().currentHealth <= 0)
            {
                break;
            }

            targetEnemy.GetComponent<Enemy>().TakeDamage(attack); 
            Debug.Log("Attacking enemy: " + targetEnemy.name);
            yield return new WaitForSeconds(attackSpeed);
        }

        targetEnemy = null; 
        isAttacking = false;
    }

    IEnumerator CCWeapon1()
    {
        while (targetEnemy != null && Vector2.Distance(transform.position, targetEnemy.transform.position) <= attackRange && !stopAction)
        {
            if (targetEnemy.GetComponent<Enemy>().currentHealth <= 0)
            {
                break;
            }

            Enemy enemy = targetEnemy.GetComponent<Enemy>();
            enemy.TakeDamage(attack);
            enemy.MarkForDeathStart();
            Debug.Log("Attacking enemy: " + targetEnemy.name);
            yield return new WaitForSeconds(attackSpeed);
        }

        targetEnemy = null;
        isAttacking = false;
    }
    */
}
