using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TroopAutoAttack;

public class HealerAutoHeal : MonoBehaviour
{
    public GameObject TroopModel;
    TroopAnimationsManager TroopAnimator;
    TroopClass troopClass;

    public GameObject shootingPoint;

    public bool autoHealEnabled = false;
    public int healAmount = 10; // Amount of health to heal per heal
    public float detectionRange = 3f; // Range within which the healer can detect allies
    public float healRange = 1.5f; // Range within which the healer can heal allies
    public float healCooldown = 1f; // Time between heals
    public float AnimationDelay = 0.4f;
    public float ShootingDelay = 0.5f;
    float delay = 0;
    public float moveSpeed = 2f; // Speed at which the healer moves towards the ally

    private float lastHealTime = 0f;
    public GameObject targetAlly;
    private Rigidbody2D rb;

    private TroopEnergy troopEnergy;
    public TroopWeapon troopWeapon;

    public GameObject healSpritePrefab;
    public Transform shootingPointTransform;

    // Public references for LineRenderer and start position offset
    public LineRenderer lineRenderer;
    public Vector3 startPositionOffset;

    public int segments = 50; // Number of segments for the circle
    public float lineWidth = 0.1f; // Width of the line
    public Color lineColor = Color.green;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        troopEnergy = GetComponent<TroopEnergy>();
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false; // Initially disable the line renderer
        }

        TroopAnimator = TroopModel.GetComponent<TroopAnimationsManager>();
        troopClass = gameObject.GetComponent<TroopClass>();


        DetermineHeal();

    }

    void DetermineHeal()
    {
        switch (troopWeapon.selectedWeapon) //determine heal and line renderer based on selected weapon
        {
            case TroopWeapon.Weapon.Weapon1_Healer:
                healAmount = 50;
                healCooldown = 1.5f; // 1.5
                AnimationDelay = 0f;
                ShootingDelay = 0.1f;

                TroopAnimator.TroopOnWeapon1();
                break;
            case TroopWeapon.Weapon.Weapon2_Healer:
                healAmount = 20;
                healCooldown = 2f; //1
                AnimationDelay = 0f;
                ShootingDelay = 0f;

                lineRenderer.positionCount = segments + 1;
                lineRenderer.loop = true;
                lineRenderer.startWidth = lineWidth;
                lineRenderer.endWidth = lineWidth;
                lineRenderer.startColor = lineColor;
                lineRenderer.endColor = lineColor;
                lineRenderer.useWorldSpace = false;
                lineRenderer.enabled = false;

                TroopAnimator.TroopOnWeapon2();
                break;
        }
    }

    void Update()
    {
        if (Killdozer.gameOver)
        {
            return;
        }

        DetermineHeal();

        if (autoHealEnabled)
        {
            if (targetAlly == null)
            {
                FindTarget();
                TroopAnimator.TroopAttackOff();
                delay = 0;
            }
            else
            {
                HealTarget();

                if (troopWeapon.selectedWeapon == TroopWeapon.Weapon.Weapon1_Healer)
                {
                    if (targetAlly != null)
                    {
                        Troop allyTroop = targetAlly.GetComponent<Troop>();
                        if (allyTroop != null && allyTroop.currentHealth == allyTroop.maxHealth)
                        {
                            FindTarget(); // Change target if the current ally is fully healed
                            TroopAnimator.TroopAttackOff();
                            delay = 0;
                        }
                    }
                }

                if (targetAlly != null && targetAlly.gameObject.activeInHierarchy == false) //change target if target is dead
                {
                    targetAlly = null;
                }
            }
        }
    }

    void FindTarget()
    {
        TroopAnimator.TroopAttackOff();
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        float closestDistance = Mathf.Infinity;
        GameObject closestAlly = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Troop") && hitCollider.gameObject != this.gameObject)
            {
                float distanceToAlly = Vector2.Distance(transform.position, hitCollider.transform.position);
                Troop allyTroop = hitCollider.GetComponent<Troop>();

                /*
                if (distanceToAlly < closestDistance && troopWeapon.selectedWeapon == TroopWeapon.Weapon.Weapon2_Healer)
                {
                    closestDistance = distanceToAlly;
                    closestAlly = hitCollider.gameObject;
                }
                else if (allyTroop != null && allyTroop.currentHealth < allyTroop.maxHealth && troopWeapon.selectedWeapon == TroopWeapon.Weapon.Weapon1_Healer)
                {
                    closestDistance = distanceToAlly;
                    closestAlly = hitCollider.gameObject;
                }
                */

                if (allyTroop != null && allyTroop.currentHealth < allyTroop.maxHealth)
                {
                    closestDistance = distanceToAlly;
                    closestAlly = hitCollider.gameObject;
                }
            }
        }

        if (closestAlly != null)
        {
            targetAlly = closestAlly;
        }
    }

    void MoveTowardsTarget()
    {
        if (targetAlly != null)
        {
            float distanceToAlly = Vector2.Distance(transform.position, targetAlly.transform.position);
            if (distanceToAlly > healRange)
            {
                Vector2 direction = (targetAlly.transform.position - transform.position).normalized;
                direction.y = 0;
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
            }
        }
    }

    void HealTarget()
    {
        switch (troopWeapon.selectedWeapon)
        {
            case TroopWeapon.Weapon.Weapon1_Healer:
                Healer_Weapon1Heal();
                break;
            case TroopWeapon.Weapon.Weapon2_Healer:
                Healer_Weapon2Heal();
                break;
        }
    }


    void Healer_Weapon1Heal()
    {
        if (troopClass.isMoving == false) //refering to the TroopClass on "(moving == false)" then this autoattack is activated......
        {
            if (targetAlly != null)
            {
                // to check whether Troop is attacking Left or Right
                if (targetAlly.transform.position.x < gameObject.transform.position.x)
                {
                    gameObject.GetComponent<TroopClass>().GoingLeft = true;
                }
                else if (targetAlly.transform.position.x > gameObject.transform.position.x)
                {
                    gameObject.GetComponent<TroopClass>().GoingLeft = false;
                }

                float distanceToAlly = Vector2.Distance(transform.position, targetAlly.transform.position);
                if (distanceToAlly <= healRange)
                {
                    //delay bc troops need a window of time to get into attack stance when transitioning from walking to attacking
                    delay = delay + Time.deltaTime;
                    if (delay > AnimationDelay)
                    {
                        TroopAnimator.TroopAttackOn();
                        
                    }

                    if (delay >= ShootingDelay) //if no delay bfr shooting, troop will shoot bfr even setting their weapon in the right position
                    {
                        delay = 0;

                        if (Time.time >= lastHealTime + healCooldown)
                        {
                            Troop allyTroop = targetAlly.GetComponent<Troop>();
                            if (allyTroop != null)
                            {
                                StartCoroutine(ShowHealNeedle(targetAlly.transform));
                                allyTroop.currentHealth = Mathf.Min(allyTroop.currentHealth + healAmount, allyTroop.maxHealth);
                                FindObjectOfType<AudioManager>().Play("heal");
                                lastHealTime = Time.time;
                                //Debug.Log(targetAlly.name + " healed by " + healAmount + " to " + allyTroop.currentHealth + " health.");
                                allyTroop.UpdateHUD();
                                troopEnergy.GainPower();
                                //gameObject.GetComponent<Troop>().UpdateHUD();
                                //StartCoroutine(ShowHealTracer(targetAlly.transform))

                                if (allyTroop.currentHealth == allyTroop.maxHealth)
                                {
                                    targetAlly = null; //stops healing once target ally is at full health
                                }
                            }
                        }
                    }
                }
                else
                {
                    targetAlly = null; // Lost range, find another target
                }
            }
        }
        else
        {
            delay = 0;
        }
    }
    public GameObject HealAOE;
    void Healer_Weapon2Heal()
    {
        if (troopClass.isMoving == false) //refering to the TroopClass on "(moving == false)" then this autoattack is activated......
        {
            float distanceToAlly = Vector2.Distance(transform.position, targetAlly.transform.position);
            if (distanceToAlly <= healRange)
            {
                delay = delay + Time.deltaTime;
                if (delay > AnimationDelay)
                {
                    TroopAnimator.TroopAttackOn();

                }

                if (delay >= ShootingDelay) //if no delay bfr shooting, troop will shoot bfr even setting their weapon in the right position
                {
                    delay = 0;

                    if (Time.time >= lastHealTime + healCooldown)
                    {
                        Collider2D[] alliesInRange = Physics2D.OverlapCircleAll(transform.position, healRange, LayerMask.GetMask("Troop"));
                        Instantiate(HealAOE, transform.position, Quaternion.identity);
                        foreach (Collider2D allyCollider in alliesInRange)
                        {
                            Troop allyTroop = allyCollider.GetComponent<Troop>();

                            if (allyTroop != null && allyTroop.gameObject != gameObject) //heals everyone except herself
                            {
                                //StartCoroutine(ShowHealAOE(allyTroop.transform));
                                allyTroop.currentHealth = Mathf.Min(allyTroop.currentHealth + healAmount, allyTroop.maxHealth);
                                FindObjectOfType<AudioManager>().Play("heal");
                                Debug.Log(allyTroop.name + " healed by " + healAmount + " to " + allyTroop.currentHealth + " health.");
                                allyTroop.UpdateHUD();

                                if (allyTroop.currentHealth >= allyTroop.maxHealth)
                                {
                                    Debug.Log(allyTroop.name + " is at full health.");
                                    targetAlly = null; //stop healing if target's health is full
                                }
                            }
                        }
                        lastHealTime = Time.time;
                        troopEnergy.GainPower();
                        //gameObject.GetComponent<Troop>().UpdateHUD();
                    }
                }

            }
            else
            {
                targetAlly = null; // Lost range, find another target
            }
        }
        else
        {
            delay = 0;
        }
    }

    /*
    IEnumerator ShowHealTracer(Transform target) //weapon 1
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, shootingPoint.transform.position);
            lineRenderer.SetPosition(1, new Vector2(target.transform.position.x, target.transform.position.y + 0.8f));

            float elapsedTime = 0f;
            float tracerDuration = 0.5f; // Duration for which the tracer is visible

            while (elapsedTime < tracerDuration)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            lineRenderer.enabled = false;
        }
    }
    */

    IEnumerator ShowHealNeedle(Transform target)
    {
        // Add a delay before the first shot
        //yield return new WaitForSeconds(0.5f);

        GameObject healSprite = Instantiate(healSpritePrefab, shootingPointTransform.position, Quaternion.identity);
        Vector3 startPosition = shootingPointTransform.position;

        if (target == null || target.gameObject.activeInHierarchy == false)
        {
            Destroy(healSprite);
            yield return null;
        }

        Vector3 targetPosition = new Vector2(target.position.x, target.position.y + 0.5f);

        // Calculate the direction and angle
        Vector3 direction = (targetPosition - startPosition).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate the sprite to face the target
        healSprite.transform.rotation = Quaternion.Euler(0, 0, angle);

        float elapsedTime = 0f;
        float tracerDuration = 0.2f; // Duration for which the tracer is visible

        while (elapsedTime < tracerDuration)
        {
            elapsedTime += Time.deltaTime;
            healSprite.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / tracerDuration);

            if(target == null || target.gameObject.activeInHierarchy == false)
            {
                Destroy(healSprite);
                yield return null;
            }

            yield return null;
        }

        Destroy(healSprite);
    }

    private IEnumerator ShowHealAOE(Transform target) //weapon 2
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            float elapsedTime = 0f;
            float growDuration = 0.5f; // Duration for the circle to grow
            float maxRadius = healRange; // Maximum radius for the circle

            while (elapsedTime < growDuration)
            {
                float currentRadius = Mathf.Lerp(0, maxRadius, elapsedTime / growDuration);
                DrawCircle(currentRadius);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            DrawCircle(maxRadius); // Ensure the circle reaches its maximum size
            yield return new WaitForSeconds(0.15f); // Duration for the circle to stay

            lineRenderer.enabled = false;
        }
    }

    private void DrawCircle(float radius)
    {
        float angle = 0f;
        for (int i = 0; i < (segments + 1); i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            angle += (360f / segments);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw heal range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, healRange);

        // Draw start position offset
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + startPositionOffset, 0.1f);
    }
}
