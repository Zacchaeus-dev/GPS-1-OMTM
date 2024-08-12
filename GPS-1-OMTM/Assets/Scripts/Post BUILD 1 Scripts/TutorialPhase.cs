using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Build;
using UnityEngine;
using static WaveSystem;

public class TutorialPhase : MonoBehaviour
{
    public bool tutorialOn;
    public GameObject tank;
    public GameObject cc;
    public GameObject healer;
    public TroopHUD tankHUD;
    public TroopHUD ccHUD;
    public TroopHUD healerHUD;
    public GameObject kdd;
    public CameraSystem cameraSystem;
    public GameObject dummyPrefab;
    public Transform dummyTransform;
    public Enemy dummyEnemy;
    private bool respawning;
    private bool dummyDiedOnce;
    private GameObject dummy;
    public WaveSystem waveSystem;
    public GameObject tutorialPanel;
    public GameObject objectivePanel;
    public GameObject edgePanTutorial;
    public GameObject tutorialInstructions;
    public GameObject instruction1;
    public GameObject instruction2;
    public GameObject instruction3;
    public GameObject instruction4;
    public GameObject instruction4A;
    public GameObject instruction5;
    public GameObject instruction7;
    public GameObject instruction8;
    public bool instruction8On;
    public GameObject labels;
    public GameObject kddButton;
    public GameObject wavesObject;
    public GameObject rightClickPosition;
    public GameObject wavePopUp;
    public GameObject preWave1Screen;
    public GameObject skipTutorialButton;
    private bool tutorialSkipped;
    private bool canRespawn = true;
    public TroopEnergy dpsPower;

    void Start()
    {
        if (tutorialOn)
        {
            TutorialStart();
        }
    }

    private void Update()
    {
        /*
        if (waveSystem.wave1Started == true)
        {
            if (dummy != null)
            {
                Destroy(dummy);
            }

            return;
        }
        */

        if (waveSystem.currentState == WaveSystem.WaveState.Start && tutorialOn == false)
        {
            if (dummy != null)
            {
                Destroy(dummy);
            }

            return;
        }

        if (dummyEnemy != null)
        {
            if (dummyEnemy.dummyDead == true && !respawning && tutorialOn == true && canRespawn == true)
            {
                if (!dummyDiedOnce)
                {
                    dummyDiedOnce = true;
                    instruction3.SetActive(false);
                    instruction4.SetActive(true);
                    labels.SetActive(false);

                    StartCoroutine(Instruction4ADelay());
                }

                respawning = true;
                StartCoroutine(RespawnDummy());
            }
        }
    }

    IEnumerator Instruction4ADelay()
    {
        yield return new WaitForSeconds(3f);

        if(tutorialSkipped == false)
        {
            instruction4A.SetActive(true);
            kddButton.SetActive(true);
        }
        else
        {
            Debug.Log("a");
        }
    }

    public void TutorialStart()
    {
        tank.SetActive(false);
        cc.SetActive(false);
        healer.SetActive(false);
        tankHUD.DimOn();
        ccHUD.DimOn();
        healerHUD.DimOn();
        kdd.SetActive(false);
        healerHUD.DimOn();
        dummy = Instantiate(dummyPrefab, dummyTransform.position, Quaternion.identity);
        dummyEnemy = dummy.GetComponent<Enemy>();
        tutorialInstructions.SetActive(true);
        instruction1.SetActive(true);
        labels.SetActive(true);
        wavesObject.SetActive(false);
        rightClickPosition.SetActive(true);
        skipTutorialButton.SetActive(true);
        //waveSystem.gameObject.SetActive(false);
    }

    IEnumerator TutorialEnd()
    {
        //waveSystem.gameObject.SetActive(true);
        DestroyObjectsWithTag("EnergyOrb");
        tank.SetActive(true);
        cc.SetActive(true);
        healer.SetActive(true);
        tankHUD.DimOff();
        ccHUD.DimOff();
        healerHUD.DimOff();
        kdd.SetActive(true);
        healerHUD.DimOff();
        instruction4.SetActive(false); //prevent bug
        instruction5.SetActive(false);
        wavesObject.SetActive(true);
        waveSystem.TeleportTroopsToKilldozer();
        cameraSystem.FocusOnKilldozer();
        StartCoroutine(DefocusKilldozer());
        instruction8.SetActive(true);
        instruction8On = true;
        dpsPower.UseAllPower();
        dpsPower.DisableUltimateVisual();
        tutorialOn = false;

        yield return new WaitForSeconds(0.1f);
    }

    public void EnableEdgePanTutorial()
    {
        instruction8On = false;
        instruction7.SetActive(true);
        edgePanTutorial.SetActive(true);
        StartCoroutine(CloseEdgePanTutorial());
    }

    void DestroyObjectsWithTag(string tag)
    {
        GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objectsToDestroy)
        {
            Destroy(obj);
        }
    }

    public void KillDummy()
    {
        canRespawn = false;
        if (dummy != null)
        {
            dummy.SetActive(false);
        }
    }

    IEnumerator RespawnDummy()
    {
        yield return new WaitForSeconds(5f);

        //GameObject dummy = Instantiate(dummyPrefab, dummyTransform.position, Quaternion.identity);
        if (dummy != null && canRespawn == true)
        {
            dummy.SetActive(true);
            dummyEnemy.currentHealth = dummyEnemy.maxHealth;
        }

        respawning = false;
    }

    public void CloseInstruction3()
    {
        Time.timeScale = 1.0f;
        instruction3.SetActive(false);
        skipTutorialButton.SetActive(true);
        FindObjectOfType<AudioManager>().Play("button");
    }

    public void CloseTutorialPanel()
    {
        Time.timeScale = 1.0f;
        tutorialPanel.SetActive(false);
        //show pre wave 1 pop up
        preWave1Screen.SetActive(true);
        wavePopUp.SetActive(true);

        instruction5.SetActive(false); //prevent bug
        StartCoroutine(WaveAnimation());
        //StartCoroutine(TutorialEnd());
        FindObjectOfType<AudioManager>().Play("button");
    }

    IEnumerator WaveAnimation()
    {
        skipTutorialButton.SetActive(false);

        yield return new WaitForSeconds(3f); //animation duration

        instruction5.SetActive(false); //prevent bug
        wavePopUp.SetActive(false);
        preWave1Screen.SetActive(false);
        StartCoroutine(TutorialEnd());
    }

    public void CloseObjectivePanel()
    {
        objectivePanel.SetActive(false);
        tutorialPanel.SetActive(true);
        FindObjectOfType<AudioManager>().Play("button");
    }

    IEnumerator DefocusKilldozer()
    {
        yield return new WaitForSeconds(0.5f);

        cameraSystem.DefocusKilldozer();
    }

    IEnumerator CloseEdgePanTutorial()
    {
        yield return new WaitForSeconds(10f);

        edgePanTutorial.SetActive(false);
        instruction7.SetActive(false);
    }

    public void SkipTutorial()
    {
        FindObjectOfType<AudioManager>().Play("button");
        KillDummy();
        rightClickPosition.SetActive(false);
        instruction1.SetActive(false);
        instruction2.SetActive(false);
        instruction3.SetActive(false);
        instruction4.SetActive(false);
        instruction4A.SetActive(false);
        instruction5.SetActive(false);
        instruction7.SetActive(false);
        instruction8.SetActive(false);
        labels.SetActive(false);
        dpsPower.UseAllPower();
        tutorialSkipped = true;

        preWave1Screen.SetActive(true);
        wavePopUp.SetActive(true);
        StartCoroutine(WaveAnimation());
    }
}
