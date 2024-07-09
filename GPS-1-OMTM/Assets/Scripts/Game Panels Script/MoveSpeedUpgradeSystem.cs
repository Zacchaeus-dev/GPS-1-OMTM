using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveSpeedUpgradeSystem : MonoBehaviour
{
    public GameObject TroopType;
    public EnergySystem energySystem;
    public TroopClass troopClassScript;
    public Text moveSpeedTierText;
    public Text moveSpeedCostText;

    private int moveSpeedTier = 0;
    private int[] moveSpeedCosts = { 200, 300, 400, 500, 600 };
    private int[] moveSpeedValues = { 10, 20, 30, 40, 50 };

    public Button moveSpeedUpgradeButton;
    public Button moveSpeedPlusButton;
    public Button moveSpeedMinusButton;

    void Start()
    {
        energySystem = FindObjectOfType<EnergySystem>();
        troopClassScript = TroopType.GetComponent<TroopClass>();

        moveSpeedUpgradeButton.onClick.AddListener(() => UpgradeMoveSpeed());
        moveSpeedPlusButton.onClick.AddListener(() => ChangeTier(ref moveSpeedTier, 1, 5, moveSpeedTierText, moveSpeedCostText, moveSpeedCosts));
        moveSpeedMinusButton.onClick.AddListener(() => ChangeTier(ref moveSpeedTier, -1, 5, moveSpeedTierText, moveSpeedCostText, moveSpeedCosts));

        UpdateUI();
    }

    void ChangeTier(ref int tier, int change, int maxTier, Text tierText, Text costText, int[] costs)
    {
        tier = Mathf.Clamp(tier + change, 0, maxTier - 1);
        tierText.text = "Move Speed Tier: " + (tier + 1).ToString();
        costText.text = "Cost: " + costs[tier].ToString() + " Energy";
    }

    void UpgradeMoveSpeed()
    {
        if (moveSpeedTier < moveSpeedCosts.Length && energySystem.currentEnergy >= moveSpeedCosts[moveSpeedTier])
        {
            energySystem.UseEnergy(moveSpeedCosts[moveSpeedTier]);
            troopClassScript.moveSpeed += moveSpeedValues[moveSpeedTier];
            Debug.Log("Move speed upgraded to tier " + (moveSpeedTier + 1) + " with value " + moveSpeedValues[moveSpeedTier]);
            moveSpeedTier++;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        moveSpeedTierText.text = "Move Speed Tier: " + (moveSpeedTier + 1).ToString();
        moveSpeedCostText.text = "Cost: " + moveSpeedCosts[moveSpeedTier].ToString() + " Energy";
    }
}
