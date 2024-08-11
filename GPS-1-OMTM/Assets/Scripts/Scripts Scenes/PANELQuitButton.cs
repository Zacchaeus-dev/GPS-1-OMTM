using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PANELQuitButton : MonoBehaviour
{
    public void Back()
    {
        FindObjectOfType<AudioManager>().Play("button");
        ResetTimeScale();
        SceneManager.LoadScene(0); // Load the main menu scene
    }

    public void Restart()
    {
        FindObjectOfType<AudioManager>().Play("button");
        ResetTimeScale();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Restart the current scene
    }

    // Reset time scale to 1 to ensure the game is not paused when switching scenes
    private void ResetTimeScale()
    {
        if (Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }
    }
}
