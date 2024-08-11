using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMainMenu : MonoBehaviour
{
    public void Back()
    {
        FindObjectOfType<AudioManager>().Play("button");
        SceneManager.LoadScene(0);
    }    
    
    public void Restart()
    {
        FindObjectOfType<AudioManager>().Play("button");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
