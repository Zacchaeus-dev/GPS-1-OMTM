using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public GameObject settingsPanel;
    public Slider volumeSlider;
    public AudioSource testAudioSource;

    void Start()
    {
        // Ensure the volume slider is set to the current volume level
        //volumeSlider.value = AudioListener.volume;
        float initialVolume = 0.5f;
        SetVolume(initialVolume);

        // Add a listener to call the method when the slider value changes
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }

    public void SetVolume(float volume)
    {
        // Set the game's global volume to the slider value
        AudioListener.volume = volume;

        // Optionally, set the volume of the test audio source
        if (testAudioSource != null)
        {
            testAudioSource.volume = volume;
        }
    }
}
