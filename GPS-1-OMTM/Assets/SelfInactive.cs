using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfInactive : MonoBehaviour
{
    float time;
    void Update()
    {
        if (gameObject.activeSelf == true)
        {
            time += Time.deltaTime;

            if (time > 4)
            {
                time = 0;
                gameObject.SetActive(false);
            }
        }
    }
}
