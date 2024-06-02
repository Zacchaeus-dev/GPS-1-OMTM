using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;

public class Troop : MonoBehaviour
{
    public bool invincible = false;
    public bool selected = false;

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

    void Start()
    {
        currentHealth = maxHealth;
        collider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // For testing only, replace this with enemy attack once its done
        {
            TakeDamage(10); //deals 10 damage
        }

       HandleDropOffInput();
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
}
