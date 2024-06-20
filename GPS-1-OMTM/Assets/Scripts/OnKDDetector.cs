using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnKDDetector : MonoBehaviour
{
    public bool onKD = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Killdozer")
        {
            onKD = true;
            //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAA");
        }
    }    
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Killdozer")
        {
            onKD = false;
            //Debug.Log("BBBBBBBBBBB");
        }
    }
}
