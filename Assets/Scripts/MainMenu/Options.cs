using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Options : MonoBehaviour
{
    [SerializeField]
    private MenuManager menuManager;

    [SerializeField]
    private Toggle fullscreenToggle;

    [SerializeField]
    private Slider musicVolumeSlider;

    [SerializeField]
    private Slider sfxVolumeSlider;

    [SerializeField]
    private AudioMixer gameMixer;

    private void Start()
    {
        SetPreferences();
    }

    //set saved preferences, if saved preferences don't exist set with a predefined value
    private void SetPreferences()
    {
        //fullscreen
        bool fullscreenValue = Convert.ToBoolean(PlayerPrefs.GetInt("Fullscreen", 1));
        fullscreenToggle.isOn = fullscreenValue;
        Screen.fullScreen = fullscreenValue;

        //music volume
        float musicVolumeValue = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        musicVolumeSlider.value = musicVolumeValue;
        gameMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolumeValue) * 20);

        //SFX volume
        float sfxVolumeValue = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        sfxVolumeSlider.value = sfxVolumeValue;
        gameMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolumeValue) * 20);
    }


    //change between fullscreen and windowed based on toggle value
    public void FullscreenToggle()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
        PlayerPrefs.SetInt("Fullscreen", Convert.ToInt32(fullscreenToggle.isOn));
    }

    //change music volume by adjusting the slider
    public void MusicVolumeSlider()
    {
        gameMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolumeSlider.value) * 20);
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
    }

    //change SFX volume by adjusting the slider
    public void SFXVolumeSlider()
    {
        gameMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolumeSlider.value) * 20);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
    }


    //display main buttons
    public void BackButton()
    {
        menuManager.ChangeScreenState = ScreenState.MainButtons;
    }
}
