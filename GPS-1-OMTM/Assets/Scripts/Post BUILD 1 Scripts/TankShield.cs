using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankShield : MonoBehaviour
{
    public float ultimateDuration;
    public Transform timerBarTransform;  // Reference to the timer bar Transform

    private float timeRemaining;
    private Vector3 initialScale;

    void Start()
    {
        timeRemaining = ultimateDuration;
        initialScale = timerBarTransform.localScale;  // Store the initial scale of the timer bar
        StartCoroutine(DestroyTankShield());
        StartCoroutine(UpdateTimerUI());
    }

    IEnumerator DestroyTankShield()
    {
        yield return new WaitForSeconds(ultimateDuration);
        Destroy(gameObject);
    }

    IEnumerator UpdateTimerUI()
    {
        while (timeRemaining > 0)
        {
            timeRemaining -= 1f;
            UpdateUI();
            yield return new WaitForSeconds(1f);
        }
    }

    void UpdateUI()
    {
        if (timerBarTransform != null)
        {
            float newScaleX = (timeRemaining / ultimateDuration) * initialScale.x;
            timerBarTransform.localScale = new Vector3(newScaleX, initialScale.y, initialScale.z);
        }
    }
}
