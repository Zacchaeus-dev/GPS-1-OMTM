using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopAutoAttack : MonoBehaviour
{
    public int attackDamage = 10; // Attack damage
    public float attackInterval = 1.0f; // Time between attacks
    public float attackRange = 1.5f; // Range within which the troop can attack enemies
    private bool isAutoAttacking = false;
    private GameObject targetEnemy;
    private float attackTimer = 0f;

    void Update()
    {
        if (isAutoAttacking)
        {
            FindAndAttackEnemy();
        }
    }

    public void ToggleAutoAttack()
    {
        isAutoAttacking = !isAutoAttacking;
        if (!isAutoAttacking)
        {
            targetEnemy = null;
        }
        Debug.Log("Auto-attack toggled: " + isAutoAttacking);
    }

    private void FindAndAttackEnemy()
    {
        if (targetEnemy == null)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    targetEnemy = hitCollider.gameObject;
                    break;
                }
            }
        }

        if (targetEnemy != null)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                Attack();
                attackTimer = 0f;
            }
        }
    }

    private void Attack()
    {
        if (targetEnemy != null)
        {
            EnemyHealth enemyHealth = targetEnemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
                Debug.Log("Attacked enemy: " + targetEnemy.name + " for " + attackDamage + " damage.");

                if (enemyHealth.health <= 0)
                {
                    targetEnemy = null;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
