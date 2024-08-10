using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public Button button; // Reference to the Button component
    public Text buttonText; // Reference to the Text component of the button
    public Color disabledColor = Color.gray; // Color to use when the button is disabled
    public Color enabledColor = Color.white; // Color to use when the button is enabled



    private void Update()
    {
        if (button.interactable == false)
        {
            UpdateButtonAppearance();
        }
        else 
        {
            UpdateButtonAppearance();
        }
    }
    // Method to disable the button
    /*public void DisableButton()
    {
        button.interactable = false;
        UpdateButtonAppearance();
    }*/

    // Method to enable the button
    /*public void EnableButton()
    {
        button.interactable = true;
        UpdateButtonAppearance();
    }*/

    // Method to update the appearance of the button based on its state
    private void UpdateButtonAppearance()
    {
        if (button.interactable)
        {
            buttonText.color = enabledColor;
        }
        else
        {
            buttonText.color = disabledColor;
        }
    }

    // Optionally, you can add a method to toggle the button state
    public void ToggleButtonState()
    {
        button.interactable = !button.interactable;
        UpdateButtonAppearance();
    }
}
