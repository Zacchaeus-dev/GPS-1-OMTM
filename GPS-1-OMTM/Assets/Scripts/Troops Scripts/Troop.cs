using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
//using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UI;

public class Troop : MonoBehaviour
{
    private Camera mainCamera;
    public TroopController2D troopController2D;
    private TroopAutoAttack troopAutoAttack;
    public bool invincible = false;
    public bool selected = false;
    public bool stopAction = false;

    public TroopHUD troopHUD;

    //Troop stats
    public int maxHealth;
    public int currentHealth;

    //Drop off platforms
    private Collider2D boxCollider;
    private Collider2D capsuleCollider;
    private bool troopOnPlatform = false;
    private bool troopOnGround = false;
    private Rigidbody2D rb;

    //Attacks
    private GameObject targetEnemy;
    //private bool isAttacking;
    public bool canAttack = true;

    // Ultimate
    public Ultimate ultimate = Ultimate.None;
    public bool ultimateOnCooldown = false;
    public float ultimateCooldown = 5f;
    public float ultimateDuration = 0f;
    //private bool ccClickingOnLocation = false;
    //private bool tankClickingOnLocation = false;

    // Ultimate Objects
    public GameObject tankShield;
    public GameObject tauntMine;

    // UI 
    public Image ultimateImage;
    public Image ultimateCooldownOverlay;
    public Image ultimateDurationOverlay;
    public GameObject ultimateReady;

    // Cooldown time
    private float ultimateCooldownTimeRemaining;

    // Duration time
    private float ultimateDurationTimeRemaining;

    public GameObject highlight;
    public GameObject arrow;

    // Energy
    private TroopEnergy troopEnergy;

    // Shield
    public int maxShield;
    public int currentShield;
    public bool shieldOn = false;
    public bool reducingShield = false;
    public GameObject shieldOverlay;
    public GameObject troopShield;

    private Image iconBorder;
    private Color originalColor;

    //visual effect from damaged
    public SpriteRenderer[] troopSprite;
    public Color DamagedColor;
    public Color NormalColor;
    float timer;
    private bool tookdamage;

    public bool troopOnKilldozer = false;

    public TutorialPhase tutorialPhase;
    public GameObject instruction4A;
    public GameObject instruction5;
    //public GameObject instruction6;
    public GameObject kddButton;
    public GameObject tutorialPanel;

    public GameObject tpObject;
    public Animator animator;
    public GameObject model;
    public GameObject damageIndicator;
    public bool death;

    public bool dpsUltiOn;
    //public GameObject castingRangePrefab; 
    //private GameObject castingRangeIndicator;

    public bool movingRight;

    public TroopWeapon troopWeapon;

    [Header(" Art / Animations ")]
    // Animation
    public GameObject TroopModel;
    TroopAnimationsManager TroopAnimatorTroopAnimator;
    public GameObject AttackModelGauntlets;
    public GameObject AttackModel2ndSniper;
    public float UltiDelay;
    
    public enum Ultimate
    {
        None,
        Ultimate_DPS,
        Ultimate_Tank,
        Ultimate_CC,
        Ultimate_Healer,
    }

    void Start()
    {
        mainCamera = Camera.main;
        currentHealth = maxHealth;
        boxCollider = GetComponent<BoxCollider2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        troopEnergy = GetComponent<TroopEnergy>();
        troopAutoAttack = GetComponent<TroopAutoAttack>();

        iconBorder = troopHUD.gameObject.GetComponent<Image>();
        originalColor = iconBorder.color;
        StartCoroutine(InitializeHUD());

        if (damageIndicator.activeInHierarchy == true)
        {
            damageIndicator.SetActive(false);
        }
    }

    void Update()
    {
        if (currentHealth > maxHealth) { currentHealth = maxHealth; }
        
        HandleUltimateInput();

        if (targetEnemy != null)
        {
            //MoveTowardsEnemy();
        }

        CheckGround();
        UpdateUltimateUI();
        DrainShield();

        if (tookdamage == true)
        {
            foreach(SpriteRenderer sprite in troopSprite)
            {
                sprite.color = DamagedColor;
            }
            //troopSprite.color = DamagedColor;
            timer = timer + Time.deltaTime;

            if (timer >= 0.3)
            {
                foreach (SpriteRenderer sprite in troopSprite)
                {
                    sprite.color = NormalColor;
                }
                //troopSprite.color = NormalColor;
                timer = 0;
                tookdamage = false;
            }
        }

        if (death == true)
        {
            gameObject.GetComponent<TroopClass>().enabled = false;
        }
        else
        {
            gameObject.GetComponent<TroopClass>().enabled = true;
        }
        
    }

