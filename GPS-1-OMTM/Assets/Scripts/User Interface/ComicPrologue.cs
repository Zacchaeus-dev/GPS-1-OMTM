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
    public GameObject mainMenu; // Reference to the main menu GameObject
    public Button nextButton; // Reference to the next button
    public Button backButton; // Reference to the back button
    public float delayBeforeButton = 2f; // Delay before showing the next button
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

        // Add listeners for the buttons
        nextButton.onClick.AddListener(ShowNextPanel);
        backButton.onClick.AddListener(ShowPreviousPanel);
    }

    public void StartComicPrologue()
    {
        if (comicPanels.Length > 0)
        {
            currentPanelIndex = 0;
            comicPanels[currentPanelIndex].SetActive(true);
            mainMenu.SetActive(false); // Disable the main menu
            StartCoroutine(ShowNextButtonWithDelay());
        }
        FindObjectOfType<AudioManager>().Play("button");
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
            StartCoroutine(ShowNextButtonWithDelay());
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

        // Deactivate the current panel
        comicPanels[currentPanelIndex].SetActive(false);

        // Check if there are previous panels to show
        if (currentPanelIndex > 0)
        {
            currentPanelIndex--;
            comicPanels[currentPanelIndex].SetActive(true);
            nextButton.gameObject.SetActive(false); // Hide the next button
            backButton.gameObject.SetActive(false); // Hide the back button
            StartCoroutine(ShowNextButtonWithDelay());
        }
        else
        {
            // Go back to the main menu if at the first panel
            mainMenu.SetActive(true);
            gameObject.SetActive(false); // Hide the comic prologue
        }
        FindObjectOfType<AudioManager>().Play("button");
    }

    IEnumerator ShowNextButtonWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeButton);
        nextButton.gameObject.SetActive(true);
        if (currentPanelIndex > 0) // Show the back button only if not on the first panel
        {
            backButton.gameObject.SetActive(true);
        }
    }
}
