using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class TroopClass : MonoBehaviour
{
    private Camera mainCamera;
    private Vector2 targetPosition;
    public bool isMoving;
    //private bool canMoveY;
    private bool canClimb;
    //private bool onLadder;
    private Collider2D nearestVert;
    private Vector2 vertPosition;
    private Rigidbody2D rb;
    public string onPlatform;

    public float moveSpeed = 5f; // Speed of the troop movement

    public GameObject TroopController2D;
    TroopController2D controllerScript;

    public GameObject OnKDCollider;
    OnKDDetector OnKDScript;

    public Vector2 mousePosition = new Vector2(10000,10000);
    public RaycastHit2D hit;
    public Transform killdozer;

    float timer;
    public Animator attackAnimation;

    private void Start()
    {
        mainCamera = Camera.main;
        rb = gameObject.GetComponent<Rigidbody2D>();
        controllerScript = TroopController2D.GetComponent<TroopController2D>();
        OnKDScript = OnKDCollider.GetComponent<OnKDDetector>();
    }

    public void SetTroopTargetPosition(Vector2 mP, RaycastHit2D h)
    {
        mousePosition = mP;
        hit = h;



            // 1. if click on any part of killdozer
            if (hit.collider != null && hit.collider.CompareTag("[PF] KD Middle-Ground"))
            {
                if (onPlatform == "KD Middle-Ground")
                {

                    targetPosition = new Vector2(mousePosition.x, transform.position.y);
                    vertPosition = Vector2.zero;
                    nearestVert = null;
                    
                }
                else if (onPlatform == "Ground" || onPlatform == "KD Ground")
                {
                    FindAndGoToNearestVerticallyKD(mousePosition);
                    Debug.Log("Killdozer's Middle-Ground!!!");

                }
                else if (onPlatform == "Upper-Ground 1")
                {

                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS ON 1 === //
                    if (killdozer.GetComponent<Killdozer>().directPathfinding == 1)
                    {
                        targetPosition = new Vector2(mousePosition.x, transform.position.y);
                        vertPosition = Vector2.zero;
                        nearestVert = null;
                    }
                    else 
                    {
                        if (onPlatform != "Ground")
                        {

                            FindAndGoToNearestVertically1(mousePosition);
                        }
                    }
                
                }
                else if (onPlatform == "Upper-Ground 2")
                {

                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS ON 1 === //
                    if (killdozer.GetComponent<Killdozer>().directPathfinding == 2)
                    {
                        targetPosition = new Vector2(mousePosition.x, transform.position.y);
                        vertPosition = Vector2.zero;
                        nearestVert = null;
                    }
                    else
                    {
                        if (onPlatform != "Ground")
                        {
                            FindAndGoToNearestVertically2(mousePosition);
                        }
                    }

                }
                else if (onPlatform == "Upper-Ground 3")
                {


                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS ON 1 === //
                    if (killdozer.GetComponent<Killdozer>().directPathfinding == 3)
                    {
                        targetPosition = new Vector2(mousePosition.x, transform.position.y);
                        vertPosition = Vector2.zero;
                        nearestVert = null;
                    }
                    else
                    {
                        if (onPlatform != "Ground")
                        {
                            FindAndGoToNearestVertically3(mousePosition);
                        }
                    }

                }                
                else if (onPlatform == "Upper-Ground 4")
                {


                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS ON 1 === //
                    if (killdozer.GetComponent<Killdozer>().directPathfinding == 4)
                    {
                        targetPosition = new Vector2(mousePosition.x, transform.position.y);
                        vertPosition = Vector2.zero;
                        nearestVert = null;
                    }
                    else
                    {
                        if (onPlatform != "Ground")
                        {
                            FindAndGoToNearestVertically4(mousePosition);
                        }
                    }

                }
            }

            // 2a. if click on UG 1
            else if (hit.collider != null && hit.collider.CompareTag("[PF] Upper-Ground 1"))
            {

                if (onPlatform == "Upper-Ground 1")
                {

                    
                    targetPosition = new Vector2(mousePosition.x, transform.position.y);
                    vertPosition = Vector2.zero;
                    nearestVert = null;
                    
                }
                else if (onPlatform == "Upper-Ground 2")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically2(mousePosition);
                    }

                }
                else if (onPlatform == "Upper-Ground 3")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically3(mousePosition);
                    }

                }
                else if (onPlatform == "Upper-Ground 4")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically4(mousePosition);
                    }

                }
                else if (onPlatform == "KD Middle-Ground")
                {

                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS ON 1 === //
                    if (killdozer.GetComponent<Killdozer>().directPathfinding == 1)
                    {
                        targetPosition = new Vector2(mousePosition.x, transform.position.y);
                        vertPosition = Vector2.zero;
                        nearestVert = null;
                    }
                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS other than 1 === //
                    else
                    {
                        if (onPlatform != "Ground")
                        {
                            FindAndGoToNearestVerticallyKD(mousePosition);
                        }
                    }

                }
                else if (onPlatform == "Ground")
                {

                    FindAndGoToNearestVertically1(mousePosition);
                }
                else if (onPlatform == "KD Ground")
                {

                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS ON 1 === //
                    if (killdozer.GetComponent<Killdozer>().directPathfinding == 1)
                    {
                            FindAndGoToNearestVerticallyKD(mousePosition);
                    }
                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS other than 1 === //
                    else
                    {
                        FindAndGoToNearestVertically1(mousePosition);
                    }
                    
                }
            }

            // 2b. if click on UG 2
            else if (hit.collider != null && hit.collider.CompareTag("[PF] Upper-Ground 2"))
            {

                if (onPlatform == "Upper-Ground 2")
                {
                    
                    targetPosition = new Vector2(mousePosition.x, transform.position.y);
                    vertPosition = Vector2.zero;
                    nearestVert = null;
                    
                }
                else if (onPlatform == "Upper-Ground 1")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically1(mousePosition);
                    }

                }
                else if (onPlatform == "Upper-Ground 3")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically3(mousePosition);
                    }

                }
                else if (onPlatform == "Upper-Ground 4")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically4(mousePosition);
                    }

                }
                else if (onPlatform == "KD Middle-Ground")
                {

                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS ON 2 === //
                    if (killdozer.GetComponent<Killdozer>().directPathfinding == 2)
                    {
                        targetPosition = new Vector2(mousePosition.x, transform.position.y);
                        vertPosition = Vector2.zero;
                        nearestVert = null;
                    }
                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS other than 2 === //
                    else
                    {
                        if (onPlatform != "Ground")
                        {
                            FindAndGoToNearestVerticallyKD(mousePosition);
                        }
                    }
                }
                else if (onPlatform == "Ground")
                {

                    FindAndGoToNearestVertically2(mousePosition);
                }
                else if (onPlatform == "KD Ground")
                {

                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS ON 2 === //
                    if (killdozer.GetComponent<Killdozer>().directPathfinding == 2)
                    {
                        FindAndGoToNearestVerticallyKD(mousePosition);
                    }
                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS other than 2 === //
                    else
                    {
                        FindAndGoToNearestVertically2(mousePosition);
                    }

                }
        }

            // 2c. if click on UG 3      
            else if (hit.collider != null && hit.collider.CompareTag("[PF] Upper-Ground 3"))
            {

                if (onPlatform == "Upper-Ground 3")
                {
                    
                    targetPosition = new Vector2(mousePosition.x, transform.position.y);
                    vertPosition = Vector2.zero;
                    nearestVert = null;
                    
                }
                else if (onPlatform == "Upper-Ground 1")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically1(mousePosition);
                    }

                }
                else if (onPlatform == "Upper-Ground 2")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically2(mousePosition);
                    }

                }                
                else if (onPlatform == "Upper-Ground 4")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically4(mousePosition);
                    }

                }
                else if (onPlatform == "KD Middle-Ground")
                {
                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS ON 3 === //
                    if (killdozer.GetComponent<Killdozer>().directPathfinding == 3)
                    {
                        targetPosition = new Vector2(mousePosition.x, transform.position.y);
                        vertPosition = Vector2.zero;
                        nearestVert = null;
                    }
                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS other than 3 === //
                    else
                    {
                        if (onPlatform != "Ground")
                        {
                            FindAndGoToNearestVerticallyKD(mousePosition);
                        }
                    }
                }
                else if (onPlatform == "Ground")
                {

                    FindAndGoToNearestVertically3(mousePosition);
                }
                else if (onPlatform == "KD Ground")
                {

                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS ON 3 === //
                    if (killdozer.GetComponent<Killdozer>().directPathfinding == 3)
                    {
                        FindAndGoToNearestVerticallyKD(mousePosition);
                    }
                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS other than 3 === //
                    else
                    {
                        FindAndGoToNearestVertically3(mousePosition);
                    }
                }
            }            
            
            // 2d. if click on UG 4      
            else if (hit.collider != null && hit.collider.CompareTag("[PF] Upper-Ground 4"))
            {

                if (onPlatform == "Upper-Ground 4")
                {
                Debug.Log("FAK");
                    targetPosition = new Vector2(mousePosition.x, transform.position.y);
                    vertPosition = Vector2.zero;
                    nearestVert = null;
                    
                }
                else if (onPlatform == "Upper-Ground 1")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically1(mousePosition);
                    }

                }
                else if (onPlatform == "Upper-Ground 2")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically2(mousePosition);
                    }

                }                
                else if (onPlatform == "Upper-Ground 3")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically3(mousePosition);
                    }
                }
                else if (onPlatform == "KD Middle-Ground")
                {
                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS ON 4 === //
                    if (killdozer.GetComponent<Killdozer>().directPathfinding == 4)
                    {
                        targetPosition = new Vector2(mousePosition.x, transform.position.y);
                        vertPosition = Vector2.zero;
                        nearestVert = null;
                    }
                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS other than 4 === //
                    else
                    {
                        if (onPlatform != "Ground")
                        {
                            FindAndGoToNearestVerticallyKD(mousePosition);
                        }
                    }
                }
                else if (onPlatform == "Ground")
                {

                    FindAndGoToNearestVertically4(mousePosition);
                }
                else if (onPlatform == "KD Ground")
                {

                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS ON 4 === //
                    if (killdozer.GetComponent<Killdozer>().directPathfinding == 4)
                    {
                        FindAndGoToNearestVerticallyKD(mousePosition);
                    }
                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS other than 4 === //
                    else
                    {
                        FindAndGoToNearestVertically4(mousePosition);
                    }
                }
            }

            // 3a. If click on Ground OR ON KD GROUND
            else if (hit.collider != null && hit.collider.CompareTag("[PF] Ground") || hit.collider != null && hit.collider.CompareTag("[PF] KD Ground") || hit.collider != null && hit.collider.CompareTag("[PF] KD Ground (Bug Fixer)"))
            {
                if (onPlatform == "Ground" || onPlatform == "KD Ground")
                {
                    targetPosition = new Vector2(mousePosition.x, transform.position.y); // Default to only X movement
                    vertPosition = Vector2.zero;
                    nearestVert = null;
                }
                else if (onPlatform == "KD Middle-Ground")
                {

                    FindAndGoToNearestVerticallyKD(mousePosition);
                }
                else if (onPlatform == "Upper-Ground 1")
                {
                    FindAndGoToNearestVertically1(mousePosition);
                }
                else if (onPlatform == "Upper-Ground 2")
                {

                    FindAndGoToNearestVertically2(mousePosition);
                }
                else if (onPlatform == "Upper-Ground 3")
                {

                    FindAndGoToNearestVertically3(mousePosition);
                }                
                else if (onPlatform == "Upper-Ground 4")
                {

                    FindAndGoToNearestVertically4(mousePosition);
                }
            }

            // 3b. If click on verticallies, still move go on Ground
            else if (hit.collider != null && hit.collider.CompareTag("[PF] Vertically 1") || hit.collider != null && hit.collider.CompareTag("[PF] KD Vertically 1"))
            {
                if (onPlatform == "Ground" || onPlatform == "KD Ground")
                {
                    targetPosition = new Vector2(mousePosition.x, transform.position.y); // Default to only X movement
                    vertPosition = Vector2.zero;
                    nearestVert = null;
                }
                else if (onPlatform == "KD Middle-Ground")
                {

                    FindAndGoToNearestVerticallyKD(mousePosition);
                }
                else if (onPlatform == "Upper-Ground 1")
                {

                    FindAndGoToNearestVertically1(mousePosition);
                }
                else if (onPlatform == "Upper-Ground 2")
                {

                    FindAndGoToNearestVertically2(mousePosition);
                }
                else if (onPlatform == "Upper-Ground 3")
                {

                    FindAndGoToNearestVertically3(mousePosition);
                }                
                else if (onPlatform == "Upper-Ground 4")
                {

                    FindAndGoToNearestVertically4(mousePosition);
                }
            }

            // == send over to <TroopClass> script == //
            if (controllerScript.selectedTroop != null)
            {
                GetComponent<TroopClass>().SetTargetPosition(targetPosition, vertPosition, nearestVert);
            }
        



    }

    void FindAndGoToNearestVerticallyKD(Vector2 mousePosition)
    {
        GameObject[] verts = GameObject.FindGameObjectsWithTag("[PF] KD Vertically 1");

        float closest = Mathf.Infinity;
        GameObject closestVert = null;
        for (int i = 0; i < verts.Length; i++)  // List of gameObjects to search through
        {
            float dist = Vector3.Distance(verts[i].transform.position, mousePosition); // Distance to mouse position
            
            if (dist < closest)
            {
                closest = dist;
                closestVert = verts[i];
            }
        }

        if (closestVert != null)
        {
            Collider2D closestVertCollider = closestVert.GetComponent<BoxCollider2D>();
            nearestVert = closestVertCollider;
            vertPosition = new Vector2(closestVertCollider.transform.position.x, transform.position.y); // Move to vert X position first
            targetPosition = new Vector2(mousePosition.x, transform.position.y);

            
        }
        
    }

    void FindAndGoToNearestVertically1(Vector2 mousePosition)
    {
        GameObject[] verts = GameObject.FindGameObjectsWithTag("[PF] Vertically 1");

        float closest = Mathf.Infinity;
        GameObject closestVert = null;
        for (int i = 0; i < verts.Length; i++)  // List of gameObjects to search through
        {
            //float dist = Vector3.Distance(verts[i].transform.position, mousePosition); // Distance to mouse position
            float dist = Vector3.Distance(verts[i].transform.position, transform.position); 
            
            if (dist < closest)
            {
                closest = dist;
                closestVert = verts[i];
            }
        }

        if (closestVert != null)
        {
            Collider2D closestVertCollider = closestVert.GetComponent<BoxCollider2D>();
            nearestVert = closestVertCollider;
            vertPosition = new Vector2(closestVertCollider.transform.position.x, transform.position.y); // Move to vert X position first
            targetPosition = new Vector2(mousePosition.x, transform.position.y);

            
        }
    }

    void FindAndGoToNearestVertically2(Vector2 mousePosition)
    {
        GameObject[] verts = GameObject.FindGameObjectsWithTag("[PF] Vertically 2");

        float closest = Mathf.Infinity;
        GameObject closestVert = null;
        for (int i = 0; i < verts.Length; i++)  // List of gameObjects to search through
        {
            //float dist = Vector3.Distance(verts[i].transform.position, mousePosition); // Distance to mouse position
            float dist = Vector3.Distance(verts[i].transform.position, transform.position); 
            
            if (dist < closest)
            {
                closest = dist;
                closestVert = verts[i];
            }
        }

        if (closestVert != null)
        {
            Collider2D closestVertCollider = closestVert.GetComponent<BoxCollider2D>();
            nearestVert = closestVertCollider;
            vertPosition = new Vector2(closestVertCollider.transform.position.x, transform.position.y); // Move to vert X position first
            targetPosition = new Vector2(mousePosition.x, transform.position.y);

            
        }
    }

    void FindAndGoToNearestVertically3(Vector2 mousePosition)
    {
        GameObject[] verts = GameObject.FindGameObjectsWithTag("[PF] Vertically 3");

        float closest = Mathf.Infinity;
        GameObject closestVert = null;
        for (int i = 0; i < verts.Length; i++)  // List of gameObjects to search through
        {
            //float dist = Vector3.Distance(verts[i].transform.position, mousePosition); // Distance to mouse position
            float dist = Vector3.Distance(verts[i].transform.position, transform.position); 
            
            if (dist < closest)
            {
                closest = dist;
                closestVert = verts[i];
            }
        }

        if (closestVert != null)
        {
            Collider2D closestVertCollider = closestVert.GetComponent<BoxCollider2D>();
            nearestVert = closestVertCollider;
            vertPosition = new Vector2(closestVertCollider.transform.position.x, transform.position.y); // Move to vert X position first
            targetPosition = new Vector2(mousePosition.x, transform.position.y);

            
        }
    }    
    
    void FindAndGoToNearestVertically4(Vector2 mousePosition)
    {
        GameObject[] verts = GameObject.FindGameObjectsWithTag("[PF] Vertically 4");

        float closest = Mathf.Infinity;
        GameObject closestVert = null;
        for (int i = 0; i < verts.Length; i++)  // List of gameObjects to search through
        {
            //float dist = Vector3.Distance(verts[i].transform.position, mousePosition); // Distance to mouse position
            float dist = Vector3.Distance(verts[i].transform.position, transform.position); 
            
            if (dist < closest)
            {
                closest = dist;
                closestVert = verts[i];
            }
        }

        if (closestVert != null)
        {
            Collider2D closestVertCollider = closestVert.GetComponent<BoxCollider2D>();
            nearestVert = closestVertCollider;
            vertPosition = new Vector2(closestVertCollider.transform.position.x, transform.position.y); // Move to vert X position first
            targetPosition = new Vector2(mousePosition.x, transform.position.y);

            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
 
        if (onPlatform == "Ground" && collision.tag == "[PF] Upper-Ground 1")
        {
            Debug.Log("Upper Ground 1");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "Upper-Ground 1";
            SetTroopTargetPosition(mousePosition, hit);
        }
        else if (onPlatform == "Ground" && collision.tag == "[PF] Upper-Ground 2")
        {
            Debug.Log("Upper Ground 2");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "Upper-Ground 2";
            SetTroopTargetPosition(mousePosition, hit);
        }
        else if (onPlatform == "Ground" && collision.tag == "[PF] Upper-Ground 3")
        {
            Debug.Log("Upper Ground 3");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "Upper-Ground 3";
            SetTroopTargetPosition(mousePosition, hit);
        }        
        else if (onPlatform == "Ground" && collision.tag == "[PF] Upper-Ground 4")
        {
            Debug.Log("Upper Ground 4");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "Upper-Ground 4";
            SetTroopTargetPosition(mousePosition, hit);
        }
        else if (onPlatform == "Ground" && collision.tag == "[PF] KD Middle-Ground" || onPlatform == "KD Ground" && collision.tag == "[PF] KD Middle-Ground")
        {
            Debug.Log("KD Middle Ground");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "KD Middle-Ground";
            SetTroopTargetPosition(mousePosition, hit);
        }
        else if (onPlatform == "Upper-Ground 1" && collision.tag == "[PF] Ground Check"
            || onPlatform == "Upper-Ground 2" && collision.tag == "[PF] Ground Check"
            || onPlatform == "Upper-Ground 3" && collision.tag == "[PF] Ground Check"
            || onPlatform == "Upper-Ground 4" && collision.tag == "[PF] Ground Check")
        {
            Debug.Log("Ground Check");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "Ground";
            SetTroopTargetPosition(mousePosition, hit);
        }
        else if (onPlatform == "KD Middle-Ground" && collision.tag == "[PF] Ground Check")
        {
            Debug.Log("KD Ground Check");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "KD Ground";
            SetTroopTargetPosition(mousePosition, hit);
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isMoving == false)
        {
            if (onPlatform == "KD Middle-Ground" && collision.tag == "[PF] Upper-Ground 1")
            {
                
                canClimb = false;
                //isMoving = true;

                nearestVert = null;
                onPlatform = "Upper-Ground 1";
                //SetTroopTargetPosition(mousePosition, hit);

            }
            if (onPlatform == "KD Middle-Ground" && collision.tag == "[PF] Upper-Ground 2")
            {

                canClimb = false;
                //isMoving = true;

                nearestVert = null;
                onPlatform = "Upper-Ground 2";
                //SetTroopTargetPosition(mousePosition, hit);

            }
            if (onPlatform == "KD Middle-Ground" && collision.tag == "[PF] Upper-Ground 3")
            {

                canClimb = false;
                //isMoving = true;

                nearestVert = null;
                onPlatform = "Upper-Ground 3";
                //SetTroopTargetPosition(mousePosition, hit);

            }
            if (onPlatform == "KD Middle-Ground" && collision.tag == "[PF] Upper-Ground 4")
            {

                canClimb = false;
                //isMoving = true;

                nearestVert = null;
                onPlatform = "Upper-Ground 4";
                //SetTroopTargetPosition(mousePosition, hit);

            }
        }
        if (onPlatform == "Upper-Ground 1" && collision.tag == "[PF] KD Middle-Ground"
            || onPlatform == "Upper-Ground 2" && collision.tag == "[PF] KD Middle-Ground"
            || onPlatform == "Upper-Ground 3" && collision.tag == "[PF] KD Middle-Ground"
            || onPlatform == "Upper-Ground 4" && collision.tag == "[PF] KD Middle-Ground")
        {
           
            canClimb = false;
           //isMoving = true;

            nearestVert = null;
            onPlatform = "KD Middle-Ground";
            FindAndGoToNearestVerticallyKD(mousePosition);
           //SetTroopTargetPosition(mousePosition, hit);

        }


        if (onPlatform == "Ground" && collision.tag == "[PF] KD Ground (Bug Fixer)" || onPlatform == "Ground" && collision.tag == "[PF] KD Ground" || onPlatform == "Ground" && collision.tag == "[PF] KD Vertically 1")
        {

            onPlatform = "KD Ground";
            SetTroopTargetPosition(mousePosition, hit);

        }

        if (isMoving == false)
        {
            if (onPlatform == "KD Ground" && collision.tag == "[PF] Ground" && collision.tag != "[PF] KD Ground (Bug Fixer)" && collision.tag != "[PF] Vertically 1" 
                && collision.tag != "[PF] Vertically 2" && collision.tag != "[PF] Vertically 3" && collision.tag != "[PF] Vertically 4" && collision.tag != "[PF] Ground Checker")
            {
                onPlatform = "Ground";
                SetTroopTargetPosition(mousePosition, hit);

            }
        }


    }


    public void SetTargetPosition(Vector2 targetPosition, Vector2 vertPosition, Collider2D nearestVert)
    {
        this.targetPosition = targetPosition;
        this.vertPosition = vertPosition;
        this.nearestVert = nearestVert;
        isMoving = true;
    }


    public void Update()
    {
        if (isMoving)
        {
            Move();
            attackAnimation.SetBool("Attack", false);
        }
        if (canClimb)
        {
            ClimbAndMove();
        }

        // To move with Killdozer
        if (OnKDScript.onKD == true) // || onPlatform == "Upper-Ground 1" || onPlatform == "Upper-Ground 2" || onPlatform == "Upper-Ground 3" || onPlatform == "Upper-Ground 4")
        {
            transform.SetParent(killdozer);
        }
        else
        {
            transform.SetParent(null);
        }

/*        timer = timer + Time.deltaTime;
        if (timer >= 1)
        {
            Debug.Log("WHOOOOO");
            Move();
            timer = 0;
        }*/
    }

    void Move()
    {
        if (onPlatform == "Ground" || onPlatform == "KD Ground")
        {
            if (nearestVert != null) // if changing elevation
            {
                if (canClimb == false)
                {
                    transform.position = Vector2.MoveTowards(transform.position, vertPosition, moveSpeed * Time.deltaTime); // move to nearest vert
                    if (Vector2.Distance(transform.position, vertPosition) < 0.1f) // when reached vert, activate climb
                    {
                        transform.position = vertPosition;
                        canClimb = true;
                        Debug.Log("STARTING CLIMB NOW");
                    }
                }
            }
            else // if staying on same elevation
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                targetPosition.y = transform.position.y;
                if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
                {
                    transform.position = targetPosition;
                    isMoving = false;

                    //Debug.Log("Troop arrived at target position: " + targetPosition);
                }
            }
        }

        else if (onPlatform == "Upper-Ground 1" || onPlatform == "Upper-Ground 2" || onPlatform == "Upper-Ground 3" || onPlatform == "Upper-Ground 4" || onPlatform == "KD Middle-Ground")
        {
            if (nearestVert != null)
            {
                if (canClimb == false)
                {
                    transform.position = Vector2.MoveTowards(transform.position, vertPosition, moveSpeed * Time.deltaTime); // move to nearest vert\
                    if (Vector2.Distance(transform.position, vertPosition) < 0.1f) // when reached vert, activate climb
                    {
                        //transform.position = new Vector2((vertPosition.x + 0.4f), vertPosition.y);
                        transform.position = vertPosition;
                        canClimb = true;
                        Debug.Log("CLIMB DOWN NOW");
                    }
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                targetPosition.y = transform.position.y;
                if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
                {
                    transform.position = targetPosition;
                    isMoving = false;

                    //Debug.Log("Troop arrived at target position: " + targetPosition);
                }
            }
        }
    }

    void ClimbAndMove()
    {
        isMoving = false;

        if (onPlatform == "Ground" || onPlatform == "KD Ground")
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y + 20), moveSpeed * Time.deltaTime);
        }
        else if (onPlatform == "Upper-Ground 1" || onPlatform == "Upper-Ground 2" || onPlatform == "Upper-Ground 3" || onPlatform == "Upper-Ground 4" || onPlatform == "KD Middle-Ground")
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y - 20), moveSpeed * Time.deltaTime);
        }
    }

}
