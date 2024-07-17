using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerAutoHeal : MonoBehaviour
{
    public bool autoHealEnabled = false;
    public int healAmount = 10; // Amount of health to heal per heal
    public float detectionRange = 3f; // Range within which the healer can detect allies
    public float healRange = 1.5f; // Range within which the healer can heal allies
    public float healCooldown = 1f; // Time between heals
    public float moveSpeed = 2f; // Speed at which the healer moves towards the ally

    private float lastHealTime = 0f;
    public GameObject targetAlly;
    private Rigidbody2D rb;

    private TroopEnergy troopEnergy;
    public TroopWeapon troopWeapon;

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

        DetermineHeal();
    }

    void DetermineHeal()
    {
        switch (troopWeapon.selectedWeapon) //determine heal and line renderer based on selected weapon
        {
            case TroopWeapon.Weapon.Weapon1_Healer:
                healAmount = 50;
                healCooldown = 1.5f;
                break;
            case TroopWeapon.Weapon.Weapon2_Healer:
                healAmount = 20;
                healCooldown = 5f;

                lineRenderer.positionCount = segments + 1;
                lineRenderer.loop = true;
                lineRenderer.startWidth = lineWidth;
                lineRenderer.endWidth = lineWidth;
                lineRenderer.startColor = lineColor;
                lineRenderer.endColor = lineColor;
                lineRenderer.useWorldSpace = false;
                lineRenderer.enabled = false;
                break;
        }
    }

    void Update()
    {
        if (autoHealEnabled)
        {
            if (targetAlly == null)
            {
                FindTarget();
            }
            else
            {
                HealTarget();
            }
        }
    }

    void FindTarget()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        float closestDistance = Mathf.Infinity;
        GameObject closestAlly = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Troop") && hitCollider.gameObject != this.gameObject)
            {
                float distanceToAlly = Vector2.Distance(transform.position, hitCollider.transform.position);
                if (distanceToAlly < closestDistance)
                {
                    closestDistance = distanceToAlly;
                    closestAlly = hitCollider.gameObject;
                }
            }
        }

        targetAlly = closestAlly;
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
        if (targetAlly != null)
        {
            float distanceToAlly = Vector2.Distance(transform.position, targetAlly.transform.position);
            if (distanceToAlly <= healRange)
            {
                if (Time.time >= lastHealTime + healCooldown)
                {
                    Troop allyTroop = targetAlly.GetComponent<Troop>();
                    if (allyTroop != null)
                    {
                        allyTroop.currentHealth = Mathf.Min(allyTroop.currentHealth + healAmount, allyTroop.maxHealth);
                        lastHealTime = Time.time;
                        Debug.Log(targetAlly.name + " healed by " + healAmount + " to " + allyTroop.currentHealth + " health.");
                        allyTroop.UpdateHUD();
                        troopEnergy.GainPower();
                        StartCoroutine(ShowHealTracer(targetAlly.transform));
                    }
                }
            }
            else
            {
                targetAlly = null; // Lost range, find another target
            }
        }
    }

    void Healer_Weapon2Heal()
    {
        if (Time.time >= lastHealTime + healCooldown)
        {
            Collider2D[] alliesInRange = Physics2D.OverlapCircleAll(transform.position, healRange, LayerMask.GetMask("Troop"));

            foreach (Collider2D allyCollider in alliesInRange)
            {
                Troop allyTroop = allyCollider.GetComponent<Troop>();

                if (allyTroop != null && allyTroop.gameObject != gameObject) //heals everyone except herself
                {
                    StartCoroutine(ShowHealAOE(allyTroop.transform));
                    allyTroop.currentHealth = Mathf.Min(allyTroop.currentHealth + healAmount, allyTroop.maxHealth);
                    Debug.Log(allyTroop.name + " healed by " + healAmount + " to " + allyTroop.currentHealth + " health.");
                    allyTroop.UpdateHUD();
                }
            }
            lastHealTime = Time.time;
            troopEnergy.GainPower();
        }
    }

    IEnumerator ShowHealTracer(Transform target)
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position + startPositionOffset);
            lineRenderer.SetPosition(1, target.position);

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

    private IEnumerator ShowHealAOE(Transform target)
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
