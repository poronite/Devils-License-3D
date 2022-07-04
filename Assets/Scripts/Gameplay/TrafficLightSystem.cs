using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum TrafficLightGroup
{
    Group1,
    Group2
}


public class TrafficLightSystem : MonoBehaviour
{
    private static TrafficLightSystem trafficLightSystemInstance;

    [SerializeField]
    private List<TrafficLight> trafficLightsGroup1;

    [SerializeField]
    private List<TrafficLight> trafficLightsGroup2;

    [SerializeField]
    private float duration = 8f;

    [SerializeField]
    private float yellowLightDelay = 0.5f;

    [SerializeField]
    private float allRedLightDelay = 2f;

    private float timeElapsed = 0f;

    private TrafficLightGroup currentGroupGreen = TrafficLightGroup.Group1;

    private Coroutine ChangeColorCoroutine = null;

    private WaitForSeconds yellowLightWaitForSeconds;
    private WaitForSeconds allRedLightWaitForSeconds;


    public static TrafficLightSystem TrafficLightSystemInstance
    {
        get
        {
            return trafficLightSystemInstance;
        }
    }


    private void Awake()
    {
        trafficLightSystemInstance = this;

        yellowLightWaitForSeconds = new WaitForSeconds(yellowLightDelay);
        allRedLightWaitForSeconds = new WaitForSeconds(allRedLightDelay);
    }


    private void Start()
    {
        TurnOnTrafficLights();
    }


    private void OnDestroy()
    {
        trafficLightSystemInstance = null;
    }


    //turn on traffic lights at the start of the game
    private void TurnOnTrafficLights()
    {
        //set all red
        foreach (TrafficLight trafficLight in trafficLightsGroup1)
        {
            trafficLight.CurrentLightState = LightState.Red;
        }

        foreach (TrafficLight trafficLight in trafficLightsGroup2)
        {
            trafficLight.CurrentLightState = LightState.Red;
        }

        //set lights that are part of currentGroupGreen to green
        switch (currentGroupGreen)
        {
            case TrafficLightGroup.Group1:
                foreach (TrafficLight trafficLight in trafficLightsGroup1)
                {
                    trafficLight.CurrentLightState = LightState.Green;
                }
                break;
            case TrafficLightGroup.Group2:
                foreach (TrafficLight trafficLight in trafficLightsGroup2)
                {
                    trafficLight.CurrentLightState = LightState.Green;
                }
                break;
            default:
                break;
        }
    }


    private void Update()
    {
        ChangeColorDelay();               
    }


    //between each change color there is a delay determined by variable duration
    private void ChangeColorDelay()
    {
        if (ChangeColorCoroutine == null)
        {
            timeElapsed += Time.deltaTime;

            if (timeElapsed >= duration)
            {
                timeElapsed = 0f;
                ChangeColorCoroutine = StartCoroutine(ChangeColorGroup());
            }
        }
    }


    //change color of a traffic light group
    IEnumerator ChangeColorGroup()
    {
        //change the traffic light group that is green to yellow
        switch (currentGroupGreen)
        {
            case TrafficLightGroup.Group1:
                foreach (TrafficLight trafficLight in trafficLightsGroup1)
                {
                    trafficLight.CurrentLightState = LightState.Yellow;
                }
                break;
            case TrafficLightGroup.Group2:
                foreach (TrafficLight trafficLight in trafficLightsGroup2)
                {
                    trafficLight.CurrentLightState = LightState.Yellow;
                }
                break;
            default:
                break;
        }

        //yellow light duration
        yield return yellowLightWaitForSeconds;

        //change the lights to green or red
        switch (currentGroupGreen)
        {
            case TrafficLightGroup.Group1:
                //change yellow lights to red
                foreach (TrafficLight trafficLight in trafficLightsGroup1)
                {
                    trafficLight.CurrentLightState = LightState.Red;
                }

                //all red delay
                yield return allRedLightWaitForSeconds;

                //change previous red lights to green
                foreach (TrafficLight trafficLight in trafficLightsGroup2)
                {
                    trafficLight.CurrentLightState = LightState.Green;
                }
                currentGroupGreen = TrafficLightGroup.Group2;

                break;
            case TrafficLightGroup.Group2:
                //change yellow lights to red
                foreach (TrafficLight trafficLight in trafficLightsGroup2)
                {
                    trafficLight.CurrentLightState = LightState.Red;
                }

                //all red delay
                yield return allRedLightWaitForSeconds;

                //change previous red lights to green
                foreach (TrafficLight trafficLight in trafficLightsGroup1)
                {
                    trafficLight.CurrentLightState = LightState.Green;
                }
                currentGroupGreen = TrafficLightGroup.Group1;

                break;
            default:
                break;
        }

        ChangeColorCoroutine = null;
    }


    //when the protagonist uses his power on a traffic light
    public IEnumerator ModifyTrafficLight(TrafficLight target)
    {
        target.CurrentLightState = LightState.Yellow;

        yield return yellowLightWaitForSeconds;

        target.CurrentLightState = LightState.Red;

        yield return allRedLightWaitForSeconds; //After a while reset to green light

        target.CurrentLightState = LightState.Green;
    }
}
