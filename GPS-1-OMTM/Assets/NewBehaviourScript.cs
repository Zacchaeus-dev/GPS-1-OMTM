using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class timerselfinactive : MonoBehaviour
{
    public float time;
    public float duration = 10;
    public float respawn = 10;
    public TextMeshProUGUI respawntimer;


    private void OnEnable()
    {
        respawn = 10;
        StartCoroutine(respawna());
        
    }

    IEnumerator respawna()
    {
        for (int i = 0; respawn >= 0; respawn--)
        {
            respawntimer.text = respawn.ToString();
            
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


