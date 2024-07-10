using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RepairSystem : MonoBehaviour
{
    public Killdozer KDScript; // Reference to the Killdozer script
    public EnergySystem energySystem; // Reference to the EnergySystem script

    public Text repairCostText; // UI Text to display repair cost
    public Button repairButton; // UI Button for repair action

    public int repairCost = 400; // Cost of repairing 100 health

    void Start()
    {
        energySystem = FindObjectOfType<EnergySystem>();
        KDScript = FindObjectOfType<Killdozer>();

        repairButton.onClick.AddListener(Repair);

        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    void Repair()
    {
        if (energySystem.currentEnergy >= repairCost && KDScript.currentHealth < KDScript.maxHealth)
        {
            energySystem.UseEnergy(repairCost); // Deduct energy for repair
            KDScript.currentHealth = Mathf.Min(KDScript.maxHealth, KDScript.currentHealth + 100); // Repair 100 health, but not exceed max health

            Debug.Log("Repair successful. Current Health: " + KDScript.currentHealth);
            UpdateUI();
        }
        else
        {
            Debug.Log("Repair failed. Not enough energy or Killdozer is at max health.");
        }
    }

    void UpdateUI()
    {
        repairCostText.text = "Repair Cost: " + repairCost.ToString() + " Energy";
        bool canRepair = energySystem.currentEnergy >= repairCost && KDScript.currentHealth < KDScript.maxHealth;
        repairButton.interactable = canRepair;

        // Additional debug logs to understand why the button is disabled
        if (energySystem.currentEnergy < repairCost)
        {
            Debug.Log("Repair button disabled due to insufficient energy. Current Energy: " + energySystem.currentEnergy);
        }
        if (KDScript.currentHealth >= KDScript.maxHealth)
        {
            Debug.Log("Repair button disabled because Killdozer is at max health. Current Health: " + KDScript.currentHealth + ", Max Health: " + KDScript.maxHealth);
        }

        Debug.Log("Repair button status updated. Interactable: " + canRepair);
    }
}

