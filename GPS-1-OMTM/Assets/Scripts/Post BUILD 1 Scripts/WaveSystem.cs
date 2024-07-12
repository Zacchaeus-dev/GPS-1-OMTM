using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSystem : MonoBehaviour
{
    public enum WaveState { Start, Prewave, InWave, Break, Transition, End }
    public WaveState currentState;

    [System.Serializable]
    public class MiniWave
    {
        public List<GameObject> enemiesToSpawn1 = new List<GameObject>();
        public List<Transform> spawnLocations1 = new List<Transform>();
        public float timeBetweenSpawns1;
        public float timeUntilSpawningEnds1;

        public List<GameObject> enemiesToSpawn2 = new List<GameObject>();
        public List<Transform> spawnLocations2 = new List<Transform>();
        public float timeBetweenSpawns2;
        public float timeUntilSpawningEnds2;

        public List<GameObject> enemiesToSpawn3 = new List<GameObject>();
        public List<Transform> spawnLocations3 = new List<Transform>();
        public float timeBetweenSpawns3;
        public float timeUntilSpawningEnds3;

        public List<GameObject> enemiesToSpawn4 = new List<GameObject>();
        public List<Transform> spawnLocations4 = new List<Transform>();
        public float timeBetweenSpawns4;
        public float timeUntilSpawningEnds4;

        public List<GameObject> enemiesToSpawn5 = new List<GameObject>();
        public List<Transform> spawnLocations5 = new List<Transform>();
        public float timeBetweenSpawns5;
        public float timeUntilSpawningEnds5;

        public List<GameObject> enemiesToSpawn6 = new List<GameObject>();
        public List<Transform> spawnLocations6 = new List<Transform>();
        public float timeBetweenSpawns6;
        public float timeUntilSpawningEnds6;

        public List<GameObject> enemiesToSpawn7 = new List<GameObject>();
        public List<Transform> spawnLocations7 = new List<Transform>();
        public float timeBetweenSpawns7;
        public float timeUntilSpawningEnds7;

        public List<GameObject> enemiesToSpawn8 = new List<GameObject>();
        public List<Transform> spawnLocations8 = new List<Transform>();
        public float timeBetweenSpawns8;
        public float timeUntilSpawningEnds8;

        public List<GameObject> enemiesToSpawn9 = new List<GameObject>();
        public List<Transform> spawnLocations9 = new List<Transform>();
        public float timeBetweenSpawns9;
        public float timeUntilSpawningEnds9;

        public List<GameObject> enemiesToSpawn10 = new List<GameObject>();
        public List<Transform> spawnLocations10 = new List<Transform>();
        public float timeBetweenSpawns10;
        public float timeUntilSpawningEnds10;

        public List<GameObject> enemiesToSpawn11 = new List<GameObject>();
        public List<Transform> spawnLocations11 = new List<Transform>();
        public float timeBetweenSpawns11;
        public float timeUntilSpawningEnds11;

        public List<GameObject> enemiesToSpawn12 = new List<GameObject>();
        public List<Transform> spawnLocations12 = new List<Transform>();
        public float timeBetweenSpawns12;
        public float timeUntilSpawningEnds12;
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
    private float inwaveTimer1 = 0;
    private float inwaveTimer2 = 0;
    private float inwaveTimer3 = 0;
    private float inwaveTimer4 = 0;
    private float inwaveTimer5 = 0;
    private float inwaveTimer6 = 0;
    private float inwaveTimer7 = 0;
    private float inwaveTimer8 = 0;
    private float inwaveTimer9 = 0;
    private float inwaveTimer10 = 0;
    private float inwaveTimer11 = 0;
    private float inwaveTimer12 = 0;
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
    public GameObject settingsPanel;
    public CameraSystem cameraSystem;
    public GameObject killdozer;
    public Animator killdozerAnimator;
    public static bool transitioning = false;
    private bool teleported = false;
    private bool transitioned = false;

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
            case WaveState.Transition:
                HandleTransition();
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

        //Debug.Log("Animation Starts"); //wave animation
        //change wave number
        //change enemy images
        //animator.settrigger("a");
        StartCoroutine(WaveAnimation());

        /*
        StartMiniWave();
        inwaveTimer1 = waves[currentWaveIndex].miniWaves[currentMiniWaveIndex].timeUntilSpawningEnds1;
        inwaveTimer2 = waves[currentWaveIndex].miniWaves[currentMiniWaveIndex].timeUntilSpawningEnds2;
        currentState = WaveState.InWave;
        */

        startButton.SetActive(false);
    }

    IEnumerator WaveAnimation()
    {
        yield return new WaitForSeconds(1f); //animation duration

        //Debug.Log("Animation Ends");

        //currentMiniWaveIndex = 0;
        StartMiniWave();
        var index = waves[currentWaveIndex].miniWaves[currentMiniWaveIndex];
        inwaveTimer1 = index.timeUntilSpawningEnds1;
        inwaveTimer2 = index.timeUntilSpawningEnds2;
        inwaveTimer3 = index.timeUntilSpawningEnds3;
        inwaveTimer4 = index.timeUntilSpawningEnds4;
        inwaveTimer5 = index.timeUntilSpawningEnds5;
        inwaveTimer6 = index.timeUntilSpawningEnds6;
        inwaveTimer7 = index.timeUntilSpawningEnds7;
        inwaveTimer8 = index.timeUntilSpawningEnds8;
        inwaveTimer9 = index.timeUntilSpawningEnds9;
        inwaveTimer10 = index.timeUntilSpawningEnds10;
        inwaveTimer11 = index.timeUntilSpawningEnds11;
        inwaveTimer12 = index.timeUntilSpawningEnds12;

        currentState = WaveState.InWave;
    }

    void HandlePrewave()
    {
        if (!teleported)
        {
            TeleportTroopsToKilldozer();
            teleported = true;
        }

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

        if (aliveEnemies.Count == 0 && inwaveTimer1 <= 0 && inwaveTimer2 <= 0 && inwaveTimer3 <= 0 && inwaveTimer4 <= 0 && inwaveTimer5 <= 0 && inwaveTimer6 <= 0 && inwaveTimer7 <= 0 && inwaveTimer8 <= 0 && inwaveTimer9 <= 0 && inwaveTimer10 <= 0 && inwaveTimer11 <= 0 && inwaveTimer12 <= 0)
        {
            currentMiniWaveIndex++;
            breakTimer = waves[currentWaveIndex].breakDuration;
            currentState = WaveState.Break;
            //startButton.SetActive(true); (skips transition)
            teleported = false;
        }

        if (inwaveTimer1 > 0)
            inwaveTimer1 -= Time.deltaTime;

        if (inwaveTimer2 > 0)
            inwaveTimer2 -= Time.deltaTime;

        if (inwaveTimer3 > 0)
            inwaveTimer3 -= Time.deltaTime;

        if (inwaveTimer4 > 0)
            inwaveTimer4 -= Time.deltaTime;

        if (inwaveTimer5 > 0)
            inwaveTimer5 -= Time.deltaTime;

        if (inwaveTimer6 > 0)
            inwaveTimer6 -= Time.deltaTime;

        if (inwaveTimer7 > 0)
            inwaveTimer7 -= Time.deltaTime;

        if (inwaveTimer8 > 0)
            inwaveTimer8 -= Time.deltaTime;

        if (inwaveTimer9 > 0)
            inwaveTimer9 -= Time.deltaTime;

        if (inwaveTimer10 > 0)
            inwaveTimer10 -= Time.deltaTime;

        if (inwaveTimer11 > 0)
            inwaveTimer11 -= Time.deltaTime;

        if (inwaveTimer12 > 0)
            inwaveTimer12 -= Time.deltaTime;
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
        settingsPanel.SetActive(true);
    }

    void HandleTransition()
    {
        if (transitioned)
        {
            return;
        }

        TeleportTroopsToKilldozer();
        cameraSystem.FocusOnKilldozer();

        troop1.transform.SetParent(killdozer.transform);
        troop2.transform.SetParent(killdozer.transform);
        troop3.transform.SetParent(killdozer.transform);
        troop4.transform.SetParent(killdozer.transform);

        troop1.GetComponent<BoxCollider2D>().enabled = false;
        troop2.GetComponent<BoxCollider2D>().enabled = false;
        troop3.GetComponent<BoxCollider2D>().enabled = false;
        troop4.GetComponent<BoxCollider2D>().enabled = false;

        waveStateText.text = "Transition";
        transitioning = true;

        killdozerAnimator.SetTrigger("Move Right");

        transitioned = true;
        StartCoroutine(TransitionDelay());
    }

    IEnumerator TransitionDelay()
    {
        yield return new WaitForSeconds(5.5f);
        transitioning = false;
        prewaveTimer = waves[currentWaveIndex].prewaveDuration;
        currentState = WaveState.Prewave;

        troop1.transform.SetParent(null);
        troop2.transform.SetParent(null);
        troop3.transform.SetParent(null);
        troop4.transform.SetParent(null);

        troop1.GetComponent<BoxCollider2D>().enabled = true;
        troop2.GetComponent<BoxCollider2D>().enabled = true;
        troop3.GetComponent<BoxCollider2D>().enabled = true;
        troop4.GetComponent<BoxCollider2D>().enabled = true;

        troop1.transform.position = killdozerTransform1.position;
        troop2.transform.position = killdozerTransform2.position;
        troop3.transform.position = killdozerTransform3.position;
        troop4.transform.position = killdozerTransform4.position;

        troop1.GetComponent<TroopClass>().SetTargetPositionHere();
        troop2.GetComponent<TroopClass>().SetTargetPositionHere();
        troop3.GetComponent<TroopClass>().SetTargetPositionHere();
        troop4.GetComponent<TroopClass>().SetTargetPositionHere();

        troop1.GetComponent<Troop>().FullHeal();
        troop2.GetComponent<Troop>().FullHeal();
        troop3.GetComponent<Troop>().FullHeal();
        troop4.GetComponent<Troop>().FullHeal();

        transitioned = false;

        cameraSystem.DefocusKilldozer();

        startButton.SetActive(true);
    }

    void StartMiniWave()
    {
        startButton.SetActive(false);

        if (currentMiniWaveIndex >= waves[currentWaveIndex].miniWaves.Count)
        {
            //Debug.Log("Next");
            NextWave();
        }
        else
        {
            //Debug.Log("Spawn");
            miniWaveNumText.text = (currentMiniWaveIndex + 1).ToString();
            MiniWave currentMiniWave = waves[currentWaveIndex].miniWaves[currentMiniWaveIndex];

            /*
            if (currentMiniWave.enemiesToSpawn1.Count > 0 && currentMiniWave.spawnLocations1.Count > 0)
            {
                inwaveTimer1 = currentMiniWave.timeUntilSpawningEnds1;
                StartCoroutine(SpawnEnemies1(currentMiniWave));
            }

            if (currentMiniWave.enemiesToSpawn2.Count > 0 && currentMiniWave.spawnLocations2.Count > 0)
            {
                inwaveTimer2 = currentMiniWave.timeUntilSpawningEnds2;
                StartCoroutine(SpawnEnemies2(currentMiniWave));
            }

            if (currentMiniWave.enemiesToSpawn3.Count > 0 && currentMiniWave.spawnLocations3.Count > 0)
            {
                inwaveTimer3 = currentMiniWave.timeUntilSpawningEnds3;
                StartCoroutine(SpawnEnemies3(currentMiniWave));
            }

            if (currentMiniWave.enemiesToSpawn4.Count > 0 && currentMiniWave.spawnLocations4.Count > 0)
            {
                inwaveTimer4 = currentMiniWave.timeUntilSpawningEnds4;
                StartCoroutine(SpawnEnemies4(currentMiniWave));
            }

            if (currentMiniWave.enemiesToSpawn5.Count > 0 && currentMiniWave.spawnLocations5.Count > 0)
            {
                inwaveTimer5 = currentMiniWave.timeUntilSpawningEnds5;
                StartCoroutine(SpawnEnemies5(currentMiniWave));
            }

            if (currentMiniWave.enemiesToSpawn6.Count > 0 && currentMiniWave.spawnLocations6.Count > 0)
            {
                inwaveTimer6 = currentMiniWave.timeUntilSpawningEnds6;
                StartCoroutine(SpawnEnemies6(currentMiniWave));
            }

            if (currentMiniWave.enemiesToSpawn7.Count > 0 && currentMiniWave.spawnLocations7.Count > 0)
            {
                inwaveTimer7 = currentMiniWave.timeUntilSpawningEnds7;
                StartCoroutine(SpawnEnemies7(currentMiniWave));
            }

            if (currentMiniWave.enemiesToSpawn8.Count > 0 && currentMiniWave.spawnLocations8.Count > 0)
            {
                inwaveTimer8 = currentMiniWave.timeUntilSpawningEnds8;
                StartCoroutine(SpawnEnemies8(currentMiniWave));
            }

            if (currentMiniWave.enemiesToSpawn9.Count > 0 && currentMiniWave.spawnLocations9.Count > 0)
            {
                inwaveTimer9 = currentMiniWave.timeUntilSpawningEnds9;
                StartCoroutine(SpawnEnemies9(currentMiniWave));
            }

            if (currentMiniWave.enemiesToSpawn10.Count > 0 && currentMiniWave.spawnLocations10.Count > 0)
            {
                inwaveTimer10 = currentMiniWave.timeUntilSpawningEnds10;
                StartCoroutine(SpawnEnemies10(currentMiniWave));
            }

            if (currentMiniWave.enemiesToSpawn11.Count > 0 && currentMiniWave.spawnLocations11.Count > 0)
            {
                inwaveTimer11 = currentMiniWave.timeUntilSpawningEnds11;
                StartCoroutine(SpawnEnemies11(currentMiniWave));
            }

            if (currentMiniWave.enemiesToSpawn12.Count > 0 && currentMiniWave.spawnLocations12.Count > 0)
            {
                inwaveTimer12 = currentMiniWave.timeUntilSpawningEnds12;
                StartCoroutine(SpawnEnemies12(currentMiniWave));
            }

            currentState = WaveState.InWave;
            */

            inwaveTimer1 = currentMiniWave.timeUntilSpawningEnds1;
            inwaveTimer2 = currentMiniWave.timeUntilSpawningEnds2;
            inwaveTimer3 = currentMiniWave.timeUntilSpawningEnds3;
            inwaveTimer4 = currentMiniWave.timeUntilSpawningEnds4;
            inwaveTimer5 = currentMiniWave.timeUntilSpawningEnds5;
            inwaveTimer6 = currentMiniWave.timeUntilSpawningEnds6;
            inwaveTimer7 = currentMiniWave.timeUntilSpawningEnds7;
            inwaveTimer8 = currentMiniWave.timeUntilSpawningEnds8;
            inwaveTimer9 = currentMiniWave.timeUntilSpawningEnds9;
            inwaveTimer10 = currentMiniWave.timeUntilSpawningEnds10;
            inwaveTimer11 = currentMiniWave.timeUntilSpawningEnds11;
            inwaveTimer12 = currentMiniWave.timeUntilSpawningEnds12;

            currentState = WaveState.InWave;

            StartCoroutine(SpawnEnemies1(currentMiniWave));
            StartCoroutine(SpawnEnemies2(currentMiniWave));
            StartCoroutine(SpawnEnemies3(currentMiniWave));
            StartCoroutine(SpawnEnemies4(currentMiniWave));
            StartCoroutine(SpawnEnemies5(currentMiniWave));
            StartCoroutine(SpawnEnemies6(currentMiniWave));
            StartCoroutine(SpawnEnemies7(currentMiniWave));
            StartCoroutine(SpawnEnemies8(currentMiniWave));
            StartCoroutine(SpawnEnemies9(currentMiniWave));
            StartCoroutine(SpawnEnemies10(currentMiniWave));
            StartCoroutine(SpawnEnemies11(currentMiniWave));
            StartCoroutine(SpawnEnemies12(currentMiniWave));
            
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
            miniWaveNumText.text = "0";
            currentState = WaveState.Transition;
        }
    }

    //========================================SPAWN ENEMIES======================================
    IEnumerator SpawnEnemies1(MiniWave currentMiniWave)
    {
        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns1;
        int enemyIndex = 0;

        while (inwaveTimer1 > 0)
        {
            if (enemyIndex < currentMiniWave.enemiesToSpawn1.Count && currentMiniWave.spawnLocations1.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations1[enemyIndex % currentMiniWave.spawnLocations1.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn1[enemyIndex], spawnLocation.position, spawnLocation.rotation);
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

        //currentMiniWaveIndex++;
        //inwaveTimer1 = 0;
    }

    IEnumerator SpawnEnemies2(MiniWave currentMiniWave)
    {
        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns2;
        int enemyIndex = 0;

        while (inwaveTimer2 > 0)
        {
            if (enemyIndex < currentMiniWave.enemiesToSpawn2.Count && currentMiniWave.spawnLocations2.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations2[enemyIndex % currentMiniWave.spawnLocations2.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn2[enemyIndex], spawnLocation.position, spawnLocation.rotation);
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

        //currentMiniWaveIndex++;
        //inwaveTimer2 = 0;
    }

    IEnumerator SpawnEnemies3(MiniWave currentMiniWave)
    {
        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns3;
        int enemyIndex = 0;

        while (inwaveTimer3 > 0)
        {
            if (enemyIndex < currentMiniWave.enemiesToSpawn3.Count && currentMiniWave.spawnLocations3.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations3[enemyIndex % currentMiniWave.spawnLocations3.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn3[enemyIndex], spawnLocation.position, spawnLocation.rotation);
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

        //currentMiniWaveIndex++;
        //inwaveTimer3 = 0;
    }

    IEnumerator SpawnEnemies4(MiniWave currentMiniWave)
    {
        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns4;
        int enemyIndex = 0;

        while (inwaveTimer4 > 0)
        {
            if (enemyIndex < currentMiniWave.enemiesToSpawn4.Count && currentMiniWave.spawnLocations4.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations4[enemyIndex % currentMiniWave.spawnLocations4.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn4[enemyIndex], spawnLocation.position, spawnLocation.rotation);
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

        //currentMiniWaveIndex++;
        //inwaveTimer4 = 0;
    }

    IEnumerator SpawnEnemies5(MiniWave currentMiniWave)
    {
        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns5;
        int enemyIndex = 0;

        while (inwaveTimer5 > 0)
        {
            if (enemyIndex < currentMiniWave.enemiesToSpawn5.Count && currentMiniWave.spawnLocations5.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations5[enemyIndex % currentMiniWave.spawnLocations5.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn5[enemyIndex], spawnLocation.position, spawnLocation.rotation);
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

        //currentMiniWaveIndex++;
        //inwaveTimer5 = 0;
    }

    IEnumerator SpawnEnemies6(MiniWave currentMiniWave)
    {
        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns6;
        int enemyIndex = 0;

        while (inwaveTimer6 > 0)
        {
            if (enemyIndex < currentMiniWave.enemiesToSpawn6.Count && currentMiniWave.spawnLocations6.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations6[enemyIndex % currentMiniWave.spawnLocations6.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn6[enemyIndex], spawnLocation.position, spawnLocation.rotation);
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

        //currentMiniWaveIndex++;
        //inwaveTimer6 = 0;
    }

    IEnumerator SpawnEnemies7(MiniWave currentMiniWave)
    {
        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns7;
        int enemyIndex = 0;

        while (inwaveTimer7 > 0)
        {
            if (enemyIndex < currentMiniWave.enemiesToSpawn7.Count && currentMiniWave.spawnLocations7.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations7[enemyIndex % currentMiniWave.spawnLocations7.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn7[enemyIndex], spawnLocation.position, spawnLocation.rotation);
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

        //currentMiniWaveIndex++;
        //inwaveTimer7 = 0;
    }

    IEnumerator SpawnEnemies8(MiniWave currentMiniWave)
    {
        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns8;
        int enemyIndex = 0;

        while (inwaveTimer8 > 0)
        {
            if (enemyIndex < currentMiniWave.enemiesToSpawn8.Count && currentMiniWave.spawnLocations8.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations8[enemyIndex % currentMiniWave.spawnLocations8.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn8[enemyIndex], spawnLocation.position, spawnLocation.rotation);
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

        //currentMiniWaveIndex++;
        //inwaveTimer8 = 0;
    }

    IEnumerator SpawnEnemies9(MiniWave currentMiniWave)
    {
        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns9;
        int enemyIndex = 0;

        while (inwaveTimer9 > 0)
        {
            if (enemyIndex < currentMiniWave.enemiesToSpawn9.Count && currentMiniWave.spawnLocations9.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations9[enemyIndex % currentMiniWave.spawnLocations9.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn9[enemyIndex], spawnLocation.position, spawnLocation.rotation);
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

        //currentMiniWaveIndex++;
        //inwaveTimer9 = 0;
    }

    IEnumerator SpawnEnemies10(MiniWave currentMiniWave)
    {
        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns10;
        int enemyIndex = 0;

        while (inwaveTimer10 > 0)
        {
            if (enemyIndex < currentMiniWave.enemiesToSpawn10.Count && currentMiniWave.spawnLocations10.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations10[enemyIndex % currentMiniWave.spawnLocations10.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn10[enemyIndex], spawnLocation.position, spawnLocation.rotation);
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

        //currentMiniWaveIndex++;
        //inwaveTimer10 = 0;
    }

    IEnumerator SpawnEnemies11(MiniWave currentMiniWave)
    {
        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns11;
        int enemyIndex = 0;

        while (inwaveTimer11 > 0)
        {
            if (enemyIndex < currentMiniWave.enemiesToSpawn11.Count && currentMiniWave.spawnLocations11.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations11[enemyIndex % currentMiniWave.spawnLocations11.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn11[enemyIndex], spawnLocation.position, spawnLocation.rotation);
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

        //currentMiniWaveIndex++;
        //inwaveTimer11 = 0;
    }

    IEnumerator SpawnEnemies12(MiniWave currentMiniWave)
    {
        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns12;
        int enemyIndex = 0;

        while (inwaveTimer12 > 0)
        {
            if (enemyIndex < currentMiniWave.enemiesToSpawn12.Count && currentMiniWave.spawnLocations12.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations12[enemyIndex % currentMiniWave.spawnLocations12.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn12[enemyIndex], spawnLocation.position, spawnLocation.rotation);
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

        //currentMiniWaveIndex++;
        //inwaveTimer12 = 0;
    }

    void TeleportTroopsToKilldozer()
    {
        //Vector3 offset = new Vector3(0, 0, 0); 

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

    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }
}