    IEnumerator ReduceShieldOverTime()
    {
        yield return new WaitForSeconds(1f);

        if (currentShield <= 0)
        {
            shieldOn = false;
            if (troopShield.activeInHierarchy == true)
            {
                troopShield.SetActive(false);
            }
            if (shieldOverlay.activeInHierarchy == true) 
            {
                shieldOverlay.SetActive(false);
            }
        }

        reducingShield = true;
    }

    void HandleUltimateInput()
    {
        /*
        if (selected && Input.GetKeyDown(KeyCode.R) && ccClickingOnLocation) // Cancel clicking on location if it's already active
        {
            ccClickingOnLocation = false;
            Debug.Log("CC Ultimate targeting cancelled.");
        }
        else if (selected && Input.GetKeyDown(KeyCode.R) && tankClickingOnLocation)
        {
            tankClickingOnLocation = false;
            Debug.Log("Tank Ultimate targeting cancelled.");
        }
        */

        if (selected && Input.GetKeyDown(KeyCode.R) && !ultimateOnCooldown)
        {
            // Start the ultimate if not on cooldown
            StartCoroutine(UseUltimate(ultimate));
        }

        /*
        if (ccClickingOnLocation && Input.GetMouseButtonDown(0)) //CC's ultimate
        {
            StartCoroutine(HandleCCUltimateTargeting());
        }
        if (tankClickingOnLocation && Input.GetMouseButtonDown(0)) // Tank's ultimate
        {
            StartCoroutine(HandleTankUltimateTargeting());
        }
        */
    }

    IEnumerator InitializeHUD()
    {
        yield return new WaitForSeconds(0.1f);

        UpdateHUD();
    }

    public void UpdateHUD()
    {
        if (troopHUD != null)
        {
            troopHUD.SetHUD(this);
        }
    }

