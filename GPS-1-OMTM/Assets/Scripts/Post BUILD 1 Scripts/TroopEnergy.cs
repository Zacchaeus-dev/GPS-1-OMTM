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

    public enum PowerMethod
    {
        DPS_CC_Healer,
        Tank,
    }

    void Start()
    {
        currentPower = 0;
        UpdateText();
    }

    public void UpdateText()
    {
        powerText.text = currentPower.ToString();
    }

    public void GainPower()
    {
        if (currentPower + powerGainAmount <= maxPower)
        {
            currentPower = currentPower + powerGainAmount;
        }

        UpdateText();
    }

    public void UseAllPower()
    {
        currentPower = 0;
        UpdateText();
    }

    public void GetPowerPercentage()
    {
        powerPercent = (currentPower / maxPower * 100f);
    }
}
