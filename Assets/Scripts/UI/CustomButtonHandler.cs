using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class CustomButtonHandler : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    [SerializeField]
    private AudioSource buttonHoverSFX;

    private float minPitch = 1.0f;
    private float maxPitch = 1.1f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        PlayButtonHoverSound();
    }

    private void PlayButtonHoverSound()
    {
        Random.InitState((int)Time.unscaledTime);

        buttonHoverSFX.pitch = Random.Range(minPitch, maxPitch);

        buttonHoverSFX.Play();
    }
}
