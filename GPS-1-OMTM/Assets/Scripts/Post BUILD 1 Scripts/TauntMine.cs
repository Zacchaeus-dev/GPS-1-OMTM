using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TauntMine : MonoBehaviour
{
    public int tauntMineDamage;
    public float tauntMineRadius;
    public float timeUntilExplode = 7.5f;

    Collider2D[] enemies;

    void Start()
    {
        Vector3 spawnPosition = gameObject.transform.position;
        enemies = Physics2D.OverlapCircleAll(spawnPosition, tauntMineRadius);

        TauntEnemies();

        StartCoroutine(ExplodeMine());
    }

    void TauntEnemies()
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
            }
        }
    }

    IEnumerator ExplodeMine()
    {
        yield return new WaitForSeconds(timeUntilExplode);

        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    Debug.Log("Explode");
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
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, tauntMineRadius);
    }
}
