using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KDStatsScript : MonoBehaviour
{
    public GameObject KD;
    public GameObject EnergySystem;
    public Killdozer KDScript;
    public EnergySystem energy;

    public Text maxHealthKD;
    public Text totalEnergy;
    public Text currentHealthKD;

    void Start()
    {
        KDScript = KD.GetComponent<Killdozer>();
        energy = EnergySystem.GetComponent<EnergySystem>();

        UpdateUI();
    }

    public void UpdateUI()
    {     
        maxHealthKD.text = "Max Health: " + KDScript.maxHealth.ToString();
        currentHealthKD.text = "Current Health: " + KDScript.currentHealth.ToString();

        int CurrentEnergy = (int)energy.currentEnergy; //show energy in int
        totalEnergy.text = "Total Energy: " + CurrentEnergy.ToString();
    }
}
