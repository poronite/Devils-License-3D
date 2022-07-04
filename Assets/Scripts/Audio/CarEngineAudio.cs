using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngineAudio : MonoBehaviour
{
    [SerializeField]
    private AudioSource engineStart;

    [SerializeField]
    private AudioSource engineLoop;

    [SerializeField]
    private AudioSource engineStop;

    public void SetAccelerationAudio(AccelerationState state)
    {
        switch (state)
        {
            case AccelerationState.Fast:
            case AccelerationState.Slow:
                StartEngine();
                break;
            case AccelerationState.Stop:
                StopEngine();
                break;
            default:
                break;
        }
    }

    private void StartEngine()
    {
        engineStart.Play();

        engineLoop.PlayDelayed(engineStart.clip.length);
    }

    private void StopEngine()
    {
        engineLoop.Stop();

        engineStop.Play();
    }
}
