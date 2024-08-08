using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Destroy());
    }

    // Update is called once per frame
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(2);

        Destroy(gameObject);
    }
}
