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
    private GameObject dummy;
    public GameObject tutorialPanel;
    public GameObject objectivePanel;

    void Start()
    {
        if (tutorialOn)
        {
            TutorialStart();
        }
    }

    private void Update()
    {
        if (dummyEnemy.dummyDead == true && !respawning)
        {
            respawning = true;
            StartCoroutine(RespawnDummy());
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
    }

    public void TutorialEnd()
    {
        tank.SetActive(true);
        cc.SetActive(true);
        healer.SetActive(true);
        tankHUD.DimOff();
        ccHUD.DimOff();
        healerHUD.DimOff();
        kdd.SetActive(true);
        healerHUD.DimOff();
        tutorialOn = false; 
    }

    IEnumerator RespawnDummy()
    {
        yield return new WaitForSeconds(3f);

        //GameObject dummy = Instantiate(dummyPrefab, dummyTransform.position, Quaternion.identity);
        dummy.SetActive(true);
        dummyEnemy.currentHealth = dummyEnemy.maxHealth;
        respawning = false;
    }

    public void CloseTutorialPanel()
    {
        tutorialPanel.SetActive(false);
        objectivePanel.SetActive(true);
    }

    public void CloseObjectivePanel()
    {
        objectivePanel.SetActive(false);
        TutorialEnd();
    }
}
