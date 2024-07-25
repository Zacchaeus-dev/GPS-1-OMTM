using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image fillImage; // Reference to the image to be filled
    public float holdTime = 2f; // Time required to hold the button
    public UnityEvent onHoldComplete; // Event to trigger when the button is held long enough

    private bool isHolding = false;
    private float holdCounter = 0f;

    void Start()
    {
        fillImage.fillAmount = 0f;
    }

    void Update()
    {
        if (isHolding)
        {
            holdCounter += Time.deltaTime;
            fillImage.fillAmount = holdCounter / holdTime;

            if (holdCounter >= holdTime)
            {
                onHoldComplete.Invoke(); // Trigger the event
                Debug.Log("Activated");
                ResetHold();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        holdCounter = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isHolding && holdCounter < holdTime)
        {
            ResetHold();
        }
    }

    private void ResetHold()
    {
        isHolding = false;
        holdCounter = 0f;
        fillImage.fillAmount = 0f;
    }
}
