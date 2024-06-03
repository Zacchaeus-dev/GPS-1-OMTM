using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;

public class Troop : MonoBehaviour
{
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
    private new Collider2D collider;
    private bool troopOnPlatform = false;

    //Attack enemy
    private GameObject targetEnemy;
    private bool isAttacking;

    void Start()
    {
        currentHealth = maxHealth;
        collider = GetComponent<Collider2D>();
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
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            troopOnPlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            troopOnPlatform = false;
        }
    }

    void HandleDropOffInput()
    {
        if (Input.GetKeyDown(KeyCode.F) && selected && troopOnPlatform)
        {
            Debug.Log("Drop Off");
            collider.enabled = false;
            StartCoroutine(EnableCollider());
        }
    }

    IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(0.8f);

        collider.enabled = true;
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
            targetEnemy.GetComponent<Enemy>().TakeDamage(attack); // enemy take damage equal to troop's attack
            Debug.Log("Attacking enemy: " + targetEnemy.name);
            yield return new WaitForSeconds(attackSpeed); 
        }

        targetEnemy = null; //deselect target enemy
        isAttacking = false;
    }
}
