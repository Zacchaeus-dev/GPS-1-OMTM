using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MoveSpeedUpgradeSystem : MonoBehaviour
{
    public GameObject TroopType;
    public EnergySystem energySystem;
    public TroopClass troopClassScript;
    public Text moveSpeedTierText;
    public Text moveSpeedCostText;
    public Text energyText;
    public StatsScript statsScript;


    public int moveSpeedTier = 0;
    public int[] moveSpeedCosts = { 200, 300, 400, 500, 600 };
    public int[] moveSpeedValues = { 2, 1, 2 };

    public Button moveSpeedPlusButton;
    public Button moveSpeedMinusButton;


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
        troopClassScript = TroopType.GetComponent<TroopClass>();
        statsScript = FindObjectOfType<StatsScript>();

        moveSpeedPlusButton.onClick.AddListener(() => BuyUpgrade(ref moveSpeedTier, moveSpeedCosts, moveSpeedTierText, moveSpeedCostText));
        moveSpeedMinusButton.onClick.AddListener(() => SellUpgrade(ref moveSpeedTier, moveSpeedCosts, moveSpeedTierText, moveSpeedCostText));

        UpdateUI();
    }

    void BuyUpgrade(ref int tier, int[] costs, Text tierText, Text costText)
    {
        if (tier < costs.Length && energySystem.currentEnergy >= costs[tier])
        {
            energySystem.UseEnergy(costs[tier]);
            troopClassScript.moveSpeed += moveSpeedValues[tier];
            Debug.Log("Move speed upgraded to tier " + (tier + 1) + " with value " + moveSpeedValues[tier]);
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
            troopClassScript.moveSpeed -= moveSpeedValues[tier];
            Debug.Log("Move speed downgraded to tier " + (tier + 1));
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
        moveSpeedTierText.text = "Move Speed Tier: " + (moveSpeedTier + 1).ToString();
        moveSpeedCostText.text = moveSpeedTier < moveSpeedCosts.Length ? "Cost: " + moveSpeedCosts[moveSpeedTier].ToString() + " Energy" : "Max Tier Reached";

        bool canUpgrade = moveSpeedTier < moveSpeedCosts.Length && energySystem.currentEnergy >= moveSpeedCosts[moveSpeedTier];
        moveSpeedPlusButton.interactable = canUpgrade;
        moveSpeedMinusButton.interactable = moveSpeedTier > 0;

        Debug.Log("Move speed upgrade button status updated. Interactable: " + canUpgrade);

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
