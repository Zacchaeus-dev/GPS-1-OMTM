using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public AudioManager audioManager;
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start()
    {
        // Initialize sliders with saved values
        float savedBGMVolume = PlayerPrefs.GetFloat("BGMVolume", 0.75f);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        bgmSlider.value = savedBGMVolume;
        sfxSlider.value = savedSFXVolume;

        // Add listeners to sliders
        bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    public void OnBGMVolumeChanged(float value)
    {
        // Update BGM volume in AudioManager
        audioManager.SetBGMVolume(value);
        // Save the new value
        PlayerPrefs.SetFloat("BGMVolume", value);
        PlayerPrefs.Save();
    }

    public void OnSFXVolumeChanged(float value)
    {
        // Update SFX volume in AudioManager
        audioManager.SetSFXVolume(value);
        // Save the new value
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    public void UpdateSliders()
    {
        if (audioManager != null)
        {
            bgmSlider.value = audioManager.bgmVolume;
            sfxSlider.value = audioManager.sfxVolume;
        }
    }
}
