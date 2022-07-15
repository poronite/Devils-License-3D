using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignSelectionUI : MonoBehaviour
{
    [SerializeField]
    private Sprite STOP, Speed30, Speed50, StraightAhead, TurnLeft, TurnRight, None;

    [SerializeField]
    private Button[] buttons = new Button[3];

    public Dictionary<int, TrafficSigns> Signs = new Dictionary<int, TrafficSigns>();

    //when this UI element is activated
    //display the possible signs that the player can change to
    private void OnEnable()
    {
        for (int i = 0; i < Signs.Count; i++)
        {
            switch (Signs[i])
            {
                case TrafficSigns.StraightAheadSign:
                    buttons[i].interactable = true;
                    buttons[i].image.sprite = StraightAhead;
                    break;
                case TrafficSigns.TurnLeftSign:
                    buttons[i].interactable = true;
                    buttons[i].image.sprite = TurnLeft;
                    break;
                case TrafficSigns.TurnRightSign:
                    buttons[i].interactable = true;
                    buttons[i].image.sprite = TurnRight;
                    break;
                case TrafficSigns.StopSign:
                    buttons[i].interactable = true;
                    buttons[i].image.sprite = STOP;
                    break;
                case TrafficSigns.VelocityLimitSign30:
                    buttons[i].interactable = true;
                    buttons[i].image.sprite = Speed30;
                    break;
                case TrafficSigns.VelocitySign50:
                    buttons[i].interactable = true;
                    buttons[i].image.sprite = Speed50;
                    break;
                case TrafficSigns.None:
                    buttons[i].interactable = false;
                    buttons[i].image.sprite = None;
                    break;
                default:
                    break;
            }
        }
    }
}
