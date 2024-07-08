using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSystem : MonoBehaviour
{
    public enum WaveState { Start, Prewave, InWave, Break, End }
    public WaveState currentState;

    [System.Serializable]
    public class MiniWave
    {
        public List<GameObject> enemiesToSpawn;
        public List<Transform> spawnLocations;
        public float timeBetweenSpawns;
        public float timeUntilSpawningEnds;
    }

    [System.Serializable]
    public class Wave
    {
        public int waveNum;
        public List<MiniWave> miniWaves;
        public float prewaveDuration;
        public float breakDuration;
    }

    public List<Wave> waves;
    private int currentWaveIndex = 0;
    private int currentMiniWaveIndex = 0;
    private float prewaveTimer;
    private float inwaveTimer;
    private float breakTimer;
    private List<GameObject> aliveEnemies = new List<GameObject>();
    public GameObject startButton;
    public GameObject commandCentreButton; 
    public GameObject kccPanel;
    public TextMeshProUGUI waveStateText;
    public TextMeshProUGUI waveNumText;
    public TextMeshProUGUI miniWaveNumText;
    public GameObject troop1;
    public GameObject troop2;
    public GameObject troop3;
    public GameObject troop4;
    public Transform killdozerTransform1;
    public Transform killdozerTransform2;
    public Transform killdozerTransform3;
    public Transform killdozerTransform4;

    void Start()
    {
        startButton.SetActive(true);
        waveNumText.text = "0";
        miniWaveNumText.text = "0";
        waveStateText.text = "Pre Wave";

        TeleportTroopsToKilldozer();
        UpdateButtonState();
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

        UpdateButtonState();
    }

    public void StartWave()
    {
        waveStateText.text = "Pre Wave";
        waveNumText.text = waves[currentWaveIndex].waveNum.ToString();

        //prewaveTimer = waves[currentWaveIndex].prewaveDuration;
        //currentState = WaveState.Prewave;

        StartMiniWave();
        inwaveTimer = waves[currentWaveIndex].miniWaves[currentMiniWaveIndex].timeUntilSpawningEnds;
        currentState = WaveState.InWave;

        startButton.SetActive(false);
    }

    void HandlePrewave()
    {
        TeleportTroopsToKilldozer();
        waveStateText.text = "Pre Wave";

        if (prewaveTimer > 0)
        {
            prewaveTimer -= Time.deltaTime;
        }
        else
        {
            StartMiniWave();
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
            StartMiniWave();
        }
    }

    void HandleEnd()
    {
        waveStateText.text = "End";
    }

    void StartMiniWave()
    {
        if (currentMiniWaveIndex >= waves[currentWaveIndex].miniWaves.Count)
        {
            NextWave();
        }
        else
        {
            miniWaveNumText.text = (currentMiniWaveIndex + 1).ToString(); 
            MiniWave currentMiniWave = waves[currentWaveIndex].miniWaves[currentMiniWaveIndex];
            inwaveTimer = currentMiniWave.timeUntilSpawningEnds;
            currentState = WaveState.InWave;
            StartCoroutine(SpawnEnemies(currentMiniWave));
        }
    }

    void NextWave()
    {
        currentMiniWaveIndex = 0;
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Count)
        {
            currentState = WaveState.End;
        }
        else
        {
            waveNumText.text = waves[currentWaveIndex].waveNum.ToString();
            prewaveTimer = waves[currentWaveIndex].prewaveDuration;
            currentState = WaveState.Prewave;
        }
    }

    IEnumerator SpawnEnemies(MiniWave currentMiniWave)
    {
        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns;
        int enemyIndex = 0;

        while (inwaveTimer > 0)
        {
            if (enemyIndex < currentMiniWave.enemiesToSpawn.Count && currentMiniWave.spawnLocations.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations[enemyIndex % currentMiniWave.spawnLocations.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn[enemyIndex], spawnLocation.position, spawnLocation.rotation);
                aliveEnemies.Add(enemy);
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                enemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                enemyIndex++;
            }
            else
            {
                Debug.LogWarning("Not enough spawn locations or enemies to spawn for the number of enemies in this mini wave.");
                break;
            }
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        currentMiniWaveIndex++;
        inwaveTimer = 0;  
    }

    void TeleportTroopsToKilldozer()
    {
        troop1.transform.position = killdozerTransform1.position;
        troop2.transform.position = killdozerTransform2.position;
        troop3.transform.position = killdozerTransform3.position;
        troop4.transform.position = killdozerTransform4.position;
    }

    void UpdateButtonState()
    {
        bool isActive = currentState == WaveState.Start || currentState == WaveState.Prewave || currentState == WaveState.Break;
        commandCentreButton.SetActive(isActive);

        if (!isActive)
        {
            kccPanel.SetActive(false);
        }
    }

    /*
    public enum WaveState { Start, Prewave, InWave, Break, End }
    public WaveState currentState;

    [System.Serializable]
    public class MiniWave
    {
        public List<GameObject> enemiesToSpawn1;
        public List<GameObject> enemiesToSpawn2;
        public Transform spawnLocation1;
        public Transform spawnLocation2;
        public float timeBetweenSpawns1;
        public float timeBetweenSpawns2;
        public float timeUntilSpawningEnds1;
        public float timeUntilSpawningEnds2;
    }

    [System.Serializable]
    public class Wave
    {
        public int waveNum;
        public List<MiniWave> miniWaves;
        public float prewaveDuration;
        public float breakDuration;
    }

    public List<Wave> waves;
    private int currentWaveIndex = 0;
    private int currentMiniWaveIndex = 0;
    private int currentMiniWaveIndex1 = 0;
    private int currentMiniWaveIndex2 = 0;
    private float prewaveTimer;
    private float inwaveTimer1;
    private float inwaveTimer2;
    private float breakTimer;
    private List<GameObject> aliveEnemies = new List<GameObject>();
    public GameObject startButton;
    public GameObject commandCentreButton;
    public GameObject kccPanel;
    public TextMeshProUGUI waveStateText;
    public TextMeshProUGUI waveNumText;
    public TextMeshProUGUI miniWaveNumText;
    public GameObject troop1;
    public GameObject troop2;
    public GameObject troop3;
    public GameObject troop4;
    public Transform killdozerTransform1;
    public Transform killdozerTransform2;
    public Transform killdozerTransform3;
    public Transform killdozerTransform4;

    void Start()
    {
        startButton.SetActive(true);
        waveNumText.text = "0";
        miniWaveNumText.text = "0";
        waveStateText.text = "Pre Wave";

        TeleportTroopsToKilldozer();
        UpdateButtonState();
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

        UpdateButtonState();
    }

    public void StartWave()
    {
        waveStateText.text = "Pre Wave";
        waveNumText.text = waves[currentWaveIndex].waveNum.ToString();

        prewaveTimer = waves[currentWaveIndex].prewaveDuration;
        currentState = WaveState.Prewave;

        startButton.SetActive(false);
    }

    void HandlePrewave()
    {
        waveStateText.text = "Pre Wave";
        waveNumText.text = waves[currentWaveIndex].waveNum.ToString();

        //prewaveTimer = waves[currentWaveIndex].prewaveDuration;
        //currentState = WaveState.Prewave;

        StartMiniWave();
        inwaveTimer1 = waves[currentWaveIndex].miniWaves[currentMiniWaveIndex].timeUntilSpawningEnds1;
        inwaveTimer2 = waves[currentWaveIndex].miniWaves[currentMiniWaveIndex].timeUntilSpawningEnds2;
        currentState = WaveState.InWave;

        startButton.SetActive(false);
    }

    void HandleInWave()
    {
        waveStateText.text = "In Wave";

        if (aliveEnemies.Count == 0 && inwaveTimer1 <= 0 && inwaveTimer2 <= 0)
        {
            breakTimer = waves[currentWaveIndex].breakDuration;
            currentState = WaveState.Break;
        }

        if (inwaveTimer1 > 0)
            inwaveTimer1 -= Time.deltaTime;

        if (inwaveTimer2 > 0)
            inwaveTimer2 -= Time.deltaTime;
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
            StartMiniWave();
        }
    }

    void HandleEnd()
    {
        waveStateText.text = "End";
    }

    void StartMiniWave()
    {
        if (currentMiniWaveIndex >= waves[currentWaveIndex].miniWaves.Count)
        {
            NextWave();
        }
        else
        {
            miniWaveNumText.text = (currentMiniWaveIndex + 1).ToString();
            MiniWave currentMiniWave = waves[currentWaveIndex].miniWaves[currentMiniWaveIndex];

            inwaveTimer1 = currentMiniWave.timeUntilSpawningEnds1;
            inwaveTimer2 = currentMiniWave.timeUntilSpawningEnds2;

            currentState = WaveState.InWave;

            StartCoroutine(SpawnEnemies1(currentMiniWave));
            StartCoroutine(SpawnEnemies2(currentMiniWave));
        }
    }

    void NextWave()
    {
        currentMiniWaveIndex = 0;
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Count)
        {
            currentState = WaveState.End;
        }
        else
        {
            waveNumText.text = waves[currentWaveIndex].waveNum.ToString();
            prewaveTimer = waves[currentWaveIndex].prewaveDuration;
            currentState = WaveState.Prewave;
        }
    }

    IEnumerator SpawnEnemies1(MiniWave currentMiniWave)
    {
        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns1;
        float inwaveTimer = currentMiniWave.timeUntilSpawningEnds1;
        List<GameObject> enemiesToSpawn = currentMiniWave.enemiesToSpawn1;
        Transform spawnLocation = currentMiniWave.spawnLocation1;

        int enemyIndex = 0;

        while (inwaveTimer > 0)
        {
            if (enemyIndex < enemiesToSpawn.Count)
            {
                GameObject enemy = Instantiate(enemiesToSpawn[enemyIndex], spawnLocation.position, spawnLocation.rotation);
                aliveEnemies.Add(enemy);
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                enemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                enemyIndex++;
            }
            else
            {
                Debug.LogWarning("Not enough enemies to spawn for the number of enemies in this mini wave (Location 1).");
                break;
            }
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        currentMiniWaveIndex1++;
    }

    IEnumerator SpawnEnemies2(MiniWave currentMiniWave)
    {
        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns2;
        float inwaveTimer = currentMiniWave.timeUntilSpawningEnds2;
        List<GameObject> enemiesToSpawn = currentMiniWave.enemiesToSpawn2;
        Transform spawnLocation = currentMiniWave.spawnLocation2;

        int enemyIndex = 0;

        while (inwaveTimer > 0)
        {
            if (enemyIndex < enemiesToSpawn.Count)
            {
                GameObject enemy = Instantiate(enemiesToSpawn[enemyIndex], spawnLocation.position, spawnLocation.rotation);
                aliveEnemies.Add(enemy);
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                enemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                enemyIndex++;
            }
            else
            {
                Debug.LogWarning("Not enough enemies to spawn for the number of enemies in this mini wave (Location 2).");
                break;
            }
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        currentMiniWaveIndex2++;
    }

    void TeleportTroopsToKilldozer()
    {
        troop1.transform.position = killdozerTransform1.position;
        troop2.transform.position = killdozerTransform2.position;
        troop3.transform.position = killdozerTransform3.position;
        troop4.transform.position = killdozerTransform4.position;
    }

    void UpdateButtonState()
    {
        bool isActive = currentState == WaveState.Start || currentState == WaveState.Prewave || currentState == WaveState.Break;
        commandCentreButton.SetActive(isActive);

        if (!isActive)
        {
            kccPanel.SetActive(false);
        }
    }
    */
}
