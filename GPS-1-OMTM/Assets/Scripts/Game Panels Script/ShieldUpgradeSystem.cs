using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldUpgradeSystem : MonoBehaviour
{
    public GameObject TroopType;
    public EnergySystem energySystem;
    public Text shieldTierText;
    public Text shieldCostText;

    private int shieldTier = 0;
    private int[] shieldCosts = { 300, 500, 900 };
    private int[] shieldValues = { 25, 50, 75 };

    public Button shieldUpgradeButton;
    public Button shieldUpgradePlusButton;
    public Button shieldUpgradeMinusButton;

    void Start()
    {
        energySystem = FindObjectOfType<EnergySystem>();

        shieldUpgradeButton.onClick.AddListener(() => UpgradeShield());
        shieldUpgradePlusButton.onClick.AddListener(() => ChangeTier(ref shieldTier, 1, 3, shieldTierText, shieldCostText, shieldCosts));
        shieldUpgradeMinusButton.onClick.AddListener(() => ChangeTier(ref shieldTier, -1, 3, shieldTierText, shieldCostText, shieldCosts));

        UpdateUI();
    }

    void ChangeTier(ref int tier, int change, int maxTier, Text tierText, Text costText, int[] costs)
    {
        tier = Mathf.Clamp(tier + change, 0, maxTier - 1);
        tierText.text = "Shield Tier: " + (tier + 1).ToString();
        costText.text = "Cost: " + costs[tier].ToString() + " Energy";
    }

    void UpgradeShield()
    {
        if (shieldTier < shieldCosts.Length && energySystem.currentEnergy >= shieldCosts[shieldTier])
        {
            energySystem.UseEnergy(shieldCosts[shieldTier]);
            // Apply the shield upgrade logic here
            Debug.Log("Shield upgraded to tier " + (shieldTier + 1) + " with value " + shieldValues[shieldTier]);
            shieldTier++;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        shieldTierText.text = "Shield Tier: " + (shieldTier + 1).ToString();
        shieldCostText.text = "Cost: " + shieldCosts[shieldTier].ToString() + " Energy";
    }
}
