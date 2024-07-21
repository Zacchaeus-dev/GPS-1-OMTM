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
    public TextMeshProUGUI buttonText;

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
                buttonText.text = "Weapon 1";
                break;
            case TroopWeapon.Weapon.Weapon2_DPS:
                buttonText.text = "Weapon 2";
                break;
            case TroopWeapon.Weapon.Weapon1_Tank:
                buttonText.text = "Weapon 1";
                break;
            case TroopWeapon.Weapon.Weapon2_Tank:
                buttonText.text = "Weapon 2";
                break;
            case TroopWeapon.Weapon.Weapon1_CC:
                buttonText.text = "Weapon 1";
                break;
            case TroopWeapon.Weapon.Weapon2_CC:
                troopWeapon.selectedWeapon = TroopWeapon.Weapon.Weapon1_CC;
                buttonText.text = "Weapon 2";
                break;
            case TroopWeapon.Weapon.Weapon1_Healer:
                buttonText.text = "Weapon 1";
                break;
            case TroopWeapon.Weapon.Weapon2_Healer:
                buttonText.text = "Weapon 2";
                break;
        }
    }

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
}
