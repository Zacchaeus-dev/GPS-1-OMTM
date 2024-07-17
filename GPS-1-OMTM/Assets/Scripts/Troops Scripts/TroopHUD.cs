using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TroopHUD : MonoBehaviour
{
    public Slider hpSlider;
    public GameObject hpFill;
    public Slider powerSlider;
    public GameObject powerFill;
    public Slider shieldSlider;
    public GameObject shieldFill;
    public TextMeshProUGUI currentHealth;

    public TroopController2D troopController2D;
    public CameraSystem cameraSystem;
    public GameObject troop;
    private float doubleClickTimeLimit = 0.3f;
    private float lastClickTime;

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

        TroopEnergy troopEnergy = _troop.GetComponent<TroopEnergy>();

        powerSlider.maxValue = troopEnergy.maxPower;
        powerSlider.value = troopEnergy.currentPower;

        if (troopEnergy.currentPower <= 0)
        {
            powerFill.SetActive(false);
        }
        else
        {
            powerFill.SetActive(true);
        }

        shieldSlider.maxValue = _troop.maxShield;
        shieldSlider.value = _troop.currentShield;

        if (_troop.currentShield <= 0)
        {
            shieldFill.SetActive(false);
        }
        else
        {
            shieldFill.SetActive(true);
        }

        currentHealth.text = _troop.currentHealth.ToString();
    }

    public void ButtonClick()
    {
        float timeSinceLastClick = Time.time - lastClickTime;
        lastClickTime = Time.time;

        if (cameraSystem.isZoomedOut == true)
        {
            return;
        }

        if (timeSinceLastClick <= doubleClickTimeLimit)
        {
            cameraSystem.FocusOnTroop(troop);
        }
        else if (troop != null)
        {
            troopController2D.DeselectTroop();
            troopController2D.SelectTroop(troop);
        }
        else
        {
            troopController2D.SelectTroop(troop);
        }
    }
}