    /*
    IEnumerator HandleCCUltimateTargeting()
    {
        // activate ult animation

        gameObject.GetComponent<TroopAutoAttack>().DeactivateAttackVisuals();
        gameObject.GetComponent<TroopAutoAttack>().autoAttackEnabled = false;
        TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOn();


        yield return new WaitForSeconds(UltiDelay);


        Vector3 newPosition = transform.position;
        Vector2 MousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(MousePosition, Vector2.zero);

        foreach (var Hit in hits)
        {
            if (Hit.collider != null && Hit.collider.CompareTag("[TP] Ground") || Hit.collider.CompareTag("[PF] Ground") || Hit.collider.CompareTag("[PF] Upper-Ground 1") || Hit.collider.CompareTag("[PF] Upper-Ground 2")
                || Hit.collider.CompareTag("[PF] Upper-Ground 3") || Hit.collider.CompareTag("[PF] Upper-Ground 4") || Hit.collider.CompareTag("[PF] Upper-Ground 1 (2)")
                || Hit.collider.CompareTag("[PF] Upper-Ground 2 (2)") || Hit.collider.CompareTag("[PF] Upper-Ground 3 (2)") || Hit.collider.CompareTag("[PF] Upper-Ground 4 (2)"))
            {
                if (Hit.collider.CompareTag("[TP] Ground") || Hit.collider.CompareTag("[PF] Ground"))
                {
                    newPosition.x = MousePosition.x;
                    newPosition.y = -3;
                }
                else if (Hit.collider.CompareTag("[PF] Upper-Ground 1") || Hit.collider.CompareTag("[PF] Upper-Ground 2") || Hit.collider.CompareTag("[PF] Upper-Ground 3")
                        || Hit.collider.CompareTag("[PF] Upper-Ground 4"))
                {
                    newPosition.x = MousePosition.x;
                    newPosition.y = 3;
                }
                else if (Hit.collider.CompareTag("[PF] Upper-Ground 1 (2)") || Hit.collider.CompareTag("[PF] Upper-Ground 1 (3)")
                        || Hit.collider.CompareTag("[PF] Upper-Ground 1 (4)"))
                {
                    newPosition.x = MousePosition.x;
                    newPosition.y = 8;
                }
                break;
            }
        }

        Instantiate(tauntMine, newPosition, Quaternion.identity);
        ccClickingOnLocation = false;

        troopEnergy.UseAllPower();
        ultimateOnCooldown = true;
        ultimateCooldownTimeRemaining = ultimateCooldown;
        ultimateDurationTimeRemaining = ultimateDuration;
        Debug.Log("CC Ultimate Activated");

        StartCoroutine(Ultimate_CC_End());

        gameObject.GetComponent<TroopAutoAttack>().autoAttackEnabled = true;
        TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOff();

    }

    
    IEnumerator HandleTankUltimateTargeting()
    {
        // activate ult animation

        TroopModel.GetComponent<TroopAnimationsManager>().TroopAttackOff();
        gameObject.GetComponent<TroopAutoAttack>().autoAttackEnabled = false;
        TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOn();

        yield return new WaitForSeconds(UltiDelay);

        Vector3 newPosition = transform.position;
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);

        string currentTroopTag = GetCurrentTroopTag();
        if (string.IsNullOrEmpty(currentTroopTag))
        {
            Debug.Log("Trigger Tag is null or empty");
            tankClickingOnLocation = false;

            gameObject.GetComponent<TroopAutoAttack>().autoAttackEnabled = true;
            TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOff();
            yield break;
        }

        bool validTarget = false;

        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                string hitTag = hit.collider.tag;

                if ((hitTag == "[TP] Ground" && currentTroopTag == "[TP] Ground") ||
                    (hitTag == "[PF] Ground" && currentTroopTag == "[PF] Ground") ||
                    (hitTag == "[PF] Upper-Ground 1" && currentTroopTag == "[PF] Upper-Ground 1") ||
                    (hitTag == "[PF] Upper-Ground 2" && currentTroopTag == "[PF] Upper-Ground 2") ||
                    (hitTag == "[PF] Upper-Ground 3" && currentTroopTag == "[PF] Upper-Ground 3") ||
                    (hitTag == "[PF] Upper-Ground 4" && currentTroopTag == "[PF] Upper-Ground 4") ||
                    (hitTag == "[PF] Upper-Ground 1 (2)" && currentTroopTag == "[PF] Upper-Ground 1 (2)") ||
                    (hitTag == "[PF] Upper-Ground 2 (2)" && currentTroopTag == "[PF] Upper-Ground 2 (2)") ||
                    (hitTag == "[PF] Upper-Ground 3 (2)" && currentTroopTag == "[PF] Upper-Ground 3 (2)") ||
                    (hitTag == "[PF] Upper-Ground 4 (2)" && currentTroopTag == "[PF] Upper-Ground 4 (2)"))
                {
                    newPosition.x = mousePosition.x;
                    newPosition.y = hitTag == "Ground" ? -1 :
                                    hitTag.Contains("Upper-Ground 1") ? 5 :
                                    hitTag.Contains("Upper-Ground 2") ? 5 :
                                    hitTag.Contains("Upper-Ground 3") ? 5 :
                                    hitTag.Contains("Upper-Ground 4") ? 5 :
                                    hitTag.Contains("Upper-Ground 1 (2)") ? 8 :
                                    hitTag.Contains("Upper-Ground 2 (2)") ? 8 :
                                    hitTag.Contains("Upper-Ground 3 (2)") ? 8 :
                                    hitTag.Contains("Upper-Ground 4 (2)") ? 8 : newPosition.y;
                    validTarget = true;

                    gameObject.GetComponent<TroopAutoAttack>().autoAttackEnabled = true;
                    TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOff();
                    break;
                }
                else
                {
                    newPosition.x = mousePosition.x;
                    tankClickingOnLocation = false;
                    Debug.Log("Tank Ultimate targeting cancelled.");

                    gameObject.GetComponent<TroopAutoAttack>().autoAttackEnabled = true;
                    TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOff();
                    yield break;
                }
            }
        }

        if (!validTarget)
        {
            Debug.Log("Invalid target for Shield's Position. Must click on the same elevation.");
            tankClickingOnLocation = false;

            gameObject.GetComponent<TroopAutoAttack>().autoAttackEnabled = true;
            TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOff();
            yield break;
        }

        GameObject TankShield = Instantiate(tankShield, transform.position, Quaternion.identity);
        TankShield.GetComponent<BoxCollider2D>().enabled = false; // Disable collider initially

        tankClickingOnLocation = false;
        troopEnergy.UseAllPower();
        ultimateOnCooldown = true;
        ultimateCooldownTimeRemaining = ultimateCooldown;
        ultimateDurationTimeRemaining = ultimateDuration;

        StartCoroutine(MoveShieldToPosition(TankShield, newPosition));

        //yield return new WaitForSeconds(UltiDelay);
        gameObject.GetComponent<TroopAutoAttack>().autoAttackEnabled = true;
        TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOff();

    }

    string GetCurrentTroopTag()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y), Vector2.zero);
        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("[TP] Ground") || hit.collider.CompareTag("[PF] Ground") || hit.collider.CompareTag("[PF] Upper-Ground 1") || hit.collider.CompareTag("[PF] Upper-Ground 2")
            || hit.collider.CompareTag("[PF] Upper-Ground 3") || hit.collider.CompareTag("[PF] Upper-Ground 4") || hit.collider.CompareTag("[PF] Upper-Ground 1 (2)")
            || hit.collider.CompareTag("[PF] Upper-Ground 2 (2)") || hit.collider.CompareTag("[PF] Upper-Ground 3 (2)") || hit.collider.CompareTag("[PF] Upper-Ground 4 (2)"))
            {
                //Debug.Log(hit.collider.tag);
                return hit.collider.tag;
            }
        }
        return null;
    }
    */

