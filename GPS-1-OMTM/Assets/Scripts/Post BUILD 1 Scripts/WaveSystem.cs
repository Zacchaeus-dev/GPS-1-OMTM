using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

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
        public List<GameObject> objectsToDisableDuringWave;
        public List<GameObject> objectsToEnableDuringWave;
        public GameObject spawnMarkers;
        public Vector3 leftBorder;
        public Vector3 rightBorder;
        public float minX; //enemy indicators border
        public float maxX;
    }

    public List<Wave> waves;
    public int currentWaveIndex = 0;
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
    public GameObject startBorder;
    public GameObject commandCentreButton;
    public GameObject kccPanel;
    public TextMeshProUGUI waveStateText;
    public TextMeshProUGUI waveNumText;
    public TextMeshProUGUI miniWaveNumText;
    public GameObject troop1;
    public GameObject troop2;
    public GameObject troop3;
    public GameObject troop4;
    public Animator troop1TPAnimator;
    public Animator troop2TPAnimator;
    public Animator troop3TPAnimator;
    public Animator troop4TPAnimator;
    public GameObject troop1Model;
    public GameObject troop2Model;
    public GameObject troop3Model;
    public GameObject troop4Model;
    public TroopAnimationsManager troop1AM;
    public TroopAnimationsManager troop2AM;
    public TroopAnimationsManager troop3AM;
    public TroopAnimationsManager troop4AM;
    public Transform killdozerTransform1;
    public Transform killdozerTransform2;
    public Transform killdozerTransform3;
    public Transform killdozerTransform4;
    public GameObject settingsPanel;
    public CameraSystem cameraSystem;
    public IndicatorManager indicatorManager;
    public GameObject killdozer;
    public Animator killdozerAnimator;
    public static bool transitioning = false;
    private bool teleported = false;
    private bool transitioned = false;
    private bool timerOn;
    private float timerDuration;
    private float timerDurationRemaining;
    public Image timerFill;
    public TextMeshProUGUI timerText;
    public GameObject wavePopUp;
    public GameObject wave1Screen;
    public GameObject wave2Screen;
    public GameObject wave3Screen;
    public GameObject wave4Screen;
    public bool wave1Started;
    public TutorialPhase tutorialPhase;
    public GameObject instruction7;
    public GameObject instruction8;
    public GameObject edgePanTutorial;
    int maxEnemies = 80;
    private bool ending = false;
    public TroopHUD troop1HUD;
    public TroopHUD troop2HUD;
    public TroopHUD troop3HUD;
    public TroopHUD troop4HUD;
    private bool startSpawning;
    private bool tpAnimationON;
    public TroopController2D troopController2D;

    public GameObject inWaveBarObject;
    public Image inWaveProgressBar;
    public GameObject breakWaveBarObject;
    public Image breakProgressBar;

    void Start()
    {
        startButton.SetActive(true);
        startBorder.SetActive(true);
        waveNumText.text = "0";
        miniWaveNumText.text = "0";
        waveStateText.text = "Pre Wave";

        TeleportTroopsToKilldozer();
        UpdateButtonState();
        UpdateTimerUI();

        foreach (GameObject obj in waves[currentWaveIndex].objectsToDisableDuringWave) //disable objects for that wave
        {
            if (obj != null)
            {
                obj.SetActive(false);
                //Debug.Log("Disabled " + obj.name);
            }
        }

        foreach (GameObject obj in waves[currentWaveIndex].objectsToEnableDuringWave) //enable objects for that wave
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        waves[currentWaveIndex].spawnMarkers.SetActive(true);
        cameraSystem.leftBorder = waves[currentWaveIndex].leftBorder;
        cameraSystem.rightBorder = waves[currentWaveIndex].rightBorder;
        indicatorManager.minX = waves[currentWaveIndex].minX;
        indicatorManager.maxX = waves[currentWaveIndex].maxX;   
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

        if (tutorialPhase.tutorialOn == false)
        {
            UpdateButtonState();
        }

        UpdateTimerUI();
    }

    public void SkipWave()
    {
        waves[currentWaveIndex].prewaveDuration = 0;
        waves[currentWaveIndex].breakDuration = 0;
        inwaveTimer1 = 0;
        inwaveTimer2 = 0;
        inwaveTimer3 = 0;
        inwaveTimer4 = 0;
        inwaveTimer5 = 0;
        inwaveTimer6 = 0;
        inwaveTimer7 = 0;
        inwaveTimer8 = 0;
        inwaveTimer9 = 0;
        inwaveTimer10 = 0;
        inwaveTimer11 = 0;
        inwaveTimer12 = 0;
        aliveEnemies.Clear();

        UpdateTimerUI();
    }

    public void StartWave()
    {
        instruction7.SetActive(false);
        instruction8.SetActive(false);
        edgePanTutorial.SetActive(false);

        waveStateText.text = "Pre Wave";
        waveNumText.text = waves[currentWaveIndex].waveNum.ToString();

        wave1Started = true;

        waves[currentWaveIndex].spawnMarkers.SetActive(false);

        switch (waves[currentWaveIndex].waveNum)
        {
            case 1:
                wave1Screen.SetActive(true);
                wave2Screen.SetActive(false);
                wave3Screen.SetActive(false);
                wave4Screen.SetActive(false);
                break;
            case 2:
                wave1Screen.SetActive(false);
                wave2Screen.SetActive(true);
                wave3Screen.SetActive(false);
                wave4Screen.SetActive(false);
                break;
            case 3:
                wave1Screen.SetActive(false);
                wave2Screen.SetActive(false);
                wave3Screen.SetActive(true);
                wave4Screen.SetActive(false);
                break;
            case 4:
                wave1Screen.SetActive(false);
                wave2Screen.SetActive(false);
                wave3Screen.SetActive(false);
                wave4Screen.SetActive(true);
                break;
        }
        FindObjectOfType<AudioManager>().Play("button");
        wavePopUp.SetActive(true);

        StartCoroutine(WaveAnimation());

        // bgm
        FindObjectOfType<AudioManager>().Play("BGM");
        startButton.SetActive(false);
        startBorder.SetActive(false);
    }

    IEnumerator WaveAnimation()
    {
        yield return new WaitForSeconds(3f); //animation duration

        wavePopUp.SetActive(false);

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

        //Debug.Log(currentWaveIndex.ToString());

        foreach (GameObject obj in waves[currentWaveIndex].objectsToDisableDuringWave) //disable objects for that wave
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        foreach (GameObject obj in waves[currentWaveIndex].objectsToEnableDuringWave) //enable objects for that wave
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        if (prewaveTimer > 0)
        {
            prewaveTimer -= Time.deltaTime;
            int prewavetime = (int)prewaveTimer;
            timerText.text =  prewavetime.ToString() + "s";
        }
        else
        {
            timerOn = false;
            StartWave();
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
            startSpawning = false;  
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

        UpdateInWaveProgressBar();
    }

    void HandleBreak()
    {
        waveStateText.text = "Break";
        FindObjectOfType<AudioManager>().Dim("BGM");
        if (breakTimer > 0)
        {
            breakTimer -= Time.deltaTime;
            UpdateBreakProgressBar();
        }
        else
        {
            /*
            foreach (GameObject obj in waves[currentWaveIndex].objectsToDisableDuringWave) //re enable objects
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }

            foreach (GameObject obj in waves[currentWaveIndex].objectsToEnableDuringWave) //re disable objects
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
            */

            StartMiniWave();
        }
    }

    void HandleEnd()
    {
        if (ending)
        {
            return;
        }

        ending = true;
        waveStateText.text = "End";
        TeleportTroopsToKilldozerWithAnimation();
        cameraSystem.FocusOnKilldozer();

        StartCoroutine(TPEndAnimationDelay());
        StartCoroutine(EndDelay());
    }

    IEnumerator TPEndAnimationDelay()
    {
        yield return new WaitForSeconds(2f);

        troop1.transform.SetParent(killdozer.transform);
        troop2.transform.SetParent(killdozer.transform);
        troop3.transform.SetParent(killdozer.transform);
        troop4.transform.SetParent(killdozer.transform);

        troop1.transform.position = killdozerTransform1.position;
        troop2.transform.position = killdozerTransform2.position;
        troop3.transform.position = killdozerTransform3.position;
        troop4.transform.position = killdozerTransform4.position;

        troop1.GetComponent<TroopClass>().onPlatform = "KD Middle-Ground";
        troop2.GetComponent<TroopClass>().onPlatform = "KD Middle-Ground";
        troop3.GetComponent<TroopClass>().onPlatform = "KD Middle-Ground";
        troop4.GetComponent<TroopClass>().onPlatform = "KD Middle-Ground";

        troop1.GetComponent<BoxCollider2D>().enabled = false;
        troop2.GetComponent<BoxCollider2D>().enabled = false;
        troop3.GetComponent<BoxCollider2D>().enabled = false;
        troop4.GetComponent<BoxCollider2D>().enabled = false;

        killdozerAnimator.SetTrigger("Move Right");
    }

    IEnumerator EndDelay()
    {
        yield return new WaitForSeconds(6f);

        cameraSystem.DefocusKilldozer();

        yield return new WaitForSeconds(1.5f);

        settingsPanel.SetActive(true);
        FindObjectOfType<AudioManager>().Stop("BGM");
        FindObjectOfType<AudioManager>().Play("WinBGM");
    }

    void HandleTransition()
    {
        if (transitioned)
        {
            return;
        }
        transitioned = true;
        TeleportTroopsToKilldozerWithAnimation();
        cameraSystem.FocusOnKilldozer();

        StartCoroutine(TPAnimationDelay());
        StartCoroutine(TeleportDelay());
    }

    IEnumerator TeleportDelay()
    {
        yield return new WaitForSeconds(2f);

        troop1.transform.SetParent(killdozer.transform);
        troop2.transform.SetParent(killdozer.transform);
        troop3.transform.SetParent(killdozer.transform);
        troop4.transform.SetParent(killdozer.transform);

        troop1.transform.position = killdozerTransform1.position;
        troop2.transform.position = killdozerTransform2.position;
        troop3.transform.position = killdozerTransform3.position;
        troop4.transform.position = killdozerTransform4.position;

        troop1.GetComponent<TroopClass>().onPlatform = "KD Middle-Ground";
        troop2.GetComponent<TroopClass>().onPlatform = "KD Middle-Ground";
        troop3.GetComponent<TroopClass>().onPlatform = "KD Middle-Ground";
        troop4.GetComponent<TroopClass>().onPlatform = "KD Middle-Ground";

        troop1.GetComponent<BoxCollider2D>().enabled = false;
        troop2.GetComponent<BoxCollider2D>().enabled = false;
        troop3.GetComponent<BoxCollider2D>().enabled = false;
        troop4.GetComponent<BoxCollider2D>().enabled = false;

        waveStateText.text = "Transition";
        transitioning = true;

        killdozerAnimator.SetTrigger("Move Right");

        tpAnimationON = false;

        StartCoroutine(TransitionDelay());
    }

        IEnumerator TransitionDelay()
    {
        if (waves[currentWaveIndex].waveNum != 4)
        {
            yield return new WaitForSeconds(5.5f);
        }
        else
        {
            yield return new WaitForSeconds(12.5f); //transition is longer for wave 4
        }
        
        transitioning = false;
        prewaveTimer = waves[currentWaveIndex].prewaveDuration;
        timerDuration = prewaveTimer;
        timerDurationRemaining = timerDuration;
        timerOn = true;
        UpdateTimerUI();
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

        troop1HUD.EnableTroopBars();
        troop2HUD.EnableTroopBars();
        troop3HUD.EnableTroopBars();
        troop4HUD.EnableTroopBars();

        troop1.GetComponent<Troop>().FullHeal();
        troop2.GetComponent<Troop>().FullHeal();
        troop3.GetComponent<Troop>().FullHeal();
        troop4.GetComponent<Troop>().FullHeal();

        transitioned = false;

        cameraSystem.DefocusKilldozer();

        startButton.SetActive(true);
        startBorder.SetActive(true);

        waves[currentWaveIndex].spawnMarkers.SetActive(true);
        cameraSystem.leftBorder = waves[currentWaveIndex].leftBorder;
        cameraSystem.rightBorder = waves[currentWaveIndex].rightBorder;
        indicatorManager.minX = waves[currentWaveIndex].minX;
        indicatorManager.maxX = waves[currentWaveIndex].maxX;
        cameraSystem.UpdateZoomPosition();
    }

    void StartMiniWave()
    {
        FindObjectOfType<AudioManager>().Undim("BGM");

        startButton.SetActive(false);
        startBorder.SetActive(false);

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

            if (startSpawning == true)
            {
                return;
            }

            startSpawning = true;
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
        foreach (GameObject obj in waves[currentWaveIndex].objectsToDisableDuringWave) //re enable objects
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        foreach (GameObject obj in waves[currentWaveIndex].objectsToEnableDuringWave) //re disable objects
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        currentMiniWaveIndex = 0;
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Count)
        {
            currentState = WaveState.End;
            //Debug.Log("Transitioning to end");
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
        if (aliveEnemies.Count >= maxEnemies)
        {
            Debug.Log("Reached maximum enemy limit.");
            currentMiniWave.enemiesToSpawn1.Clear();
            yield break;
        }

        if (currentMiniWave == null)
        {
            Debug.Log("currentMiniWave is null.");
            yield break;
        }

        if (currentMiniWave.spawnLocations1 == null || currentMiniWave.enemiesToSpawn1 == null)
        {
            Debug.Log("spawnLocations1 or enemiesToSpawn1 in currentMiniWave is null.");
            yield break;
        }

        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns1;
        int enemyIndex = 0;

        while (inwaveTimer1 > 0 && enemyIndex < currentMiniWave.enemiesToSpawn1.Count)
        {
            if (aliveEnemies.Count >= maxEnemies)
            {
                Debug.Log("Reached maximum enemy limit during spawning.");
                break;
            }

            if (currentMiniWave.spawnLocations1.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations1[enemyIndex % currentMiniWave.spawnLocations1.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn1[enemyIndex], spawnLocation.position, spawnLocation.rotation);
                aliveEnemies.Add(enemy);

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                FlyingEnemy flyingEnemyScript = null;

                if (enemyScript == null)
                {
                    flyingEnemyScript = enemy.GetComponent<FlyingEnemy>();
                }

                if (enemyScript != null)
                {
                    enemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else if (flyingEnemyScript != null)
                {
                    flyingEnemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else
                {
                    Debug.Log("Spawned enemy does not have Enemy or FlyingEnemy component.");
                }

                enemyIndex++;
            }
            else
            {
                Debug.Log("Not enough spawn locations or enemies to spawn for the number of enemies in Spawner 1.");
                break;
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        if (enemyIndex >= currentMiniWave.enemiesToSpawn1.Count)
        {
            Debug.Log("All enemies in current mini-wave have been spawned.");
        }
    }

    IEnumerator SpawnEnemies2(MiniWave currentMiniWave)
    {
        if (aliveEnemies.Count >= maxEnemies)
        {
            Debug.Log("Reached maximum enemy limit.");
            currentMiniWave.enemiesToSpawn2.Clear();
            yield break;
        }

        if (currentMiniWave == null)
        {
            Debug.Log("currentMiniWave is null.");
            yield break;
        }

        if (currentMiniWave.spawnLocations2 == null || currentMiniWave.enemiesToSpawn2 == null)
        {
            Debug.Log("spawnLocations2 or enemiesToSpawn2 in currentMiniWave is null.");
            yield break;
        }

        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns2;
        int enemyIndex = 0;

        while (inwaveTimer2 > 0 && enemyIndex < currentMiniWave.enemiesToSpawn2.Count)
        {
            if (aliveEnemies.Count >= maxEnemies)
            {
                Debug.Log("Reached maximum enemy limit during spawning.");
                break;
            }

            if (currentMiniWave.spawnLocations2.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations2[enemyIndex % currentMiniWave.spawnLocations2.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn2[enemyIndex], spawnLocation.position, spawnLocation.rotation);
                aliveEnemies.Add(enemy);

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                FlyingEnemy flyingEnemyScript = null;

                if (enemyScript == null)
                {
                    flyingEnemyScript = enemy.GetComponent<FlyingEnemy>();
                }

                if (enemyScript != null)
                {
                    enemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else if (flyingEnemyScript != null)
                {
                    flyingEnemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else
                {
                    Debug.Log("Spawned enemy does not have Enemy or FlyingEnemy component.");
                }

                enemyIndex++;
            }
            else
            {
                Debug.Log("Not enough spawn locations or enemies to spawn for the number of enemies in Spawner 2.");
                break;
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        if (enemyIndex >= currentMiniWave.enemiesToSpawn2.Count)
        {
            Debug.Log("All enemies have been spawned for Spawner 2.");
        }
    }

    IEnumerator SpawnEnemies3(MiniWave currentMiniWave)
    {
        if (aliveEnemies.Count >= maxEnemies)
        {
            Debug.Log("Reached maximum enemy limit.");
            currentMiniWave.enemiesToSpawn3.Clear();
            yield break;
        }

        if (currentMiniWave == null)
        {
            Debug.Log("currentMiniWave is null.");
            yield break;
        }

        if (currentMiniWave.spawnLocations3 == null || currentMiniWave.enemiesToSpawn3 == null)
        {
            Debug.Log("spawnLocations3 or enemiesToSpawn3 in currentMiniWave is null.");
            yield break;
        }

        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns3;
        int enemyIndex = 0;

        while (inwaveTimer3 > 0 && enemyIndex < currentMiniWave.enemiesToSpawn3.Count)
        {
            if (aliveEnemies.Count >= maxEnemies)
            {
                Debug.Log("Reached maximum enemy limit during spawning.");
                break;
            }

            if (currentMiniWave.spawnLocations3.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations3[enemyIndex % currentMiniWave.spawnLocations3.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn3[enemyIndex], spawnLocation.position, spawnLocation.rotation);
                aliveEnemies.Add(enemy);

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                FlyingEnemy flyingEnemyScript = null;

                if (enemyScript == null)
                {
                    flyingEnemyScript = enemy.GetComponent<FlyingEnemy>();
                }

                if (enemyScript != null)
                {
                    enemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else if (flyingEnemyScript != null)
                {
                    flyingEnemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else
                {
                    Debug.Log("Spawned enemy does not have Enemy or FlyingEnemy component.");
                }

                enemyIndex++;
            }
            else
            {
                Debug.Log("Not enough spawn locations or enemies to spawn for the number of enemies in Spawner 3.");
                break;
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        if (enemyIndex >= currentMiniWave.enemiesToSpawn3.Count)
        {
            Debug.Log("All enemies have been spawned for Spawner 3.");
        }
    }

    IEnumerator SpawnEnemies4(MiniWave currentMiniWave)
    {
        if (aliveEnemies.Count >= maxEnemies)
        {
            Debug.Log("Reached maximum enemy limit.");
            currentMiniWave.enemiesToSpawn4.Clear();
            yield break;
        }

        if (currentMiniWave == null)
        {
            Debug.Log("currentMiniWave is null.");
            yield break;
        }

        if (currentMiniWave.spawnLocations4 == null || currentMiniWave.enemiesToSpawn4 == null)
        {
            Debug.Log("spawnLocations4 or enemiesToSpawn4 in currentMiniWave is null.");
            yield break;
        }

        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns4;
        int enemyIndex = 0;

        while (inwaveTimer4 > 0 && enemyIndex < currentMiniWave.enemiesToSpawn4.Count)
        {
            if (aliveEnemies.Count >= maxEnemies)
            {
                Debug.Log("Reached maximum enemy limit during spawning.");
                break;
            }

            if (currentMiniWave.spawnLocations4.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations4[enemyIndex % currentMiniWave.spawnLocations4.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn4[enemyIndex], spawnLocation.position, spawnLocation.rotation);
                aliveEnemies.Add(enemy);

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                FlyingEnemy flyingEnemyScript = null;

                if (enemyScript == null)
                {
                    flyingEnemyScript = enemy.GetComponent<FlyingEnemy>();
                }

                if (enemyScript != null)
                {
                    enemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else if (flyingEnemyScript != null)
                {
                    flyingEnemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else
                {
                    Debug.Log("Spawned enemy does not have Enemy or FlyingEnemy component.");
                }

                enemyIndex++;
            }
            else
            {
                Debug.Log("Not enough spawn locations or enemies to spawn for the number of enemies in Spawner 4.");
                break;
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        if (enemyIndex >= currentMiniWave.enemiesToSpawn4.Count)
        {
            Debug.Log("All enemies have been spawned for Spawner 4.");
        }
    }

    IEnumerator SpawnEnemies5(MiniWave currentMiniWave)
    {
        if (aliveEnemies.Count >= maxEnemies)
        {
            Debug.Log("Reached maximum enemy limit.");
            currentMiniWave.enemiesToSpawn5.Clear();
            yield break;
        }

        if (currentMiniWave == null)
        {
            Debug.Log("currentMiniWave is null.");
            yield break;
        }

        if (currentMiniWave.spawnLocations5 == null || currentMiniWave.enemiesToSpawn5 == null)
        {
            Debug.Log("spawnLocations5 or enemiesToSpawn5 in currentMiniWave is null.");
            yield break;
        }

        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns5;
        int enemyIndex = 0;

        while (inwaveTimer5 > 0 && enemyIndex < currentMiniWave.enemiesToSpawn5.Count)
        {
            if (aliveEnemies.Count >= maxEnemies)
            {
                Debug.Log("Reached maximum enemy limit during spawning.");
                break;
            }

            if (currentMiniWave.spawnLocations5.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations5[enemyIndex % currentMiniWave.spawnLocations5.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn5[enemyIndex], spawnLocation.position, spawnLocation.rotation);
                aliveEnemies.Add(enemy);

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                FlyingEnemy flyingEnemyScript = null;

                if (enemyScript == null)
                {
                    flyingEnemyScript = enemy.GetComponent<FlyingEnemy>();
                }

                if (enemyScript != null)
                {
                    enemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else if (flyingEnemyScript != null)
                {
                    flyingEnemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else
                {
                    Debug.Log("Spawned enemy does not have Enemy or FlyingEnemy component.");
                }

                enemyIndex++;
            }
            else
            {
                Debug.Log("Not enough spawn locations or enemies to spawn for the number of enemies in Spawner 5.");
                break;
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        if (enemyIndex >= currentMiniWave.enemiesToSpawn5.Count)
        {
            Debug.Log("All enemies have been spawned for Spawner 5.");
        }
    }

    IEnumerator SpawnEnemies6(MiniWave currentMiniWave)
    {
        if (aliveEnemies.Count >= maxEnemies)
        {
            Debug.Log("Reached maximum enemy limit.");
            currentMiniWave.enemiesToSpawn6.Clear();
            yield break;
        }

        if (currentMiniWave == null)
        {
            Debug.Log("currentMiniWave is null.");
            yield break;
        }

        if (currentMiniWave.spawnLocations6 == null || currentMiniWave.enemiesToSpawn6 == null)
        {
            Debug.Log("spawnLocations6 or enemiesToSpawn6 in currentMiniWave is null.");
            yield break;
        }

        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns6;
        int enemyIndex = 0;

        while (inwaveTimer6 > 0 && enemyIndex < currentMiniWave.enemiesToSpawn6.Count)
        {
            if (aliveEnemies.Count >= maxEnemies)
            {
                Debug.Log("Reached maximum enemy limit during spawning.");
                break;
            }

            if (currentMiniWave.spawnLocations6.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations6[enemyIndex % currentMiniWave.spawnLocations6.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn6[enemyIndex], spawnLocation.position, spawnLocation.rotation);
                aliveEnemies.Add(enemy);

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                FlyingEnemy flyingEnemyScript = null;

                if (enemyScript == null)
                {
                    flyingEnemyScript = enemy.GetComponent<FlyingEnemy>();
                }

                if (enemyScript != null)
                {
                    enemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else if (flyingEnemyScript != null)
                {
                    flyingEnemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else
                {
                    Debug.Log("Spawned enemy does not have Enemy or FlyingEnemy component.");
                }

                enemyIndex++;
            }
            else
            {
                Debug.Log("Not enough spawn locations or enemies to spawn for the number of enemies in Spawner 6.");
                break;
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        if (enemyIndex >= currentMiniWave.enemiesToSpawn6.Count)
        {
            Debug.Log("All enemies have been spawned for Spawner 6.");
        }
    }

    IEnumerator SpawnEnemies7(MiniWave currentMiniWave)
    {
        if (aliveEnemies.Count >= maxEnemies)
        {
            Debug.Log("Reached maximum enemy limit.");
            currentMiniWave.enemiesToSpawn7.Clear();
            yield break;
        }

        if (currentMiniWave == null)
        {
            Debug.Log("currentMiniWave is null.");
            yield break;
        }

        if (currentMiniWave.spawnLocations7 == null || currentMiniWave.enemiesToSpawn7 == null)
        {
            Debug.Log("spawnLocations7 or enemiesToSpawn7 in currentMiniWave is null.");
            yield break;
        }

        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns7;
        int enemyIndex = 0;

        while (inwaveTimer7 > 0 && enemyIndex < currentMiniWave.enemiesToSpawn7.Count)
        {
            if (aliveEnemies.Count >= maxEnemies)
            {
                Debug.Log("Reached maximum enemy limit during spawning.");
                break;
            }

            if (currentMiniWave.spawnLocations7.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations7[enemyIndex % currentMiniWave.spawnLocations7.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn7[enemyIndex], spawnLocation.position, spawnLocation.rotation);
                aliveEnemies.Add(enemy);

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                FlyingEnemy flyingEnemyScript = null;

                if (enemyScript == null)
                {
                    flyingEnemyScript = enemy.GetComponent<FlyingEnemy>();
                }

                if (enemyScript != null)
                {
                    enemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else if (flyingEnemyScript != null)
                {
                    flyingEnemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else
                {
                    Debug.Log("Spawned enemy does not have Enemy or FlyingEnemy component.");
                }

                enemyIndex++;
            }
            else
            {
                Debug.Log("Not enough spawn locations or enemies to spawn for the number of enemies in Spawner 7.");
                break;
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        if (enemyIndex >= currentMiniWave.enemiesToSpawn7.Count)
        {
            Debug.Log("All enemies have been spawned for Spawner 7.");
        }
    }

    IEnumerator SpawnEnemies8(MiniWave currentMiniWave)
    {
        if (aliveEnemies.Count >= maxEnemies)
        {
            Debug.Log("Reached maximum enemy limit.");
            currentMiniWave.enemiesToSpawn8.Clear();
            yield break;
        }

        if (currentMiniWave == null)
        {
            Debug.Log("currentMiniWave is null.");
            yield break;
        }

        if (currentMiniWave.spawnLocations8 == null || currentMiniWave.enemiesToSpawn8 == null)
        {
            Debug.Log("spawnLocations8 or enemiesToSpawn8 in currentMiniWave is null.");
            yield break;
        }

        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns8;
        int enemyIndex = 0;

        while (inwaveTimer8 > 0 && enemyIndex < currentMiniWave.enemiesToSpawn8.Count)
        {
            if (aliveEnemies.Count >= maxEnemies)
            {
                Debug.Log("Reached maximum enemy limit during spawning.");
                break;
            }

            if (currentMiniWave.spawnLocations8.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations8[enemyIndex % currentMiniWave.spawnLocations8.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn8[enemyIndex], spawnLocation.position, spawnLocation.rotation);
                aliveEnemies.Add(enemy);

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                FlyingEnemy flyingEnemyScript = null;

                if (enemyScript == null)
                {
                    flyingEnemyScript = enemy.GetComponent<FlyingEnemy>();
                }

                if (enemyScript != null)
                {
                    enemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else if (flyingEnemyScript != null)
                {
                    flyingEnemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else
                {
                    Debug.Log("Spawned enemy does not have Enemy or FlyingEnemy component.");
                }

                enemyIndex++;
            }
            else
            {
                Debug.Log("Not enough spawn locations or enemies to spawn for the number of enemies in Spawner 8.");
                break;
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        if (enemyIndex >= currentMiniWave.enemiesToSpawn8.Count)
        {
            Debug.Log("All enemies have been spawned for Spawner 8.");
        }
    }

    IEnumerator SpawnEnemies9(MiniWave currentMiniWave)
    {
        if (aliveEnemies.Count >= maxEnemies)
        {
            Debug.Log("Reached maximum enemy limit.");
            currentMiniWave.enemiesToSpawn9.Clear();
            yield break;
        }

        if (currentMiniWave == null)
        {
            Debug.Log("currentMiniWave is null.");
            yield break;
        }

        if (currentMiniWave.spawnLocations9 == null || currentMiniWave.enemiesToSpawn9 == null)
        {
            Debug.Log("spawnLocations9 or enemiesToSpawn9 in currentMiniWave is null.");
            yield break;
        }

        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns9;
        int enemyIndex = 0;

        while (inwaveTimer9 > 0 && enemyIndex < currentMiniWave.enemiesToSpawn9.Count)
        {
            if (aliveEnemies.Count >= maxEnemies)
            {
                Debug.Log("Reached maximum enemy limit during spawning.");
                break;
            }

            if (currentMiniWave.spawnLocations9.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations9[enemyIndex % currentMiniWave.spawnLocations9.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn9[enemyIndex], spawnLocation.position, spawnLocation.rotation);
                aliveEnemies.Add(enemy);

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                FlyingEnemy flyingEnemyScript = null;

                if (enemyScript == null)
                {
                    flyingEnemyScript = enemy.GetComponent<FlyingEnemy>();
                }

                if (enemyScript != null)
                {
                    enemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else if (flyingEnemyScript != null)
                {
                    flyingEnemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else
                {
                    Debug.Log("Spawned enemy does not have Enemy or FlyingEnemy component.");
                }

                enemyIndex++;
            }
            else
            {
                Debug.Log("Not enough spawn locations or enemies to spawn for the number of enemies in Spawner 9.");
                break;
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        if (enemyIndex >= currentMiniWave.enemiesToSpawn9.Count)
        {
            Debug.Log("All enemies have been spawned for Spawner 9.");
        }
    }

    IEnumerator SpawnEnemies10(MiniWave currentMiniWave)
    {
        if (aliveEnemies.Count >= maxEnemies)
        {
            Debug.Log("Reached maximum enemy limit.");
            currentMiniWave.enemiesToSpawn10.Clear();
            yield break;
        }

        if (currentMiniWave == null)
        {
            Debug.Log("currentMiniWave is null.");
            yield break;
        }

        if (currentMiniWave.spawnLocations10 == null || currentMiniWave.enemiesToSpawn10 == null)
        {
            Debug.Log("spawnLocations10 or enemiesToSpawn10 in currentMiniWave is null.");
            yield break;
        }

        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns10;
        int enemyIndex = 0;

        while (inwaveTimer10 > 0 && enemyIndex < currentMiniWave.enemiesToSpawn10.Count)
        {
            if (aliveEnemies.Count >= maxEnemies)
            {
                Debug.Log("Reached maximum enemy limit during spawning.");
                break;
            }

            if (currentMiniWave.spawnLocations10.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations10[enemyIndex % currentMiniWave.spawnLocations10.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn10[enemyIndex], spawnLocation.position, spawnLocation.rotation);
                aliveEnemies.Add(enemy);

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                FlyingEnemy flyingEnemyScript = null;

                if (enemyScript == null)
                {
                    flyingEnemyScript = enemy.GetComponent<FlyingEnemy>();
                }

                if (enemyScript != null)
                {
                    enemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else if (flyingEnemyScript != null)
                {
                    flyingEnemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else
                {
                    Debug.Log("Spawned enemy does not have Enemy or FlyingEnemy component.");
                }

                enemyIndex++;
            }
            else
            {
                Debug.Log("Not enough spawn locations or enemies to spawn for the number of enemies in Spawner 10.");
                break;
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        if (enemyIndex >= currentMiniWave.enemiesToSpawn10.Count)
        {
            Debug.Log("All enemies have been spawned for Spawner 10.");
        }
    }

    IEnumerator SpawnEnemies11(MiniWave currentMiniWave)
    {
        if (aliveEnemies.Count >= maxEnemies)
        {
            Debug.Log("Reached maximum enemy limit.");
            currentMiniWave.enemiesToSpawn11.Clear();
            yield break;
        }

        if (currentMiniWave == null)
        {
            Debug.Log("currentMiniWave is null.");
            yield break;
        }

        if (currentMiniWave.spawnLocations11 == null || currentMiniWave.enemiesToSpawn11 == null)
        {
            Debug.Log("spawnLocations11 or enemiesToSpawn11 in currentMiniWave is null.");
            yield break;
        }

        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns11;
        int enemyIndex = 0;

        while (inwaveTimer11 > 0 && enemyIndex < currentMiniWave.enemiesToSpawn11.Count)
        {
            if (aliveEnemies.Count >= maxEnemies)
            {
                Debug.Log("Reached maximum enemy limit during spawning.");
                break;
            }

            if (currentMiniWave.spawnLocations11.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations11[enemyIndex % currentMiniWave.spawnLocations11.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn11[enemyIndex], spawnLocation.position, spawnLocation.rotation);
                aliveEnemies.Add(enemy);

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                FlyingEnemy flyingEnemyScript = null;

                if (enemyScript == null)
                {
                    flyingEnemyScript = enemy.GetComponent<FlyingEnemy>();
                }

                if (enemyScript != null)
                {
                    enemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else if (flyingEnemyScript != null)
                {
                    flyingEnemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else
                {
                    Debug.Log("Spawned enemy does not have Enemy or FlyingEnemy component.");
                }

                enemyIndex++;
            }
            else
            {
                Debug.Log("Not enough spawn locations or enemies to spawn for the number of enemies in Spawner 11.");
                break;
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        if (enemyIndex >= currentMiniWave.enemiesToSpawn11.Count)
        {
            Debug.Log("All enemies have been spawned for Spawner 11.");
        }
    }

    IEnumerator SpawnEnemies12(MiniWave currentMiniWave)
    {
        if (aliveEnemies.Count >= maxEnemies)
        {
            Debug.Log("Reached maximum enemy limit.");
            currentMiniWave.enemiesToSpawn12.Clear();
            yield break;
        }

        if (currentMiniWave == null)
        {
            Debug.Log("currentMiniWave is null.");
            yield break;
        }

        if (currentMiniWave.spawnLocations12 == null || currentMiniWave.enemiesToSpawn12 == null)
        {
            Debug.Log("spawnLocations12 or enemiesToSpawn12 in currentMiniWave is null.");
            yield break;
        }

        float timeBetweenSpawns = currentMiniWave.timeBetweenSpawns12;
        int enemyIndex = 0;

        while (inwaveTimer12 > 0 && enemyIndex < currentMiniWave.enemiesToSpawn12.Count)
        {
            if (aliveEnemies.Count >= maxEnemies)
            {
                Debug.Log("Reached maximum enemy limit during spawning.");
                break;
            }

            if (currentMiniWave.spawnLocations12.Count > 0)
            {
                Transform spawnLocation = currentMiniWave.spawnLocations12[enemyIndex % currentMiniWave.spawnLocations12.Count];
                GameObject enemy = Instantiate(currentMiniWave.enemiesToSpawn12[enemyIndex], spawnLocation.position, spawnLocation.rotation);
                aliveEnemies.Add(enemy);

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                FlyingEnemy flyingEnemyScript = null;

                if (enemyScript == null)
                {
                    flyingEnemyScript = enemy.GetComponent<FlyingEnemy>();
                }

                if (enemyScript != null)
                {
                    enemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else if (flyingEnemyScript != null)
                {
                    flyingEnemyScript.onDeath += () => aliveEnemies.Remove(enemy);
                }
                else
                {
                    Debug.Log("Spawned enemy does not have Enemy or FlyingEnemy component.");
                }

                enemyIndex++;
            }
            else
            {
                Debug.Log("Not enough spawn locations or enemies to spawn for the number of enemies in Spawner 12.");
                break;
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        if (enemyIndex >= currentMiniWave.enemiesToSpawn12.Count)
        {
            Debug.Log("All enemies have been spawned for Spawner 12.");
        }
    }

    public void TeleportTroopsToKilldozer()
    {
        //Vector3 offset = new Vector3(0, 0, 0); 

        troop1.transform.position = killdozerTransform1.position;
        troop2.transform.position = killdozerTransform2.position;
        troop3.transform.position = killdozerTransform3.position;
        troop4.transform.position = killdozerTransform4.position;

        troop1.GetComponent<TroopClass>().SetTargetPositionHere();
        troop2.GetComponent<TroopClass>().SetTargetPositionHere();
        troop3.GetComponent<TroopClass>().SetTargetPositionHere();
        troop4.GetComponent<TroopClass>().SetTargetPositionHere();
    }


    public void TeleportTroopsToKilldozerWithAnimation()
    {
        if (tpAnimationON)
        {
            return;
        }

        tpAnimationON = true;
        troop1TPAnimator.gameObject.SetActive(true);
        troop2TPAnimator.gameObject.SetActive(true);
        troop3TPAnimator.gameObject.SetActive(true);
        troop4TPAnimator.gameObject.SetActive(true);

        troop1TPAnimator.SetTrigger("Death");
        troop2TPAnimator.SetTrigger("Death");
        troop3TPAnimator.SetTrigger("Death");
        troop4TPAnimator.SetTrigger("Death");

        StartCoroutine(TPAnimationDelay());
    }

    IEnumerator TPAnimationDelay()
    {
        troop1AM.TroopIdle();
        troop2AM.TroopIdle();
        troop3AM.TroopIdle();
        troop4AM.TroopIdle();

        yield return new WaitForSeconds(0.75f);

        troopController2D.DeselectTroop();

        

        troop1Model.SetActive(false);
        troop2Model.SetActive(false);
        troop3Model.SetActive(false);
        troop4Model.SetActive(false);

        troop1HUD.DisableTroopBars();
        troop2HUD.DisableTroopBars();
        troop3HUD.DisableTroopBars();
        troop4HUD.DisableTroopBars();

        yield return new WaitForSeconds(0.75f);

        troop1.transform.position = killdozerTransform1.position;
        troop2.transform.position = killdozerTransform2.position;
        troop3.transform.position = killdozerTransform3.position;
        troop4.transform.position = killdozerTransform4.position;

        troop1.GetComponent<TroopClass>().SetTargetPositionHere();
        troop2.GetComponent<TroopClass>().SetTargetPositionHere();
        troop3.GetComponent<TroopClass>().SetTargetPositionHere();
        troop4.GetComponent<TroopClass>().SetTargetPositionHere();

        troop1TPAnimator.SetTrigger("Death");
        troop2TPAnimator.SetTrigger("Death");
        troop3TPAnimator.SetTrigger("Death");
        troop4TPAnimator.SetTrigger("Death");

        yield return new WaitForSeconds(0.75f);

        troop1Model.SetActive(true);
        troop2Model.SetActive(true);
        troop3Model.SetActive(true);
        troop4Model.SetActive(true);

        troop1AM.TroopIdle();
        troop2AM.TroopIdle();
        troop3AM.TroopIdle();
        troop4AM.TroopIdle();
    }

    void UpdateButtonState()
    {
        bool isActive;


            isActive = currentState == WaveState.Start || currentState == WaveState.Prewave || currentState == WaveState.Break;


        //bool isActive = currentState == WaveState.Start || currentState == WaveState.Prewave || currentState == WaveState.Break;
        //isActive = currentState == WaveState.Prewave || currentState == WaveState.Break;

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

    void UpdateTimerUI()
    {
        if (timerOn)
        {
            if (timerDurationRemaining > 0)
            {
                timerDurationRemaining -= Time.deltaTime;
                timerFill.fillAmount = 1 - (timerDurationRemaining / timerDuration);
                timerFill.enabled = true;
            }
        }
        else
        {
            timerFill.fillAmount = 0;
            timerFill.enabled = false;
        }
    }

    void UpdateInWaveProgressBar()
    {
        if (currentState == WaveState.InWave)
        {
            inWaveBarObject.SetActive(true);
            breakWaveBarObject.SetActive(false);
        }
        else
        {
            inWaveBarObject.SetActive(false);
            return;
        }

        MiniWave currentMiniWave = waves[currentWaveIndex].miniWaves[currentMiniWaveIndex];
        // Find the maximum timeUntilSpawningEnds across all spawners
        float maxDuration = Mathf.Max(
            currentMiniWave.timeUntilSpawningEnds1, currentMiniWave.timeUntilSpawningEnds2, currentMiniWave.timeUntilSpawningEnds3,
            currentMiniWave.timeUntilSpawningEnds4, currentMiniWave.timeUntilSpawningEnds5, currentMiniWave.timeUntilSpawningEnds6,
            currentMiniWave.timeUntilSpawningEnds7, currentMiniWave.timeUntilSpawningEnds8, currentMiniWave.timeUntilSpawningEnds9,
            currentMiniWave.timeUntilSpawningEnds10, currentMiniWave.timeUntilSpawningEnds11, currentMiniWave.timeUntilSpawningEnds12
        );

        float remainingDuration = Mathf.Max(
            inwaveTimer1, inwaveTimer2, inwaveTimer3, inwaveTimer4, inwaveTimer5, inwaveTimer6,
            inwaveTimer7, inwaveTimer8, inwaveTimer9, inwaveTimer10, inwaveTimer11, inwaveTimer12
        );

        //float remainingDuration = inwaveTimer1;

        inWaveProgressBar.fillAmount = remainingDuration / maxDuration;
    }

    void UpdateBreakProgressBar()
    {
        if (currentState == WaveState.Break)
        {
            breakWaveBarObject.SetActive(true);
            inWaveBarObject.SetActive(false);
        }
        else
        {
            breakWaveBarObject.SetActive(false);
            return;
        }

        float maxDuration = waves[currentWaveIndex].breakDuration;
        breakProgressBar.fillAmount = breakTimer / maxDuration;
    }

    float GetMaxInWaveDuration()
    {
        MiniWave currentMiniWave = waves[currentWaveIndex].miniWaves[currentMiniWaveIndex];
        return Mathf.Max(currentMiniWave.timeUntilSpawningEnds1, currentMiniWave.timeUntilSpawningEnds2, currentMiniWave.timeUntilSpawningEnds3, currentMiniWave.timeUntilSpawningEnds4, currentMiniWave.timeUntilSpawningEnds5, currentMiniWave.timeUntilSpawningEnds6, currentMiniWave.timeUntilSpawningEnds7, currentMiniWave.timeUntilSpawningEnds8, currentMiniWave.timeUntilSpawningEnds9, currentMiniWave.timeUntilSpawningEnds10, currentMiniWave.timeUntilSpawningEnds11, currentMiniWave.timeUntilSpawningEnds12);
    }

}
