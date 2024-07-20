using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopPathfindBlockDetectorRight : MonoBehaviour
{
    public GameObject Troop;
    TroopClass PathfindingScript;

    private void Start()
    {
        PathfindingScript = Troop.GetComponent<TroopClass>();
    }
    void Update()
    {
        int layerMask = LayerMask.GetMask("Enemy");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero, Mathf.Infinity, layerMask);

        if (PathfindingScript.ONCE2 == false)
        {
            PathfindingScript.BlockedRight = false;
        }

        if (hit.collider != null && hit.collider.CompareTag("Enemy")) // to check if enemy is colliding with troop, if true play below
        {
            if (PathfindingScript.ONCE2 == false)
            {
                PathfindingScript.BlockedRight = true;
                PathfindingScript.SetTroopTargetPosition(transform.position, hit);
                PathfindingScript.ONCE2 = true;
                PathfindingScript.arrow.GetComponent<TroopPathfindArrow>().pathfindIcon.SetBool("x", true);
                PathfindingScript.arrow.GetComponent<TroopPathfindArrow>().DeactivateX();
                
            }
        }
        else
        {
            return;
        }
    }
}
