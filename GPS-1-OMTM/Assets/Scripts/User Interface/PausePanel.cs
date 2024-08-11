using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanel : MonoBehaviour
{
    [Header("Pause Panel")]
    public GameObject pausePanel; // Reference to the settings panel GameObject

    private bool isPaused = false; // Whether the game is currently paused

    void Update()
    {
        // Check for 'Esc' key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        // Show the pause panel and pause the game
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game time
        isPaused = true;
    }

    public void ResumeGame()
    {
        // Hide the pause panel and resume the game
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // Resume the game time
        isPaused = false;
    }

    // Public method to manually pause the game
    public void Pause()
    {
        PauseGame();
    }
}
