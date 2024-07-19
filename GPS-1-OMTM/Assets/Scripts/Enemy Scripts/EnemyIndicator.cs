using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIndicator : MonoBehaviour
{
    public enum Tier { Tier1, Tier2, Tier3 }
    public Tier enemyTier;

    private CameraSystem cameraSystem;

    private void Start()
    {
        cameraSystem = FindObjectOfType<CameraSystem>();
    }

    private void Update()
    {
        CheckVisibility();
    }

    private void CheckVisibility()
    {
        if (cameraSystem == null) return;

        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
        bool onScreen = screenPos.x > 0 && screenPos.x < 1 && screenPos.y > 0 && screenPos.y < 1;

        if (onScreen)
        {
            IndicatorManager.Instance.RemoveIndicator(this);
        }
        else
        {
            IndicatorManager.Instance.AddIndicator(this, cameraSystem.transform.position.x);
        }
    }
}
