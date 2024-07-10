using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.MaterialProperty;

public class ShieldUpgradeSystem : MonoBehaviour
{
    public GameObject TroopType;
    public EnergySystem energySystem;
    public Text shieldTierText;
    public Text shieldCostText;
    public Text energyText;
    public StatsScript statsScript;
    public TroopClass troopClassScript;

    private int shieldTier = 0;
    private int[] shieldCosts = { 300, 500, 900 };
    private int[] shieldValues = { 25, 50, 75 };

    public Button shieldUpgradePlusButton;
    public Button shieldUpgradeMinusButton;



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

        shieldUpgradePlusButton.onClick.AddListener(() => BuyUpgrade(ref shieldTier, shieldCosts, shieldTierText, shieldCostText));
        shieldUpgradeMinusButton.onClick.AddListener(() => SellUpgrade(ref shieldTier, shieldCosts, shieldTierText, shieldCostText));

        UpdateUI();
    }

    void BuyUpgrade(ref int tier, int[] costs, Text tierText, Text costText)
    {
        if (tier < costs.Length && energySystem.currentEnergy >= costs[tier])
        {
            energySystem.UseEnergy(costs[tier]);
            // Apply the shield upgrade logic here
            Debug.Log("Shield upgraded to tier " + (tier + 1) + " with value " + shieldValues[tier]);
            tier++;
            UpdateUI();
            statsScript.UpdateUI();

        }
    }

    void SellUpgrade(ref int tier, int[] costs, Text tierText, Text costText)
    {
        if (tier > 0)
        {
            tier--;
            energySystem.currentEnergy += costs[tier];
            Debug.Log("Shield downgraded to tier " + (tier + 1));
            UpdateUI();
            statsScript.UpdateUI();

        }
    }

    public void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        shieldTierText.text = "Shield Tier: " + (shieldTier + 1).ToString();
        shieldCostText.text = shieldTier < shieldCosts.Length ? "Cost: " + shieldCosts[shieldTier].ToString() + " Energy" : "Max Tier Reached";

        bool canUpgrade = shieldTier < shieldCosts.Length && energySystem.currentEnergy >= shieldCosts[shieldTier];
        shieldUpgradePlusButton.interactable = canUpgrade;
        shieldUpgradeMinusButton.interactable = shieldTier > 0;

        Debug.Log("Shield upgrade button status updated. Interactable: " + canUpgrade);

  

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
