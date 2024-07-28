using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ComicPrologue : MonoBehaviour
{
    [Header("Comic Images")]
    public Image[] comicImages; // Array of comic images

    [Header("Next Scene")]
    public string sceneToLoad; // Name of the scene to load after the comic prologue

    private int currentImageIndex = 0; // Current image index

    void Start()
    {
        // Initially set all images to inactive
        foreach (Image img in comicImages)
        {
            img.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // On left mouse click or tap
        {
            ShowNextImage();
        }
    }

    public void StartComicPrologue()
    {
        if (comicImages.Length > 0)
        {
            comicImages[currentImageIndex].gameObject.SetActive(true);
        }
    }

    void ShowNextImage()
    {
        if (comicImages.Length == 0)
            return;

        // Deactivate current image
        comicImages[currentImageIndex].gameObject.SetActive(false);

        // Check if there are more images to show
        if (currentImageIndex < comicImages.Length - 1)
        {
            currentImageIndex++;
            comicImages[currentImageIndex].gameObject.SetActive(true);
        }
        else
        {
            // Load the next scene if no more images
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
