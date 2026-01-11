using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; // Required for handling Sliders

public class SoundmixerManager : MonoBehaviour
{
    [Header("Audio Components")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("UI Sliders (Drag your sliders here)")]
    [SerializeField] private UnityEngine.UI.Slider masterSlider;
    [SerializeField] private UnityEngine.UI.Slider musicSlider;
    [SerializeField] private UnityEngine.UI.Slider sfxSlider;

    private void Start()
    {
        // 1. Load Master Volume
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            float savedMaster = PlayerPrefs.GetFloat("MasterVolume");
            audioMixer.SetFloat("Master", savedMaster);

            // Update the visual slider if it is assigned
            if (masterSlider != null) masterSlider.value = savedMaster;
        }

        // 2. Load Music Volume
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float savedMusic = PlayerPrefs.GetFloat("MusicVolume");
            audioMixer.SetFloat("Music", savedMusic);

            if (musicSlider != null) musicSlider.value = savedMusic;
        }

        // 3. Load SFX Volume
        if (PlayerPrefs.HasKey("SoundFXVolume"))
        {
            float savedFX = PlayerPrefs.GetFloat("SoundFXVolume");
            audioMixer.SetFloat("SoundFX", savedFX);

            if (sfxSlider != null) sfxSlider.value = savedFX;
        }
    }

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("Master", level);
        PlayerPrefs.SetFloat("MasterVolume", level);
    }

    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat("SoundFX", level);
        PlayerPrefs.SetFloat("SoundFXVolume", level);
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("Music", level);
        PlayerPrefs.SetFloat("MusicVolume", level);
    }
}