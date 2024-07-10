using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyRegenUpgradeSystem : MonoBehaviour
{
    public EnergySystem energySystem;
    public Text energyRegenTierText;
    public Text energyRegenCostText;
    public Text energyRegenRateText;
    public Text energyText;
    public Button energyRegenPlusButton;
    public Button energyRegenMinusButton;

    private int energyRegenTier = 0;
    private int[] energyRegenCosts = { 2000, 4000, 6000 };
    private int[] energyRegenRates = { 15, 20, 25 };
    private int baseRegenRate = 10;

    void Start()
    {
        energySystem = FindObjectOfType<EnergySystem>();

        energyRegenPlusButton.onClick.AddListener(() => BuyUpgrade(ref energyRegenTier, energyRegenCosts, energyRegenTierText, energyRegenCostText, energyRegenRateText));
        energyRegenMinusButton.onClick.AddListener(() => SellUpgrade(ref energyRegenTier, energyRegenCosts, energyRegenTierText, energyRegenCostText, energyRegenRateText));

        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    void BuyUpgrade(ref int tier, int[] costs, Text tierText, Text costText, Text rateText)
    {
        if (tier < costs.Length && energySystem.currentEnergy >= costs[tier])
        {
            energySystem.UseEnergy(costs[tier]);
            energySystem.regenRate = baseRegenRate + energyRegenRates[tier];
            Debug.Log("Energy regeneration upgraded to tier " + (tier + 1) + " with rate " + energyRegenRates[tier] + " per second");
            tier++;
            UpdateUI();
        }
    }

    void SellUpgrade(ref int tier, int[] costs, Text tierText, Text costText, Text rateText)
    {
        if (tier > 0)
        {
            tier--;
            energySystem.currentEnergy += costs[tier];
            energySystem.regenRate = baseRegenRate + (tier > 0 ? energyRegenRates[tier - 1] : 0);
            Debug.Log("Energy regeneration downgraded to tier " + (tier + 1));
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        energyRegenTierText.text = "Energy Regen Tier: " + (energyRegenTier + 1).ToString();
        energyRegenCostText.text = energyRegenTier < energyRegenCosts.Length ? "Cost: " + energyRegenCosts[energyRegenTier].ToString() + " Energy" : "Max Tier Reached";
        energyRegenRateText.text = "Regen Rate: " + energySystem.regenRate.ToString() + " Energy/sec";
        energyText.text = "Current Energy: " + energySystem.currentEnergy.ToString();

        bool canUpgrade = energyRegenTier < energyRegenCosts.Length && energySystem.currentEnergy >= energyRegenCosts[energyRegenTier];
        energyRegenPlusButton.interactable = canUpgrade;
        energyRegenMinusButton.interactable = energyRegenTier > 0;

        Debug.Log("Energy regen upgrade button status updated. Interactable: " + canUpgrade);
    }
}
