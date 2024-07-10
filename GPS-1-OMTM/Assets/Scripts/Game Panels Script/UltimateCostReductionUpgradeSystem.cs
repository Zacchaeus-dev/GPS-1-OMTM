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
    public Text energyText;
    public StatsScript statsScript;
    public TroopClass troopClassScript;

    private int ultimateCostReductionTier = 0;
    private int[] ultimateCostReductionCosts = { 300, 600, 900 };
    private float[] ultimateCostReductions = { 0.05f, 0.10f, 0.15f };

    public Button ultimateCostReductionPlusButton;
    public Button ultimateCostReductionMinusButton;


    public TroopAutoAttack troopAAScript;
    public HealerAutoHeal healerAHScript;
    public Troop troopScript;
    public Text maxHealthText;
    public Text currentHealthText;
    public Text attackText;
    public Text attackSpeedText;
    public Text attackRangeText;
    public Text moveSpeedText;

    public bool isHealer;

    void Start()
    {
        energySystem = FindObjectOfType<EnergySystem>();
        statsScript = FindObjectOfType<StatsScript>();

        ultimateCostReductionPlusButton.onClick.AddListener(() => BuyUpgrade(ref ultimateCostReductionTier, ultimateCostReductionCosts, ultimateCostReductionTierText, ultimateCostReductionCostText));
        ultimateCostReductionMinusButton.onClick.AddListener(() => SellUpgrade(ref ultimateCostReductionTier, ultimateCostReductionCosts, ultimateCostReductionTierText, ultimateCostReductionCostText));

        UpdateUI();
    }

    void BuyUpgrade(ref int tier, int[] costs, Text tierText, Text costText)
    {
        if (tier < costs.Length && energySystem.currentEnergy >= costs[tier])
        {
            energySystem.UseEnergy(costs[tier]);
            // Apply the ultimate cost reduction logic here
            Debug.Log("Ultimate cost reduction upgraded to tier " + (tier + 1) + " with reduction " + (ultimateCostReductions[tier] * 100) + "%");
            tier++;
            UpdateUI();
        }
    }

    void SellUpgrade(ref int tier, int[] costs, Text tierText, Text costText)
    {
        if (tier > 0)
        {
            tier--;
            energySystem.currentEnergy += costs[tier];
            Debug.Log("Ultimate cost reduction downgraded to tier " + (tier + 1));
            UpdateUI();
        }
    }

    public void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        ultimateCostReductionTierText.text = "Ultimate Cost Reduction Tier: " + (ultimateCostReductionTier + 1).ToString();
        ultimateCostReductionCostText.text = ultimateCostReductionTier < ultimateCostReductionCosts.Length ? "Cost: " + ultimateCostReductionCosts[ultimateCostReductionTier].ToString() + " Energy" : "Max Tier Reached";

        bool canUpgrade = ultimateCostReductionTier < ultimateCostReductionCosts.Length && energySystem.currentEnergy >= ultimateCostReductionCosts[ultimateCostReductionTier];
        ultimateCostReductionPlusButton.interactable = canUpgrade;
        ultimateCostReductionMinusButton.interactable = ultimateCostReductionTier > 0;

        Debug.Log("Ultimate cost reduction button status updated. Interactable: " + canUpgrade);


        if (!isHealer)
        {
            maxHealthText.text = "Max Health: " + troopScript.maxHealth.ToString();
            currentHealthText.text = "Current Health: " + troopScript.currentHealth.ToString();
            attackText.text = "Attack Damage: " + troopAAScript.attackDamage.ToString();
            attackSpeedText.text = "Attack Speed: " + troopAAScript.attackCooldown.ToString();
            attackRangeText.text = "Attack Range: " + troopAAScript.attackRange.ToString();
            moveSpeedText.text = "Movement Speed: " + troopClassScript.moveSpeed.ToString();
        }
        else
        {
            maxHealthText.text = "Max Health: " + troopScript.maxHealth.ToString();
            currentHealthText.text = "Current Health: " + troopScript.currentHealth.ToString();
            attackText.text = "Heal Amount: " + healerAHScript.healAmount.ToString();
            attackSpeedText.text = "Heal Speed: " + healerAHScript.healCooldown.ToString();
            attackRangeText.text = "Heal Range: " + healerAHScript.healRange.ToString();
            moveSpeedText.text = "Movement Speed: " + troopClassScript.moveSpeed.ToString();
        }
    }
}
