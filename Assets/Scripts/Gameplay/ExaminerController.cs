using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExaminerController : MonoBehaviour
{
    private GameFlow gameManagerInstance;

    private UIManager uiManagerInstance;

    private TrafficLightSystem trafficLightSystemInstance;

    private VehicleAI car;

    

    [SerializeField]
    private float maxPowerTimeLeft = 5.0f;

    [SerializeField]
    private float maxDistanceMistake = 2.0f;

    private TrafficSign targetTrafficSign;

    private TrafficLight targetTrafficLight;

    private float powerTimeLeft;

    private bool canUseSkill = true;

    private TrafficSigns previousSignType = TrafficSigns.None;


    //time left for slowdown state
    public float PowerTimeLeft
    {
        get
        {
            return powerTimeLeft;
        }

        set
        {
            powerTimeLeft = value;

            //update UI
            uiManagerInstance.UpdatePowerEnergyBarFill(powerTimeLeft / maxPowerTimeLeft);
        }
    }


    public bool CanUseSkill
    {
        get
        {
            return canUseSkill;
        }

        set
        {
            canUseSkill = value;

            //update UI
            uiManagerInstance.UpdateSkillAvailability(canUseSkill);
        }
    }


    private void Start()
    {
        gameManagerInstance = GameFlow.GameManagerInstance;

        uiManagerInstance = UIManager.UIManagerInstance;

        trafficLightSystemInstance = TrafficLightSystem.TrafficLightSystemInstance;

        car = GetComponent<VehicleAI>();

        PowerTimeLeft = maxPowerTimeLeft;
    }

    private void Update()
    {
        RegulatePower();

        //Debug.Log($"Current State: {gameManagerInstance.State} | Skill availabilty: {CanUseSkill} | Energy left: {PowerTimeLeft}");
    }


    //Update remaining power time based on state
    private void RegulatePower()
    {
        //I change the powerTimeLeft with the accessor to trigger the function to update the UI
        switch (gameManagerInstance.State)
        {
            case GameState.Normal:
                if (PowerTimeLeft < maxPowerTimeLeft) //get power back
                {
                    PowerTimeLeft = Mathf.Clamp(PowerTimeLeft + Time.deltaTime, 0f, maxPowerTimeLeft);
                }
                else if (PowerTimeLeft >= maxPowerTimeLeft && !CanUseSkill) //end skill cooldown
                {
                    CanUseSkill = true;
                }
                break;
            case GameState.Power:
                if (PowerTimeLeft > 0.0f)
                {
                    //unscaledDeltaTime to empty the power bar normally since this state runs at a reduced scale time
                    PowerTimeLeft = PowerTimeLeft - Time.unscaledDeltaTime; 

                    //I don't use clamp here like above because I use the below if for that
                    if (PowerTimeLeft <= 0.0f)
                    {
                        PowerTimeLeft = 0.0f; //clamp to 0

                        gameManagerInstance.State = GameState.Normal; //deactivate power state
                    }
                }
                break;
            default:
                break;
        }
    }


    //Activate or deactivate the power based on state when the player presses Power Key
    public void DevilPower()
    {
        switch (gameManagerInstance.State)
        {
            case GameState.Normal:

                GameObject trafficTarget = null;

                if (car.CurrentWaypointTrafficTarget)
                {
                    trafficTarget = car.CurrentWaypointTrafficTarget;
                }

                if (trafficTarget)
                {
                    if (CanUseSkill && PowerTimeLeft > 0.0f) //requirements to use skill
                    {
                        //traffic sign version
                        if (trafficTarget.TryGetComponent(out targetTrafficSign))
                        {
                            if (targetTrafficSign.TrafficSignType == TrafficSigns.PedestrianSign)
                            {
                                //trigger dudes
                            }
                            else
                            {
                                uiManagerInstance.ToggleSignSelectionCircle(true, targetTrafficSign.PossibleSigns);

                                gameManagerInstance.State = GameState.Power;
                            }

                            

                        }
                        else if (trafficTarget.TryGetComponent(out targetTrafficLight) && targetTrafficLight.CurrentLightState == LightState.Green) //traffic light version
                        {
                            ChangeLightSkill();
                        }
                    }
                }
                break;
            case GameState.Power:
                gameManagerInstance.State = GameState.Normal;
                break;
            default:
                break;
        }
    }


    //change sign based on option selected while in slowdown state
    public void ChangeSignSkill(int signNumber)
    {
        //only activate if game is slowed down
        if (gameManagerInstance.State == GameState.Power)
        {
            if (targetTrafficSign.ChangeTrafficSignTypeSkill(signNumber, out previousSignType) != TrafficSigns.None)
            {
                gameManagerInstance.State = GameState.Normal;

                EnterSkillCooldown();

                VerifyMistake();
            }            
        }
    }

    //if driver is too close to sign add a mistake
    private void VerifyMistake()
    {
        float distance = Vector3.Distance(transform.position, targetTrafficSign.TrafficSignWaypoint.transform.position);

        Debug.Log("Distance to target: " + distance);

        if (distance < maxDistanceMistake)
        {
            //assign previous sign when mistake is done to guarantee that the driver will actually do the mistake
            car.PreviousSignType = previousSignType;

            gameManagerInstance.NumMistakes += 1;
        }
    }


    //change color of traffic light (only works from green to red)
    private void ChangeLightSkill()
    {
        //activate skill
        StartCoroutine(trafficLightSystemInstance.ModifyTrafficLight(targetTrafficLight));

        EnterSkillCooldown();
    }

    //put skill on cooldown
    private void EnterSkillCooldown()
    {
        CanUseSkill = false;
        PowerTimeLeft = 0.0f;
    }
}
