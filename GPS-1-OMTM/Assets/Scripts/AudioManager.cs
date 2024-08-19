using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Soundclass[] sounds;
    public static AudioManager Instance { get; private set; }
    public float bgmVolume;
    public float sfxVolume;

    private static AudioManager instance;

    // Event that will be triggered when the volume changes
    public static event Action OnVolumeChange;

    public void Awake()
    {
/*        if (instance == null)
        {
            instance = this;
             // Make AudioManager persistent across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);*/
        // Load saved volume settings
        //bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.75f);
       // sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        bgmVolume = 0.5f;
        //sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        foreach (Soundclass s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = false;

            // Adjust initial volume based on type
/*            if (s.type == SoundType.BGM)
            {
                s.source.volume = bgmVolume;
            }
            else if (s.type == SoundType.SFX)
            {
                s.source.volume = sfxVolume;
            }*/
        }
    }

    public void Play(string name)
    {
        Soundclass s = Array.Find(sounds, sound => sound.name == name);

        if (s.name == "Orb")
        {
            s.source.pitch = s.pitch + UnityEngine.Random.Range(0f, 0.5f);
        }
        s.source.Play();
    }

    public void Stop(string name)
    {
        Soundclass s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }

    public void Dim(string name)
    {
        Soundclass s = Array.Find(sounds, sound => sound.name == name);
        s.source.volume = 0.05f;
    }

    public void Undim(string name)
    {
        Soundclass s = Array.Find(sounds, sound => sound.name == name);
        s.source.volume = 0.10f;
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        foreach (Soundclass s in sounds)
        {
            if (s.type == SoundType.BGM)
            {
                s.source.volume = bgmVolume;
            }
        }
        //PlayerPrefs.SetFloat("BGMVolume", volume); // Save the volume setting

        //OnVolumeChange?.Invoke(); // Trigger the event
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        foreach (Soundclass s in sounds)
        {
            if (s.type == SoundType.SFX)
            {
                s.source.volume = sfxVolume;
            }
        }
        //layerPrefs.SetFloat("SFXVolume", volume); // Save the volume setting

        //OnVolumeChange?.Invoke(); // Trigger the event
    }

/*    public void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event
    }

    public void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe to prevent memory leaks
    }*/

/*    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reapply the saved volume settings when the scene is loaded
        ApplyVolumeSettings();
    }

    public void ApplyVolumeSettings()
    {
        Debug.Log("Applying BGM Volume: " + bgmVolume);
        Debug.Log("Applying SFX Volume: " + sfxVolume);

        foreach (Soundclass s in sounds)
        {
            if (s.type == SoundType.BGM)
            {
                s.source.volume = bgmVolume;
            }
            else if (s.type == SoundType.SFX)
            {
                s.source.volume = sfxVolume;
            }
        }
    }*/
}
