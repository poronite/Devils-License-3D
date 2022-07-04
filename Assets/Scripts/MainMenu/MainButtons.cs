using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainButtons : MonoBehaviour
{
    [SerializeField]
    private MenuManager menuManager;

    [SerializeField]
    private AudioSource gameStartSound;


    //trigger the coroutine below to change scene
    public void PlayButton()
    {
        StartCoroutine(PlayGame());
    }


    //do a black fade in and load the game
    public IEnumerator PlayGame()
    {
        //Don't allow the player to interact with the menu while game is fading the screen and loading new scene
        menuManager.buttonsCanvasGroup.interactable = false;

        gameStartSound.Play();

        yield return new WaitForSeconds(gameStartSound.clip.length - 1f);

        yield return StartCoroutine(menuManager.loadingScreen.LoadingScreenFade(1, 2f));

        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }


    //display the options by changing the screen state
    //see ChangeScreenState for more details on what happens when changing state
    public void OptionsButton()
    {
        menuManager.ChangeScreenState = ScreenState.Options;
    }


    //Exit game
    public void ExitButton()
    {
        Application.Quit();
    }
}
