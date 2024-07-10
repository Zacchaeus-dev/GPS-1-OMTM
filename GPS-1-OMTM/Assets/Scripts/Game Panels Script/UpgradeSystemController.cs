/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSystemController : MonoBehaviour
{
    public ShieldUpgradeSystem shieldUpgradeSystem;
    public UltimateCostReductionUpgradeSystem ultimateCostReductionUpgradeSystem;
    public MoveSpeedUpgradeSystem moveSpeedUpgradeSystem;

    public Button buyButton;

    private enum UpgradeType { Shield, UltimateCostReduction, MoveSpeed }
    private UpgradeType selectedUpgradeType;

    void Start()
    {
        buyButton.onClick.AddListener(() => BuyUpgrade());
        SelectShieldUpgrade();
    }

    public void SelectShieldUpgrade()
    {
        selectedUpgradeType = UpgradeType.Shield;
    }

    public void SelectUltimateCostReductionUpgrade()
    {
        selectedUpgradeType = UpgradeType.UltimateCostReduction;
    }

    public void SelectMoveSpeedUpgrade()
    {
        selectedUpgradeType = UpgradeType.MoveSpeed;
    }

    void BuyUpgrade()
    {
        switch (selectedUpgradeType)
        {
            case UpgradeType.Shield:
                shieldUpgradeSystem.UpgradeShield();
                break;
            case UpgradeType.UltimateCostReduction:
                ultimateCostReductionUpgradeSystem.UpgradeUltimateCostReduction();
                break;
            case UpgradeType.MoveSpeed:
                moveSpeedUpgradeSystem.UpgradeMoveSpeed();
                break;
        }
    }
}
*/