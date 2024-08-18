using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ComicPrologue : MonoBehaviour
{
    [Header("Comic Panels")]
    public GameObject[] comicPanels; // Array of comic panel GameObjects

    [Header("Next Scene")]
    public string sceneToLoad; // Name of the scene to load after the comic prologue

    [Header("UI Elements")]
    //public GameObject mainMenu; // Reference to the main menu GameObject
    public Button nextButton; // Reference to the next button
    public Button backButton; // Reference to the back button
    public Button skipButton; // Reference to the skip button
    public GameObject BG;
    public GameObject PausePanel;
    private float delayBeforeButton = 2f; // Delay before showing the next button
    public FadeManager fadeManager;

    private int currentPanelIndex = 0; // Current panel index

    void Start()
    {
        // Initially hide all comic panels and the buttons
        foreach (var panel in comicPanels)
        {
            panel.SetActive(false);
        }
        nextButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false); // Show skip button initially
        BG.SetActive(false);
        PausePanel.SetActive(false);

        // Add listeners for the buttons
        nextButton.onClick.AddListener(ShowNextPanel);
        backButton.onClick.AddListener(ShowPreviousPanel);
        skipButton.onClick.AddListener(SkipToNextScene);

        FindObjectOfType<AudioManager>().Play("BGM");
    }

    public void StartComicPrologue()
    {
        if (comicPanels.Length > 0)
        {
            currentPanelIndex = 0;
            comicPanels[currentPanelIndex].SetActive(true);
            //mainMenu.SetActive(false); // Disable the main menu
            BG.SetActive(true);
            PausePanel.SetActive(true);
            skipButton.gameObject.SetActive(false);
            StartCoroutine(ShowNextButtonWithDelay(0));
        }
        FindObjectOfType<AudioManager>().Play("button");
        FindObjectOfType<AudioManager>().Stop("BGM");
        FindObjectOfType<AudioManager>().Play("BGM2");
    }

    void ShowNextPanel()
    {
        if (comicPanels.Length == 0)
            return;

        comicPanels[currentPanelIndex].SetActive(false);

        if (currentPanelIndex < comicPanels.Length - 1)
        {
            currentPanelIndex++;
            comicPanels[currentPanelIndex].SetActive(true);
            nextButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(false);
            StartCoroutine(ShowNextButtonWithDelay(0));

            if (currentPanelIndex == comicPanels.Length - 1)
            {
                nextButton.gameObject.SetActive(false);
                StartCoroutine(ShowNextButtonWithDelay(2));
            }
        }
        else
        {
            // Trigger fade out and scene load
            fadeManager.FadeOutAndLoadScene();
        }
        FindObjectOfType<AudioManager>().Play("button");
    }

    void ShowPreviousPanel()
    {
        if (comicPanels.Length == 0)
            return;

        comicPanels[currentPanelIndex].SetActive(false);

        if (currentPanelIndex > 0)
        {
            currentPanelIndex--;
            comicPanels[currentPanelIndex].SetActive(true);
            nextButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(false);
            StartCoroutine(ShowNextButtonWithDelay(0));
        }
        else
        {
            //mainMenu.SetActive(true);
            gameObject.SetActive(false); // Hide the comic prologue
        }
        FindObjectOfType<AudioManager>().Play("button");
    }

    IEnumerator ShowNextButtonWithDelay(float delayBeforeButton)
    {
        nextButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(delayBeforeButton);
        nextButton.gameObject.SetActive(true);
        skipButton.gameObject.SetActive(true);
        if (currentPanelIndex > 0) // Show the back button only if not on the first panel
        {
            backButton.gameObject.SetActive(true);
        }
        if (currentPanelIndex == comicPanels.Length - 1)
        {
            skipButton.gameObject.SetActive(false);
        }
      
    }

    // New function for skipping to the next scene
    void SkipToNextScene()
    {
        fadeManager.FadeOutAndLoadScene();
        FindObjectOfType<AudioManager>().Play("button");
        FindObjectOfType<AudioManager>().Stop("BGM2");
    }
}
