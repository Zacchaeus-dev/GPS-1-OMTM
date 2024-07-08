using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KDHealthUI : MonoBehaviour
{
    public TMP_Text kdHealth;
 
    public GameObject killdozer;
    Killdozer killdozerScript;
    void Start()
    {
        killdozerScript = killdozer.GetComponent<Killdozer>();
        kdHealth.text = "Killdozer Health : " + killdozerScript.currentHealth;
    }

    public void KDHealthText()
    {
        kdHealth.text = "Killdozer Health : " + killdozerScript.currentHealth;
    }
}
