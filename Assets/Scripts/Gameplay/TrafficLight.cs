using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LightState
{
    Green,
    Yellow,
    Red
}

public class TrafficLight : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer greenLightRenderer;

    [SerializeField]
    private MeshRenderer yellowLightRenderer;

    [SerializeField]
    private MeshRenderer redLightRenderer;

    [SerializeField]
    private Material[] lights = new Material[6];

    private LightState currentLightState = LightState.Green;

    //when changing the state of traffic light
    //change also the traffic light's graphics
    public LightState CurrentLightState
    {
        get
        {
            return currentLightState;
        }

        set
        {
            currentLightState = value;

            ChangeLightGraphics();
        }
    }


    //change traffic light's color
    private void ChangeLightGraphics()
    {
        greenLightRenderer.material = lights[0];
        yellowLightRenderer.material = lights[2];
        redLightRenderer.material = lights[4];

        switch (currentLightState)
        {
            case LightState.Green:
                greenLightRenderer.material = lights[1];
                break;
            case LightState.Yellow:
                yellowLightRenderer.material = lights[3];
                break;
            case LightState.Red:
                redLightRenderer.material = lights[5];
                break;
            default:
                break;
        }
    }
}
