using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfInactive : MonoBehaviour
{
    public float time;
    public float duration = 4;
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
