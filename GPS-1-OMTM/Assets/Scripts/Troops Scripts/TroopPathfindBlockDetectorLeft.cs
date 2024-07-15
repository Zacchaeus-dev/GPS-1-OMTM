using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopPathfindBlockDetectorLeft : MonoBehaviour
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

        if (PathfindingScript.ONCE == false)
        {
            PathfindingScript.BlockedLeft = false;
        }

        if (hit.collider.CompareTag("Enemy"))
        {
            Debug.Log(hit.collider.gameObject);
            Debug.Log("CANNOT MOVE LEFT");

            if (PathfindingScript.ONCE == false)
            {
                PathfindingScript.BlockedLeft = true;
                PathfindingScript.SetTroopTargetPosition(transform.position, hit);
                PathfindingScript.ONCE = true;
                PathfindingScript.arrow.GetComponent<TroopPathfindArrow>().pathfindIcon.SetBool("x", true);
                PathfindingScript.arrow.GetComponent<TroopPathfindArrow>().DeactivateX();
            }

        }
    }
}
