using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class Killdozer : MonoBehaviour
{
   /* public float speed;
    public Transform destination; // Destination point

    private Rigidbody2D rb;
    public bool isMoving = true;*/

    public int maxHealth;
    public int currentHealth;
    public bool invincible = false;
    public Text Health;
    public TextMeshProUGUI uiHealth;

    public int directPathfinding;

    public static bool gameOver = false;

    public GameObject leftTarget;
    public GameObject rightTarget;

    public GameObject redOverlay;

    public GameObject KDHitEffect;
    public GameObject KDHP;
    public GameObject KDHPBarObject;
    public Image KDHPBar;

    public Animator healthAnimator;

    /*public GameObject KDUI;
    KDHealthUI KDUIScript;*/

    void Start()
    {
        gameOver = false;
        //rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        //KDUIScript = KDUI.GetComponent<KDHealthUI>();

        UpdateHealth();
    }

    void Update()
    {
/*        if (isMoving)
        {
            // Move the Killdozer towards the destination
            Vector2 position = Vector2.MoveTowards(transform.position, destination.position, speed * Time.fixedDeltaTime);
            rb.MovePosition(position);

            // Check if the Killdozer has reached the destination
            if (Vector2.Distance(transform.position, destination.position) < 0.1f)
            {
                Debug.Log("Killdozer has reached the destination.");
                isMoving = false;

                //win screen
                SceneManager.LoadScene("WinScreen");
            }
        }*/
    }

    /* // Use this when a stop point is made
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Stop"))
        {
            // Stop the Killdozer when it reaches a stop point
            isMoving = false;
            Debug.Log("Killdozer reached a stop point.");
        }
    }
    */
    public void FixedUpdate()
    {
        if(Health != null)
        {
           Health.text  = Mathf.FloorToInt(currentHealth).ToString();
        }
    }

    public void UpdateHealth()
    {
        if (uiHealth != null)
        {
            uiHealth.text = currentHealth.ToString();

            if (currentHealth < 100)
            {
                uiHealth.text = "0"+currentHealth.ToString();
            }
            else if (currentHealth < 10)
            {
                uiHealth.text = "00" + currentHealth.ToString();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth > 0)
        {
            FindObjectOfType<AudioManager>().Play("KDHit");
            KDHP.SetActive(true);
            KDHitEffect.SetActive(true);
            KDHP.GetComponent<SelfInactive>().time = 0;
        }

        if (invincible)
        {
            return;
        }

        if (damage <= currentHealth)
        {
            currentHealth -= damage;
            healthAnimator.SetTrigger("Damaged");
            redOverlay.SetActive(true);
            StartCoroutine(DisableOverlay());
        }
        else if (damage > currentHealth)
        {
            currentHealth = 0;
        }
        //Debug.Log("Killdozer took " + damage + " damage.");

        UpdateHealth();

        if (currentHealth <= 0)
        {
            StartCoroutine(Death());
        }

        KDHPBar.fillAmount = ((float)currentHealth / (float)maxHealth);
    }
    bool dieOnce = false;
    public CameraShake cameraShake;
    public GameObject GameplayUI;
    IEnumerator Death()
    {
        if (dieOnce == false)
        {
            dieOnce = true;
            FindObjectOfType<AudioManager>().Stop("BGM3");
            FindObjectOfType<AudioManager>().Play("KDDeath");

            StartCoroutine(cameraShake.Shake(0.010f, 0.015f));
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(cameraShake.Shake(0.010f, 0.015f));
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(cameraShake.Shake(0.010f, 0.015f));
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(cameraShake.Shake(0.010f, 0.015f));


            settingsPanel.SetActive(true);
            GameplayUI.SetActive(false);
            gameOver = true;

            yield return new WaitForSeconds(1);
            FindObjectOfType<AudioManager>().Play("LoseBGM");

            //gameObject.SetActive(false);
            

            

        }



        // Put death animation or effects

        //Debug.Log("Killdozer is dead");

        //lose screen
        //SceneManager.LoadScene("GameOver");
    }

    IEnumerator DisableOverlay()
    {
        yield return new WaitForSeconds(0.2f);

        redOverlay.SetActive(false);
    }


    //FOR PATHFINDING FROM KILLDOZER MIDDLE-GROUND TO A UPPERGROUND

/*    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "[PF] Upper-Ground 1")
        {
            directPathfinding = 1;
            //Debug.Log("ENTERING DIRECTPATHING 1 MODE" + directPathfinding);
        }
        if (collision.gameObject.tag == "[PF] Upper-Ground 2")
        {
            directPathfinding = 2;
            //Debug.Log("ENTERING DIRECTPATHING 2 MODE" + directPathfinding);
        }
        if (collision.gameObject.tag == "[PF] Upper-Ground 3")
        {
            directPathfinding = 3;
            //Debug.Log("ENTERING DIRECTPATHING 3  MODE" + directPathfinding);
        }        
        if (collision.gameObject.tag == "[PF] Upper-Ground 4")
        {
            directPathfinding = 4;
            //Debug.Log("ENTERING DIRECTPATHING 4  MODE" + directPathfinding);
        }

    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "[PF] Upper-Ground 1" || collision.gameObject.tag == "[PF] Upper-Ground 2" 
            || collision.gameObject.tag == "[PF] Upper-Ground 3" || collision.gameObject.tag == "[PF] Upper-Ground 4")
        {
            directPathfinding = 0;
            //Debug.Log("EXITING DIRECT PATHING MODE " + directPathfinding);
        }
    }*/

    public GameObject settingsPanel;

    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }
}
