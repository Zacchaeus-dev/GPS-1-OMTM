using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy: MonoBehaviour
{
    public bool invincible = false;

    //Enemy stats
    public int maxHealth;
    public int currentHealth;
    public int attack;
    public float attackSpeed;
    public float attackRange;
    public float moveSpeed = 5f;

    void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(int damage)
    {
        if (invincible)
        {
            return;
        }

        currentHealth -= damage;
        Debug.Log("Enemy took " + damage + " damage.");

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        // Put death animation or effects

        Debug.Log("Enemy is dead");
        Destroy(gameObject);
    }
}
