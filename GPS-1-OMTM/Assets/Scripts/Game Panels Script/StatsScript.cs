using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.MaterialProperty;

public class StatsScript : MonoBehaviour
{
    public GameObject TroopType;
    public Troop troopScript;
    //public List <Troop> troops = new List<Troop>();
    
    public Text maxHealthText;
    public Text currentHealthText;
    public Text attackText;
    public Text attackSpeedText;
    public Text attackRangeText;
    public Text moveSpeedText;
    // Start is called before the first frame update
    void Start()
    {
        //troop = GetComponent<Troop>();
      troopScript = TroopType.GetComponent<Troop>();
        UpdateUI();
    }

    public void UpdateUI()
    {
        //troopScript = TroopType.GetComponent<Troop>();
        maxHealthText.text = "Max Health: " + troopScript.maxHealth.ToString();
        attackText.text = "Attack: " + troopScript.attack.ToString();
        attackSpeedText.text = "Attack Speed: " + troopScript.attackSpeed.ToString();
        attackRangeText.text = "Attack Range: " + troopScript.attackRange.ToString();
        moveSpeedText.text = "Move Speed: " + troopScript.moveSpeed.ToString();

        Debug.Log("Switch");
        
    }


    // Update is called once per frame
    /*void Update()
    {
        UpdateUI();
    }*/
}
