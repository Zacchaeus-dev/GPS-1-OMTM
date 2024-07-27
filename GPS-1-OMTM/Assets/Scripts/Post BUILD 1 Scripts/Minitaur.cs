using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minitaur : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public float walkSpeed = 2f;
    public float dashSpeed = 10f;
    public float detectionRangeX1 = 10f;
    public float detectionRangeY1 = 10f;
    public float detectionRangeX2 = 5f;
    public float detectionRangeY2 = 5f;
    public float detectionRangeX3 = 1.5f;
    public float detectionRangeY3 = 1.5f;
    public int attackDamage = 50;
    public float chargeTime = 2f; // Time to charge before dashing

    private GameObject targetTroop;
    private bool isCharging = false;
    private bool isDashing = false;

    private Rigidbody2D rb;

    [Header(" Art / Animations ")]

    public GameObject EnemyModel;
    TroopAnimationsManager Animator;

    void Start()
    {
        Animator = EnemyModel.GetComponent<TroopAnimationsManager>();
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        DetectTroops();
        HandleMovementAndAttack();

/*        if (moveRight == true)
        {
            EnemyModel.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (moveRight == false)
        {
            EnemyModel.transform.rotation = Quaternion.Euler(0, 0, 0);
        }*/
    }

    void DetectTroops()
    {
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(detectionRangeX1, detectionRangeY1), 0);
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

    void HandleMovementAndAttack()
    {
        if (targetTroop == null)
            return;

        float distanceToTroop = Vector2.Distance(transform.position, targetTroop.transform.position);

        if (distanceToTroop <= Mathf.Max(detectionRangeX3, detectionRangeY3) && isDashing)
        {
            AttackTroop();
        }
        else if (distanceToTroop <= Mathf.Max(detectionRangeX2, detectionRangeY2) && !isCharging && !isDashing)
        {
            StartCoroutine(ChargeAndDash());
        }
        else if (distanceToTroop <= Mathf.Max(detectionRangeX1, detectionRangeY1) && !isCharging && !isDashing)
        {
            MoveTowardsTroop(walkSpeed);
        }
    }

    void MoveTowardsTroop(float speed)
    {
        Vector2 direction = (targetTroop.transform.position - transform.position).normalized;
        rb.velocity = direction * speed;
    }

    IEnumerator ChargeAndDash()
    {
        isCharging = true;
        rb.velocity = Vector2.zero; // Stop moving
        yield return new WaitForSeconds(chargeTime);
        isCharging = false;
        isDashing = true;
        MoveTowardsTroop(dashSpeed);
    }

    void AttackTroop()
    {
        Animator.TroopAttackOn();
        if (targetTroop != null)
        {
            Troop troop = targetTroop.GetComponent<Troop>();
            if (troop != null)
            {
                troop.TakeDamage(attackDamage);
                Debug.Log("Minitaur attacked troop: " + troop.name + " for " + attackDamage + " damage.");
            }
        }
        StartCoroutine(Death());
    }
  
    public void TakeDamage(int damage)
    {
        StartCoroutine(TakeDamageWithDelay(damage));
    }

    IEnumerator TakeDamageWithDelay(int damage)
    {
        yield return new WaitForSeconds(2f);
        currentHealth -= damage;
        Debug.Log(name + " took " + damage + " damage, remaining health: " + currentHealth);

        if (currentHealth <= 0)
        {
            StartCoroutine(Death());
        }
    }

/*    void Die()
    {
        Debug.Log(name + " died.");
        Destroy(gameObject);
    }*/

    IEnumerator Death()
    {
        Animator.TroopDies();

/*        while (i < energyOrbDropNum)
        {
            energyOrb.DropEnergyOrb();
            i++;
        }

        if (isDummy)
        {
            dummyDead = true;
            i = 0; //reset energy dropped amount
            gameObject.SetActive(false);
            yield break;
        }

        onDeath.Invoke();*/


        yield return new WaitForSeconds(0.75f);
        Destroy(gameObject);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector2(detectionRangeX1, detectionRangeY1));

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector2(detectionRangeX2, detectionRangeY2));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector2(detectionRangeX3, detectionRangeY3));
    }
}
