using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;

public class Troop : MonoBehaviour
{
    public TroopController2D troopController2D;
    public bool invincible = false;
    public bool selected = false;
    public bool stopAction = false;

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

    //Attack enemy
    private GameObject targetEnemy;
    private bool isAttacking;

    //Fall damage
    private bool isFalling = false;
    private float fallStartHeight;
    public float fallDamageThreshold = 1f; // Height at which fall damage starts to apply
    public float fallDamageMultiplier = 5f;

    void Start()
    {
        currentHealth = maxHealth;
        boxCollider = GetComponent<BoxCollider2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
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

        if (targetEnemy != null)
        {
            MoveTowardsEnemy();
        }

        CheckFalling();
        CheckGround();
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
        Debug.Log(gameObject.name + " took " + damage + " damage.");

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
    }

    void HandleDropOffInput()
    {
        if (Input.GetKeyDown(KeyCode.F) && selected && troopOnPlatform)  //only drop off the selected troop if its on a platform
        {
            Debug.Log("Drop Off");
            boxCollider.enabled = false;
            capsuleCollider.enabled = false;

            rb.velocity = new Vector2(0, -10);
            StartCoroutine(EnableCollider());
        }
    }

    IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(0.01f);

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
                StartCoroutine(AttackEnemy());
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
}
