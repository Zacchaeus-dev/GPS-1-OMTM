using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyOrb : MonoBehaviour
{
    public int energyAmount = 10; // Amount of energy to add to the EnergySystem

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Troop"))
        {
            TroopEnergy otherTroopEnergy = other.GetComponent<TroopEnergy>();
            if (otherTroopEnergy != null)
            {
                // Add energy to the troop
                otherTroopEnergy.currentPower += energyAmount;
                otherTroopEnergy.UpdateText();

                // Optionally, play a sound, deactivate the energy orb, etc.
                Destroy(gameObject); // Destroy the energy orb after collision
            }

            // Check if the EnergySystem component exists on the other GameObject
            EnergySystem energySystem = FindObjectOfType<EnergySystem>(); // Find EnergySystem in the scene
            if (energySystem != null)
            {
                // Add energy to the EnergySystem
                energySystem.currentEnergy += energyAmount;
                energySystem.currentEnergy = Mathf.Clamp(energySystem.currentEnergy, 0f, energySystem.maxEnergy);

                // Optionally, update the UI directly if needed
                if (energySystem.energyText != null)
                {
                    energySystem.energyText.text = Mathf.FloorToInt(energySystem.currentEnergy).ToString();
                }
            }

            // Optionally, play a sound, deactivate the energy orb, etc.
            Destroy(gameObject); // Destroy the energy orb after collision with troop
        }
    }
}