    IEnumerator UseUltimate(Ultimate _ultimate)
    {
        if (troopEnergy.currentPower < troopEnergy.maxPower)
        {
            Debug.Log("Not enough energy");
            yield break;
        }

        switch (_ultimate)
        {
            case Ultimate.Ultimate_DPS:
                dpsUltiOn = true;
                troopEnergy.UseAllPower();
                StartCoroutine(Ultimate_DPS());

                // activate ult animation

                gameObject.GetComponent<TroopAutoAttack>().DeactivateAttackVisuals();
                gameObject.GetComponent<TroopAutoAttack>().autoAttackEnabled = false;
                TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOn();
                FindObjectOfType<AudioManager>().Play("lvlup");
                yield return new WaitForSeconds(0.9f);
                TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOff();
                yield return new WaitForSeconds(UltiDelay - 0.9f);

                switch (troopWeapon.selectedWeapon)
                {
                    case TroopWeapon.Weapon.Weapon1_DPS:
                        TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOff();
                        AttackModelGauntlets.SetActive(true);// SetActive Floating Gauntlets
                        break;
                    case TroopWeapon.Weapon.Weapon2_DPS:
                        TroopModel.GetComponent<TroopAnimationsManager>().TroopIdleOn();
                        //TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOff();
                        AttackModel2ndSniper.SetActive(true);// SetActive Left Sniper
                        break;
                }
                        
                gameObject.GetComponent<TroopAutoAttack>().autoAttackEnabled = true;

                break;
            case Ultimate.Ultimate_Tank:
                StartCoroutine(Ultimate_Tank());


                break;
            case Ultimate.Ultimate_CC:
                StartCoroutine(Ultimate_CC());


                break;
            case Ultimate.Ultimate_Healer:
                troopEnergy.UseAllPower();
                // activate ult animation
                TroopModel.GetComponent<TroopAnimationsManager>().TroopAttackOff();
                gameObject.GetComponent<HealerAutoHeal>().autoHealEnabled = false;
                TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOn();

                yield return new WaitForSeconds(UltiDelay);

                yield return StartCoroutine(Ultimate_Healer());




                break;
        }
    }

    void UpdateUltimateUI()
    {
        if (ultimateOnCooldown)
        {
            if (ultimateDurationTimeRemaining > 0)
            {
                ultimateReady.SetActive(false);
                ultimateDurationTimeRemaining -= Time.deltaTime;
                ultimateDurationOverlay.fillAmount = 1 - (ultimateDurationTimeRemaining / ultimateDuration);
                ultimateDurationOverlay.enabled = true;
            }
            else
            {
                ultimateDurationOverlay.fillAmount = 0;
                ultimateDurationOverlay.enabled = false;

                ultimateCooldownTimeRemaining -= Time.deltaTime;
                ultimateCooldownOverlay.fillAmount = 1 - (ultimateCooldownTimeRemaining / ultimateCooldown);
                ultimateCooldownOverlay.enabled = true;
            }
        }
        else
        {
            ultimateCooldownOverlay.fillAmount = 0;
            ultimateCooldownOverlay.enabled = false;
            ultimateDurationOverlay.fillAmount = 0;
            ultimateDurationOverlay.enabled = false;
            ultimateReady.SetActive(true);
        }
    }

