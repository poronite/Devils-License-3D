using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignSelectionUI : MonoBehaviour
{
    [SerializeField]
    private Sprite STOP, Speed30, Speed50, StraightAhead, TurnLeft, TurnRight, None;

    [SerializeField]
    private Image[] buttons = new Image[3];

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
                    buttons[i].sprite = StraightAhead;
                    break;
                case TrafficSigns.TurnLeftSign:
                    buttons[i].sprite = TurnLeft;
                    break;
                case TrafficSigns.TurnRightSign:
                    buttons[i].sprite = TurnRight;
                    break;
                case TrafficSigns.StopSign:
                    buttons[i].sprite = STOP;
                    break;
                case TrafficSigns.VelocityLimitSign30:
                    buttons[i].sprite = Speed30;
                    break;
                case TrafficSigns.VelocitySign50:
                    buttons[i].sprite = Speed50;
                    break;
                case TrafficSigns.None:
                    buttons[i].sprite = None;
                    break;
                default:
                    break;
            }
        }
    }
}
