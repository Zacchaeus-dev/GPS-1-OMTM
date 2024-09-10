using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class timerselfinactive : MonoBehaviour
{
    public float time;
    public float duration = 10;
    public float respawn = 11;
    public TextMeshProUGUI respawntimer;

    public void Start()
    {
        StartCoroutine(respawna());
    }

    IEnumerator respawna()
    {
        for (int i = 0; respawn >= 0; respawn--)
        {
            respawntimer.text = (respawn - 1).ToString();
            
            if (respawn <= 0)
            {
                break;
            }
            yield return new WaitForSeconds(1);
        }
    }
    void Update()
    {
        if (gameObject.activeSelf == true)
        {
            time += Time.deltaTime;

            if (time > duration)
            {
                time = 0;
                gameObject.SetActive(false);
            }
        }

        
    }
}


