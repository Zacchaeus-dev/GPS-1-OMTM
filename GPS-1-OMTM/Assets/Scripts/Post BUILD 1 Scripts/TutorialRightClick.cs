using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRightClick : MonoBehaviour
{
    public TroopController2D troopController2D;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Troop"))
        {
            troopController2D.TutorialRightClick();
            gameObject.SetActive(false);
        }
    }
}
