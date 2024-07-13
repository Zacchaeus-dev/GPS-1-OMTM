using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopWeapon : MonoBehaviour
{
    public Weapon selectedWeapon = Weapon.None;

    public enum Weapon
    {
        None,
        Weapon1_DPS,
        Weapon2_DPS,
        Weapon1_Tank,
        Weapon2_Tank,
        Weapon1_CC,
        Weapon2_CC,
        Weapon1_Healer,
        Weapon2_Healer
    }
}
