using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    // Start is called before the first frame update
    public float waitTime = 2;
    void Start()
    {
        StartCoroutine(Destroy());
    }

    // Update is called once per frame
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(waitTime);

        Destroy(gameObject);
    }
}
