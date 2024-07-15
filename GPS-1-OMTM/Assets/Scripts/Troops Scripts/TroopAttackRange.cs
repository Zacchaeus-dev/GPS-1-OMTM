using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopAttackRange : MonoBehaviour
{
    private float attackRange = 5f;
    public int segments = 50;
    private Color lineColor = Color.white;
    public float lineWidth = 0.1f;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.positionCount = segments + 1;
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false; // Use local space to ensure the circle moves with the troop

        // Set line renderer properties
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;

        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        yield return new WaitForSeconds(0.1f);

        TroopAutoAttack autoAttack = GetComponent<TroopAutoAttack>();
        if (autoAttack != null)
        {
            attackRange = autoAttack.attackRange;
        }
        else
        {
            HealerAutoHeal healer = GetComponent<HealerAutoHeal>();
            attackRange = healer.healRange;
        }
        
    }

    public void DrawCircle()
    {
        lineRenderer.enabled = true;

        float angle = 0f;
        for (int i = 0; i < (segments + 1); i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * attackRange;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * attackRange;

            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            angle += (360f / segments);
        }
    }

    public void DisableCircle()
    {
        lineRenderer.enabled = false;
    }
}
