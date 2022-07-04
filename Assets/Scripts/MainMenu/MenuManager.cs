using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public enum ScreenState
{
    MainButtons,
    Options
}

public class MenuManager : MonoBehaviour
{
    private ScreenState currentScreenState;

    [SerializeField]
    private EventSystem input;

    [SerializeField]
    private GameObject mainButtons;
    [SerializeField]
    private GameObject options;

    [SerializeField]
    public CanvasGroup buttonsCanvasGroup;
    [SerializeField]
    public LoadingScreenController loadingScreen;

    [SerializeField]
    private GameObject lastSelectedMainButton;
    [SerializeField]
    private GameObject lastSelectedOption;

    private GameObject lastSelectedUI;



    //when changing screen state the below happens
    public ScreenState ChangeScreenState
    {
        set
        {
            //store lastSelectedUI of last screen
            if (lastSelectedUI != null)
            {
                switch (currentScreenState)
                {
                    case ScreenState.MainButtons:
                        lastSelectedMainButton = lastSelectedUI;
                        break;
                    case ScreenState.Options:
                        lastSelectedOption = lastSelectedUI;
                        break;
                    default:
                        break;
                }
            }
            

            //change state
            currentScreenState = value;

            //close all screens
            mainButtons.SetActive(false);
            options.SetActive(false);

            //open screen based on state
            switch (currentScreenState)
            {
                case ScreenState.MainButtons:
                    mainButtons.SetActive(true);
                    lastSelectedUI = lastSelectedMainButton;
                    break;
                case ScreenState.Options:
                    options.SetActive(true);
                    lastSelectedUI = lastSelectedOption;
                    break;
                default:
                    break;
            }

            //see update comment below
            //EventSystem.current.SetSelectedGameObject(lastSelectedUI);
        }
    }


    //start scene with main buttons screen opened
    private void Start()
    {
        ChangeScreenState = ScreenState.MainButtons;
    }

    //decided to remove this because of the sound of the buttons
    //refresh current selected UI based on screen state
    //private void Update()
    //{
    //    if (EventSystem.current.currentSelectedGameObject != null)
    //    {
    //        lastSelectedUI = EventSystem.current.currentSelectedGameObject;
    //
    //    }
    //    else
    //    {
    //        EventSystem.current.SetSelectedGameObject(lastSelectedUI);
    //    }
    //}
}
