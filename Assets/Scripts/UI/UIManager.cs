using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager uiManagerInstance;

    [SerializeField]
    private Image powerEnergyBarFill;

    [SerializeField]
    private Color SkillAvailableColor;

    [SerializeField]
    private Color SkillUnavailableColor;

    [SerializeField]
    private SignSelectionUI SignSelectionCircle;

    [SerializeField]
    private Image NumMistakesImage;

    [SerializeField]
    private Sprite NumMistakesSprite;

    [SerializeField]
    private TextMeshProUGUI TimerText;

    [SerializeField]
    private TextMeshProUGUI VelocimeterText;

    [SerializeField]
    private AnimationCurve VelocimeterCurve;

    [SerializeField]
    private Animator AshAnimator;

    [SerializeField]
    private Animator LizAnimator;

    [SerializeField]
    private GameObject VictoryScreen;

    [SerializeField]
    private GameObject DefeatScreen;

    [SerializeField]
    private GameObject PauseMenu;

    //[SerializeField] (not used in itch.io version)
    //private VoicesController voiceController;


    public static UIManager UIManagerInstance
    {
        get
        {
            return uiManagerInstance;
        }
    }


    private void Awake()
    {
        //guarantee there's only one instance of the manager
        uiManagerInstance = this;

        OrganizeHierarchy();
    }

    //just so that I don't have to always deactivate these object when testing game
    private void OrganizeHierarchy()
    {
        if (SignSelectionCircle.gameObject.activeInHierarchy)
            SignSelectionCircle.gameObject.SetActive(false);

        if (VictoryScreen.activeInHierarchy)
            VictoryScreen.SetActive(false);

        if (DefeatScreen.activeInHierarchy)
            DefeatScreen.SetActive(false);

        if (PauseMenu.activeInHierarchy)
            PauseMenu.SetActive(false);
    }

    private void OnDestroy()
    {
        uiManagerInstance = null;
    }


    //display energy left for slowdown state
    public void UpdatePowerEnergyBarFill(float timeLeft)
    {
        powerEnergyBarFill.fillAmount = timeLeft;
    }

    //change energy's color based on skill's availability
    public void UpdateSkillAvailability(bool availability)
    {
        if (availability)
        {
            powerEnergyBarFill.color = SkillAvailableColor;
        }
        else
        {
            powerEnergyBarFill.color = SkillUnavailableColor;
        }
    }

    //display or not the sign selection circle
    //if display get the possible signs
    public void ToggleSignSelectionCircle(bool activate, Dictionary<int, TrafficSigns> signs = null)
    {
        if (activate)
            SignSelectionCircle.Signs = signs;

        SignSelectionCircle.gameObject.SetActive(activate);
    }


    //display number of mistakes that the driver has made
    public void UpdateNumMistakes(int numMistakes)
    {
        NumMistakesImage.rectTransform.sizeDelta = new Vector2(NumMistakesImage.rectTransform.sizeDelta.x, NumMistakesSprite.rect.height * numMistakes);
        TriggerCharMistakeAnimations();
    }


    //display time left before player loses
    public void UpdateTimer(int time)
    {
        int minutes = time / 60;

        int seconds = time % 60;

        string minutesText = string.Empty;

        string secondsText = string.Empty;

        if (minutes < 10)
            minutesText = $"0{minutes}";
        else
            minutesText = $"{minutes}";

        if (seconds < 10)
            secondsText = $"0{seconds}";
        else
            secondsText = $"{seconds}";

        TimerText.text = $"{minutesText}:{secondsText}";
    }


    //display car's velocity
    public void UpdateVelocimeter(float speed)
    {
        int speedKMh = (int)VelocimeterCurve.Evaluate(speed);

        VelocimeterText.text = $"{speedKMh} km/h";
    }


    //when making a mistake trigger Ash and Liz animations
    private void TriggerCharMistakeAnimations()
    {
        AshAnimator.SetTrigger("Happy");
        LizAnimator.SetTrigger("Mistake");

        //play voices too (not used in itch.io version)
        //voiceController.PlayVoice(Character.Liz, Action.Mistake);
    }


    //on win
    public void ActivateVictoryScreen()
    {
        VictoryScreen.SetActive(true);

        //play voice too (not used in itch.io version)
        //voiceController.PlayVoice(Character.Ash, Action.WinningGame);
    }

    //on lose
    public void ActivateDefeatScreen()
    {
        DefeatScreen.SetActive(true);

        //play voice too (not used in itch.io version)
        //voiceController.PlayVoice(Character.Ash, Action.LosingGame);
    }

    //activate or deactivate pause menu
    public void TogglePauseMenu(bool pause)
    {
        PauseMenu.SetActive(pause);
    }
}
