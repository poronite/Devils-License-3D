using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AccelerationState
{
    Fast,
    Slow,
    Stop
}

public class VehicleAI : MonoBehaviour
{
    //Variables
    private float speed;

    [SerializeField]
    private float maxSpeed = 6;

    [SerializeField]
    private float minSpeed = 3;

    [SerializeField]
    private float accelerationDuration = 1f;

    [SerializeField] //for now it's serializable
    private DriverTemplate examinee;

    private AccelerationState currentAccelerationState = AccelerationState.Fast;

    private AccelerationState lastAccelerationState = AccelerationState.Stop;

    //used to make sure that the coroutine doesn't overlap
    private Coroutine ChangingSpeed = null;

    [SerializeField]
    private float rotationSpeed = 120;

    [SerializeField]
    private float raycastLength = 4;

    private Rigidbody rb;

    private Vector3 raycastOriginOffset = new Vector3(0, 0.5f, 0);

    private Waypoint currentDestination;

    [SerializeField]
    private Waypoint startWaypoint;

    private bool stoppedLastFrame = false;

    private GameObject waypointTrafficTarget;

    private TrafficLight currentCollidingTrafficLight = null;

    //store the previous type when activating skill
    private TrafficSigns previousSignType = TrafficSigns.None;

    private UIManager uiManagerInstance;

    private CarEngineAudio carEngine;

    public GameObject CurrentWaypointTrafficTarget
    {
        get
        {
            return waypointTrafficTarget;
        }
    }

    public DriverTemplate Examinee
    {
        get
        {
            return examinee;
        }
    }

    public TrafficSigns PreviousSignType
    {
        set
        {
            previousSignType = value;
        }
    }


    //Functions
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        uiManagerInstance = UIManager.UIManagerInstance;

        carEngine = transform.GetChild(1).gameObject.GetComponent<CarEngineAudio>();

        if (examinee != null)
            accelerationDuration += examinee.ReactionTime;

        SetNewDestination();

        ChangingSpeed = StartCoroutine(ChangeSpeed(AccelerationState.Fast, accelerationDuration));

