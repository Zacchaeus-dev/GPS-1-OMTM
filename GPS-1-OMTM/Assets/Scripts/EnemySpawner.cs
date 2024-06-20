using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject killdozer;
    public List<GameObject> enemies; // List of enemy prefabs 
    public float spawnInterval = 5f; // Time interval between spawns
    public float activationRange = 10f; // Range within which the spawner activates
    public List<Transform> spawnPoints;
    private bool canSpawn = true;
    public float killdozerOffset = 2.5f;

    void Update()
    {
        CheckKilldozerDistance();
    }

    void CheckKilldozerDistance()
    {
        for(int i = spawnPoints.Count - 1; i >= 0; i--)
        {
            Transform spawnPoint = spawnPoints[i];
            float distanceToKilldozer = Vector3.Distance(spawnPoint.position, killdozer.transform.position);

            if (distanceToKilldozer <= activationRange && canSpawn)
            {
                SpawnEnemy(spawnPoint);
                canSpawn = false;
                StartCoroutine(SpawnDelay(spawnPoint));
            }

            // Remove spawn point if the killdozer has passed it
            if (killdozer.transform.position.x > spawnPoint.position.x - killdozerOffset)
            {
                spawnPoints.RemoveAt(i);
            }
        }
    }

    IEnumerator SpawnDelay(Transform spawnPoint)
    {
        yield return new WaitForSeconds(spawnInterval);
        canSpawn = true;
    }

    void SpawnEnemy(Transform spawnPoint)
    {
        if (enemies.Count == 0)
        {
            Debug.LogWarning("No enemies available to spawn.");
            return;
        }

        int randomEnemyIndex = Random.Range(0, enemies.Count);
        GameObject enemyToSpawn = enemies[randomEnemyIndex];

        Instantiate(enemyToSpawn, spawnPoint.position, spawnPoint.rotation);
        //Debug.Log("Spawned: " + enemyToSpawn.name + " at " + spawnPoint.position);
        canSpawn = true;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                Gizmos.DrawWireSphere(spawnPoint.position, activationRange);
            }
        }
    }
}
