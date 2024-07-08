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
    public Text totalenergy;
    //public Text maxHealthKDText;



    // Start is called before the first frame update
    void Start()
    {
        KDScript = KD.GetComponent<Killdozer>();
        energy = EnergySystem.GetComponent<EnergySystem>();

        UpdateUI();
    }

    // Update is called once per frame
    public void UpdateUI()
    {
      
        //maxHealthKD.text = "Max Health: " + KDScript.maxHealth.ToString();
        totalenergy.text = "TotalEnergy: " + energy.currentEnergy.ToString();
    }
}
