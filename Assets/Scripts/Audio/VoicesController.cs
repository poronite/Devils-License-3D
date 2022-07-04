using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum Character
{
    Ash,
    Liz
}

public enum Action
{
    GameStart,
    LosingGame,
    WinningGame,
    Mistake
}

public class VoicesController : MonoBehaviour
{
    [SerializeField]
    private float gameStartDelay = 1.0f;

    [SerializeField]
    private float delayLizStart = 0.5f;

    //Ash voices
    [SerializeField]
    private AudioSource[] ashGameStart;

    [SerializeField]
    private AudioSource[] ashLosingGame;

    [SerializeField]
    private AudioSource[] ashWinningGame;

    //Liz voices
    [SerializeField]
    private AudioSource[] lizGameStart;

    [SerializeField]
    private AudioSource[] lizMistake;


    private void Start()
    {
        PlayVoice(Character.Ash, Action.GameStart, gameStartDelay);
        PlayVoice(Character.Liz, Action.GameStart, gameStartDelay + ashGameStart[0].clip.length + delayLizStart);
    }


    public void PlayVoice(Character name, Action action, float delay = 0.0f)
    {
        //audio can be 1 file or multiple files
        AudioSource[] voiceArray = new AudioSource[1];

        switch (name)
        {
            case Character.Ash:
                switch (action)
                {
                    case Action.GameStart:
                        voiceArray = ashGameStart;
                        break;
                    case Action.LosingGame:
                        voiceArray = ashLosingGame;
                        break;
                    case Action.WinningGame:
                        voiceArray = ashWinningGame;
                        break;
                    case Action.Mistake:
                        break;
                    default:
                        break;
                }
                break;
            case Character.Liz:
                switch (action)
                {
                    case Action.GameStart:
                        voiceArray = lizGameStart;
                        break;
                    case Action.LosingGame:
                        
                        break;
                    case Action.WinningGame:
                        break;
                    case Action.Mistake:
                        voiceArray = lizMistake;
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }

        int audioIndex = 0;

        if (voiceArray.Length > 1)
        {
            Random.InitState((int)Time.unscaledTime);

            audioIndex = Random.Range(0, voiceArray.Length);
        }

        voiceArray[audioIndex].PlayDelayed(delay);
    }
}
