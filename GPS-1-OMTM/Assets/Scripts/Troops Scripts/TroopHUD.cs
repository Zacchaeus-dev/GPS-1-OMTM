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
    public TextMeshProUGUI currentHealthText;
    public TextMeshProUGUI currentPowerText;
    public TextMeshProUGUI currentShieldText;
    public Image ultimatePowerOverlay;
    public TextMeshProUGUI powerPercentage;
    public GameObject dimOverlay;

    public GameObject bgBarObject;
    public Image bgBar;
    public GameObject hpBarObject;
    public Image hpBar;
    public GameObject powerBarObject;
    public Image powerBar;
    public GameObject shieldBarObject;
    public Image shieldBar;

    public GameObject medicMessage;

    public TroopController2D troopController2D;
    public CameraSystem cameraSystem;
    public GameObject troop;
    private float doubleClickTimeLimit = 0.3f;
    private float lastClickTime;
    public TutorialPhase tutorialPhase;
    public bool isDPS;

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

        currentHealthText.text = _troop.currentHealth.ToString();
        currentPowerText.text = troopEnergy.currentPower.ToString();
        currentShieldText.text = _troop.currentShield.ToString();

        float percentage = ((float)troopEnergy.currentPower / (float)troopEnergy.maxPower) * 100f;
        //Debug.Log("Calculated Percentage: " + percentage);
        percentage = (int)percentage;
        powerPercentage.text =  percentage.ToString() + "%";

        //ultimatePowerOverlay.fillAmount = 1 - ((float)troopEnergy.currentPower / (float)troopEnergy.maxPower);

        if (troopEnergy.currentPower == troopEnergy.maxPower)
        {
            ultimatePowerOverlay.gameObject.SetActive(false);
            powerPercentage.gameObject.SetActive(false);
        }
        else
        {
            ultimatePowerOverlay.gameObject.SetActive(true);
            powerPercentage.gameObject.SetActive(true);
        }

        hpBar.fillAmount = ((float)_troop.currentHealth / (float)_troop.maxHealth);
        powerBar.fillAmount = ((float)troopEnergy.currentPower / (float)troopEnergy.maxPower);
        shieldBar.fillAmount = (float)_troop.currentShield / (float)_troop.maxShield;

        if (_troop.currentHealth <= (_troop.maxHealth / 10) && _troop.currentHealth > 0)
        {
            medicMessage.SetActive(true);
        }
        else
        {
            medicMessage.SetActive(false);
        }
    }

    public void ButtonClick()
    {
        if (tutorialPhase.tutorialOn && isDPS == false)
        {
            return;
        }

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

    public void DimOn()
    {
        if (dimOverlay != null)
        {
            dimOverlay.SetActive(true);
        }
    }

    public void DimOff()
    {
        if (dimOverlay != null)
        {
            dimOverlay.SetActive(false);
        }
    }

    public void EnableTroopBars()
    {
        bgBarObject.SetActive(true);
        hpBarObject.SetActive(true);
        powerBarObject.SetActive(true);
        shieldBarObject.SetActive(true);
    }

    public void DisableTroopBars()
    {
        bgBarObject.SetActive(false);
        hpBarObject.SetActive(false);
        powerBarObject.SetActive(false);
        shieldBarObject.SetActive(false);   
    }
}
