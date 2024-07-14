using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropEnergyOrbOnDeath : MonoBehaviour
{
    public GameObject energyOrbPrefab; // Energy orb prefab to spawn

    private void Start()
    {
        // Ensure energyOrbPrefab is set in the Inspector
        if (energyOrbPrefab == null)
        {
            Debug.LogError("Energy Orb Prefab is not assigned in DropEnergyOrbOnDeath script on " + gameObject.name);
        }
    }

    public void DropEnergyOrb()
    {
        if (energyOrbPrefab != null)
        {
            // Spawn the energy orb at the enemy's position
            GameObject energyOrb = Instantiate(energyOrbPrefab, transform.position, Quaternion.identity);

            // Optionally add any other behavior or components to the energy orb GameObject here
        }
        else
        {
            Debug.LogWarning("Energy Orb Prefab is not assigned in DropEnergyOrbOnDeath script on " + gameObject.name);
        }
    }

    /*
    void OnDestroy()
    {
        // This function is called when the GameObject is destroyed (when the enemy is killed)
        DropEnergyOrb();
    }
    */
}
