using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private bool isAttacking;
    public bool canAttack = true;

    // Ultimate
    public Ultimate ultimate = Ultimate.None;
    private bool ultimateOnCooldown = false;
    public float ultimateCooldown = 5f;
    public float ultimateDuration = 0f;
    private bool ccClickingOnLocation = false;
    private bool tankClickingOnLocation = false;

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

    private Image iconBorder;
    private Color originalColor;

    public bool troopOnKilldozer = false;

    [Header(" Art / Animations ")]
    // Animation
    public GameObject TroopModel;
    TroopAnimationsManager TroopAnimator;
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
    }

    void Update()
    {
        HandleUltimateInput();

        if (targetEnemy != null)
        {
            //MoveTowardsEnemy();
        }

        CheckGround();
        UpdateUltimateUI();
        DrainShield();
    }

    IEnumerator ReduceShieldOverTime()
    {
        yield return new WaitForSeconds(1f);

        if (currentShield <= 0)
        {
            shieldOn = false;
        }

        reducingShield = true;
    }

    void HandleUltimateInput()
    {
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
        else if (selected && Input.GetKeyDown(KeyCode.R) && !ultimateOnCooldown)
        {
            // Start the ultimate if not on cooldown
            StartCoroutine(UseUltimate(ultimate));
        }
        if (ccClickingOnLocation && Input.GetMouseButtonDown(0)) //CC's ultimate
        {
            HandleCCUltimateTargeting();
        }
        if (tankClickingOnLocation && Input.GetMouseButtonDown(0)) // Tank's ultimate
        {
            HandleTankUltimateTargeting();
        }
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

    void HandleCCUltimateTargeting()
    {
        Vector3 newPosition = transform.position;
        Vector2 MousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(MousePosition, Vector2.zero);

        foreach (var Hit in hits)
        {
            if (Hit.collider != null && (Hit.collider.CompareTag("[TP] Ground") || Hit.collider.CompareTag("[TP] Platform") || Hit.collider.CompareTag("[TP] Platform 1")))
            {
                if (Hit.collider.CompareTag("[TP] Ground"))
                {
                    newPosition.x = MousePosition.x;
                    newPosition.y = -3;
                }
                else if (Hit.collider.CompareTag("[TP] Platform"))
                {
                    newPosition.x = MousePosition.x;
                    newPosition.y = 3;
                }
                else if (Hit.collider.CompareTag("[TP] Platform 1"))
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
    }

    void HandleTankUltimateTargeting()
    {
        Vector3 newPosition = transform.position;
        Vector2 MousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(MousePosition, Vector2.zero);

        string currentTroopTag = GetCurrentTroopTag();
        if (string.IsNullOrEmpty(currentTroopTag))
        {
            Debug.Log("Trigger Tag is null or empty");
            tankClickingOnLocation = false;
            return;
        }
        bool validTarget = false;

        foreach (var Hit in hits)
        {
            if (Hit.collider != null && (Hit.collider.CompareTag("[TP] Ground") || Hit.collider.CompareTag("[TP] Platform") || Hit.collider.CompareTag("[TP] Platform 1")))
            {
                if (Hit.collider.CompareTag("[TP] Ground") && Hit.collider.CompareTag(currentTroopTag))
                {
                    newPosition.x = MousePosition.x;
                    newPosition.y = -1;
                    validTarget = true;
                }
                else if (Hit.collider.CompareTag("[TP] Platform") && Hit.collider.CompareTag(currentTroopTag))
                {
                    newPosition.x = MousePosition.x;
                    newPosition.y = 5;
                    validTarget = true;
                }
                else if (Hit.collider.CompareTag("[TP] Platform 1") && Hit.collider.CompareTag(currentTroopTag))
                {
                    newPosition.x = MousePosition.x;
                    newPosition.y = 8;
                    validTarget = true;
                }
                else
                {
                    validTarget = false;
                }
                break;
            }
        }

        if (!validTarget)
        {
            Debug.Log("Invalid target for Shield's Position. Must click on the same elevation.");
            tankClickingOnLocation = false;
            return;
        }

        GameObject TankShield = Instantiate(tankShield, newPosition, Quaternion.identity);
        tankClickingOnLocation = false;

        troopEnergy.UseAllPower();
        ultimateOnCooldown = true;
        ultimateCooldownTimeRemaining = ultimateCooldown;
        ultimateDurationTimeRemaining = ultimateDuration;
        Debug.Log("Tank Ultimate Activated");

        Vector3 shieldPosition = TankShield.transform.position;
        Vector3 tankPosition = transform.position;

        Collider2D[] enemiesInRange = Physics2D.OverlapAreaAll(tankPosition, shieldPosition);

        foreach (var enemyCollider in enemiesInRange)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(50);
                StartCoroutine(StunEnemy(enemy, 2f)); //stun duration
            }
        }

        StartCoroutine(DestroyTankShield(TankShield));
    }

    string GetCurrentTroopTag()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y), Vector2.zero);
        foreach (var hit in hits)
        {
            if (hit.collider != null && (hit.collider.CompareTag("[TP] Ground") || hit.collider.CompareTag("[TP] Platform") || hit.collider.CompareTag("[TP] Platform 1")))
            {
                return hit.collider.tag;
            }
        }
        return null;
    }

    public TroopWeapon troopWeapon;
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
                troopEnergy.UseAllPower();
                StartCoroutine(Ultimate_DPS());

                // activate ult animation

                gameObject.GetComponent<TroopAutoAttack>().DeactivateAttackVisuals();
                gameObject.GetComponent<TroopAutoAttack>().autoAttackEnabled = false;
                TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOn();

                yield return new WaitForSeconds(UltiDelay);

                switch (troopWeapon.selectedWeapon)
                {
                    case TroopWeapon.Weapon.Weapon1_DPS:
                        TroopModel.GetComponent<TroopAnimationsManager>().TroopUltiOff();
                        AttackModelGauntlets.SetActive(true);// SetActive Floating Gauntlets
                        break;
                    case TroopWeapon.Weapon.Weapon2_DPS:
                        TroopModel.GetComponent<TroopAnimationsManager>().TroopIdleOn();
                        AttackModel2ndSniper.SetActive(true);// SetActive Left Sniper
                        break;
                }
                        
                gameObject.GetComponent<TroopAutoAttack>().autoAttackEnabled = true;

                break;
            case Ultimate.Ultimate_Tank:
                Ultimate_Tank();
                break;
            case Ultimate.Ultimate_CC:
                Ultimate_CC();
                break;
            case Ultimate.Ultimate_Healer:
                troopEnergy.UseAllPower();
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
        ultimateOnCooldown = true;
        ultimateCooldownTimeRemaining = ultimateCooldown;
        ultimateDurationTimeRemaining = ultimateDuration;
        Debug.Log("DPS Ultimate Activated");

        //berserk
        //add attack and attack speed 
        troopAutoAttack.attackDamage += 25;
        troopAutoAttack.attackCooldown -= 0.075f; //3% increase in speed

        yield return new WaitForSeconds(100);//ultimateDuration);
        troopAutoAttack.attackDamage -= 25;
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

        yield return new WaitForSeconds(ultimateCooldown);
        ultimateOnCooldown = false;

    }

    void Ultimate_Tank()
    {
        tankClickingOnLocation = true;
    }

    IEnumerator DestroyTankShield(GameObject TankShield)
    {
        yield return new WaitForSeconds(ultimateDuration);

        Destroy(TankShield);


        yield return new WaitForSeconds(ultimateCooldown);
        ultimateOnCooldown = false;
    }

    private IEnumerator StunEnemy(Enemy enemy, float duration)
    {
        enemy.Stun(true);
        yield return new WaitForSeconds(duration);
        enemy.Stun(false);
    }

    void Ultimate_CC()
    {
        ccClickingOnLocation = true;
    }

    IEnumerator Ultimate_CC_End()
    {
        yield return new WaitForSeconds(ultimateDuration);

        //explosion is in taunt mine script

        yield return new WaitForSeconds(ultimateCooldown);
        ultimateOnCooldown = false;
    }

    IEnumerator Ultimate_Healer()
    {
        ultimateOnCooldown = true;
        ultimateCooldownTimeRemaining = ultimateCooldown;
        ultimateDurationTimeRemaining = ultimateDuration;
        Debug.Log("Healer Ultimate Activated");

        //golden fleece
        GainShield(troopController2D.troop1);
        GainShield(troopController2D.troop2);
        GainShield(troopController2D.troop3);
        GainShield(gameObject);

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
            currentHealth -= damage;
        }

        troopHUD.SetHUD(this);

        if (currentHealth <= 0)
        {
            StartCoroutine(Death());
        }

        if (troopEnergy != null)
        {
            if (troopEnergy.powerMethod == TroopEnergy.PowerMethod.Tank)
            {
                troopEnergy.GainPower();
            }
        }
    }

    public void FullHeal()
    {
        currentHealth = maxHealth;
        Debug.Log(gameObject.name + " is fully healed");
        UpdateHUD();
    }

    IEnumerator Death()
    {
        Debug.Log(gameObject.name + " is dead");

        // Deactivate the troop's pathfind arrow
        gameObject.GetComponent<TroopClass>().arrow.SetActive(false);

        // Death Animation
        TroopModel.GetComponent<TroopAnimationsManager>().TroopDies();
        gameObject.GetComponent<TroopAutoAttack>().DeactivateAttackVisuals();
        gameObject.GetComponent<TroopAutoAttack>().autoAttackEnabled = false; // stops shooting

        yield return new WaitForSeconds(2f);

        // Notify troopController2D to respawn this troop
        troopController2D.HandleRespawn(this);

        // Deactivate the troop
        gameObject.SetActive(false);
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
        this.isAttacking = false;
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