    IEnumerator Ultimate_DPS()
    {
        GetComponent<TroopClass>().SetTargetPositionHere();
        ultimateOnCooldown = true;
        ultimateCooldownTimeRemaining = ultimateCooldown;
        ultimateDurationTimeRemaining = ultimateDuration;
        Debug.Log("DPS Ultimate Activated");

        //berserk
        //add attack and attack speed 
        troopAutoAttack.DPSUltBuff += 25;
        troopAutoAttack.attackCooldown -= 0.075f; //3% increase in speed


        if (tutorialPhase != null && tutorialPhase.tutorialOn) //tutorial
        {
            if (instruction5.activeInHierarchy == true)
            {
                instruction5.SetActive(false);
                //instruction6.SetActive(true);

                StartCoroutine(TutorialDelay());
            }
        }

        yield return new WaitForSeconds(ultimateDuration);//ultimateDuration);
        
        //if (dpsUltiOn == true)
        //{
            Ultimate_DPS_End();
        //}
    }

    public void Ultimate_DPS_End()
    {
        if (dpsUltiOn == false)
        {
            return;
        }

        //Debug.Log("DPS Ulti Ended");

        TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOff();
        troopEnergy.DisableUltimateVisual();
        troopAutoAttack.DPSUltBuff -= 25;
        troopAutoAttack.attackCooldown += 0.075f;

        switch (troopWeapon.selectedWeapon)
        {
            case TroopWeapon.Weapon.Weapon1_DPS:
                AttackModelGauntlets.SetActive(false);// SetActive Floating Gauntlets
                break;
            case TroopWeapon.Weapon.Weapon2_DPS:
                TroopModel.GetComponent<TroopAnimationsManager>().TroopIdleOff();
                TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOff();
                AttackModel2ndSniper.SetActive(false);// SetActive Floating Snipers
                break;
        }

        dpsUltiOn = false;

        //yield return new WaitForSeconds(ultimateCooldown);
        ultimateOnCooldown = false;
    }

    
    IEnumerator TutorialDelay()
    {
        yield return new WaitForSeconds(1f);

        //instruction6.SetActive(false);
        //tutorialPanel.SetActive(true);
        //Debug.Log("Tutorial Panel On");
        //Time.timeScale = 0.0f;

        instruction4A.SetActive(true);
        kddButton.SetActive(true);
    }
    
    public GameObject UltCircle;
    public CameraShake cameraShake;
    IEnumerator Ultimate_Tank()
    {
        GetComponent<TroopClass>().SetTargetPositionHere();
        //tankClickingOnLocation = true;
        
        TroopModel.GetComponent<TroopAnimationsManager>().TroopAttackOff();
        gameObject.GetComponent<TroopAutoAttack>().autoAttackEnabled = false;
        TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOn();

        Vector3 shieldOffset = new Vector3(40, 0, 0);

        if (!movingRight)
        {
            shieldOffset = new Vector3(-40, 0, 0);
        }

        Vector3 newPosition = transform.position + shieldOffset;

        Instantiate(UltCircle, newPosition, Quaternion.identity);

        troopEnergy.UseAllPower();
        ultimateOnCooldown = true;
        ultimateCooldownTimeRemaining = ultimateCooldown;
        ultimateDurationTimeRemaining = ultimateDuration;

        yield return new WaitForSeconds(UltiDelay);
        FindObjectOfType<AudioManager>().Play("Slam");
        
        GameObject TankShield = Instantiate(tankShield, transform.position, Quaternion.identity);
        TankShield.GetComponent<BoxCollider2D>().enabled = false; // Disable collider initially
        StartCoroutine(cameraShake.Shake(0.010f, 0.015f));
        //tankClickingOnLocation = false;
        yield return new WaitForSeconds(0.1f);

        troopEnergy.DisableUltimateVisual();

        TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOff();
        TroopModel.GetComponent<TroopAnimationsManager>().TroopWalkOn();

        StartCoroutine(MoveShieldToPosition(TankShield, newPosition));

        gameObject.GetComponent<TroopAutoAttack>().autoAttackEnabled = true;


    }

    IEnumerator MoveShieldToPosition(GameObject shield, Vector3 targetPosition)
    {
        float speed = 100f;
        while ((targetPosition - shield.transform.position).magnitude > 0.1f)
        {
            shield.transform.position = Vector3.MoveTowards(shield.transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        shield.transform.position = targetPosition;

        
        shield.GetComponent<BoxCollider2D>().enabled = true; // Enable collider

        Vector3 shieldPosition = shield.transform.position;
        Vector3 tankPosition = transform.position;

        Collider2D[] enemiesInRange = Physics2D.OverlapAreaAll(tankPosition, shieldPosition);
        foreach (var enemyCollider in enemiesInRange)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log("Enemy took damage from Tank's Ulti");
                enemy.TakeDamage(50);
                //StartCoroutine(StunEnemy(enemyCollider.gameObject, 2f, false)); // Stun duration
            }
            else
            {
                FlyingEnemy flyingEnemy = enemyCollider.GetComponent<FlyingEnemy>();
                if (flyingEnemy != null)
                {
                    Debug.Log("Flying enemy took damage from Tank's Ulti");
                    flyingEnemy.TakeDamage(50);
                    //StartCoroutine(StunEnemy(enemyCollider.gameObject, 2f, true));
                }
            }
        }

        TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOff();

        StartCoroutine(DestroyTankShield(shield));
    }

