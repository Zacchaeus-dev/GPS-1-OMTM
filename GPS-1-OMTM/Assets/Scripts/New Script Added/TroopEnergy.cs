using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TroopEnergy : MonoBehaviour
{
    public int maxEnergy;
    public int currentEnergy;
    public int energyGainAmount;
    public EnergyMethod energyMethod;
    public TextMeshProUGUI energyText;

    public enum EnergyMethod
    {
        DPS_CC_Healer, 
        Tank, 
    }

    void Start()
    {
        currentEnergy = 0;
        UpdateText();
    }

    public void UpdateText()
    {
        energyText.text = currentEnergy.ToString();
    }

    public void GainEnergy()
    {
        if (currentEnergy + energyGainAmount <= maxEnergy)
        {
            currentEnergy = currentEnergy + energyGainAmount;
        }

        UpdateText();
    }

    public void UseAllEnergy()
    {
        currentEnergy = 0;
        UpdateText();
    }
}
