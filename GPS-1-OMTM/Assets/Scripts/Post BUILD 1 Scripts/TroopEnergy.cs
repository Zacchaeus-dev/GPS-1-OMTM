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

        UpdateHUD();
    }

    public void UseAllPower()
    {
        currentPower = 0;
        UpdateHUD();
    }

    public void GetPowerPercentage()
    {
        powerPercent = (currentPower / maxPower * 100f);
    }
}
