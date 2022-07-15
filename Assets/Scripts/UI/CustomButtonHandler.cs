using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

enum UIType
{
    Button,
    Slider,
    Toggle
}

public class CustomButtonHandler : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    [SerializeField]
    private AudioSource buttonHoverSFX;

    [SerializeField]
    private CanvasGroup parentCanvasGroup;

    [SerializeField]
    private UIType uiType;

    private float minPitch = 1.0f;
    private float maxPitch = 1.1f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        bool canInteractWith = false;

        switch (uiType)
        {
            case UIType.Button:
                gameObject.TryGetComponent(out Button button);
                canInteractWith = button.interactable && parentCanvasGroup.interactable;
                break;
            case UIType.Slider:
                gameObject.TryGetComponent(out Slider slider);
                canInteractWith = slider.interactable && parentCanvasGroup.interactable;
                break;
            case UIType.Toggle:
                gameObject.TryGetComponent(out Toggle toggle);
                canInteractWith = toggle.interactable && parentCanvasGroup.interactable;
                break;
            default:
                break;
        }

        if (EventSystem.current.currentSelectedGameObject != gameObject && canInteractWith)
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
