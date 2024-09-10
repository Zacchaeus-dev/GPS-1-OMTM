using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TauntMine : MonoBehaviour
{
    public int tauntMineDamage;
    public float tauntMineRadius;
    public float timeUntilExplode;
    public float pullForce;
    private bool taunting = false;

    Collider2D[] enemies;

    void Start()
    {
        Vector3 spawnPosition = gameObject.transform.position;
        enemies = Physics2D.OverlapCircleAll(spawnPosition, tauntMineRadius);

        PullEnemies();
        AddToEnemyTargets();

        StartCoroutine(ExplodeMine());

        cameraShake = FindObjectOfType<CameraSystem>().GetComponent<CameraShake>();
    }

    private void Update()
    {
        if(taunting)
        {
            TauntEnemies();
        }
    }

    void AddToEnemyTargets()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.GetComponent<Enemy>().AddTargetToList(gameObject.transform);
                }
                else
                {
                    //FlyingEnemy flyingEnemyScript = enemy.GetComponent<FlyingEnemy>();
                    //flyingEnemyScript.GetComponent<FlyingEnemy>().AddTargetToList(gameObject.transform);
                }
            }
        }

        taunting = true;
    }

    void PullEnemies()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    Vector2 direction = (transform.position - enemy.transform.position).normalized;
                    if (enemy.GetComponent<Enemy>() != null || enemy.GetComponent<FlyingEnemy>() != null)
                    {
                        enemyRb.AddForce(direction * pullForce, ForceMode2D.Impulse);

                        if (enemy.GetComponent<FlyingEnemy>() != null)
                        {
                            enemy.GetComponent<FlyingEnemy>().enabled = false;
                            enemy.GetComponent<Rigidbody2D>().constraints = ~RigidbodyConstraints2D.FreezePosition;
                            enemy.GetComponent<PolygonCollider2D>().isTrigger = false;
                        }
                    }
                }
            }
        }
    }

    void TauntEnemies()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {

                        Vector2 direction = (transform.position - enemy.transform.position).normalized;
                        enemyRb.AddForce(direction * 1, ForceMode2D.Impulse);
                        Debug.Log(direction);
                    

                }
            }
        }
    }

    public GameObject CC_AOE_ExplosionEffect;
    public CameraShake cameraShake;
    IEnumerator ExplodeMine()
    {
        for (int i = 0; i <= timeUntilExplode; i++)
        {
            FindObjectOfType<AudioManager>().Play("troop4");
            yield return new WaitForSeconds(1);
        }
        

        taunting = false;

        Vector3 spawnPosition = gameObject.transform.position; //reset the radius
        enemies = Physics2D.OverlapCircleAll(spawnPosition, tauntMineRadius);

        Debug.Log("Explode");
        FindObjectOfType<AudioManager>().Play("Bomb");
        

        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    if (enemyScript.markedForDeath)
                    {
                        //take more damage if already marked
                        enemyScript.TakeDamage(tauntMineDamage * 2);
                    }
                    else
                    {
                        enemyScript.TakeDamage(tauntMineDamage);
                        enemyScript.SlowedSpeedStart();
                        enemyScript.MarkForDeathStart();
                    }
                }
                else
                {
                    FlyingEnemy flyingEnemyScript = enemy.GetComponent<FlyingEnemy>();
                    if (flyingEnemyScript != null)
                    {
                        if (flyingEnemyScript.markedForDeath)
                        {
                            //take more damage if already marked
                            flyingEnemyScript.TakeDamage(tauntMineDamage * 2);
                        }
                        else
                        {
                            flyingEnemyScript.TakeDamage(tauntMineDamage);
                            flyingEnemyScript.SlowedSpeedStart();
                            flyingEnemyScript.MarkForDeathStart();
                        }
                    }
                }
            }
            //StartCoroutine(cameraShake.Shake(0.005f, 0.00015f));
        }
        Instantiate(CC_AOE_ExplosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, tauntMineRadius);
    }
}
