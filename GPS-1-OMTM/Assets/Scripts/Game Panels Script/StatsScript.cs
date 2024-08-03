using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using static UnityEditor.MaterialProperty;

public class StatsScript : MonoBehaviour
{
    public GameObject TroopType;
    public Troop troopScript;
    public TroopAutoAttack troopAAScript;
    public TroopClass troopClassScript;
    public HealerAutoHeal healerAHScript;
    //public List <Troop> troops = new List<Troop>();
    
    public Text maxHealthText;
    public Text currentHealthText;
    public Text attackText;
    public Text attackSpeedText;
    public Text attackRangeText;
    public Text moveSpeedText;

    public bool isHealer;

    void Start()
    {
        //troop = GetComponent<Troop>();
        troopScript = TroopType.GetComponent<Troop>(); //max health,current health
        troopAAScript = TroopType.GetComponent<TroopAutoAttack>(); //attack speed and attack range
        troopClassScript = TroopType.GetComponent<TroopClass>(); //movement speed
        UpdateUI();

        if (isHealer == true)
        {
            healerAHScript = TroopType.GetComponent<HealerAutoHeal>();
        }
    }

    public void UpdateUI()
    {
        //troopScript = TroopType.GetComponent<Troop>();

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

        //Debug.Log("Switch");       
    }

  
}
