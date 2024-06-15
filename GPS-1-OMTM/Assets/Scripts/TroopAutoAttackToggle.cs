using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopAutoAttackToggle : MonoBehaviour
{
    private TroopController2D troopController;

    void Start()
    {
        troopController = FindObjectOfType<TroopController2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && troopController.selectedTroop != null)
        {
            TroopAutoAttack autoAttack = troopController.selectedTroop.GetComponent<TroopAutoAttack>();
            if (autoAttack != null)
            {
                autoAttack.autoAttackEnabled = !autoAttack.autoAttackEnabled;
                Debug.Log("Auto Attack " + (autoAttack.autoAttackEnabled ? "Enabled" : "Disabled"));
            }
        }
    }
}