        lastAccelerationState = currentAccelerationState;
    }


    //when colliding with waypoint, do the following:
    //if that waypoint's traffic target is equal to the car's traffic target, act depending on the sign
    //change the current destination based on chosenPath, and if that waypoint has a traffic target assign it to the car's traffic target, so that the player can activate the skill
    private void SetNewDestination()
    {
        //car has a route
        if (currentDestination != null)
        {
            int chosenPath = -1;

            //if current destination has a traffic sign/light assigned to it
            if (currentDestination.TrafficTarget != null && currentDestination.TrafficTarget == waypointTrafficTarget)
            {
                if (waypointTrafficTarget.GetComponent<TrafficLight>())
                {
                    TrafficLight targetTrafficLight = waypointTrafficTarget.GetComponent<TrafficLight>();
                }
                else if (waypointTrafficTarget.GetComponent<TrafficSign>())
                {
                    TrafficSigns targetTrafficSignType = TrafficSigns.None;

                    if (previousSignType == TrafficSigns.None)
                    {
                        targetTrafficSignType = waypointTrafficTarget.GetComponent<TrafficSign>().TrafficSignType;
                    }
                    else
                    {
                        targetTrafficSignType = previousSignType;
                    }

                    switch (targetTrafficSignType)
                    {
                        //mandatory direction signs
                        case TrafficSigns.StraightAheadSign:
                            chosenPath = 0;
                            break;
                        case TrafficSigns.TurnLeftSign:
                            chosenPath = 1;
                            break;
                        case TrafficSigns.TurnRightSign:
                            chosenPath = 2;
                            break;

                        //other signs related to velocity
                        case TrafficSigns.PedestrianSign:
                            break;
                        case TrafficSigns.StopSign:
                            if (ChangingSpeed == null)
                                ChangingSpeed = StartCoroutine(ChangeSpeed(AccelerationState.Stop, accelerationDuration, true));
                            break;
                        case TrafficSigns.VelocityLimitSign30:
                            if (ChangingSpeed == null)
                                ChangingSpeed = StartCoroutine(ChangeSpeed(AccelerationState.Slow, accelerationDuration));
                            break;
                        case TrafficSigns.VelocitySign50:
                            if (ChangingSpeed == null)
                                ChangingSpeed = StartCoroutine(ChangeSpeed(AccelerationState.Fast, accelerationDuration));
                            break;
                        default:
                            break;
                    }
                }
            }

            //if waypoint has multiple nextWaypoints and no mandatory direction sign
            if (chosenPath == -1)
            {
                bool[] possiblePaths = new bool[3] { false, false, false };

                if (currentDestination.NextWaypointForward != null)
                {
                    possiblePaths[0] = true;
                }
                if (currentDestination.NextWaypointLeft != null)
                {
                    possiblePaths[1] = true;
                }
                if (currentDestination.NextWaypointRight != null)
                {
                    possiblePaths[2] = true;
                }

                Random.InitState((int)Time.unscaledTime);
                //while path is not chosen loop and randomize new path
                while (chosenPath == -1)
                {
                    int selectedPath = Random.Range(0, possiblePaths.Length);

                    //if selected path doesn't exist loop again
                    if (possiblePaths[selectedPath] == true)
                    {
                        chosenPath = selectedPath;
                    }
                    else
                    {
                        chosenPath = -1;
                    }
                }
            }

            //decide next destination based on chosen path
            switch (chosenPath)
            {
                case 0:
                    currentDestination = currentDestination.NextWaypointForward;
                    break;
                case 1:
                    currentDestination = currentDestination.NextWaypointLeft;
                    break;
                case 2:
                    currentDestination = currentDestination.NextWaypointRight;
                    break;
                default:
                    break;
            }

            //target destination waypoint
            if (currentDestination.TrafficTarget != null)
            {
                waypointTrafficTarget = currentDestination.TrafficTarget;
            }
            else
            {
                waypointTrafficTarget = null;
            }
        }
        else //car doesn't have a route
        {
            currentDestination = startWaypoint;
        }

        //Debug.Log(currentDestination.name);
    }


    //change the car's speed based on state over a certain duration
    //if car stopped because of stop sign reaccelerate again after a while
    IEnumerator ChangeSpeed(AccelerationState state, float duration, bool isStopSign = false)
    {
        float time = 0f;

        float initialSpeed = speed;

        float finalSpeed = 0f;

        //if supposed to accelerate, set speed based on destination waypoint sign speed limit
        switch (state)
        {
            case AccelerationState.Fast:
                finalSpeed = maxSpeed;

                if (initialSpeed == 0)
                {
                    carEngine.SetAccelerationAudio(state);
                }
                break;
            case AccelerationState.Slow:
                finalSpeed = minSpeed;

                if (initialSpeed == 0)
                {
                    carEngine.SetAccelerationAudio(state);
                }
                break;
            case AccelerationState.Stop:
                finalSpeed = 0f;

                if (initialSpeed >= 0)
                {
                    carEngine.SetAccelerationAudio(state);
                }
                break;
            default:
                break;
        }

        //slow or speed up the car gradually over a certain period of time
        while (time < duration)
        {
            speed = Mathf.Lerp(initialSpeed, finalSpeed, time / duration);

            time += Time.deltaTime;

            yield return null;
        }

        speed = finalSpeed;

        //wait 3 extra seconds and accelerate again if there is a stop sign
        if (state == AccelerationState.Stop && isStopSign)
        {
            Debug.Log("Stop sign");

            time = 0f;

            yield return new WaitForSeconds(3f);

            AccelerationState newState = AccelerationState.Fast;

            if (initialSpeed == 3)
            {
                newState = AccelerationState.Slow;
            }
            else
            {
                newState = AccelerationState.Fast;
            }

            carEngine.SetAccelerationAudio(newState);

            while (time < duration)
            {
                speed = Mathf.Lerp(speed, initialSpeed, time / duration);

                time += Time.deltaTime;

                yield return null;
            }

            speed = initialSpeed;
        }

        //set coroutine variable to null so that the coroutine can execute again if needed
        ChangingSpeed = null;
    }



    //function that moves the car (with transform.position)
    private void MoveRotateCar()
    {
        //direction to the destination
        Vector3 direction = (currentDestination.transform.position - rb.position).normalized;

        //rotate car
        rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime));

        //move car
        rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
    }


    ///<summary>Verify if there are obstacles in front of the vehicle.</summary>
    ///<returns>Returns false if there are obstacles, otherwise returns true.</returns>
    private void PathClear()
    {
        //stoppedLastFrame variable is used to prevent this function 
        //from always accelerating the car when the raycast isn't hitting anything


        RaycastHit hit;

        Debug.DrawRay(transform.position + raycastOriginOffset, transform.forward * raycastLength, Color.cyan);

        if (Physics.Raycast(transform.position + raycastOriginOffset, transform.forward, out hit, raycastLength))
        {
            switch (hit.collider.tag)
            {
                case "TrafficLight":
                    //only stop at traffic lights that have the lights that the are of the same direction of the car
                    Debug.Log($"Name: {hit.collider.gameObject.name} | Light state: {hit.collider.GetComponent<TrafficLight>().CurrentLightState}");

                    if (SameDirection(transform.forward, hit.collider.transform.forward) && hit.collider.GetComponent<TrafficLight>().CurrentLightState != LightState.Green)
                    {
                        Debug.Log($"Light state: {hit.collider.GetComponent<TrafficLight>().CurrentLightState}");

                        if (!stoppedLastFrame)
                        {
                            Debug.Log($"Stopped at {hit.collider.name}");
                            stoppedLastFrame = true;
                            lastAccelerationState = currentAccelerationState;
                            currentAccelerationState = AccelerationState.Stop;

                            if (ChangingSpeed == null)
                            {
                                ChangingSpeed = StartCoroutine(ChangeSpeed(currentAccelerationState, accelerationDuration));
                            }
                        }
                        
                    }
                    else
                    {
                        currentAccelerationState = lastAccelerationState;

                        if (ChangingSpeed == null)
                        {
                            ChangingSpeed = StartCoroutine(ChangeSpeed(currentAccelerationState, accelerationDuration));
                        }
                    }
                    
                    break;
                case "Player":
                case "Vehicle":
                case "Pedestrian":
                    if (!stoppedLastFrame)
                    {
                        Debug.Log("Stopped to avoid an acident.");
                        stoppedLastFrame = true;
                        lastAccelerationState = currentAccelerationState;
                        currentAccelerationState = AccelerationState.Stop;

                        if (ChangingSpeed == null)
                        {
                            ChangingSpeed = StartCoroutine(ChangeSpeed(currentAccelerationState, accelerationDuration));
                        }
                    }
                    
                    break;
                default:
                    break;
            }
        }
        else
        {
            //when not in front of any obstacles
            if (stoppedLastFrame == true && ChangingSpeed == null)
            {
                stoppedLastFrame = false;
                currentAccelerationState = lastAccelerationState;
                ChangingSpeed = StartCoroutine(ChangeSpeed(currentAccelerationState, accelerationDuration));
            }
        }
        
    }


    //update speed (km/h) displayed on velocimeter UI
    private void UpdateVelocimeter()
    {
        uiManagerInstance.UpdateVelocimeter(speed);
    }


    //if colliding with a red traffic light area, add a mistake
    private void DetectTrafficLightMistake()
    {
        if (currentCollidingTrafficLight != null && currentCollidingTrafficLight.CurrentLightState == LightState.Red)
        {
            currentCollidingTrafficLight = null;
            GameFlow.GameManagerInstance.NumMistakes += 1;
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        //set new destination if car arrived to waypoint
        if (other.gameObject == currentDestination.gameObject)
        {
            Debug.Log("New destination");

            SetNewDestination();

            //reset previous sign type after driver drives according to the wrong sign (previous sign)
            previousSignType = TrafficSigns.None;
        }

        //in case player collides with anything
        if (gameObject.CompareTag("Player"))
        {
            switch (other.tag)
            {
                case "TrafficLight": //store this object while player is inside it in case it changes color to red                    
                    if (SameDirection(transform.forward, other.transform.forward) && currentCollidingTrafficLight == null)
                    {
                        currentCollidingTrafficLight = other.GetComponent<TrafficLight>();
                    }
                    break;
                case "Pedestrian":
                    //increase mistake
                    break;
                default:
                    break;
            }
        }
    }

    //if car and traffic light are in the same direction
    private bool SameDirection(Vector3 car, Vector3 trafficLight)
    {
        return Vector3.Dot(car, trafficLight) > 0.8f;
    }

    private void OnTriggerExit(Collider other)
    {
        if (gameObject.CompareTag("Player"))
        {
            if (other.TryGetComponent(out TrafficLight light) && light == currentCollidingTrafficLight)
            {
                currentCollidingTrafficLight = null;
            }
        }
    }


    private void FixedUpdate()
    {
        PathClear();

        MoveRotateCar();

        DetectTrafficLightMistake();

        UpdateVelocimeter();
    }
}
