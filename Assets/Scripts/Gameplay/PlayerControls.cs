using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private PlayerInput playerInput;

    private GameFlow gameManagerInstance;

    private ExaminerController player;

    private void Awake()
    {
        SetInputs();
    }

    private void Start()
    {
        gameManagerInstance = GameFlow.GameManagerInstance;

        player = GetComponent<ExaminerController>();
    }

    private void OnEnable()
    {
        playerInput.Player.Enable();
    }

    private void OnDisable()
    {
        playerInput.Player.Disable();
    }


    private void SetInputs()
    {
        //create new instance of playerInput
        playerInput = new PlayerInput();

        //enable action map
        playerInput.Player.Enable();


        //toggle devil power
        playerInput.Player.DevilPower.performed += context =>
        {
            player.DevilPower();
        };

        //Change signs
        playerInput.Player.SignUp.performed += context =>
        {
            //change selected sign to the sign in the upper side of the UI
            player.ChangeSignSkill(0);
        };

        playerInput.Player.SignLeft.performed += context =>
        {
            //change selected sign to the sign in the left side of the UI
            player.ChangeSignSkill(1);
        };

        playerInput.Player.SignRight.performed += context =>
        {
            //change selected sign to the sign in the right side of the UI
            player.ChangeSignSkill(2);
        };

        //Pause/Resume
        playerInput.Player.Pause.performed += context =>
        {
            //pause or resume the game
            switch (gameManagerInstance.State)
            {
                case GameState.Normal:
                    gameManagerInstance.State = GameState.Pause;
                    break;
                case GameState.Pause:
                    gameManagerInstance.State = GameState.Normal;
                    break;
                default:
                    break;
            }
        };
    }
}