    IEnumerator DestroyTankShield(GameObject shield)
    {
        yield return new WaitForSeconds(0.2f);

        shield.GetComponent<Rigidbody2D>().gravityScale = 1f;

        //yield return new WaitForSeconds(ultimateDuration);
        //Destroy(TankShield);
        //destroy shield is inside tank shield script

        yield return new WaitForSeconds(ultimateCooldown);
        ultimateOnCooldown = false;
    }

    private IEnumerator StunEnemy(GameObject _enemy, float duration, bool isFlying)
    {
        if (!isFlying)
        {
            Enemy enemy = _enemy.GetComponent<Enemy>();
            enemy.Stun(true);
        }
        else
        {
            FlyingEnemy flyingEnemy = _enemy.GetComponent<FlyingEnemy>();
            flyingEnemy.Stun(true);
        }
        
        yield return new WaitForSeconds(duration);

        if (!isFlying)
        {
            Enemy enemy = _enemy.GetComponent<Enemy>();
            enemy.Stun(false);
        }
        else
        {
            FlyingEnemy flyingEnemy = _enemy.GetComponent<FlyingEnemy>();
            flyingEnemy.Stun(false);
        }
    }

    IEnumerator Ultimate_CC()
    {
        GetComponent<TroopClass>().SetTargetPositionHere();
        //ccClickingOnLocation = true;


        gameObject.GetComponent<TroopAutoAttack>().DeactivateAttackVisuals();
        gameObject.GetComponent<TroopAutoAttack>().autoAttackEnabled = false;
        TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOn();

        //yield return new WaitForSeconds(UltiDelay);

        /*
        Vector3 mineOffset = new Vector3(20, -2, 0);

        if (!movingRight)
        {
            mineOffset = new Vector3(-20, -2, 0);
        }

        Vector3 newPosition = transform.position + mineOffset;

        Instantiate(tauntMine, newPosition, Quaternion.identity);
        ccClickingOnLocation = false;
        */

        Vector3 newPosition = transform.position;
        Vector2 MousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(MousePosition, Vector2.zero);

        foreach (var Hit in hits)
        {
            if (Hit.collider != null && Hit.collider.CompareTag("[TP] Ground") || Hit.collider.CompareTag("[PF] Ground") || Hit.collider.CompareTag("[PF] Upper-Ground 1") || Hit.collider.CompareTag("[PF] Upper-Ground 2")
                || Hit.collider.CompareTag("[PF] Upper-Ground 3") || Hit.collider.CompareTag("[PF] Upper-Ground 4") || Hit.collider.CompareTag("[PF] Upper-Ground 1 (2)")
                || Hit.collider.CompareTag("[PF] Upper-Ground 2 (2)") || Hit.collider.CompareTag("[PF] Upper-Ground 3 (2)") || Hit.collider.CompareTag("[PF] Upper-Ground 4 (2)") || Hit.collider.CompareTag("[TP] Upper-Ground 4")
                || Hit.collider.CompareTag("[PF] KD Upper-Ground") || Hit.collider.CompareTag("[PF] KD Middle-Ground"))
            {
                if (Hit.collider.CompareTag("[TP] Ground") || Hit.collider.CompareTag("[PF] Ground"))
                {
                    newPosition.x = MousePosition.x;
                    newPosition.y = -3;
                }
                else if (Hit.collider.CompareTag("[PF] Upper-Ground 1") || Hit.collider.CompareTag("[PF] Upper-Ground 2") || Hit.collider.CompareTag("[PF] Upper-Ground 3")
                        || Hit.collider.CompareTag("[PF] Upper-Ground 4") || Hit.collider.CompareTag("[TP] Upper-Ground 4") || Hit.collider.CompareTag("[PF] KD Middle-Ground"))
                {
                    newPosition.x = MousePosition.x;
                    newPosition.y = 3;
                    Debug.Log("YES");
                }
                else if (Hit.collider.CompareTag("[PF] Upper-Ground 1 (2)") || Hit.collider.CompareTag("[PF] Upper-Ground 2 (2)")
                        || Hit.collider.CompareTag("[PF] Upper-Ground 3 (2)") || Hit.collider.CompareTag("[PF] Upper-Ground 4 (2)") || Hit.collider.CompareTag("[PF] KD Upper-Ground"))
                {
                    newPosition.x = MousePosition.x;
                    newPosition.y = 8;
                }
                break;
            }
/*            if (Hit.collider != null)
            {
                if (Hit.collider.CompareTag("[TP] Ground") || Hit.collider.CompareTag("[PF] Ground"))
                {
                    newPosition.x = MousePosition.x;
                    newPosition.y = -3;
                }
                else if (Hit.collider.CompareTag("[PF] Upper-Ground 1") || Hit.collider.CompareTag("[PF] Upper-Ground 2") || Hit.collider.CompareTag("[PF] Upper-Ground 3")
                        || Hit.collider.CompareTag("[PF] Upper-Ground 4") || Hit.collider.CompareTag("[TP] Upper-Ground 4") || Hit.collider.CompareTag("[PF] KD Middle-Ground"))
                {
                    newPosition.x = MousePosition.x;
                    newPosition.y = 3;
                    Debug.Log("YES");
                }
                else if (Hit.collider.CompareTag("[PF] Upper-Ground 1 (2)") || Hit.collider.CompareTag("[PF] Upper-Ground 2 (2)")
                        || Hit.collider.CompareTag("[PF] Upper-Ground 3 (2)") || Hit.collider.CompareTag("[PF] Upper-Ground 4 (2)") || Hit.collider.CompareTag("[PF] KD Upper-Ground"))
                {
                    newPosition.x = MousePosition.x;
                    newPosition.y = 8;
                }
                break;
            }*/
        }

        Instantiate(UltCircle, newPosition, Quaternion.identity);
        FindObjectOfType<AudioManager>().Play("troop2");

        troopEnergy.UseAllPower();
        ultimateOnCooldown = true;
        ultimateCooldownTimeRemaining = ultimateCooldown;
        ultimateDurationTimeRemaining = ultimateDuration;
        Debug.Log("CC Ultimate Activated");

        yield return new WaitForSeconds(UltiDelay);

        Instantiate(tauntMine, newPosition, Quaternion.identity);
        
        //ccClickingOnLocation = false;

        StartCoroutine(Ultimate_CC_End());

        gameObject.GetComponent<TroopAutoAttack>().autoAttackEnabled = true;
        TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOff();

        ultimateOnCooldown = false;
        troopEnergy.DisableUltimateVisual();
        yield return new WaitForSeconds(6);
        StartCoroutine(cameraShake.Shake(0.010f, 0.015f));
    }

