using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponChanger : MonoBehaviour
{
    public GameObject troop;
    public TroopUnit troopUnit;
    private TroopWeapon troopWeapon;
    private TroopWeapon.Weapon selectedWeapon;
    //public TextMeshProUGUI buttonText;
    public TextMeshProUGUI button1Text;
    public TextMeshProUGUI button2Text;
    //public TextMeshProUGUI weaponName;
    //public TextMeshProUGUI weaponDescription;
    public GameObject weapon1Name;
    public GameObject weapon2Name;
    public GameObject weapon1Description;
    public GameObject weapon2Description;

    public enum TroopUnit
    {
        DPS,
        Tank,
        CC,
        Healer,
    }

    void Start()
    {
        troopWeapon = troop.GetComponent<TroopWeapon>();
        selectedWeapon = troopWeapon.selectedWeapon;

        switch (selectedWeapon)
        {
            case TroopWeapon.Weapon.Weapon1_DPS:
                button1Text.text = "-SELECTED-";
                button2Text.text = "";
                weapon1Name.SetActive(true);
                weapon2Name.SetActive(false);
                weapon1Description.SetActive(true);
                weapon2Description.SetActive(false);

                break;
            case TroopWeapon.Weapon.Weapon2_DPS:
                button1Text.text = "";
                button2Text.text = "-SELECTED-";
                weapon1Name.SetActive(false);
                weapon2Name.SetActive(true);
                weapon1Description.SetActive(false);
                weapon2Description.SetActive(true);
                break;
            case TroopWeapon.Weapon.Weapon1_Tank:
                button1Text.text = "-SELECTED-";
                button2Text.text = "";
                weapon1Name.SetActive(true);
                weapon2Name.SetActive(false);
                weapon1Description.SetActive(true);
                weapon2Description.SetActive(false);
                break;
            case TroopWeapon.Weapon.Weapon2_Tank:
                button1Text.text = "";
                button2Text.text = "-SELECTED-";
                weapon1Name.SetActive(false);
                weapon2Name.SetActive(true);
                weapon1Description.SetActive(false);
                weapon2Description.SetActive(true);
                break;
            case TroopWeapon.Weapon.Weapon1_CC:
                button1Text.text = "-SELECTED-";
                button2Text.text = "";
                weapon1Name.SetActive(true);
                weapon2Name.SetActive(false);
                weapon1Description.SetActive(true);
                weapon2Description.SetActive(false);
                break;
            case TroopWeapon.Weapon.Weapon2_CC:
                button1Text.text = "";
                button2Text.text = "-SELECTED-";
                weapon1Name.SetActive(false);
                weapon2Name.SetActive(true);
                weapon1Description.SetActive(false);
                weapon2Description.SetActive(true);
                break;
            case TroopWeapon.Weapon.Weapon1_Healer:
                button1Text.text = "-SELECTED-";
                button2Text.text = "";
                weapon1Name.SetActive(true);
                weapon2Name.SetActive(false);
                weapon1Description.SetActive(true);
                weapon2Description.SetActive(false);
                break;
            case TroopWeapon.Weapon.Weapon2_Healer:
                button1Text.text = "";
                button2Text.text = "-SELECTED-";
                weapon1Name.SetActive(false);
                weapon2Name.SetActive(true);
                weapon1Description.SetActive(false);
                weapon2Description.SetActive(true);
                break;
        }
    }

    /*
    public void ChangeWeapon()
    {
        switch(troopUnit)
        {
            case TroopUnit.DPS:
                switch(selectedWeapon)
                {
                    case TroopWeapon.Weapon.Weapon1_DPS:
                        troopWeapon.selectedWeapon = TroopWeapon.Weapon.Weapon2_DPS;
                        buttonText.text = "Weapon 2";
                        break;
                    case TroopWeapon.Weapon.Weapon2_DPS:
                        troopWeapon.selectedWeapon = TroopWeapon.Weapon.Weapon1_DPS;
                        buttonText.text = "Weapon 1";
                        break;
                }
                break;
            case TroopUnit.Tank:
                switch (selectedWeapon)
                {
                    case TroopWeapon.Weapon.Weapon1_Tank:
                        troopWeapon.selectedWeapon = TroopWeapon.Weapon.Weapon2_Tank;
                        buttonText.text = "Weapon 2";
                        break;
                    case TroopWeapon.Weapon.Weapon2_Tank:
                        troopWeapon.selectedWeapon = TroopWeapon.Weapon.Weapon1_Tank;
                        buttonText.text = "Weapon 1";
                        break;
                }
                break;
            case TroopUnit.CC:
                switch (selectedWeapon)
                {
                    case TroopWeapon.Weapon.Weapon1_CC:
                        troopWeapon.selectedWeapon = TroopWeapon.Weapon.Weapon2_CC;
                        buttonText.text = "Weapon 2";
                        break;
                    case TroopWeapon.Weapon.Weapon2_CC:
                        troopWeapon.selectedWeapon = TroopWeapon.Weapon.Weapon1_CC;
                        buttonText.text = "Weapon 1";
                        break;
                }
                break;
            case TroopUnit.Healer:
                switch (selectedWeapon)
                {
                    case TroopWeapon.Weapon.Weapon1_Healer:
                        troopWeapon.selectedWeapon = TroopWeapon.Weapon.Weapon2_Healer;
                        buttonText.text = "Weapon 2";
                        break;
                    case TroopWeapon.Weapon.Weapon2_Healer:
                        troopWeapon.selectedWeapon = TroopWeapon.Weapon.Weapon1_Healer;
                        buttonText.text = "Weapon 1";
                        break;
                }
                break;
        }
    }
    */
    public GameObject UltiError;
    IEnumerator ErrorUlti()
    {
        
        UltiError.SetActive(true);
        yield return new WaitForSeconds(1);
        //UltiError.SetActive(false);
    }
  
    public void ChangeToWeapon1()
    {
        FindObjectOfType<AudioManager>().Play("button");

        if (troop.GetComponent<Troop>().ultimateOnCooldown == true)
        {
            if (UltiError.activeSelf == false)
            {
                StartCoroutine(ErrorUlti());
            }
            FindObjectOfType<AudioManager>().Play("NoUlt");
            return;
        }

        switch (troopUnit)
        {
            case TroopUnit.DPS:
                troopWeapon.selectedWeapon = TroopWeapon.Weapon.Weapon1_DPS;
                button1Text.text = "-SELECTED-";
                button2Text.text = "";
                weapon1Name.SetActive(true);
                weapon2Name.SetActive(false);
                weapon1Description.SetActive(true);
                weapon2Description.SetActive(false);
                break;
            case TroopUnit.Tank:
                troopWeapon.selectedWeapon = TroopWeapon.Weapon.Weapon1_Tank;
                button1Text.text = "-SELECTED-";
                button2Text.text = "";
                weapon1Name.SetActive(true);
                weapon2Name.SetActive(false);
                weapon1Description.SetActive(true);
                weapon2Description.SetActive(false);
                break;
            case TroopUnit.CC:
                troopWeapon.selectedWeapon = TroopWeapon.Weapon.Weapon1_CC;
                button1Text.text = "-SELECTED-";
                button2Text.text = "";
                weapon1Name.SetActive(true);
                weapon2Name.SetActive(false);
                weapon1Description.SetActive(true);
                weapon2Description.SetActive(false);
                break;
            case TroopUnit.Healer:
                troopWeapon.selectedWeapon = TroopWeapon.Weapon.Weapon1_Healer;
                button1Text.text = "-SELECTED-";
                button2Text.text = "";
                weapon1Name.SetActive(true);
                weapon2Name.SetActive(false);
                weapon1Description.SetActive(true);
                weapon2Description.SetActive(false);
                break;
        }
    }

    public void ChangeToWeapon2()
    {
        FindObjectOfType<AudioManager>().Play("button");

        if (troop.GetComponent<Troop>().ultimateOnCooldown == true)
        {
            if (UltiError.activeSelf == false)
            {
                StartCoroutine(ErrorUlti());
            }
            FindObjectOfType<AudioManager>().Play("NoUlt");
            return;
        }

        switch (troopUnit)
        {
            case TroopUnit.DPS:
                troopWeapon.selectedWeapon = TroopWeapon.Weapon.Weapon2_DPS;
                button1Text.text = "";
                button2Text.text = "-SELECTED-";
                weapon1Name.SetActive(false);
                weapon2Name.SetActive(true);
                weapon1Description.SetActive(false);
                weapon2Description.SetActive(true);
                break;
            case TroopUnit.Tank:
                troopWeapon.selectedWeapon = TroopWeapon.Weapon.Weapon2_Tank;
                button1Text.text = "";
                button2Text.text = "-SELECTED-";
                weapon1Name.SetActive(false);
                weapon2Name.SetActive(true);
                weapon1Description.SetActive(false);
                weapon2Description.SetActive(true);

                break;
            case TroopUnit.CC:
                troopWeapon.selectedWeapon = TroopWeapon.Weapon.Weapon2_CC;
                button1Text.text = "";
                button2Text.text = "-SELECTED-";
                weapon1Name.SetActive(false);
                weapon2Name.SetActive(true);
                weapon1Description.SetActive(false);
                weapon2Description.SetActive(true);
                break;
            case TroopUnit.Healer:
                troopWeapon.selectedWeapon = TroopWeapon.Weapon.Weapon2_Healer;
                button1Text.text = "";
                button2Text.text = "-SELECTED-";
                weapon1Name.SetActive(false);
                weapon2Name.SetActive(true);
                weapon1Description.SetActive(false);
                weapon2Description.SetActive(true);
                break;
        }
    }
}
