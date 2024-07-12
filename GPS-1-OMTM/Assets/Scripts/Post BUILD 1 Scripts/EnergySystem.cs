using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnergySystem : MonoBehaviour
{
    [Header("Energy Settings")]
    public float maxEnergy = 5000f; // Maximum energy value
    public float regenRate = 1f; // Energy regeneration rate per second

    public float currentEnergy;
    public Text energyText; // Reference to UI Text to display energy
    public Text energyTextDPS;
    public Text energyTextTank;
    public Text energyTextCC;
    public Text energyTextHealer;
    public Text energyTextKD;

    public int energyAmount = 10; // Amount of energy to add to the troop

    public WaveSystem waveSystem;

    void Start()
    {
        currentEnergy = 0f;
        //StartCoroutine(RegenerateEnergy());
    }

    void Update()
    {
        RegenerateEnergy();
        UpdateEnergyUI();
    }

    /*
    IEnumerator RegenerateEnergy()
    {
        while (waveSystem.currentState == WaveSystem.WaveState.InWave || waveSystem.currentState == WaveSystem.WaveState.Break))
        {
            currentEnergy = currentEnergy + 1;
            yield return new WaitForSeconds(0.1f);
        }
        
    }
    */

    // Method to regenerate energy over time
    private void RegenerateEnergy()
    {
        if (currentEnergy < maxEnergy && (waveSystem.currentState == WaveSystem.WaveState.InWave || waveSystem.currentState == WaveSystem.WaveState.Break))
        {
            currentEnergy += regenRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy); // Ensure energy does not exceed maximum
        }
    }

    // Method to use energy, returns true if there was enough energy to use, false otherwise
    public bool UseEnergy(float amount)
    {
        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;
            return true;
        }
        return false;
    }

    // Method to get the current energy value
    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }

    // Method to set the current energy value
    public void SetCurrentEnergy(float energy)
    {
        currentEnergy = Mathf.Clamp(energy, 0f, maxEnergy); // Ensure energy stays within valid range
    }

    // Method to get the maximum energy value
    public float GetMaxEnergy()
    {
        return maxEnergy;
    }

    // Method to set the maximum energy value
    public void SetMaxEnergy(float maxEnergy)
    {
        this.maxEnergy = maxEnergy;
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy); // Adjust current energy if max changes
    }

    // Method to update the UI with the current energy value
    public void UpdateEnergyUI()
    {
        if (energyText != null || energyTextDPS != null || energyTextTank != null || energyTextCC != null || energyTextHealer != null || energyTextKD != null )
        {
            energyText.text = Mathf.FloorToInt(currentEnergy).ToString();
            energyTextDPS.text = Mathf.FloorToInt(currentEnergy).ToString();
            energyTextTank.text = Mathf.FloorToInt(currentEnergy).ToString();
            energyTextCC.text = Mathf.FloorToInt(currentEnergy).ToString();
            energyTextHealer.text = Mathf.FloorToInt(currentEnergy).ToString();
            energyTextKD.text = Mathf.FloorToInt(currentEnergy).ToString();
        }
    }

    
}
