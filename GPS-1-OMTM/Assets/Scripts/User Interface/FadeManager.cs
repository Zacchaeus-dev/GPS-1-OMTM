using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public Image fadeImage; // Reference to the Image component
    public float fadeDuration = 1.0f; // Duration of the fade
    public string sceneToLoad; // Scene to load after fading

    private void Start()
    {
        // Ensure the image is fully transparent at the start
        SetAlpha(0f);
    }

    public void FadeOutAndLoadScene()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        // Fade to opaque
        yield return StartCoroutine(Fade(0f, 1f));

        // Load the new scene
        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(endAlpha);
    }

    private void SetAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
}