    IEnumerator Ultimate_CC_End()
    {
        yield return new WaitForSeconds(ultimateDuration);

        //explosion is in taunt mine script

        //yield return new WaitForSeconds(ultimateCooldown);
        //ultimateOnCooldown = false;
    }

    IEnumerator Ultimate_Healer()
    {
        StartCoroutine(cameraShake.Shake(0.005f, 0.005f));
        FindObjectOfType<AudioManager>().Play("HealBomb");
        GetComponent<TroopClass>().SetTargetPositionHere();
        ultimateOnCooldown = true;
        ultimateCooldownTimeRemaining = ultimateCooldown;
        ultimateDurationTimeRemaining = ultimateDuration;
        Debug.Log("Healer Ultimate Activated");

        //golden fleece
        if (troopController2D.troop1.activeInHierarchy == true && troopController2D.troop1.GetComponent<Troop>().currentHealth > 0)
        {
            GainShield(troopController2D.troop1);
        }
        if (troopController2D.troop2.activeInHierarchy == true && troopController2D.troop2.GetComponent<Troop>().currentHealth > 0)
        {
            GainShield(troopController2D.troop2);
        }
        if (troopController2D.troop3.activeInHierarchy == true && troopController2D.troop3.GetComponent<Troop>().currentHealth > 0)
        {
            GainShield(troopController2D.troop3);
        }

        GainShield(gameObject);

        shieldOverlay.SetActive(true);

        troopEnergy.DisableUltimateVisual();

        yield return new WaitForSeconds(1.5f);
        gameObject.GetComponent<HealerAutoHeal>().autoHealEnabled = true;
        TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOff();
        yield return new WaitForSeconds(ultimateDuration);
        yield return new WaitForSeconds(ultimateCooldown);
        ultimateOnCooldown = false;
    }

