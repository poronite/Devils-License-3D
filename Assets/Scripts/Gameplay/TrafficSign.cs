using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrafficSigns
{
    None,
    StraightAheadSign,
    TurnLeftSign,
    TurnRightSign,
    PedestrianSign,
    StopSign,
    VelocityLimitSign30,
    VelocitySign50
}

public class TrafficSign : MonoBehaviour
{
    private Dictionary<int, TrafficSigns> possibleSigns = new Dictionary<int, TrafficSigns>(3);

    [SerializeField]
    private TrafficSigns trafficSignType = TrafficSigns.PedestrianSign;

    [SerializeField]
    private Waypoint trafficSignWaypoint = null;

    [SerializeField]
    private GameObject pedestrianSign, stopSign, speed30Sign, speed50Sign, straightAheadSign, turnLeftSign, turnRightSign;

    public TrafficSigns TrafficSignType
    {
        get
        {
            return trafficSignType;
        }
        set
        {
            trafficSignType = value;

            CreateDictionaryPossibleSigns();

            ChangeSignGraphics();
        }
    }

    public Dictionary<int, TrafficSigns> PossibleSigns
    {
        get
        {
            return possibleSigns;
        }
    }

    public Waypoint TrafficSignWaypoint
    {
        get
        {
            return trafficSignWaypoint;
        }
    }


    private void Start()
    {
        CreateDictionaryPossibleSigns();

        ChangeSignGraphics();
    }

    //create a dictionary for the possible signs
    //it will then be used to get the sign that the player selected
    private void CreateDictionaryPossibleSigns()
    {
        //clear previous values
        if (possibleSigns != null)
        {
            possibleSigns = new Dictionary<int, TrafficSigns>(3);
        }

        List<TrafficSigns> trafficSigns = new List<TrafficSigns>(0);

        //create array with possible trafficSigns for each current sign
        switch (trafficSignType)
        {
            case TrafficSigns.StraightAheadSign:
                trafficSigns = new List<TrafficSigns>(3) { TrafficSigns.StopSign, TrafficSigns.None, TrafficSigns.None };

                if (trafficSignWaypoint.NextWaypointLeft != null)
                {
                    trafficSigns[1] = TrafficSigns.TurnLeftSign;
                }

                if (trafficSignWaypoint.NextWaypointRight != null)
                {
                    trafficSigns[2] = TrafficSigns.TurnRightSign;
                }

                break;
            case TrafficSigns.TurnLeftSign:
                trafficSigns = new List<TrafficSigns>(3) { TrafficSigns.StopSign, TrafficSigns.None, TrafficSigns.None };

                if (trafficSignWaypoint.NextWaypointForward != null)
                {
                    trafficSigns[1] = TrafficSigns.StraightAheadSign;
                }

                if (trafficSignWaypoint.NextWaypointRight != null)
                {
                    trafficSigns[2] = TrafficSigns.TurnRightSign;
                }

                break;
            case TrafficSigns.TurnRightSign:
                trafficSigns = new List<TrafficSigns>(3) { TrafficSigns.StopSign, TrafficSigns.None, TrafficSigns.None };

                if (trafficSignWaypoint.NextWaypointForward != null)
                {
                    trafficSigns[1] = TrafficSigns.StraightAheadSign;
                }

                if (trafficSignWaypoint.NextWaypointLeft != null)
                {
                    trafficSigns[2] = TrafficSigns.TurnLeftSign;
                }

                break;
            case TrafficSigns.StopSign:
                trafficSigns = new List<TrafficSigns>(3) { TrafficSigns.VelocityLimitSign30, TrafficSigns.VelocitySign50, TrafficSigns.None };
                break;
            case TrafficSigns.VelocityLimitSign30:
                trafficSigns = new List<TrafficSigns>(3) { TrafficSigns.VelocitySign50, TrafficSigns.StopSign, TrafficSigns.None };
                break;
            case TrafficSigns.VelocitySign50:
                trafficSigns = new List<TrafficSigns>(3) { TrafficSigns.VelocityLimitSign30, TrafficSigns.StopSign, TrafficSigns.None };
                break;
            default:
                break;
        }

        //if array has 1 or more elements it means it's not a pedestrian sign
        if (trafficSigns.Count != 0)
        {
            int listSize = trafficSigns.Count;

            for (int i = 0; i < listSize; i++)
            {
                TrafficSigns randomSign = trafficSigns[Random.Range(0, trafficSigns.Count)];

                //add 1 to match the numbers that the player controls sends
                possibleSigns.Add(i, randomSign);

                trafficSigns.Remove(randomSign);
            }
        }
    }


    //skill that changes the sign's type
    public TrafficSigns ChangeTrafficSignTypeSkill(int newSign, out TrafficSigns oldType)
    {
        oldType = TrafficSigns.None;

        if (possibleSigns.TryGetValue(newSign, out TrafficSigns newType) && newType != TrafficSigns.None)
        {
            oldType = TrafficSignType;

            TrafficSignType = newType;

            return TrafficSignType; //if successful
        }
        
        return TrafficSigns.None;
    }


    //change sign's graphics based on selected sign type
    private void ChangeSignGraphics()
    {
        //if gameObject has a sign graphic delete it before instantiating a new one
        if (transform.childCount == 1)
        {
            Destroy(transform.GetChild(0).gameObject);
        }

        GameObject newSignGraphic;

        switch (TrafficSignType)
        {
            case TrafficSigns.StraightAheadSign:
                newSignGraphic = Instantiate(straightAheadSign, transform, false);
                break;
            case TrafficSigns.TurnLeftSign:
                newSignGraphic = Instantiate(turnLeftSign, transform, false);
                break;
            case TrafficSigns.TurnRightSign:
                newSignGraphic = Instantiate(turnRightSign, transform, false);
                break;
            case TrafficSigns.PedestrianSign:
                newSignGraphic = Instantiate(pedestrianSign, transform, false);
                break;
            case TrafficSigns.StopSign:
                newSignGraphic = Instantiate(stopSign, transform, false);
                break;
            case TrafficSigns.VelocityLimitSign30:
                newSignGraphic = Instantiate(speed30Sign, transform, false);
                break;
            case TrafficSigns.VelocitySign50:
                newSignGraphic = Instantiate(speed50Sign, transform, false);
                break;
            default:
                break;
        }
    }
}
