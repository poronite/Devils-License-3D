using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public enum GameState
{
    Normal, //player doing nothing and examinee is just driving
    Power, //when the player is choosing the new sign to change to
    Pause, //game paused
    Win, //player successfully fails the student
    Lose //student completes the exam
}


public class GameFlow : MonoBehaviour
{
    private static GameFlow gameManagerInstance;

    [SerializeField]
    private Volume postProcessingVolume;

    [SerializeField]
    private VolumeProfile normalProfile;

    [SerializeField]
    private VolumeProfile powerProfile;

    [SerializeField]
    private VolumeProfile pauseProfile;

    private UIManager uiManagerInstance;

    private MixerManager mixerManagerInstance;

    private CanvasGroup uiManagerCanvasGroup;

    private LoadingScreenController loadingScreen;

    private EventSystem uiInput;

    private GameState previousState = GameState.Normal;

    private GameState state = GameState.Normal;

    private int numMistakes = 0;


    public static GameFlow GameManagerInstance
    {
        get
        {
            return gameManagerInstance;
        }
    }

    //set state and change the game based on state
    public GameState State
    {
        get
        {
            return state;
        }

        set
        {
            previousState = state;

            state = value;

            ChangeGameState();
        }
    }

    //after setting value of mistakes update UI and verify if mistakes = 3
    //if so player wins
    public int NumMistakes
    {
        get
        {
            return numMistakes;
        }

        set
        {
            numMistakes = value;

            uiManagerInstance.UpdateNumMistakes(numMistakes);

            Debug.Log("Number of Mistakes: " + numMistakes);

            if (numMistakes == 3)
            {
                State = GameState.Win;
            }
        }
    }



    private void Awake()
    {
        //guarantee there's only one instance of the manager
        gameManagerInstance = this;
    }
    
    private void Start()
    {
        uiManagerInstance = UIManager.UIManagerInstance;

        mixerManagerInstance = MixerManager.MixerManagerInstance;

        uiManagerCanvasGroup = uiManagerInstance.gameObject.GetComponent<CanvasGroup>();

        loadingScreen = GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreenController>();

        uiInput = EventSystem.current;

        State = GameState.Normal;

        StartCoroutine(Timer(120f));
    }

    private void OnDestroy()
    {
        gameManagerInstance = null;
    }

    //change game based on state
    private void ChangeGameState()
    {
        switch (state)
        {
            case GameState.Normal:
                OnNormalGameState();
                break;
            case GameState.Power:
                OnPowerGameState();
                break;
            case GameState.Pause:
                OnPauseGameState();
                break;
            case GameState.Win:
                OnWinGameState();
                break;
            case GameState.Lose:
                OnLoseGameState();
                break;
            default:
                break;
        }

        mixerManagerInstance.ChangeSnapshot(state);
    }

    //when player is doing nothing
    private void OnNormalGameState()
    {
        switch (previousState)
        {
            case GameState.Power:
                //player deactivated power state
                uiManagerInstance.ToggleSignSelectionCircle(false);
                break;
            case GameState.Pause:
                //player resumed the game
                uiManagerInstance.TogglePauseMenu(false);
                break;
            default:
                break;
        }

        postProcessingVolume.profile = normalProfile;

        Time.timeScale = 1.0f;
        
    }

    //when the player entered power state and is choosing to which sign to replace
    private void OnPowerGameState()
    {
        postProcessingVolume.profile = powerProfile;
        Time.timeScale = 0.2f;
    }

    //when the player paused the game
    private void OnPauseGameState()
    {
        postProcessingVolume.profile = pauseProfile;
        Time.timeScale = 0.0f;
        uiManagerInstance.TogglePauseMenu(true);
    }

    //when the player wins the game
    private void OnWinGameState()
    {
        Time.timeScale = 0.0f;
        uiManagerInstance.ActivateVictoryScreen();
    }

    //when player loses the game
    private void OnLoseGameState()
    {
        Time.timeScale = 0.0f;
        uiManagerInstance.ActivateDefeatScreen();
    }


    //continue game after pausing
    //can't change enum directly with unity button event so had to write this function
    public void ContinueGame()
    {
        if (State == GameState.Pause)
            State = GameState.Normal;
    }


    //restart game by reseting scene
    public void RestartGame()
    {
        StartCoroutine(LoadNewScene(1));
    }

    //quit to main menu after playing the game or when in pause button
    public void QuitToMainMenu()
    {
        StartCoroutine(LoadNewScene(0));
    }

    //when restarting game or exiting to main menu
    private IEnumerator LoadNewScene(int sceneIndex)
    {
        //don't allow interactions with the buttons
        uiInput.sendNavigationEvents = false;
        uiManagerCanvasGroup.interactable = false;
        uiInput.SetSelectedGameObject(null);

        yield return StartCoroutine(loadingScreen.LoadingScreenFade(1, 1.5f));

        SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);

        //time scale is game wide so changing scene doesn't reset it back to default value
        Time.timeScale = 1.0f;        
    }

    //start timer for the game
    //if timer ends before player fails the student
    //player loses
    private IEnumerator Timer(float duration)
    {
        float elapsedTime = duration;

        while (elapsedTime > 0)
        {
            uiManagerInstance.UpdateTimer((int)elapsedTime);

            elapsedTime = Mathf.Clamp(elapsedTime - Time.deltaTime, 0, duration);

            yield return null;
        }

        State = GameState.Lose;
    }
}
