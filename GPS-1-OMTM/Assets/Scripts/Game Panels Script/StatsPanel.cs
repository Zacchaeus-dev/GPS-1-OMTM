using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsPanel : MonoBehaviour
{
    public TextMeshProUGUI maxHealthText;
    public TextMeshProUGUI currentHealthText; // This will be referenced elsewhere
    public TextMeshProUGUI attackDamageText;
    public TextMeshProUGUI attackSpeedText;
    public TextMeshProUGUI attackRangeText;
    public TextMeshProUGUI moveSpeedText;
    public Troop trphp;

    //private string selectedButton;
    public TroopStatsType TroopType;
    public void Awake()
    {
        if(TroopType == TroopStatsType.DPS)
        {
            DPS1();
        }
        else if(TroopType == TroopStatsType.TANK)
        {
            TANK1();
        }
        else if(TroopType == TroopStatsType.CC)
        {
            CC1();
        }
        else if(TroopType == TroopStatsType.HEALER)
        {
            HEALER1();
        }
        
        
        
    }
    public void Update()
    {
        if (TroopType == TroopStatsType.DPS)
        {
            currentHealthText.text = "Current Health: " + trphp.currentHealth.ToString();
    
        }
        else if (TroopType == TroopStatsType.TANK)
        {
            currentHealthText.text = "Current Health: " + trphp.currentHealth.ToString();
 
        }
        else if (TroopType == TroopStatsType.CC)
        {
            currentHealthText.text = "Current Health: " + trphp.currentHealth.ToString();

        }
        else if (TroopType == TroopStatsType.HEALER)
        {
            currentHealthText.text = "Current Health: " + trphp.currentHealth.ToString();

        }
 
    }
    public void DPS1()
    {
        maxHealthText.text = "MaxHealth: 150";
        attackDamageText.text = "Damage: 40";
        attackSpeedText.text = "Attack Speed: 4";
        attackRangeText.text = "Attack Range: 10";
        moveSpeedText.text = "Movement Speed: 8";
    }
    public void DPS2()
    {
        maxHealthText.text = "MaxHealth: 150";
        attackDamageText.text = "Damage: 35";
        attackSpeedText.text = "Attack Speed: 1";
        attackRangeText.text = "Attack Range: 10";
        moveSpeedText.text = "Movement Speed: 8";
    }
    public void TANK1()
    {
        maxHealthText.text = "MaxHealth: 1100";
        attackDamageText.text = "Damage: 20";
        attackSpeedText.text = "Attack Speed: 0.25";
        attackRangeText.text = "Attack Range: 4.5";
        moveSpeedText.text = "Movement Speed: 5";
    }
    public void TANK2()
    {
        maxHealthText.text = "MaxHealth: 750";
        attackDamageText.text = "Damage: 75";
        attackSpeedText.text = "Attack Speed: 0.5";
        attackRangeText.text = "Attack Range: 4.5";
        moveSpeedText.text = "Movement Speed: 7";
    }
    public void CC1()
    {
        maxHealthText.text = "MaxHealth: 200";
        attackDamageText.text = "Damage: 30";
        attackSpeedText.text = "Attack Speed: 1";
        attackRangeText.text = "Attack Range: 10";
        moveSpeedText.text = "Movement Speed: 7";
    }
    public void CC2()
    {
        maxHealthText.text = "MaxHealth: 200";
        attackDamageText.text = "Damage: 75";
        attackSpeedText.text = "Attack Speed: 0.125";
        attackRangeText.text = "Attack Range: 10";
        moveSpeedText.text = "Movement Speed: 7";
    }
    public void HEALER1()
    {
        maxHealthText.text = "MaxHealth: 100";
        attackDamageText.text = "Heal: 50";
        attackSpeedText.text = "Attack Speed: 0.5";
        attackRangeText.text = "Attack Range: 10";
        moveSpeedText.text = "Movement Speed: 10";
    }
    public void HEALER2()
    {
        maxHealthText.text = "MaxHealth: 100";
        attackDamageText.text = "Heal: 20";
        attackSpeedText.text = "Attack Speed: 0.125";
        attackRangeText.text = "Attack Range: 10";
        moveSpeedText.text = "Movement Speed: 10";
    }
    

}
public enum TroopStatsType
{
    DPS,
    TANK,
    CC,
    HEALER
}

