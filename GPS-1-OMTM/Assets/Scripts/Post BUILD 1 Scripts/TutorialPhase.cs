using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public GameObject instruction3;
    public GameObject instruction4;
    public GameObject instruction5;
    public GameObject instruction7;
    public GameObject labels;
    public GameObject wavesObject;

    void Start()
    {
        if (tutorialOn)
        {
            TutorialStart();
        }
    }

    private void Update()
    {
        if (waveSystem.wave1Started == true)
        {
            if (dummy != null)
            {
                Destroy(dummy);
            }

            return;
        }
        if (dummyEnemy != null)
        {
            if (dummyEnemy.dummyDead == true && !respawning)
            {
                if (!dummyDiedOnce)
                {
                    dummyDiedOnce = true;
                    instruction3.SetActive(false);
                    instruction4.SetActive(true);
                    labels.SetActive(false);
                }

                respawning = true;
                StartCoroutine(RespawnDummy());
            }
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
        //waveSystem.gameObject.SetActive(false);
    }

    public void TutorialEnd()
    {
        //waveSystem.gameObject.SetActive(true);
        tank.SetActive(true);
        cc.SetActive(true);
        healer.SetActive(true);
        tankHUD.DimOff();
        ccHUD.DimOff();
        healerHUD.DimOff();
        kdd.SetActive(true);
        healerHUD.DimOff();
        instruction5.SetActive(false);
        instruction7.SetActive(true);
        wavesObject.SetActive(true);
        waveSystem.TeleportTroopsToKilldozer();
        cameraSystem.FocusOnKilldozer();
        StartCoroutine(DefocusKilldozer());
        edgePanTutorial.SetActive(true);
        StartCoroutine(CloseEdgePanTutorial());
        tutorialOn = false;    
    }

    IEnumerator RespawnDummy()
    {
        yield return new WaitForSeconds(5f);

        //GameObject dummy = Instantiate(dummyPrefab, dummyTransform.position, Quaternion.identity);
        if (dummy != null)
        {
            dummy.SetActive(true);
            dummyEnemy.currentHealth = dummyEnemy.maxHealth;
        }

        respawning = false;
    }

    public void CloseTutorialPanel()
    {
        Time.timeScale = 1.0f;
        tutorialPanel.SetActive(false);
        objectivePanel.SetActive(true);
    }

    public void CloseObjectivePanel()
    {
        objectivePanel.SetActive(false);
        TutorialEnd();
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
}
