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

    //Fall damage
    private bool isFalling = false;
    private float fallStartHeight;
    public float fallDamageThreshold = 1f; // Height at which fall damage starts to apply
    public float fallDamageMultiplier = 5f;

    // Abilities
    public Ability ability1 = Ability.None;
    public Ability ability2 = Ability.None;
    private bool ability1OnCooldown = false;
    private bool ability2OnCooldown = false;
    public float ability1Cooldown = 5f; 
    public float ability2Cooldown = 10f;
    public float ability1Duration = 0f;
    public float ability2Duration = 0f;
    public GameObject tankShield;

    // UI 
    public Image ability1Image;
    public Image ability1CooldownOverlay;
    public Image ability1DurationOverlay;
    public Image ability2Image;
    public Image ability2CooldownOverlay;
    public Image ability2DurationOverlay;

    // Cooldown times
    private float ability1CooldownTimeRemaining;
    private float ability2CooldownTimeRemaining;

    // Duration times
    private float ability1DurationTimeRemaining;
    private float ability2DurationTimeRemaining;

    // Animation
    //public Animator attackAnimation;

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

    public enum Ability
    {
        None,
        Ability1_DPS,
        Ability2_DPS,
        Ability1_Tank,
        Ability2_Tank,
        Ability1_CC,
        Ability2_CC,
        Ability1_Healer,
        Ability2_Healer
    }

    void Start()
    {
        currentHealth = maxHealth;
        boxCollider = GetComponent<BoxCollider2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        UpdateHUD();

        //assign weapon from equipment menu
    }

    void Update()
    {
        // Check if the stop key is pressed and troop is selected
        if (selected && Input.GetKeyDown(KeyCode.S))
        {
            stopAction = true;
            StopAllCoroutines(); // Stop attacking coroutine
        }

        HandleDropOffInput();
        HandleAbilitiesInput();

        if (targetEnemy != null)
        {
            MoveTowardsEnemy();
        }

        CheckFalling();
        CheckGround();
        UpdateAbilityUI();
    }

    void HandleAbilitiesInput()
    {
        if (selected && Input.GetKeyDown(KeyCode.Q) && !ability1OnCooldown)
        {
            StartCoroutine(UseAbility(ability1));
            //attackAnimation.SetBool("Attack", true);
        }
        if (selected && Input.GetKeyDown(KeyCode.W) && !ability2OnCooldown)
        {
            StartCoroutine(UseAbility(ability2));
        }
    }

    public void UpdateHUD()
    {
        if (troopHUD != null)
        {
            troopHUD.SetHUD(this);
        }

    }

    IEnumerator UseAbility(Ability ability)
    {
        switch (ability)
        {
            case Ability.Ability1_DPS:
                yield return StartCoroutine(Ability1_DPS());
                break;
            case Ability.Ability2_DPS:
                yield return StartCoroutine(Ability2_DPS());
                break;
            case Ability.Ability1_Tank:
                yield return StartCoroutine(Ability1_Tank());
                break;
            case Ability.Ability2_Tank:
                yield return StartCoroutine(Ability2_Tank());
                break;
            case Ability.Ability1_CC:
                yield return StartCoroutine(Ability1_CC());
                break;
            case Ability.Ability2_CC:
                yield return StartCoroutine(Ability2_CC());
                break;
            case Ability.Ability1_Healer:
                yield return StartCoroutine(Ability1_Healer());
                break;
            case Ability.Ability2_Healer:
                yield return StartCoroutine(Ability2_Healer());
                break;
        }
    }

    void UpdateAbilityUI()
    {

        if (ability1OnCooldown)
        {
            if (ability1DurationTimeRemaining > 0)
            {
                ability1DurationTimeRemaining -= Time.deltaTime;
                ability1DurationOverlay.fillAmount = 1 - (ability1DurationTimeRemaining / ability1Duration); 
                ability1DurationOverlay.enabled = true;
            }
            else
            {
                ability1DurationOverlay.fillAmount = 0;
                ability1DurationOverlay.enabled = false;

                ability1CooldownTimeRemaining -= Time.deltaTime;
                ability1CooldownOverlay.fillAmount = 1 - (ability1CooldownTimeRemaining / ability1Cooldown);
                ability1CooldownOverlay.enabled = true;
            }
        }
        else
        {
            ability1CooldownOverlay.fillAmount = 0;
            ability1CooldownOverlay.enabled = false;
            ability1DurationOverlay.fillAmount = 0;
            ability1DurationOverlay.enabled = false;
        }

        /* //add this when ability 2 is implemented
        if (ability2OnCooldown)
        {
            if (ability2DurationTimeRemaining > 0)
            {
                ability2DurationTimeRemaining -= Time.deltaTime;
                ability2DurationOverlay.fillAmount = 1 - (ability2DurationTimeRemaining / ability2Duration);
                ability2DurationOverlay.enabled = true;
            }
            else
            {
                ability2DurationOverlay.fillAmount = 0;
                ability2DurationOverlay.enabled = false;

                ability2CooldownTimeRemaining -= Time.deltaTime;
                ability2CooldownOverlay.fillAmount = 1 - (ability2CooldownTimeRemaining / ability2Cooldown);
                ability2CooldownOverlay.enabled = true;
            }
        }
        else
        {
            ability2CooldownOverlay.fillAmount = 0;
            ability2CooldownOverlay.enabled = false;
            ability2DurationOverlay.fillAmount = 0;
            ability2DurationOverlay.enabled = false;
        }
        */
    }

    IEnumerator Ability1_DPS()
    {
        ability1OnCooldown = true;
        ability1CooldownTimeRemaining = ability1Cooldown;
        ability1DurationTimeRemaining = ability1Duration;
        Debug.Log("DPS Ability 1 Activated");

        //berserk
        //add attack and attack speed 
        attack += 25;
        attackSpeed -= 0.5f;
        yield return new WaitForSeconds(ability1Duration); //add this for all abilities that have a duration
        attack -= 25;
        attackSpeed += 0.5f;

        yield return new WaitForSeconds(ability1Cooldown);
        ability1OnCooldown = false;

        
    }

    IEnumerator Ability2_DPS()
    {
        ability2OnCooldown = true;
        ability2CooldownTimeRemaining = ability2Cooldown;
        ability2DurationTimeRemaining = ability2Duration;
        Debug.Log("DPS Ability 2 Activated");

        //teleportation

        yield return new WaitForSeconds(ability2Cooldown);
        ability2OnCooldown = false;
    }

    IEnumerator Ability1_Tank()
    {
        ability1OnCooldown = true;
        ability1CooldownTimeRemaining = ability1Cooldown;
        ability1DurationTimeRemaining = ability1Duration;
        Debug.Log("Tank Ability 1 Activated");

        //Wall of olympus
        Vector3 offset = new Vector3(1.5f, 0.1f, 0);
        GameObject TankShield = Instantiate(tankShield, gameObject.transform.position + offset, Quaternion.Euler(0f,0f,0f), null);
        yield return new WaitForSeconds(ability1Duration); 
        Destroy(TankShield);

        yield return new WaitForSeconds(ability1Cooldown);
        ability1OnCooldown = false;
    }

    IEnumerator Ability2_Tank()
    {
        ability2OnCooldown = true;
        ability2CooldownTimeRemaining = ability2Cooldown;
        ability2DurationTimeRemaining = ability2Duration;
        Debug.Log("Tank Ability 2 Activated");

        yield return new WaitForSeconds(ability2Cooldown);
        ability2OnCooldown = false;
    }

    IEnumerator Ability1_CC()
    {
        ability1OnCooldown = true;
        ability1CooldownTimeRemaining = ability1Cooldown;
        ability1DurationTimeRemaining = ability1Duration;
        Debug.Log("CC Ability 1 Activated");

        //friday
        yield return new WaitForSeconds(3f);

        yield return new WaitForSeconds(ability1Cooldown);
        ability1OnCooldown = false;
    }

    IEnumerator Ability2_CC()
    {
        ability2OnCooldown = true;
        ability2CooldownTimeRemaining = ability2Cooldown;
        ability2DurationTimeRemaining = ability2Duration;
        Debug.Log("CC Ability 2 Activated");

        yield return new WaitForSeconds(ability2Cooldown);
        ability2OnCooldown = false;
    }

    IEnumerator Ability1_Healer()
    {
        ability1OnCooldown = true;
        ability1CooldownTimeRemaining = ability1Cooldown;
        ability1DurationTimeRemaining = ability1Duration;
        Debug.Log("Healer Ability 1 Activated");

        //golden fleece
        /*
        int healAmount = 100;
        for (int i = 0; i < 5; i++)
        {
            currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
            yield return new WaitForSeconds(1f);
        }
        */

        yield return new WaitForSeconds(ability1Cooldown);
        ability1OnCooldown = false;
    }

    IEnumerator Ability2_Healer()
    {
        ability2OnCooldown = true;
        ability2CooldownTimeRemaining = ability2Cooldown;
        ability2DurationTimeRemaining = ability2Duration;
        Debug.Log("Healer Ability 2 Activated");

        yield return new WaitForSeconds(ability2Cooldown);
        ability2OnCooldown = false;
    }

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

    public void TakeDamage(int damage)
    {
        if (invincible)
        {
            return;
        }

        currentHealth -= damage;
        //Debug.Log(gameObject.name + " took " + damage + " damage.");

        troopHUD.SetHUD(this);

        if (currentHealth <= 0)
        {
            Death();
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
                //Debug.Log("On");
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
            //Debug.Log("Off");
        }*/
    }

    void HandleDropOffInput()
    {
        /*if (Input.GetKeyDown(KeyCode.F) && selected && troopOnPlatform)
        {
            Debug.Log("Drop Off");
            StartCoroutine(DisableAndEnableColliders());
        }*/
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

                        break;

                    case (Weapon)6: //cc weapon 2

                        break;

                    default:
                        StartCoroutine(AttackEnemy()); //can be removed once all the attacks are finished
                        break;
                }
            }
        }
    }

    
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
    

    //place troop weapon attacks here

    IEnumerator TankWeapon1() //unfinalized
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

   
  
}