    void GainShield(GameObject go)
    {
        Troop troop = go.GetComponent<Troop>();
        troop.maxShield = 500;
        troop.currentShield = 500;
        troop.shieldOn = true;
        troop.reducingShield = true;
        troop.troopShield.SetActive(true);

        troop.UpdateHUD();
    }

    void DrainShield()
    {
        if (shieldOn && reducingShield && currentShield > 0) //reduce shield over time
        {
            currentShield = currentShield - 50;
            Debug.Log(gameObject.name + "'s Current Shield: " + currentShield);
            reducingShield = false;
            UpdateHUD();
            StartCoroutine(ReduceShieldOverTime());
        }
    }
    public void TakeDamage(int damage)
    {
        if (invincible)
        {
            return;
        }

        if (currentShield > 0)
        {
            currentShield = currentShield - damage;
        }
        else
        {
            tookdamage = true;
            currentHealth -= damage;
            
            FindObjectOfType<AudioManager>().Play("MetalHit2");
        }

        damageIndicator.SetActive(true);
        StartCoroutine(DisableDamageIndicator());
        troopHUD.SetHUD(this);

        if (currentHealth <= 0)
        {
            StartCoroutine(Death());
            death = true;
        }

        if (troopEnergy != null)
        {
            if (troopEnergy.powerMethod == TroopEnergy.PowerMethod.Tank)
            {
                troopEnergy.GainPower();
                //UpdateHUD();
            }
        }
    }

    IEnumerator DisableDamageIndicator()
    {
        yield return new WaitForSeconds(0.2f);

        damageIndicator.SetActive(false);
    }

    public void FullHeal()
    {
        currentHealth = maxHealth;
        //Debug.Log(gameObject.name + " is fully healed");
        UpdateHUD();
    }

    public bool healer = false;

    IEnumerator Death()
    {
        damageIndicator.SetActive(false);
        tookdamage = false;
        //Debug.Log(gameObject.name + " is dead");

        // Deactivate the troop's pathfind arrow
        gameObject.GetComponent<TroopClass>().arrow.SetActive(false);

        // Death Animation
        TroopModel.GetComponent<TroopAnimationsManager>().TroopDies(); //causes model to tilt

        if (healer == false)
        {
            gameObject.GetComponent<TroopAutoAttack>().DeactivateAttackVisuals();
            gameObject.GetComponent<TroopAutoAttack>().autoAttackEnabled = false; // stops shooting
        }
        else if (healer == true)
        {
            TroopModel.GetComponent<TroopAnimationsManager>().TroopAttackOff();
            gameObject.GetComponent<HealerAutoHeal>().autoHealEnabled = false; // stops healing
        }

        yield return new WaitForSeconds(1f);
        
        tpObject.SetActive(true);
        FindObjectOfType<AudioManager>().Play("Teleport");
        animator.SetTrigger("Death");

        yield return new WaitForSeconds(0.5f);
        TroopModel.GetComponent<TroopAnimationsManager>().TroopIdle();
        TroopModel.GetComponent<TroopAnimationsManager>().TroopRespawn();
        yield return new WaitForSeconds(0.3f);
        model.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        // Notify troopController2D to respawn this troop
        troopController2D.HandleRespawn(this);
        tpObject.SetActive(false);
        // Deactivate the troop
        gameObject.SetActive(false);
        death = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            troopOnPlatform = true;
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            troopOnGround = true;
        }
        if (collision.gameObject.CompareTag("Killdozer"))
        {
            troopOnKilldozer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            troopOnPlatform = false;
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            troopOnGround = false;
        }
        if (collision.gameObject.CompareTag("Killdozer"))
        {
            troopOnKilldozer = false;
        }
    }

    void CheckGround()
    {
        if (troopOnGround || troopOnPlatform) //if troop is on a floor, stops troop going down
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void SetTargetEnemy(GameObject enemy, float attackRange)
    {
        this.targetEnemy = enemy;
        //this.isAttacking = false;
        this.stopAction = false;
    }

    public void DeselectTargetEnemy()
    {
        this.targetEnemy = null;
    }

    [Header(" HUD ")]
    public GameObject SelectedIcon;

    public void ChangeIconColour()
    {
        if(!selected)
        {
            //iconBorder.color = Color.yellow;
            SelectedIcon.SetActive(true);
        }
        else
        {
            //iconBorder.color = originalColor;
            SelectedIcon.SetActive(false);
        }
    }
}
