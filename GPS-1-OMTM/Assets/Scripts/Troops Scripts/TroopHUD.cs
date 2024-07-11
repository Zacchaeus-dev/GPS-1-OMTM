using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TroopHUD : MonoBehaviour
{
    public Slider hpSlider;
    public GameObject hpFill;

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
    }

    public void ButtonClick()
    {
        float timeSinceLastClick = Time.time - lastClickTime;
        lastClickTime = Time.time;

        if (timeSinceLastClick <= doubleClickTimeLimit)
        {
            cameraSystem.FocusOnTroop(troop);
        }
        else
        {
            troopController2D.SelectTroop(troop);
        }
    }
}
