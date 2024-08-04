using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;

//using UnityEditor.Timeline;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.EventSystems;

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

    //tp 
    private bool teleporting = false;
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

    public TutorialPhase tutorialPhase;
    public GameObject instruction1;
    public GameObject instruction2;
    public GameObject attackRangeLabel;
    public GameObject rightClickPosition;
    public GameObject instruction3;
    public GameObject instruction6;
    public GameObject tutorialPanel;
    public GameObject instruction8;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Killdozer.gameOver)
        {
            return;
        }

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

            // Check if the mouse is over a UI element
            //if (!EventSystem.current.IsPointerOverGameObject())
            //{
                if (hit.collider != null && hit.collider.CompareTag("Troop") && cameraSystem.isZoomedOut == false)
                {
                    if (selectedTroop != null) //if already have a troop selected
                    {
                        DeselectTroop(); //deselect old troop
                    }

                    if (tutorialPhase.tutorialOn == true)
                    {
                       if (instruction1.activeInHierarchy == true) //tutorial selecting troop
                       {
                          instruction1.SetActive(false);
                          instruction2.SetActive(true);
                          attackRangeLabel.SetActive(true);
                       }
                    }
                    else if (tutorialPhase.tutorialOn == false && tutorialPhase.instruction8On == true)
                    {
                        instruction8.SetActive(false);
                        tutorialPhase.EnableEdgePanTutorial();
                    }


                    SelectTroop(hit.collider.gameObject);
                }
                /*
                else if (cameraSystem != null && cameraSystem.isZoomedOut && selectedTroop != null && selectedTroop && energySystem.currentEnergy >= 50 && teleporting == false) //clicking when zoomed out
                {
                    Debug.Log("Teleport");
                    if (teleporting == false)
                    {
                        teleporting = true;
                        StartCoroutine(Teleportation());
                    }
                }
                */
                else if (selectedTroop != null && !EventSystem.current.IsPointerOverGameObject()) // clicking elsewhere besides ui to deselect troop
                {
                    DeselectTroop();
                }
            //}
        }
        else if (Input.GetMouseButtonDown(1) && selectedTroop != null) // Right click
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            int layerMask = LayerMask.GetMask("Pathfinding"); 
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);

            bool GoingLeft = mousePosition.x < selectedTroop.transform.position.x;
            Vector2 pos = new Vector2(mousePosition.x, transform.position.y);

            if (tutorialPhase.tutorialOn == true && instruction2.activeInHierarchy == true) //tutorial 
            {
                //TutorialRightClick();
            }

            if (GoingLeft == true)
            {
                selectedTroop.GetComponent<TroopClass>().ONCE = false;
                if (selectedTroop.GetComponent<TroopClass>().BlockedLeft == true)
                {
                    // stop moving
                    selectedTroop.GetComponent<TroopClass>().arrow.GetComponent<TroopPathfindArrow>().pathfindIcon.SetBool("x", true); // Pathfind UI
                    //selectedTroop.GetComponent<TroopClass>().GoingLeft = true;
                }
                else if (selectedTroop.GetComponent<TroopClass>().BlockedLeft == false)
                {
                    if (hit.collider != null)
                    {
                        selectedTroop.GetComponent<TroopClass>().SetTroopTargetPosition(mousePosition, hit); // start moving
                        selectedTroop.GetComponent<TroopClass>().arrow.GetComponent<TroopPathfindArrow>().pathfindIcon.SetBool("x", false); // Pathfind UI

                    }
                }
            }
            else if (GoingLeft == false)
            {
                selectedTroop.GetComponent<TroopClass>().ONCE2 = false;
                if (selectedTroop.GetComponent<TroopClass>().BlockedRight == true)
                {
                    selectedTroop.GetComponent<TroopClass>().arrow.GetComponent<TroopPathfindArrow>().pathfindIcon.SetBool("x", true);
                }
                else if (selectedTroop.GetComponent<TroopClass>().BlockedRight == false)
                {
                    if (hit.collider != null)
                    {
                        selectedTroop.GetComponent<TroopClass>().SetTroopTargetPosition(mousePosition, hit);
                        selectedTroop.GetComponent<TroopClass>().arrow.GetComponent<TroopPathfindArrow>().pathfindIcon.SetBool("x", false);
                    }
                }
            }
        }
    }

    public void TutorialLeftClick()
    {
        instruction1.SetActive(false);
        instruction2.SetActive(true);
        attackRangeLabel.SetActive(true);
        rightClickPosition.SetActive(true);
    }

    public void TutorialRightClick()
    {
        instruction2.SetActive(false);
        attackRangeLabel.SetActive(false);
        instruction3.SetActive(true);
        Time.timeScale = 0f;
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
        if (WaveSystem.transitioning == true || cameraSystem.isZoomedOut == true)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (troop1.activeInHierarchy == true)
            {
                HandleKeyPress(KeyCode.Alpha1, troop1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (troop2.activeInHierarchy == true)
            {
                HandleKeyPress(KeyCode.Alpha2, troop2);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (troop3.activeInHierarchy == true)
            {
                HandleKeyPress(KeyCode.Alpha3, troop3);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (troop4.activeInHierarchy == true)
            {
                HandleKeyPress(KeyCode.Alpha4, troop4);
            }
        }
    }

    void HandleKeyPress(KeyCode key, GameObject troop)
    {
        if (WaveSystem.transitioning == true || tutorialPhase.tutorialOn == true)
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
        Troop troopScript = selectedTroop.GetComponent<Troop>();
        troopScript.ChangeIconColour();
        troopScript.selected = true;
        //troopScript.highlight.SetActive(true);
        troopScript.arrow.SetActive(true);
        if (selectedTroop.GetComponent<TroopAttackRange>() != null)
        {
            selectedTroop.GetComponent<TroopAttackRange>().DrawCircle();
        }
        //Debug.Log("Troop selected: " + selectedTroop.name);
    }

    public void DeselectTroop() 
    {
        if (selectedTroop != null)
        {
            Troop troopScript = selectedTroop.GetComponent<Troop>();
            troopScript.ChangeIconColour();
            troopScript.selected = false;
            //troopScript.highlight.SetActive(false);
            troopScript.arrow.SetActive(false);

            if (selectedTroop.GetComponent<TroopAttackRange>() != null)
            {
                selectedTroop.GetComponent<TroopAttackRange>().DisableCircle();
            }
        }

        selectedTroop = null;
        cameraSystem.DefocusTroop();
    }

    public void HandleRespawn(Troop troop)
    {
        StartCoroutine(RespawnTroop(troop));
        //troop.TroopModel.GetComponent<TroopAnimationsManager>().TroopRespawn();
    }

    IEnumerator RespawnTroop(Troop troop)
    {
        troop.GetComponent<TroopClass>().onPlatform = "KD Middle-Ground";

        yield return new WaitForSeconds(10f);

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

        troop.tpObject.SetActive(true);
        troop.animator.SetTrigger("Death");

        DeselectTroop();

        yield return new WaitForSeconds(0.5f);

        TroopAutoAttack autoAttack = troop.gameObject.GetComponent<TroopAutoAttack>();
        if (autoAttack != null)
        {
            autoAttack.autoAttackEnabled = true;
        }
        else
        {
            troop.gameObject.GetComponent<HealerAutoHeal>().autoHealEnabled = true; 
        }

        foreach (SpriteRenderer sprite in troop.troopSprite)
        {
            sprite.color = troop.NormalColor;
        }

        //troop.TroopModel.GetComponent<TroopAnimationsManager>().TroopRespawn();
        //troop.TroopModel.GetComponent<TroopAnimationsManager>().TroopIdle();
        //troop.TroopModel.GetComponent<TroopAnimationsManager>().TroopIdleOn();

        troop.model.SetActive(true);

        //reset mouse position
        troop.GetComponent<TroopClass>().mousePosition = killdozer.position;

        StartCoroutine(DisableTPObject(troop));
    }

    IEnumerator DisableTPObject(Troop troop)
    {
        yield return new WaitForSeconds(3f);

        troop.tpObject.SetActive(false);
    }

    /*
    IEnumerator Teleportation()
    {
        Vector3 newPosition = selectedTroop.transform.position;
        Vector2 MousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(MousePosition, Vector2.zero);

        troopPathfinding = selectedTroop.GetComponent<TroopClass>(); //change the onPlatform value with this

        foreach (var Hit in hits)
        {
            if (Hit.collider != null && Hit.collider.CompareTag("[TP] Ground") || Hit.collider.CompareTag("[TP] Upper-Ground 1") || Hit.collider.CompareTag("[TP] Upper-Ground 2")
                || Hit.collider.CompareTag("[TP] Upper-Ground 3") || Hit.collider.CompareTag("[TP] Upper-Ground 4") || Hit.collider.CompareTag("[TP] Upper-Ground 1 (2)") 
                || Hit.collider.CompareTag("[TP] Upper-Ground 2 (2)") || Hit.collider.CompareTag("[TP] Upper-Ground 3 (2)") || Hit.collider.CompareTag("[TP] Upper-Ground 4 (2)"))
            {
                if (Hit.collider != null)
                {
                    if (Hit.collider.CompareTag("[TP] Ground"))
                    {
                        newPosition.x = MousePosition.x;
                        newPosition.y = -1; //Y value for ground
                        troopPathfinding.onPlatform = "Ground";
                    }
                    else if (Hit.collider.CompareTag("[TP] Upper-Ground 1"))
                    {
                        newPosition.x = MousePosition.x;
                        newPosition.y = 4.7f; //Y value for platform 1
                        troopPathfinding.onPlatform = "Upper-Ground 1";
                    }
                    else if (Hit.collider.CompareTag("[TP] Upper-Ground 2"))
                    {
                        newPosition.x = MousePosition.x;
                        newPosition.y = 4.7f; 
                        troopPathfinding.onPlatform = "Upper-Ground 2";
                    }
                    else if (Hit.collider.CompareTag("[TP] Upper-Ground 3"))
                    {
                        newPosition.x = MousePosition.x;
                        newPosition.y = 4.7f;
                        troopPathfinding.onPlatform = "Upper-Ground 3";
                    }
                    else if (Hit.collider.CompareTag("[TP] Upper-Ground 4"))
                    {
                        newPosition.x = MousePosition.x;
                        newPosition.y = 4.7f;
                        troopPathfinding.onPlatform = "Upper-Ground 4";
                    }
                    else if (Hit.collider.CompareTag("[TP] Upper-Ground 1 (2)"))
                    {
                        newPosition.x = MousePosition.x;
                        newPosition.y = 9.3f; //Y value for platform 2
                        troopPathfinding.onPlatform = "Upper-Ground 1 (2)";
                    }
                    else if (Hit.collider.CompareTag("[TP] Upper-Ground 2 (2)"))
                    {
                        newPosition.x = MousePosition.x;
                        newPosition.y = 9.3f;
                        troopPathfinding.onPlatform = "Upper-Ground 2 (2)";
                    }
                    else if (Hit.collider.CompareTag("[TP] Upper-Ground 3 (2)"))
                    {
                        newPosition.x = MousePosition.x;
                        newPosition.y = 9.3f;
                        troopPathfinding.onPlatform = "Upper-Ground 3 (2)";
                    }
                    else if (Hit.collider.CompareTag("[TP] Upper-Ground 4 (2)"))
                    {
                        newPosition.x = MousePosition.x;
                        newPosition.y = 9.3f;
                        troopPathfinding.onPlatform = "Upper-Ground 4 (2)";
                    }
                    else
                    {
                        newPosition.x = MousePosition.x;
                    }
                }
                break;
            }
        }

        if (selectedTroop == troop1)
        {
            animator = tpAnimation1.GetComponent<Animator>();
            tpAnimation1.SetActive(true);
            troop1.GetComponent<Troop>().invincible = true;
            troop1.GetComponent<TroopAutoAttack>().targetEnemy = null;
        }
        else if (selectedTroop == troop2)
        {
            animator = tpAnimation2.GetComponent<Animator>();
            tpAnimation2.SetActive(true);
            troop2.GetComponent<Troop>().invincible = true;
            troop2.GetComponent<TroopAutoAttack>().targetEnemy = null;
        }
        else if (selectedTroop == troop3)
        {
            animator = tpAnimation3.GetComponent<Animator>();
            tpAnimation3.SetActive(true);
            troop3.GetComponent<Troop>().invincible = true;
            troop3.GetComponent<TroopAutoAttack>().targetEnemy = null;
        }
        else if (selectedTroop == troop4)
        {
            animator = tpAnimation4.GetComponent<Animator>();
            tpAnimation4.SetActive(true);
            troop4.GetComponent<Troop>().invincible = true;
            troop4.GetComponent<HealerAutoHeal>().targetAlly = null;
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

        //TroopAutoAttack autoAttack = selectedTroop.GetComponent<TroopAutoAttack>();

        if (selectedTroop == troop1)
        {
            troop1.GetComponent<Troop>().invincible = false;
            //autoAttack = troop1.GetComponent<TroopAutoAttack>();
            //autoAttack.lastAttackTime = autoAttack.attackCooldown; //reset attack cooldown
            //autoAttack.canAttack = false;
        }
        else if (selectedTroop == troop2)
        {
            troop2.GetComponent<Troop>().invincible = false;
            //autoAttack = troop2.GetComponent<TroopAutoAttack>();
            //autoAttack.lastAttackTime = autoAttack.attackCooldown;
        }
        else if (selectedTroop == troop3)
        {
            troop3.GetComponent<Troop>().invincible = false;
            //autoAttack = troop3.GetComponent<TroopAutoAttack>();
            //autoAttack.lastAttackTime = autoAttack.attackCooldown;
        }
        else if (selectedTroop == troop4)
        {
            troop4.GetComponent<Troop>().invincible = false;
        }

        if (tutorialPhase.tutorialOn == false)
        {
            cameraSystem.ToggleZoom();
            teleporting = false;
        }
        if (tutorialPhase.tutorialOn == true)
        {
            teleporting = false;
            cameraSystem.isZoomedOut = false;
            StartCoroutine(TutorialDelay());
        }
    }
   

    IEnumerator TutorialDelay()
    {
        yield return new WaitForSeconds(1f);

        instruction6.SetActive(false);
        tutorialPanel.SetActive(true);
        Debug.Log("Tutorial Panel On");
        Time.timeScale = 0.0f;
    }
  */
}

