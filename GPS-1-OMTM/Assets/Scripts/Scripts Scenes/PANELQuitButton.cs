using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PANELQuitButton : MonoBehaviour
{
    public AudioManager audioManager;
    public AudioSettings audioSettings; // Reference to AudioSettings script

    public void Back()
    {
        FindObjectOfType<AudioManager>().Play("button");
        ResetTimeScale();
        SceneManager.LoadScene(0); // Load the main menu scene
        FindObjectOfType<AudioManager>().Stop("BGM3");
    }

    public void Restart()
    {
        FindObjectOfType<AudioManager>().Play("button");
        ResetTimeScale();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Restart the current scene
        FindObjectOfType<AudioManager>().Stop("BGM3");
    }

    // Reset time scale to 1 to ensure the game is not paused when switching scenes
    private void ResetTimeScale()
    {
        if (Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }
    }
/*
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Apply volume settings and update sliders after the scene has loaded
        if (audioManager != null)
        {
            audioManager.ApplyVolumeSettings();
            if (audioSettings != null)
            {
                audioSettings.UpdateSliders();
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe to prevent memory leaks
    }*/
}
