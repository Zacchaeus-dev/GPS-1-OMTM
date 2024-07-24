using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TroopEnergy : MonoBehaviour
{
    public int maxPower;
    public int currentPower;
    public int powerGainAmount;
    public float powerPercent;
    public PowerMethod powerMethod;
    public TextMeshProUGUI powerText;
    private Troop troop;
    public TutorialPhase tutorialPhase;
    public GameObject instruction4;
    public GameObject instruction5;

    public enum PowerMethod
    {
        DPS_CC_Healer,
        Tank,
    }

    void Start()
    {
        currentPower = 0;
        troop = GetComponent<Troop>();
        UpdateHUD();
    }

    public void UpdateHUD()
    {
        powerText.text = currentPower.ToString();
        troop.UpdateHUD();
    }

    public void GainPower()
    {
        if (currentPower + powerGainAmount <= maxPower)
        {
            currentPower = currentPower + powerGainAmount;
        }

        Instruction5();

        UpdateHUD();
    }

    public void UseAllPower()
    {
        currentPower = 0;
        UpdateHUD();
    }

    public void Instruction5()
    {
        if (tutorialPhase != null && tutorialPhase.tutorialOn == true)
        {
            if (currentPower == maxPower)
            {
                instruction4.SetActive(false);
                instruction5.SetActive(true);
            }
        }
    }
}
