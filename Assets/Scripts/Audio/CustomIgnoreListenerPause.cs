using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CustomIgnoreListenerPause : MonoBehaviour
{
    [SerializeField]
    private bool ignoreListenerPause = false;

    void Awake()
    {
        SetIgnoreListenerPause();
    }

    private void SetIgnoreListenerPause()
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.ignoreListenerPause = ignoreListenerPause;
    }
}
