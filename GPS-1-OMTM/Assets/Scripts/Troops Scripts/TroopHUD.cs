using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TroopHUD : MonoBehaviour
{
    public Slider hpSlider;
    public GameObject hpFill;

    public void SetHUD(Troop _troop)
    {
        hpSlider.maxValue = _troop.maxHealth;
        hpSlider.value = _troop.currentHealth;

        if (_troop.currentHealth <= 0)
        {
            hpFill.SetActive(false);
        }
        else
        {
            hpFill.SetActive(true);
        }
    }
}
