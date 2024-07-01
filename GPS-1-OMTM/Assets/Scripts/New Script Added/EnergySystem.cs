using UnityEngine;
using UnityEngine.UI;

public class EnergySystem : MonoBehaviour
{
    [Header("Energy Settings")]
    public float maxEnergy = 100f; // Maximum energy value
    public float regenRate = 1f; // Energy regeneration rate per second

    public float currentEnergy;
    public Text energyText; // Reference to UI Text to display energy

    void Start()
    {
        Debug.Log(gameObject.name);
        currentEnergy = 50f;
    }

    void Update()
    {
        RegenerateEnergy();
        UpdateEnergyUI();
    }

    // Method to regenerate energy over time
    private void RegenerateEnergy()
    {
        if (currentEnergy < maxEnergy)
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
    private void UpdateEnergyUI()
    {
        if (energyText != null)
        {
            energyText.text = Mathf.FloorToInt(currentEnergy).ToString();
        }
    }
}