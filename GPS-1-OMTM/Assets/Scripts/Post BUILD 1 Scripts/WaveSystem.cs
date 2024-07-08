using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSystem : MonoBehaviour
{
    public enum WaveState { Start, Prewave, InWave, Break, End }
    public WaveState currentState;

    [System.Serializable]
    public class Wave
    {
        public int waveNum;
        public List<GameObject> enemiesToSpawn;
        public List<Transform> spawnLocations;
        public float timeBetweenSpawns;
        public float timeUntilSpawningEnds;
        public float prewaveDuration;
        public float breakDuration;
    }

    public List<Wave> waves;
    private int currentWaveIndex = 0;
    //private float waveTimer = 5f;
    private float prewaveTimer;
    private float inwaveTimer;
    private float breakTimer;
    private List<GameObject> aliveEnemies = new List<GameObject>();
    public GameObject startButton;
    public TextMeshProUGUI waveStateText;
    public TextMeshProUGUI waveNumText;

    void Start()
    {
        startButton.SetActive(true);
        waveNumText.text = "0";
        waveStateText.text = "Pre Wave";
    }

    void Update()
    {
        switch (currentState)
        {
            case WaveState.Start:
                break;
            case WaveState.Prewave:
                HandlePrewave();
                break;
            case WaveState.InWave:
                HandleInWave();
                break;
            case WaveState.Break:
                HandleBreak();
                break;
            case WaveState.End:
                HandleEnd();
                break;
        }
    }

    public void StartWave()
    {
        waveStateText.text = "Pre Wave";
        waveNumText.text = waves[currentWaveIndex].waveNum.ToString();

        inwaveTimer = waves[currentWaveIndex].timeUntilSpawningEnds;
        currentState = WaveState.InWave;
        StartCoroutine(SpawnEnemies());

        startButton.SetActive(false);
        //NextWave();

        //prewaveTimer = waves[currentWaveIndex].prewaveDuration;
        //currentState = WaveState.Prewave;
    }

    void HandlePrewave()
    {
        waveStateText.text = "Pre Wave";

        if (prewaveTimer > 0)
        {
            prewaveTimer -= Time.deltaTime;
        }
        else
        {
            inwaveTimer = waves[currentWaveIndex].timeUntilSpawningEnds;
            currentState = WaveState.InWave;
            StartCoroutine(SpawnEnemies());
        }
    }

    void HandleInWave()
    {
        waveStateText.text = "In Wave";

        if (aliveEnemies.Count == 0 && inwaveTimer <= 0)
        {
            breakTimer = waves[currentWaveIndex].breakDuration;
            currentState = WaveState.Break;
        }
        inwaveTimer -= Time.deltaTime;
    }

    void HandleBreak()
    {
        waveStateText.text = "Break";

        if (breakTimer > 0)
        {
            breakTimer -= Time.deltaTime;
        }
        else
        {
            NextWave();
        }
    }

    void HandleEnd()
    {
        waveStateText.text = "End";
    }

    void NextWave()
    {
        currentWaveIndex++;
        waveNumText.text = waves[currentWaveIndex].waveNum.ToString();
        if (currentWaveIndex >= waves.Count)
        {
            currentState = WaveState.End;
        }
        else
        {
            prewaveTimer = waves[currentWaveIndex].prewaveDuration;
            currentState = WaveState.Prewave;
        }
    }

    IEnumerator SpawnEnemies()
    {
        Wave currentWave = waves[currentWaveIndex];
        inwaveTimer = currentWave.timeUntilSpawningEnds;
        float timeBetweenSpawns = currentWave.timeBetweenSpawns;
        int enemyIndex = 0;

        while (inwaveTimer > 0)
        {
            if (enemyIndex < currentWave.enemiesToSpawn.Count && currentWave.spawnLocations.Count > 0)
            {
                Transform spawnLocation = currentWave.spawnLocations[enemyIndex % currentWave.spawnLocations.Count];
                GameObject enemy = Instantiate(currentWave.enemiesToSpawn[enemyIndex], spawnLocation.position, spawnLocation.rotation);
                aliveEnemies.Add(enemy);
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                enemyScript.onDeath += () => aliveEnemies.Remove(enemy); 
                enemyIndex++;
            }
            else
            {
                Debug.LogWarning("Not enough spawn locations or enemies to spawn for the number of enemies in this wave.");
                break;
            }
            yield return new WaitForSeconds(timeBetweenSpawns);
            //inwaveTimer -= timeBetweenSpawns;
        }
    }
}
