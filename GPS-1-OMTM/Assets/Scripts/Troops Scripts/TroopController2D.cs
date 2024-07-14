using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;

//using UnityEditor.Timeline;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class TroopController2D : MonoBehaviour
{
    private Camera mainCamera;
    public GameObject selectedTroop;

    public float moveSpeed = 5f; // Speed of the troop movement
    public float ladderDetectionRange = 1f; // Range to detect ladders
    public float attackRange = 1.5f; // Range within which the troop can attack enemies

    //Troop Respawn 
    public float respawnTime = 5f; // respawn delay for troops
    public Vector2 respawnOffset; // offset from the killdozer's position for respawn
    public Transform killdozer; // killdozer's transform position

    //troops
    public GameObject troop1;
    public GameObject troop2;
    public GameObject troop3;
    public GameObject troop4;

    private float lastClickTime;
    private const float doubleClickTime = 0.3f;
    private KeyCode lastKeyPressed;
    private float lastKeyPressTime;
    private float doublePressTime = 0.3f;

    public CameraSystem cameraSystem;
    public EnergySystem energySystem;

    //tp animation
    private Animator animator;
    public GameObject tpAnimation1;
    public GameObject tpAnimation2;
    public GameObject tpAnimation3;
    public GameObject tpAnimation4;
    public GameObject troop1Sprite;
    public GameObject troop2Sprite;
    public GameObject troop3Sprite;
    public GameObject troop4Sprite;

    private TroopClass troopPathfinding;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleMouseInput();
        HandleNumberKeyInput();
        //HandleEnemySelection();
        DetectDoubleClick();

        if (WaveSystem.transitioning == true)
        {
            DeselectTroop();
        }
    }

    void HandleMouseInput()
    {
        if(WaveSystem.transitioning == true)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0)) // Left click
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            int layerMask = LayerMask.GetMask("Troop"); 
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);

            if (hit.collider != null && hit.collider.CompareTag("Troop"))
            {
                if (selectedTroop != null) //if already have a troop selected
                {
                    DeselectTroop(); //deselect old troop
                }
                SelectTroop(hit.collider.gameObject);
                
            }
            else if (cameraSystem != null && cameraSystem.isZoomedOut && selectedTroop != null && selectedTroop && energySystem.currentEnergy >= 50) //clicking when zoomed out
            {
                StartCoroutine(Teleportation());
            }
            else if (selectedTroop != null) // clicking elsewhere should deselect the troop
            {
                DeselectTroop();
            }

        }
        else if (Input.GetMouseButtonDown(1) && selectedTroop != null) // Right click
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            int layerMask = LayerMask.GetMask("Pathfinding"); 
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);

            bool GoingLeft = mousePosition.x < selectedTroop.transform.position.x;
            
            if (GoingLeft == true)
            {
                selectedTroop.GetComponent<TroopClass>().ONCE = false;
                if (selectedTroop.GetComponent<TroopClass>().BlockedLeft == true)
                {
                    Debug.Log("============== BLOCKED ON THE LEFT ===========");
                }
                else if (selectedTroop.GetComponent<TroopClass>().BlockedLeft == false)
                {
                    if (hit.collider != null)
                    {
                        selectedTroop.GetComponent<TroopClass>().SetTroopTargetPosition(mousePosition, hit);
                    }
                }
            }
            else if (GoingLeft == false)
            {
                selectedTroop.GetComponent<TroopClass>().ONCE2 = false;
                if (selectedTroop.GetComponent<TroopClass>().BlockedRight == true)
                {
                    Debug.Log("============== BLOCKED ON THE RUGHT ===========");
                }
                else if (selectedTroop.GetComponent<TroopClass>().BlockedRight == false)
                {
                    if (hit.collider != null)
                    {
                        selectedTroop.GetComponent<TroopClass>().SetTroopTargetPosition(mousePosition, hit);
                    }
                }
            }
        }
    }

    void DetectDoubleClick() //focus camera on troop
    {
        if (WaveSystem.transitioning == true)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0)) 
        {
            float timeSinceLastClick = Time.time - lastClickTime;
            lastClickTime = Time.time;

            if (timeSinceLastClick <= doubleClickTime)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

                if (hit.collider != null && hit.collider.CompareTag("Troop")) 
                {
                    selectedTroop = hit.collider.gameObject;
                    cameraSystem.FocusOnTroop(selectedTroop);
                }
            }
        }
    }

    void HandleNumberKeyInput()
    {
        if (WaveSystem.transitioning == true)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            HandleKeyPress(KeyCode.Alpha1, troop1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            HandleKeyPress(KeyCode.Alpha2, troop2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            HandleKeyPress(KeyCode.Alpha3, troop3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            HandleKeyPress(KeyCode.Alpha4, troop4);
        }
    }

    void HandleKeyPress(KeyCode key, GameObject troop)
    {
        if (WaveSystem.transitioning == true)
        {
            return;
        }

        if (key == lastKeyPressed && Time.time - lastKeyPressTime < doublePressTime)
        {
            // Double press
            CameraSystem cameraSystem = FindObjectOfType<CameraSystem>();
            if (cameraSystem != null)
            {
                cameraSystem.FocusOnTroop(troop);
            }
        }
        else
        {
            // Single press
            if (selectedTroop != null)
            {
                DeselectTroop();
            }
            SelectTroop(troop);

            lastKeyPressed = key;
            lastKeyPressTime = Time.time;
        }
    }

    public void SelectTroop(GameObject troop)
    {
        selectedTroop = troop;
        selectedTroop.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent rotation
        selectedTroop.GetComponent<Troop>().selected = true;
        selectedTroop.GetComponent<Troop>().highlight.SetActive(true);
        //Debug.Log("Troop selected: " + selectedTroop.name);
    }

    void DeselectTroop() 
    {
        selectedTroop.GetComponent<Troop>().selected = false; //necessary for platform drop off to work as intended
        selectedTroop.GetComponent<Troop>().highlight.SetActive(false);
        cameraSystem.DefocusTroop();
    }

 
/*    void HandleEnemySelection()
    {
        if (selectedTroop != null && Input.GetKeyDown(KeyCode.A))
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                GameObject enemy = hit.collider.gameObject;
                selectedTroop.GetComponent<Troop>().SetTargetEnemy(enemy, attackRange);
                Debug.Log("Enemy targeted: " + enemy.name);
            }
        }
    }*/

    public void HandleRespawn(Troop troop)
    {
        StartCoroutine(RespawnTroop(troop));
    }

    IEnumerator RespawnTroop(Troop troop)
    {
        yield return new WaitForSeconds(respawnTime);

        Vector2 respawnPosition = (Vector2)killdozer.position + respawnOffset;

        // Set the troop's position to the respawn location
        troop.transform.position = respawnPosition;

        // Reset troop health and target positions
        troop.currentHealth = troop.maxHealth;
        troop.UpdateHUD();

        // Reactivate the troop
        troop.gameObject.SetActive(true);
        troop.GetComponent<TroopClass>().SetTargetPositionHere();
        Debug.Log(troop.gameObject.name + " has respawned.");

        //reset mouse position
        troop.GetComponent<TroopClass>().mousePosition = killdozer.position;
    }

    IEnumerator Teleportation()
    {
        Vector3 newPosition = selectedTroop.transform.position;
        Vector2 MousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(MousePosition, Vector2.zero);

        troopPathfinding = selectedTroop.GetComponent<TroopClass>(); //change the onPlatform value with this

        foreach (var Hit in hits)
        {
            if (Hit.collider != null && Hit.collider.CompareTag("[TP] Ground") || Hit.collider.CompareTag("[TP] Platform") || Hit.collider.CompareTag("[TP] Platform 1"))
            {
                if (Hit.collider != null)
                {
                    if (Hit.collider.CompareTag("[TP] Ground"))
                    {
                        newPosition.x = MousePosition.x;
                        newPosition.y = -1; //Y value for ground

                    }
                    else if (Hit.collider.CompareTag("[TP] Platform"))
                    {
                        newPosition.x = MousePosition.x;
                        newPosition.y = 4.7f; //Y value for platform
                    }
                    else if (Hit.collider.CompareTag("[TP] Platform 1"))
                    {
                        newPosition.x = MousePosition.x;
                        newPosition.y = 9.3f; //Y value for platform
                    }
                    else
                    {
                        newPosition.x = MousePosition.x;
                    }
                }
                break;
            }
        }

        //animator = selectedTroop.GetComponent<Animator>();

        if (selectedTroop == troop1)
        {
            animator = tpAnimation1.GetComponent<Animator>();
            tpAnimation1.SetActive(true);
        }
        else if (selectedTroop == troop2)
        {
            animator = tpAnimation2.GetComponent<Animator>();
            tpAnimation2.SetActive(true);
        }
        else if (selectedTroop == troop3)
        {
            animator = tpAnimation3.GetComponent<Animator>();
            tpAnimation3.SetActive(true);
        }
        else if (selectedTroop == troop4)
        {
            animator = tpAnimation4.GetComponent<Animator>();
            tpAnimation4.SetActive(true);
        }

        animator.SetTrigger("TP");

        yield return new WaitForSeconds(0.075f);

        //disable sprite
        if (selectedTroop == troop1)
        {
            troop1Sprite.SetActive(false);
        }
        else if (selectedTroop == troop2)
        {
            troop2Sprite.SetActive(false);
        }
        else if (selectedTroop == troop3)
        {
            troop3Sprite.SetActive(false);
        }
        else if (selectedTroop == troop4)
        {
            troop4Sprite.SetActive(false);
        }

        yield return new WaitForSeconds(0.15f); //tp delay (slowed by 0.25 speed so its 0.6 seconds delay)

        //enable sprite
        if (selectedTroop == troop1)
        {
            troop1Sprite.SetActive(true);
        }
        else if (selectedTroop == troop2)
        {
            troop2Sprite.SetActive(true);
        }
        else if (selectedTroop == troop3)
        {
            troop3Sprite.SetActive(true);
        }
        else if (selectedTroop == troop4)
        {
            troop4Sprite.SetActive(true);
        }

        selectedTroop.transform.position = newPosition;
        selectedTroop.GetComponent<TroopClass>().SetTargetPositionHere();
        energySystem.UseEnergy(50f);

        animator.SetTrigger("TP");

        yield return new WaitForSeconds(0.075f);

        cameraSystem.ToggleZoom();
    }
}

