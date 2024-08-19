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
        maxHealthText.text = "Health :" +
            " 150";
        attackDamageText.text = "Damage : 40";
        attackSpeedText.text = "Atk Speed : 8";
        attackRangeText.text = "Atk Range : 100";
        moveSpeedText.text = "Move Spd : 8";
    }
    public void DPS2()
    {
        maxHealthText.text = "Health : 150";
        attackDamageText.text = "Damage : 35";
        attackSpeedText.text = "Atk Speed : 2";
        attackRangeText.text = "Atk Range : 100";
        moveSpeedText.text = "Move Spd : 8";
    }
    public void TANK1()
    {
        maxHealthText.text = "Health : 1100";
        attackDamageText.text = "Damage : 20";
        attackSpeedText.text = "Atk Speed : 20";
        attackRangeText.text = "Atk Range : 45";
        moveSpeedText.text = "Move Spd : 5";
    }
    public void TANK2()
    {
        maxHealthText.text = "Health : 750";
        attackDamageText.text = "Damage : 75";
        attackSpeedText.text = "Atk Speed : 15";
        attackRangeText.text = "Atk Range : 45";
        moveSpeedText.text = "Move Spd : 7";
    }
    public void CC1()
    {
        maxHealthText.text = "Health : 200";
        attackDamageText.text = "Damage : 30";
        attackSpeedText.text = "Atk Speed : 20";
        attackRangeText.text = "Atk Range : 100";
        moveSpeedText.text = "Move Spd : 7";
    }
    public void CC2()
    {
        maxHealthText.text = "Health : 200";
        attackDamageText.text = "Damage : 75";
        attackSpeedText.text = "Atk Speed : 10";
        attackRangeText.text = "Atk Range : 100";
        moveSpeedText.text = "Move Spd : 7";
    }
    public void HEALER1()
    {
        maxHealthText.text = "Health : 100";
        attackDamageText.text = "Heal Amt : 50";
        attackSpeedText.text = "Atk Speed : 15";
        attackRangeText.text = "Atk Range : 100";
        moveSpeedText.text = "Move Spd : 10";
    }
    public void HEALER2()
    {
        maxHealthText.text = "Health : 100";
        attackDamageText.text = "Heal Amt : 20";
        attackSpeedText.text = "Atk Speed : 10";
        attackRangeText.text = "Atk Range : 100";
        moveSpeedText.text = "Move Spd : 10";
    }
    

}
public enum TroopStatsType
{
    DPS,
    TANK,
    CC,
    HEALER
}

