using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UltimateCostReductionUpgradeSystem : MonoBehaviour
{
    public GameObject TroopType;
    public EnergySystem energySystem;
    public Text ultimateCostReductionTierText;
    public Text ultimateCostReductionCostText;

    private int ultimateCostReductionTier = 0;
    private int[] ultimateCostReductionCosts = { 300, 600, 900 };
    private float[] ultimateCostReductions = { 0.05f, 0.10f, 0.15f };

    public Button ultimateCostReductionUpgradeButton;
    public Button ultimateCostReductionPlusButton;
    public Button ultimateCostReductionMinusButton;

    void Start()
    {
        energySystem = FindObjectOfType<EnergySystem>();

        ultimateCostReductionUpgradeButton.onClick.AddListener(() => UpgradeUltimateCostReduction());
        ultimateCostReductionPlusButton.onClick.AddListener(() => ChangeTier(ref ultimateCostReductionTier, 1, 3, ultimateCostReductionTierText, ultimateCostReductionCostText, ultimateCostReductionCosts));
        ultimateCostReductionMinusButton.onClick.AddListener(() => ChangeTier(ref ultimateCostReductionTier, -1, 3, ultimateCostReductionTierText, ultimateCostReductionCostText, ultimateCostReductionCosts));

        UpdateUI();
    }

    void ChangeTier(ref int tier, int change, int maxTier, Text tierText, Text costText, int[] costs)
    {
        tier = Mathf.Clamp(tier + change, 0, maxTier - 1);
        tierText.text = "Ultimate Cost Reduction Tier: " + (tier + 1).ToString();
        costText.text = "Cost: " + costs[tier].ToString() + " Energy";
    }

    void UpgradeUltimateCostReduction()
    {
        if (ultimateCostReductionTier < ultimateCostReductionCosts.Length && energySystem.currentEnergy >= ultimateCostReductionCosts[ultimateCostReductionTier])
        {
            energySystem.UseEnergy(ultimateCostReductionCosts[ultimateCostReductionTier]);
            // Apply the ultimate cost reduction logic here
            Debug.Log("Ultimate cost reduction upgraded to tier " + (ultimateCostReductionTier + 1) + " with reduction " + (ultimateCostReductions[ultimateCostReductionTier] * 100) + "%");
            ultimateCostReductionTier++;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        ultimateCostReductionTierText.text = "Ultimate Cost Reduction Tier: " + (ultimateCostReductionTier + 1).ToString();
        ultimateCostReductionCostText.text = "Cost: " + ultimateCostReductionCosts[ultimateCostReductionTier].ToString() + " Energy";
    }
}
