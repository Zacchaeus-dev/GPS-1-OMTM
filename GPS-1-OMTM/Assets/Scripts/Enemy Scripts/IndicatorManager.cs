using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorManager : MonoBehaviour
{
    public static IndicatorManager Instance;

    public GameObject arrowPrefabTier1;
    public GameObject arrowPrefabTier2;
    public GameObject arrowPrefabTier3;

    public Canvas canvas;
    private Dictionary<EnemyIndicator.Tier, GameObject> leftIndicators = new Dictionary<EnemyIndicator.Tier, GameObject>();
    private Dictionary<EnemyIndicator.Tier, GameObject> rightIndicators = new Dictionary<EnemyIndicator.Tier, GameObject>();

    private void Awake()
    {
        Instance = this;
        //canvas = FindObjectOfType<Canvas>();
    }

    public void AddIndicator(EnemyIndicator enemy, float cameraX)
    {
        if (enemy.transform.position.x < cameraX)
        {
            if (!leftIndicators.ContainsKey(enemy.enemyTier) && leftIndicators.Count < 3)
            {
                GameObject arrow = CreateArrow(enemy.enemyTier, true);
                leftIndicators[enemy.enemyTier] = arrow;
            }
        }
        else
        {
            if (!rightIndicators.ContainsKey(enemy.enemyTier) && rightIndicators.Count < 3)
            {
                GameObject arrow = CreateArrow(enemy.enemyTier, false);
                rightIndicators[enemy.enemyTier] = arrow;
            }
        }
    }

    public void RemoveIndicator(EnemyIndicator enemy)
    {
        if (leftIndicators.ContainsKey(enemy.enemyTier) && IsAllEnemiesOnScreen(enemy.enemyTier, true))
        {
            Destroy(leftIndicators[enemy.enemyTier]);
            leftIndicators.Remove(enemy.enemyTier);
        }

        if (rightIndicators.ContainsKey(enemy.enemyTier) && IsAllEnemiesOnScreen(enemy.enemyTier, false))
        {
            Destroy(rightIndicators[enemy.enemyTier]);
            rightIndicators.Remove(enemy.enemyTier);
        }
    }

    private GameObject CreateArrow(EnemyIndicator.Tier tier, bool left)
    {
        GameObject arrowPrefab = null;
        float yOffset = 0f;

        switch (tier)
        {
            case EnemyIndicator.Tier.Tier1:
                arrowPrefab = arrowPrefabTier1;
                yOffset = 150f;
                break;
            case EnemyIndicator.Tier.Tier2:
                arrowPrefab = arrowPrefabTier2;
                yOffset = 240f;
                break;
            case EnemyIndicator.Tier.Tier3:
                arrowPrefab = arrowPrefabTier3;
                yOffset = 330f; 
                break;
        }

        GameObject arrow = Instantiate(arrowPrefab, canvas.transform);
        RectTransform rectTransform = arrow.GetComponent<RectTransform>();

        rectTransform.anchorMin = left ? new Vector2(0, 0.5f) : new Vector2(1, 0.5f);
        rectTransform.anchorMax = left ? new Vector2(0, 0.5f) : new Vector2(1, 0.5f);
        rectTransform.anchoredPosition = new Vector2(left ? 50 : -50, yOffset);

        if (left)
        {
            // Flip the arrow horizontally
            rectTransform.localScale = new Vector3(1, -1, 1);
        }

        return arrow;
    }

    private bool IsAllEnemiesOnScreen(EnemyIndicator.Tier tier, bool left)
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        float screenBoundX = left ? cameraPosition.x : cameraPosition.x + Camera.main.aspect * Camera.main.orthographicSize * 2;

        foreach (var enemy in FindObjectsOfType<EnemyIndicator>())
        {
            if (enemy.enemyTier == tier)
            {
                Vector3 screenPos = Camera.main.WorldToViewportPoint(enemy.transform.position);
                bool onScreen = screenPos.x > 0 && screenPos.x < 1 && screenPos.y > 0 && screenPos.y < 1;

                if (!onScreen && (left ? enemy.transform.position.x < cameraPosition.x : enemy.transform.position.x > screenBoundX))
                {
                    return false;
                }
            }
        }

        return true;
    }
}