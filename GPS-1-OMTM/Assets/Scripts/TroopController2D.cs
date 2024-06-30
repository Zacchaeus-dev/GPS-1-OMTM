using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
//using UnityEditor.Timeline;
using UnityEngine;

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

    public CameraSystem cameraSystem;
    public EnergySystem energySystem;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleMouseInput();
        HandleNumberKeyInput();
        //HandleEnemySelection();
    }

    void HandleMouseInput()
    {
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
                Vector3 newPosition = selectedTroop.transform.position;
                Vector2 MousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D[] hits = Physics2D.RaycastAll(MousePosition, Vector2.zero);

                foreach (var Hit in hits)
                {
                    if (Hit.collider != null && Hit.collider.CompareTag("[TP] Ground") || Hit.collider.CompareTag("[TP] Platform"))
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
                                newPosition.y = 5; //Y value for platform
                            }
                            else
                            {
                                newPosition.x = MousePosition.x;
                            }
                        }
                        break; 
                    }
                }

                selectedTroop.transform.position = newPosition;
                selectedTroop.GetComponent<TroopClass>().SetTargetPositionHere();
                Debug.Log("Troop Teleported");
                energySystem.UseEnergy(50f);
                cameraSystem.ToggleZoom();
            }
        }
        else if (Input.GetMouseButtonDown(1) && selectedTroop != null) // Right click
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            int layerMask = LayerMask.GetMask("Pathfinding"); 
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);

            if (hit.collider != null)
            {
                selectedTroop.GetComponent<TroopClass>().SetTroopTargetPosition(mousePosition, hit);
            }
        }
    }

    void HandleNumberKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            if (selectedTroop != null) 
            {
                DeselectTroop(); 
            }
            SelectTroop(troop1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) 
        {
            if (selectedTroop != null)
            {
                DeselectTroop();
            }
            SelectTroop(troop2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (selectedTroop != null)
            {
                DeselectTroop();
            }
            SelectTroop(troop3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (selectedTroop != null)
            {
                DeselectTroop();
            }
            SelectTroop(troop4);
        }
    }

    void SelectTroop(GameObject troop)
    {
        selectedTroop = troop;
        selectedTroop.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent rotation
        selectedTroop.GetComponent<Troop>().selected = true;
        selectedTroop.GetComponent<Troop>().highlight.SetActive(true);
        Debug.Log("Troop selected: " + selectedTroop.name);
    }

    void DeselectTroop() 
    {
        selectedTroop.GetComponent<Troop>().selected = false; //necessary for platform drop off to work as intended
        selectedTroop.GetComponent<Troop>().highlight.SetActive(false);
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

        Debug.Log(troop.gameObject.name + " has respawned.");

        //reset mouse position
        troop.GetComponent<TroopClass>().mousePosition = killdozer.position;
    }
}

