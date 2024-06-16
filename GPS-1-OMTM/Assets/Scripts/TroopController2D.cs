using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TroopController2D : MonoBehaviour
{
    private Camera mainCamera;
    public GameObject selectedTroop;
    private Vector2 targetPosition;
    //private bool canMoveY;
    public float moveSpeed = 5f; // Speed of the troop movement
    public float ladderDetectionRange = 1f; // Range to detect ladders
    public float attackRange = 1.5f; // Range within which the troop can attack enemies

    //Troop Respawn 
    public float respawnTime = 5f; // respawn delay for troops
    public Vector2 respawnOffset; // offset from the killdozer's position for respawn
    public Transform killdozer; // killdozer's transform position

    Collider2D nearestVert;
    private Vector2 vertPosition;


    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleMouseInput();
        HandleEnemySelection();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            int layerMask = ~LayerMask.GetMask("Killdozer"); // LayerMask that ignores the Killdozer layer
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);

            if (hit.collider != null && hit.collider.CompareTag("Troop"))
            {
                if (selectedTroop != null) //if already have a troop selected
                {
                    DeselectTroop(); //deselect old troop
                }
                SelectTroop(hit.collider.gameObject);
            }
        }
        else if (Input.GetMouseButtonDown(1) && selectedTroop != null) // Right click
        {
            SetTroopTargetPosition();
        }
    }

    void SelectTroop(GameObject troop)
    {
        selectedTroop = troop;
        selectedTroop.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent rotation
        selectedTroop.GetComponent<Troop>().selected = true;
        Debug.Log("Troop selected: " + selectedTroop.name);
    }

    void DeselectTroop() //necessary for platform drop off to work as intended
    {
        selectedTroop.GetComponent<Troop>().selected = false;
    }
    //----------------------------
    /*
    void SetTroopTargetPosition()
    {
        if (selectedTroop.GetComponent<TroopClass>().onPlatform == 1)
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null && hit.collider.CompareTag("[PF] Ground"))
            {
                targetPosition = new Vector2(mousePosition.x, selectedTroop.transform.position.y); // Default to only X movement
                Debug.Log("moving in ground plane");
                vertPosition = Vector2.zero;
                nearestVert = null;
            }
            else if (hit.collider != null && hit.collider.CompareTag("[PF] Vertically")) //ladder
            {
                targetPosition = new Vector2(mousePosition.x, selectedTroop.transform.position.y); // Default to only X movement
                Debug.Log("vertically but ground");
                vertPosition = Vector2.zero;
                nearestVert = null;
            }
            else if (hit.collider != null && hit.collider.CompareTag("[PF] Upper-Ground")) //platforms
            {
                GameObject[] verts = GameObject.FindGameObjectsWithTag("[PF] Vertically");

                float closest = 200; //add your max range here
                GameObject closestVert = null;
                for (int i = 0; i < verts.Length; i++)  //list of gameObjects to search through
                {
                    float dist = Vector3.Distance(verts[i].transform.position, selectedTroop.transform.position);
                    Debug.Log("found one!");
                    if (dist < closest)
                    {
                        closest = dist;
                        closestVert = verts[i];
                    }
                }

                Collider2D closestVertCollider = closestVert.GetComponent<BoxCollider2D>();
                nearestVert = closestVertCollider;
                vertPosition = new Vector2(closestVertCollider.transform.position.x, selectedTroop.transform.position.y); // Move to vert X position first

                targetPosition = new Vector2(mousePosition.x, selectedTroop.transform.position.y);

            }

            if (selectedTroop != null)
            {
                selectedTroop.GetComponent<TroopClass>().SetTargetPosition(targetPosition, vertPosition, nearestVert);
            }
        }

        if (selectedTroop.GetComponent<TroopClass>().onPlatform == 2) 
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null && hit.collider.CompareTag("[PF] Upper-Ground"))
            {
                targetPosition = new Vector2(mousePosition.x, selectedTroop.transform.position.y); // Default to only X movement
                Debug.Log("moving in ground plane");
                vertPosition = Vector2.zero;
                nearestVert = null;
            }
            else if (hit.collider != null && hit.collider.CompareTag("[PF] Vertically"))
            {
                GameObject[] verts = GameObject.FindGameObjectsWithTag("[PF] Vertically");

                float closest = 200; //add your max range here
                GameObject closestVert = null;
                for (int i = 0; i < verts.Length; i++)  //list of gameObjects to search through
                {
                    float dist = Vector3.Distance(verts[i].transform.position, selectedTroop.transform.position);
                    Debug.Log("found one!");
                    if (dist < closest)
                    {
                        closest = dist;
                        closestVert = verts[i];
                    }
                }

                Collider2D closestVertCollider = closestVert.GetComponent<BoxCollider2D>();
                nearestVert = closestVertCollider;
                vertPosition = new Vector2(closestVertCollider.transform.position.x, selectedTroop.transform.position.y); // Move to vert X position first

                targetPosition = new Vector2(mousePosition.x, selectedTroop.transform.position.y);

            }
            else if (hit.collider != null && hit.collider.CompareTag("[PF] Ground"))
            {
                GameObject[] verts = GameObject.FindGameObjectsWithTag("[PF] Vertically");

                float closest = 200; //add your max range here
                GameObject closestVert = null;
                for (int i = 0; i < verts.Length; i++)  //list of gameObjects to search through
                {
                    float dist = Vector3.Distance(verts[i].transform.position, selectedTroop.transform.position);
                    Debug.Log("found one!");
                    if (dist < closest)
                    {
                        closest = dist;
                        closestVert = verts[i];
                    }
                }

                Collider2D closestVertCollider = closestVert.GetComponent<BoxCollider2D>();
                nearestVert = closestVertCollider;
                vertPosition = new Vector2(closestVertCollider.transform.position.x, selectedTroop.transform.position.y); // Move to vert X position first

                targetPosition = new Vector2(mousePosition.x, selectedTroop.transform.position.y);

            }

            if (selectedTroop != null)
            {
                selectedTroop.GetComponent<TroopClass>().SetTargetPosition(targetPosition, vertPosition, nearestVert);
            }
        }
    }
    */
    //---------------------------------------------------------------
    void SetTroopTargetPosition()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("[PF] Upper-Ground"))
        {
            // Check if the troop is already on an upper ground
            if (selectedTroop.GetComponent<TroopClass>().onPlatform == 2)
            {
                // Move horizontally to the position
                targetPosition = new Vector2(hit.collider.transform.position.x, selectedTroop.transform.position.y);
                vertPosition = Vector2.zero;
                nearestVert = null;
                Debug.Log("Moving horizontally to upper ground");
            }
            else
            {
                // If troop is not on an upper ground, climb ladder
                FindAndClimbNearestLadder(mousePosition);
            }
        }
        else if (hit.collider != null && hit.collider.CompareTag("[PF] Ground"))
        {
            targetPosition = new Vector2(mousePosition.x, selectedTroop.transform.position.y); // Default to only X movement
            Debug.Log("Moving in ground plane");
            vertPosition = Vector2.zero;
            nearestVert = null;
        }
        else if (hit.collider != null && hit.collider.CompareTag("[PF] Vertically"))
        {
            targetPosition = new Vector2(mousePosition.x, selectedTroop.transform.position.y); // Default to only X movement
            Debug.Log("Vertically but ground");
            vertPosition = Vector2.zero;
            nearestVert = null;
        }

        if (selectedTroop != null)
        {
            selectedTroop.GetComponent<TroopClass>().SetTargetPosition(targetPosition, vertPosition, nearestVert);
        }
    }

    void FindAndClimbNearestLadder(Vector2 mousePosition)
    {
        GameObject[] verts = GameObject.FindGameObjectsWithTag("[PF] Vertically");

        float closest = Mathf.Infinity; 
        GameObject closestVert = null;
        for (int i = 0; i < verts.Length; i++)  // List of gameObjects to search through
        {
            float dist = Vector3.Distance(verts[i].transform.position, mousePosition); // Distance to mouse position
            Debug.Log("found one!");
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
            vertPosition = new Vector2(closestVertCollider.transform.position.x, selectedTroop.transform.position.y); // Move to vert X position first
            targetPosition = new Vector2(mousePosition.x, selectedTroop.transform.position.y);

            Debug.Log("Climbing nearest ladder");
        }
    }

    void HandleEnemySelection()
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
    }

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

        // Reset troop health
        troop.currentHealth = troop.maxHealth;

        // Reactivate the troop
        troop.gameObject.SetActive(true);

        Debug.Log(troop.gameObject.name + " has respawned.");
    }
}

