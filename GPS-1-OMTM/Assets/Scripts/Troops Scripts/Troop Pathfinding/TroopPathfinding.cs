using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class TroopClass : MonoBehaviour
{

    private Camera mainCamera;
    private Vector2 targetPosition;
    public bool isMoving;

    public bool canClimb;
    public bool climbingUp;

    private Collider2D nearestVert;
    private Vector2 vertPosition;
    private Rigidbody2D rb;
    public string onPlatform;

    public float moveSpeed = 5f; // Speed of the troop movement

    public GameObject TroopController2D;
    TroopController2D controllerScript;

    public GameObject OnKDCollider;
    OnKDDetector OnKDScript;

    public Vector2 mousePosition = new Vector2(10000, 10000);
    public RaycastHit2D hit;
    public Transform killdozer;

    public TroopWeapon troopWeapon;

    bool teleported;
    float timer;

    [Header(" Art / Animations ")]

    public GameObject TroopModel;
    TroopAnimationsManager TroopAnimator;
    public bool GoingLeft;
    float moveDelayTimer;
    float moveDelay;
    float climbDelayTimer;
    float climbDelay;
    float fallDelay;

    public float MiddleGroundYSnap;
    public float UpperGroundYSnap;

    private void Start()
    {
        mainCamera = Camera.main;
        rb = gameObject.GetComponent<Rigidbody2D>();
        controllerScript = TroopController2D.GetComponent<TroopController2D>();
        OnKDScript = OnKDCollider.GetComponent<OnKDDetector>();

        TroopAnimator = TroopModel.GetComponent<TroopAnimationsManager>();

        DetermineMoveSpeed();
        SetTargetPositionHere();
    }

    public void DetermineMoveSpeed() //determine movement speed & climbDelay + fallDelay & y-axis height based on selected weapon 
    {
        switch (troopWeapon.selectedWeapon)
        {
            case TroopWeapon.Weapon.Weapon1_DPS:
                moveSpeed = 8;
                climbDelay = 0.5f;
                fallDelay = 0.1f;
                MiddleGroundYSnap = 4.75f;
                break;
            case TroopWeapon.Weapon.Weapon2_DPS:
                moveSpeed = 8; //here is 8 over 6 to match his climbing animation better - bray
                climbDelay = 0.5f;
                fallDelay = 0.1f;
                MiddleGroundYSnap = 4.75f;
                break;
            case TroopWeapon.Weapon.Weapon1_Tank:
                moveSpeed = 5; 
                climbDelay = 0.75f;
                fallDelay = 0.5f;
                MiddleGroundYSnap = 4.75f;
                break;
            case TroopWeapon.Weapon.Weapon2_Tank:
                moveSpeed = 7;
                climbDelay = 0.5f;
                fallDelay = 0.3f;
                MiddleGroundYSnap = 4.75f;
                break;
            case TroopWeapon.Weapon.Weapon1_CC:
                moveSpeed = 7;
                climbDelay = 0.1f;
                fallDelay = 0.1f;
                MiddleGroundYSnap = 4.75f;
                break;
            case TroopWeapon.Weapon.Weapon2_CC:
                moveSpeed = 7;
                climbDelay = 0.45f;
                fallDelay = 0.3f;
                MiddleGroundYSnap = 4.75f;
                break;
            case TroopWeapon.Weapon.Weapon1_Healer:
                moveSpeed = 6;
                climbDelay = 0.5f;
                fallDelay = 0.1f;
                MiddleGroundYSnap = 4.75f;
                break;
            case TroopWeapon.Weapon.Weapon2_Healer:
                moveSpeed = 6;
                climbDelay = 0.5f;
                fallDelay = 0.1f;
                MiddleGroundYSnap = 4.75f;
                break;
        }
    }

    public void SetTargetPositionHere() //used after teleporting to stop troops from moving or climbing
    {
        targetPosition = transform.position;
        teleported = true;
        isMoving = false;
        canClimb = false;
        arrow.SetActive(false);
    }

    // After Player's Right Click, determine the position & how troop should pathfind to thr
    public void SetTroopTargetPosition(Vector2 mP, RaycastHit2D h)
    {
        if (teleported == false && gameObject.activeSelf == true)
        {
            mousePosition = mP;
            hit = h;


            // 1a. if click on any part of killdozer middleground
            if (hit.collider != null && hit.collider.CompareTag("[PF] KD Middle-Ground")
            || hit.collider != null && hit.collider.CompareTag("[PF] KD Vertically 2"))
            {
                if (onPlatform == "KD Middle-Ground")
                {

                    targetPosition = new Vector2(mousePosition.x, transform.position.y);
                    vertPosition = Vector2.zero;
                    nearestVert = null;

                }
                else if (onPlatform == "KD Upper-Ground")
                {

                    FindAndGoToNearestVerticallyKD2(mousePosition);
                    climbingUp = false;

                }
                else if (onPlatform == "Ground" || onPlatform == "KD Ground")
                {
                    FindAndGoToNearestVerticallyKD(mousePosition);
                    climbingUp = true;

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
                            climbingUp = false;
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
                            climbingUp = false;
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
                            climbingUp = false;
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
                            climbingUp = false;
                        }
                    }

                }
                else if (onPlatform == "Upper-Ground 1 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically1TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 2 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically2TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 3 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically3TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 4 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically4TWO(mousePosition);
                        climbingUp = false;
                    }
                }
            }

            // 1b. if click on any part of killdozer upperground
            else if (hit.collider != null && hit.collider.CompareTag("[PF] KD Upper-Ground"))
            {
                if (onPlatform == "KD Upper-Ground")
                {

                    targetPosition = new Vector2(mousePosition.x, transform.position.y);
                    vertPosition = Vector2.zero;
                    nearestVert = null;

                }
                else if (onPlatform == "KD Middle-Ground")
                {
                    FindAndGoToNearestVerticallyKD2(mousePosition);
                    climbingUp = true;

                }
                else if (onPlatform == "Ground" || onPlatform == "KD Ground")
                {
                    FindAndGoToNearestVerticallyKD(mousePosition);
                    climbingUp = true;
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
                            climbingUp = false;
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
                            climbingUp = false;
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
                            climbingUp = false;
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
                            climbingUp = false;
                        }
                    }

                }
                else if (onPlatform == "Upper-Ground 1 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically1TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 2 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically2TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 3 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically3TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 4 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically4TWO(mousePosition);
                        climbingUp = false;
                    }
                }

            }

            // 2a. if click on UG 1
            else if (hit.collider != null && hit.collider.CompareTag("[PF] Upper-Ground 1")
                || hit.collider != null && hit.collider.CompareTag("[PF] Vertically 1 (2)"))
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
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 3")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically3(mousePosition);
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Upper-Ground 4")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically4(mousePosition);
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Upper-Ground 1 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically1TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 2 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically2TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 3 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically3TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 4 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically4TWO(mousePosition);
                        climbingUp = false;
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
                            climbingUp = false;
                        }
                    }

                }
                else if (onPlatform == "KD Upper-Ground")
                {
                    /*                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS ON 4 === //
                                        if (killdozer.GetComponent<Killdozer>().directPathfinding == 4)
                                        {
                                            targetPosition = new Vector2(mousePosition.x, transform.position.y);
                                            vertPosition = Vector2.zero;
                                            nearestVert = null;
                                        }
                                        // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS other than 4 === //
                                        else
                                        {}*/
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVerticallyKD2(mousePosition);
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Ground")
                {
                    FindAndGoToNearestVertically1(mousePosition);
                    climbingUp = true;
                }

                /*else if (onPlatform == "KD Ground")
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

                }*/
            }

            // 3a. if click on UG 2
            else if (hit.collider != null && hit.collider.CompareTag("[PF] Upper-Ground 2")
                || hit.collider != null && hit.collider.CompareTag("[PF] Vertically 2 (2)"))
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
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Upper-Ground 3")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically3(mousePosition);
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Upper-Ground 4")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically4(mousePosition);
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Upper-Ground 1 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically1TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 2 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically2TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 3 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically3TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 4 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically4TWO(mousePosition);
                        climbingUp = false;
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
                            climbingUp = false;
                        }
                    }
                }
                else if (onPlatform == "KD Upper-Ground")
                {
                    /*                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS ON 4 === //
                                        if (killdozer.GetComponent<Killdozer>().directPathfinding == 4)
                                        {
                                            targetPosition = new Vector2(mousePosition.x, transform.position.y);
                                            vertPosition = Vector2.zero;
                                            nearestVert = null;
                                        }
                                        // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS other than 4 === //
                                        else
                                        {}*/
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVerticallyKD2(mousePosition);
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Ground")
                {

                    FindAndGoToNearestVertically2(mousePosition);
                    climbingUp = true;
                }
                /*else if (onPlatform == "KD Ground")
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

                }*/
            }

            // 4a. if click on UG 3      
            else if (hit.collider != null && hit.collider.CompareTag("[PF] Upper-Ground 3")
                || hit.collider != null && hit.collider.CompareTag("[PF] Vertically 3 (2)"))
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
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Upper-Ground 2")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically2(mousePosition);
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Upper-Ground 4")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically4(mousePosition);
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Upper-Ground 1 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically1TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 2 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically2TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 3 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically3TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 4 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically4TWO(mousePosition);
                        climbingUp = false;
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
                            climbingUp = false;
                        }
                    }
                }
                else if (onPlatform == "KD Upper-Ground")
                {
                    /*                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS ON 4 === //
                                        if (killdozer.GetComponent<Killdozer>().directPathfinding == 4)
                                        {
                                            targetPosition = new Vector2(mousePosition.x, transform.position.y);
                                            vertPosition = Vector2.zero;
                                            nearestVert = null;
                                        }
                                        // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS other than 4 === //
                                        else
                                        {}*/
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVerticallyKD2(mousePosition);
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Ground")
                {

                    FindAndGoToNearestVertically3(mousePosition);
                    climbingUp = true;
                }
                /*else if (onPlatform == "KD Ground")
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
                }*/
            }

            // 5a. if click on UG 4      
            else if (hit.collider != null && hit.collider.CompareTag("[PF] Upper-Ground 4")
            || hit.collider != null && hit.collider.CompareTag("[PF] Vertically 4 (2)"))
            {

                if (onPlatform == "Upper-Ground 4")
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
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Upper-Ground 2")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically2(mousePosition);
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Upper-Ground 3")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically3(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 1 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically1TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 2 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically2TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 3 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically3TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 4 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically4TWO(mousePosition);
                        climbingUp = false;
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
                            climbingUp = false;
                        }
                    }
                }
                else if (onPlatform == "KD Upper-Ground")
                {
                    /*                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS ON 4 === //
                                        if (killdozer.GetComponent<Killdozer>().directPathfinding == 4)
                                        {
                                            targetPosition = new Vector2(mousePosition.x, transform.position.y);
                                            vertPosition = Vector2.zero;
                                            nearestVert = null;
                                        }
                                        // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS other than 4 === //
                                        else
                                        {}*/
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVerticallyKD2(mousePosition);
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Ground")
                {

                    FindAndGoToNearestVertically4(mousePosition);
                    climbingUp = true;
                }
                /*else if (onPlatform == "KD Ground")
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
                }*/
            }

            // 5b. if click on UG 4 (2)      
            else if (hit.collider != null && hit.collider.CompareTag("[PF] Upper-Ground 1 (2)"))
            {

                if (onPlatform == "Upper-Ground 1 (2)")
                {
                    targetPosition = new Vector2(mousePosition.x, transform.position.y);
                    vertPosition = Vector2.zero;
                    nearestVert = null;

                }
                else if (onPlatform == "Upper-Ground 4 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically4TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 2 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically2TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 3 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically3TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 4")
                {
                    FindAndGoToNearestVertically4(mousePosition);
                    climbingUp = false;

                }
                else if (onPlatform == "Upper-Ground 1")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically1TWO(mousePosition);
                        climbingUp = true;
                    }

                }
                else if (onPlatform == "Upper-Ground 2")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically2(mousePosition);
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Upper-Ground 3")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically3(mousePosition);
                        climbingUp = false;
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
                            climbingUp = false;
                        }
                    }
                }
                else if (onPlatform == "KD Upper-Ground")
                {
                    /*                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS ON 4 === //
                                        if (killdozer.GetComponent<Killdozer>().directPathfinding == 4)
                                        {
                                            targetPosition = new Vector2(mousePosition.x, transform.position.y);
                                            vertPosition = Vector2.zero;
                                            nearestVert = null;
                                        }
                                        // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS other than 4 === //
                                        else
                                        {}*/
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVerticallyKD2(mousePosition);
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Ground")
                {

                    FindAndGoToNearestVertically1(mousePosition);
                    climbingUp = true;
                }
                /*else if (onPlatform == "KD Ground")
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
                }*/
            }
            // 5b. if click on UG 4 (2)      
            else if (hit.collider != null && hit.collider.CompareTag("[PF] Upper-Ground 2 (2)"))
            {

                if (onPlatform == "Upper-Ground 2 (2)")
                {
                    targetPosition = new Vector2(mousePosition.x, transform.position.y);
                    vertPosition = Vector2.zero;
                    nearestVert = null;

                }
                else if (onPlatform == "Upper-Ground 1 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically1TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 4 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically4TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 3 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically3TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 4")
                {
                    FindAndGoToNearestVertically4(mousePosition);
                    climbingUp = false;

                }
                else if (onPlatform == "Upper-Ground 1")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically1(mousePosition);
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Upper-Ground 2")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically2TWO(mousePosition);
                        climbingUp = true;
                    }

                }
                else if (onPlatform == "Upper-Ground 3")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically3(mousePosition);
                        climbingUp = false;
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
                            climbingUp = false;
                        }
                    }
                }
                else if (onPlatform == "KD Upper-Ground")
                {
                    /*                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS ON 4 === //
                                        if (killdozer.GetComponent<Killdozer>().directPathfinding == 4)
                                        {
                                            targetPosition = new Vector2(mousePosition.x, transform.position.y);
                                            vertPosition = Vector2.zero;
                                            nearestVert = null;
                                        }
                                        // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS other than 4 === //
                                        else
                                        {}*/
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVerticallyKD2(mousePosition);
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Ground")
                {

                    FindAndGoToNearestVertically2(mousePosition);
                    climbingUp = true;
                }
                /*else if (onPlatform == "KD Ground")
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
                }*/
            }
            // 5b. if click on UG 4 (2)      
            else if (hit.collider != null && hit.collider.CompareTag("[PF] Upper-Ground 3 (2)"))
            {

                if (onPlatform == "Upper-Ground 3 (2)")
                {
                    targetPosition = new Vector2(mousePosition.x, transform.position.y);
                    vertPosition = Vector2.zero;
                    nearestVert = null;

                }
                else if (onPlatform == "Upper-Ground 1 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically1TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 2 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically2TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 4 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically4TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 4")
                {
                    FindAndGoToNearestVertically4(mousePosition);
                    climbingUp = false;

                }
                else if (onPlatform == "Upper-Ground 1")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically1(mousePosition);
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Upper-Ground 2")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically2(mousePosition);
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Upper-Ground 3")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically3TWO(mousePosition);
                        climbingUp = true;
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
                            climbingUp = false;
                        }
                    }
                }
                else if (onPlatform == "KD Upper-Ground")
                {
                    /*                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS ON 4 === //
                                        if (killdozer.GetComponent<Killdozer>().directPathfinding == 4)
                                        {
                                            targetPosition = new Vector2(mousePosition.x, transform.position.y);
                                            vertPosition = Vector2.zero;
                                            nearestVert = null;
                                        }
                                        // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS other than 4 === //
                                        else
                                        {}*/
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVerticallyKD2(mousePosition);
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Ground")
                {

                    FindAndGoToNearestVertically3(mousePosition);
                    climbingUp = true;
                }
                /*else if (onPlatform == "KD Ground")
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
                }*/
            }
            // 5b. if click on UG 4 (2)      
            else if (hit.collider != null && hit.collider.CompareTag("[PF] Upper-Ground 4 (2)"))
            {

                if (onPlatform == "Upper-Ground 4 (2)")
                {
                    targetPosition = new Vector2(mousePosition.x, transform.position.y);
                    vertPosition = Vector2.zero;
                    nearestVert = null;

                }
                else if (onPlatform == "Upper-Ground 1 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically1TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 2 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically2TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 3 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically3TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 4")
                {
                    FindAndGoToNearestVertically4TWO(mousePosition);
                    climbingUp = true;

                }
                else if (onPlatform == "Upper-Ground 1")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically1(mousePosition);
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Upper-Ground 2")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically2(mousePosition);
                        climbingUp = false;
                    }

                }
                else if (onPlatform == "Upper-Ground 3")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically3(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "KD Middle-Ground")
                {
                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS ON 4 === //
                    if (killdozer.GetComponent<Killdozer>().directPathfinding == 4)
                    {
                        FindAndGoToNearestVerticallyKD2(mousePosition);
                        climbingUp = true;
                    }
                    // === IF DIRECTPATHING FROM KD's UPPERGROUNDS TO TERRAINS' UPPERGROUNDS IS other than 4 === //
                    else
                    {
                        if (onPlatform != "Ground")
                        {
                            FindAndGoToNearestVerticallyKD(mousePosition);
                            climbingUp = false;
                        }
                    }
                }
                else if (onPlatform == "KD Upper-Ground")
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
                            FindAndGoToNearestVerticallyKD2(mousePosition);
                            climbingUp = false;
                        }
                    }


                }
                else if (onPlatform == "Ground")
                {

                    FindAndGoToNearestVertically4(mousePosition);
                    climbingUp = true;
                }
                /*else if (onPlatform == "KD Ground")
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
                }*/
            }

            // 6a. If click on Ground or ground Verticallies
            else if (hit.collider != null && hit.collider.CompareTag("[PF] Ground")
            || hit.collider != null && hit.collider.CompareTag("[PF] KD Vertically 1")
            || hit.collider != null && hit.collider.CompareTag("[PF] Vertically 1")
            || hit.collider != null && hit.collider.CompareTag("[PF] Vertically 2")
            || hit.collider != null && hit.collider.CompareTag("[PF] Vertically 3")
            || hit.collider != null && hit.collider.CompareTag("[PF] Vertically 4"))
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
                    climbingUp = false;
                }
                else if (onPlatform == "KD Upper-Ground")
                {

                    FindAndGoToNearestVerticallyKD2(mousePosition);
                    climbingUp = false;
                }
                else if (onPlatform == "Upper-Ground 1")
                {
                    FindAndGoToNearestVertically1(mousePosition);
                    climbingUp = false;
                }
                else if (onPlatform == "Upper-Ground 2")
                {

                    FindAndGoToNearestVertically2(mousePosition);
                    climbingUp = false;
                }
                else if (onPlatform == "Upper-Ground 3")
                {

                    FindAndGoToNearestVertically3(mousePosition);
                    climbingUp = false;
                }
                else if (onPlatform == "Upper-Ground 4")
                {

                    FindAndGoToNearestVertically4(mousePosition);
                    climbingUp = false;
                }
                else if (onPlatform == "Upper-Ground 1 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically1TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 2 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically2TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 3 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically3TWO(mousePosition);
                        climbingUp = false;
                    }
                }
                else if (onPlatform == "Upper-Ground 4 (2)")
                {
                    if (onPlatform != "Ground")
                    {
                        FindAndGoToNearestVertically4TWO(mousePosition);
                        climbingUp = false;
                    }
                }
            }

            // if pathfind blocked by an enemy
            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("STOPPIN");
                targetPosition = transform.position;
                vertPosition = Vector2.zero;
                nearestVert = null;
            }

            // == END - now start Troop movement == //
            if (controllerScript.selectedTroop != null)
            {
                GetComponent<TroopClass>().SetTargetPosition(targetPosition, vertPosition, nearestVert, climbingUp);

                // to determine whether troop is going left or right rn, for animations

                if (nearestVert == null)
                {
                    if (targetPosition.x < gameObject.transform.position.x)
                    {
                        GoingLeft = true;
                    }
                    else if (targetPosition.x > gameObject.transform.position.x)
                    {
                        GoingLeft = false;
                    }

                }
                else if (nearestVert != null)
                {
                    if (vertPosition.x < gameObject.transform.position.x)
                    {
                        GoingLeft = true;
                    }
                    else if (vertPosition.x > gameObject.transform.position.x)
                    {
                        GoingLeft = false;
                    }
                }

                // For Pathfind Arrow

                arrow.SetActive(false);
                arrow.SetActive(true);

                // if ground level
                if (hit.collider.CompareTag("[PF] Ground")
                || hit.collider != null && hit.collider.CompareTag("[PF] KD Vertically 1")
                || hit.collider != null && hit.collider.CompareTag("[PF] Vertically 1")
                || hit.collider != null && hit.collider.CompareTag("[PF] Vertically 2")
                || hit.collider != null && hit.collider.CompareTag("[PF] Vertically 3")
                || hit.collider != null && hit.collider.CompareTag("[PF] Vertically 4"))
                {
                    arrow.transform.position = new Vector2(mousePosition.x, -2.34f);
                    arrow.GetComponent<TroopPathfindArrow>().bop();
                }


                // if middleground level
                if (hit.collider != null && hit.collider.CompareTag("[PF] KD Middle-Ground")
                || hit.collider != null && hit.collider.CompareTag("[PF] KD Vertically 2")
                || hit.collider != null && hit.collider.CompareTag("[PF] Upper-Ground 1")
                || hit.collider != null && hit.collider.CompareTag("[PF] Vertically 1 (2)")
                || hit.collider != null && hit.collider.CompareTag("[PF] Upper-Ground 2")
                || hit.collider != null && hit.collider.CompareTag("[PF] Vertically 2 (2)")
                || hit.collider != null && hit.collider.CompareTag("[PF] Upper-Ground 3")
                || hit.collider != null && hit.collider.CompareTag("[PF] Vertically 3 (2)")
                || hit.collider != null && hit.collider.CompareTag("[PF] Upper-Ground 4")
                || hit.collider != null && hit.collider.CompareTag("[PF] Vertically 4 (2)"))
                {
                    arrow.transform.position = new Vector2(mousePosition.x, 3.58f);
                    arrow.GetComponent<TroopPathfindArrow>().bop();
                }


                //if upperground level
                if (hit.collider.CompareTag("[PF] Upper-Ground 1 (2)")
                    || hit.collider.CompareTag("[PF] Upper-Ground 2 (2)")
                    || hit.collider.CompareTag("[PF] Upper-Ground 3 (2)")
                    || hit.collider.CompareTag("[PF] Upper-Ground 4 (2)")
                    || hit.collider.CompareTag("[PF] KD Upper-Ground"))
                {

                    arrow.transform.position = new Vector2(mousePosition.x, 8.24f);
                    arrow.GetComponent<TroopPathfindArrow>().bop();
                }


            }

        }
    }

    public GameObject arrow;

    public bool ONCE;
    public bool ONCE2;
    public bool BlockedLeft;
    public bool BlockedRight;

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
    void FindAndGoToNearestVerticallyKD2(Vector2 mousePosition)
    {
        GameObject[] verts = GameObject.FindGameObjectsWithTag("[PF] KD Vertically 2");

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
    void FindAndGoToNearestVertically1TWO(Vector2 mousePosition)
    {
        GameObject[] verts = GameObject.FindGameObjectsWithTag("[PF] Vertically 1 (2)");

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
    void FindAndGoToNearestVertically2TWO(Vector2 mousePosition)
    {
        GameObject[] verts = GameObject.FindGameObjectsWithTag("[PF] Vertically 2 (2)");

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
    void FindAndGoToNearestVertically3TWO(Vector2 mousePosition)
    {
        GameObject[] verts = GameObject.FindGameObjectsWithTag("[PF] Vertically 3 (2)");

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
    void FindAndGoToNearestVertically4TWO(Vector2 mousePosition)
    {
        GameObject[] verts = GameObject.FindGameObjectsWithTag("[PF] Vertically 4 (2)");

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

    // Assist Pathfinding by Changing Troop's "onPlatform" to the correct value
    private void OnTriggerEnter2D(Collider2D collision)
    {

        // from down to up
        if (onPlatform == "Ground" && collision.tag == "[PF] Upper-Ground 1" || onPlatform == "Upper-Ground 1" && collision.tag == "[PF] Upper-Ground 1")
        {
            Debug.Log("Upper Ground 1");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "Upper-Ground 1";
            SetTroopTargetPosition(mousePosition, hit);
        }
        else if (onPlatform == "Ground" && collision.tag == "[PF] Upper-Ground 2" || onPlatform == "Upper-Ground 2" && collision.tag == "[PF] Upper-Ground 2")
        {
            Debug.Log("Upper Ground 2");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "Upper-Ground 2";
            SetTroopTargetPosition(mousePosition, hit);
        }
        else if (onPlatform == "Ground" && collision.tag == "[PF] Upper-Ground 3" || onPlatform == "Upper-Ground 3" && collision.tag == "[PF] Upper-Ground 3")
        {
            Debug.Log("Upper Ground 3");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "Upper-Ground 3";
            SetTroopTargetPosition(mousePosition, hit);
        }
        else if (onPlatform == "Ground" && collision.tag == "[PF] Upper-Ground 4" || onPlatform == "Upper-Ground 4" && collision.tag == "[PF] Upper-Ground 4")
        {
            Debug.Log("Upper Ground 4");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "Upper-Ground 4";
            SetTroopTargetPosition(mousePosition, hit);
        }
        else if (onPlatform == "Ground" && collision.tag == "[PF] KD Middle-Ground")// || onPlatform == "KD Middle-Ground" && collision.tag == "[PF] KD Middle-Ground")
        {
            Debug.Log("KD Middle Ground");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "KD Middle-Ground";
            SetTroopTargetPosition(mousePosition, hit);
        }
        else if (onPlatform == "KD Middle-Ground" && collision.tag == "[PF] KD Upper-Ground")
        {
            Debug.Log("KD Upper Ground");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "KD Upper-Ground";
            SetTroopTargetPosition(mousePosition, hit);
        }
        else if (onPlatform == "Upper-Ground 1" && collision.tag == "[PF] Upper-Ground 1 (2)")
        {
            Debug.Log("Upper Ground 1 (2)");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "Upper-Ground 1 (2)";
            SetTroopTargetPosition(mousePosition, hit);
        }
        else if (onPlatform == "Upper-Ground 2" && collision.tag == "[PF] Upper-Ground 2 (2)")
        {
            Debug.Log("Upper Ground 2 (2)");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "Upper-Ground 2 (2)";
            SetTroopTargetPosition(mousePosition, hit);
        }
        else if (onPlatform == "Upper-Ground 3" && collision.tag == "[PF] Upper-Ground 3 (2)")
        {
            Debug.Log("Upper Ground 3 (2)");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "Upper-Ground 3 (2)";
            SetTroopTargetPosition(mousePosition, hit);
        }
        else if (onPlatform == "Upper-Ground 4" && collision.tag == "[PF] Upper-Ground 4 (2)")
        {
            Debug.Log("Upper Ground 4 (2)");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "Upper-Ground 4 (2)";
            SetTroopTargetPosition(mousePosition, hit);
        }

        //from up to down
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
            Debug.Log("Ground Check");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "Ground";
            SetTroopTargetPosition(mousePosition, hit);
        }
        else if (onPlatform == "KD Upper-Ground" && collision.tag == "[PF] KD Middle-Ground Check" || onPlatform == "KD Middle-Ground" && collision.tag == "[PF] KD Middle-Ground Check")
        {
            Debug.Log("KD Middle-Ground Check");
            transform.position = new Vector2(transform.position.x, MiddleGroundYSnap);//to snap to correct y position

            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "KD Middle-Ground";
            SetTroopTargetPosition(mousePosition, hit);
        }
        else if (onPlatform == "Upper-Ground 1 (2)" && collision.tag == "[PF] Upper-Ground 1 Check" || onPlatform == "Upper-Ground 1" && collision.tag == "[PF] Upper-Ground 1 Check")
        {
            Debug.Log("Upper-Ground 1 Check");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "Upper-Ground 1";
            SetTroopTargetPosition(mousePosition, hit);
        }
        else if (onPlatform == "Upper-Ground 2 (2)" && collision.tag == "[PF] Upper-Ground 2 Check" || onPlatform == "Upper-Ground 2" && collision.tag == "[PF] Upper-Ground 2 Check")
        {
            Debug.Log("Upper-Ground 2 Check");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "Upper-Ground 2";
            SetTroopTargetPosition(mousePosition, hit);
        }
        else if (onPlatform == "Upper-Ground 3 (2)" && collision.tag == "[PF] Upper-Ground 3 Check" || onPlatform == "Upper-Ground 3" && collision.tag == "[PF] Upper-Ground 3 Check")
        {
            Debug.Log("Upper-Ground 3 Check");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "Upper-Ground 3";
            SetTroopTargetPosition(mousePosition, hit);
        }
        else if (onPlatform == "Upper-Ground 4 (2)" && collision.tag == "[PF] Upper-Ground 4 Check" || onPlatform == "Upper-Ground 4" && collision.tag == "[PF] Upper-Ground 4 Check")
        {
            Debug.Log("Upper-Ground 4 Check");
            canClimb = false;
            isMoving = true;

            nearestVert = null;
            onPlatform = "Upper-Ground 4";
            SetTroopTargetPosition(mousePosition, hit);
        }

    }

    /*   MOST PROB NOT NEEDED - For when killdozer connects with another upperground
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
            canClimb = false;
       
            }

        }
    */

    public void SetTargetPosition(Vector2 targetPosition, Vector2 vertPosition, Collider2D nearestVert, bool climbingUp)
    {
        this.targetPosition = targetPosition;
        this.vertPosition = vertPosition;
        this.nearestVert = nearestVert;
        this.climbingUp = climbingUp;
        isMoving = true;
    }

    public GameObject Model;
    public void Update()
    {
        DetermineMoveSpeed();
        CorrectTroopDirection();

        if (canClimb)
        {
            ClimbAndMove();
            switch (troopWeapon.selectedWeapon)
            {
                case TroopWeapon.Weapon.Weapon1_DPS:
                    gameObject.GetComponent<TroopAutoAttack>().DeactivateAttackVisuals();
                    break;
                case TroopWeapon.Weapon.Weapon2_DPS:
                    gameObject.GetComponent<TroopAutoAttack>().DeactivateAttackVisuals();
                    break;
                case TroopWeapon.Weapon.Weapon1_Tank:
                    gameObject.GetComponent<TroopAutoAttack>().DeactivateAttackVisuals();
                    break;
                case TroopWeapon.Weapon.Weapon2_Tank:
                    gameObject.GetComponent<TroopAutoAttack>().DeactivateAttackVisuals();
                    break;
                case TroopWeapon.Weapon.Weapon1_CC:
                    gameObject.GetComponent<TroopAutoAttack>().DeactivateAttackVisuals();
                    break;
                case TroopWeapon.Weapon.Weapon2_CC:
                    gameObject.GetComponent<TroopAutoAttack>().DeactivateAttackVisuals();
                    break;
                case TroopWeapon.Weapon.Weapon1_Healer:
                    TroopAnimator.TroopAttackOff();
                    break;
                case TroopWeapon.Weapon.Weapon2_Healer:
                    TroopAnimator.TroopAttackOff();
                    break;
            }   
        }
        else
        {
            StopClimb();
        }

        if (isMoving)
        {
            Move();
            switch (troopWeapon.selectedWeapon)
            {
                case TroopWeapon.Weapon.Weapon1_DPS:
                    gameObject.GetComponent<TroopAutoAttack>().DeactivateAttackVisuals();
                    break;
                case TroopWeapon.Weapon.Weapon2_DPS:
                    gameObject.GetComponent<TroopAutoAttack>().DeactivateAttackVisuals();
                    break;
                case TroopWeapon.Weapon.Weapon1_Tank:
                    gameObject.GetComponent<TroopAutoAttack>().DeactivateAttackVisuals();
                    break;
                case TroopWeapon.Weapon.Weapon2_Tank:
                    gameObject.GetComponent<TroopAutoAttack>().DeactivateAttackVisuals();
                    break;
                case TroopWeapon.Weapon.Weapon1_CC:
                    gameObject.GetComponent<TroopAutoAttack>().DeactivateAttackVisuals();
                    break;
                case TroopWeapon.Weapon.Weapon2_CC:
                    gameObject.GetComponent<TroopAutoAttack>().DeactivateAttackVisuals();
                    break;
                case TroopWeapon.Weapon.Weapon1_Healer:
                    TroopAnimator.TroopAttackOff();
                    break;
                case TroopWeapon.Weapon.Weapon2_Healer:
                    TroopAnimator.TroopAttackOff();
                    break;
            }
        }
        else
        {
            StopMove();
        }

        if (teleported == true)
        {
            timer += Time.deltaTime;

            if (timer > 0.5f)
            {
                teleported = false;
                timer = 0;
            }
        }

    }

    void CorrectTroopDirection()
    {
        if (GoingLeft == true)
        {
            Model.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (GoingLeft == false)
        {
            Model.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    bool WalkOrStopAnimation;
    void Move()
    {

        if (WalkOrStopAnimation == false)
        {
            WalkOrStopAnimation = true;
            TroopAnimator.TroopWalkOn();
        }

        if (nearestVert != null) // if changing elevation
        {
            if (canClimb == false)
            {
                transform.position = Vector2.MoveTowards(transform.position, vertPosition, moveSpeed * Time.deltaTime); // move to nearest vert
                if (Vector2.Distance(transform.position, vertPosition) < 0.2f) // when reached vert, activate climb
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

    void StopMove()
    {
        if (WalkOrStopAnimation == true)
        {
            WalkOrStopAnimation = false;
            TroopAnimator.TroopWalkOff();
            //gameObject.GetComponent<TroopAutoAttack>().AttackModelParts.SetActive(false);
        }
    }

    void StopClimb()
    {
        if (ClimbAnimation == true)
        {
            ClimbAnimation = false;
            TroopAnimator.TroopClimbOff();
            TroopAnimator.TroopFallOff();
            //gameObject.GetComponent<TroopAutoAttack>().AttackModelParts.SetActive(false);
            climbDelayTimer = 0;
        }
    }

    bool ClimbAnimation;
    void ClimbAndMove()
    {
        isMoving = false;

        if (climbingUp == true)
        {
            if (ClimbAnimation == false)
            {
                ClimbAnimation = true;
                TroopAnimator.TroopClimbOn();
            }

            climbDelayTimer = climbDelayTimer + Time.deltaTime;

            if (climbDelayTimer > climbDelay)
            {
                switch (troopWeapon.selectedWeapon) // diff climbing speed depending on weapon
                {
                    case TroopWeapon.Weapon.Weapon1_DPS:
                        transform.position = new Vector2(transform.position.x, transform.position.y + ((moveSpeed) * Time.deltaTime));
                        break;
                    case TroopWeapon.Weapon.Weapon2_DPS:
                        transform.position = new Vector2(transform.position.x, transform.position.y + ((moveSpeed) * Time.deltaTime));
                        break;
                    case TroopWeapon.Weapon.Weapon1_Tank:
                        transform.position = new Vector2(transform.position.x, transform.position.y + ((moveSpeed + 3) * Time.deltaTime));
                        break;
                    case TroopWeapon.Weapon.Weapon2_Tank:
                        transform.position = new Vector2(transform.position.x, transform.position.y + ((moveSpeed) * Time.deltaTime));
                        break;
                    case TroopWeapon.Weapon.Weapon1_CC:
                        transform.position = new Vector2(transform.position.x, transform.position.y + ((moveSpeed) * Time.deltaTime));
                        break;
                    case TroopWeapon.Weapon.Weapon2_CC:
                        transform.position = new Vector2(transform.position.x, transform.position.y + ((moveSpeed) * Time.deltaTime));
                        break;
                    case TroopWeapon.Weapon.Weapon1_Healer:
                        transform.position = new Vector2(transform.position.x, transform.position.y + ((moveSpeed) * Time.deltaTime));
                        break;
                    case TroopWeapon.Weapon.Weapon2_Healer:
                        transform.position = new Vector2(transform.position.x, transform.position.y + ((moveSpeed) * Time.deltaTime));
                        break;
                }             
            }
        }

        else if (climbingUp == false)
        {
            if (ClimbAnimation == false)
            {
                ClimbAnimation = true;
                TroopAnimator.TroopFallOn();
            }

            climbDelayTimer = climbDelayTimer + Time.deltaTime;

            if (climbDelayTimer > fallDelay)
            {
                transform.position = new Vector2(transform.position.x, transform.position.y - ((10) * Time.deltaTime)); // same climbing speed even if diff weapon
            }
        }
    }
}
