using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixerManager : MonoBehaviour
{
    [SerializeField]
    private AudioMixer gameMixer;

    [SerializeField]
    private AudioMixerSnapshot normalStateSnapshot;
    [SerializeField]
    private AudioMixerSnapshot powerStateSnapshot;
    [SerializeField]
    private AudioMixerSnapshot pauseStateSnapshot;

    private static MixerManager mixerManagerInstance;

    public static MixerManager MixerManagerInstance
    {
        get
        {
            return mixerManagerInstance;
        }
    }

    private void Awake()
    {
        mixerManagerInstance = this;
    }

    private void Start()
    {
        SetOptionsPreferences();
        ChangeSnapshot(GameState.Normal);
    }

    //set preferences that the player chose in the options menu
    private void SetOptionsPreferences()
    {
        gameMixer.SetFloat("SFXVolume", Mathf.Log10(PlayerPrefs.GetFloat("SFXVolume", 0.8f)) * 20);
        gameMixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", 0.8f)) * 20);
    }

    //change snapshot depending on game state
    public void ChangeSnapshot(GameState state)
    {
        switch (state)
        {
            case GameState.Normal:
                AudioListener.pause = false;
                normalStateSnapshot.TransitionTo(0.0f);
                break;
            case GameState.Power:
                AudioListener.pause = false;
                powerStateSnapshot.TransitionTo(0.0f);
                break;
            case GameState.Pause:
            case GameState.Win:
            case GameState.Lose:
                AudioListener.pause = true;
                pauseStateSnapshot.TransitionTo(0.0f);
                break;
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        ChangeSnapshot(GameState.Normal);
    }
}
