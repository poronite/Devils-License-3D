using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    //Variables
    public Waypoint PreviousWaypoint;

    public Waypoint NextWaypointForward;
    public Waypoint NextWaypointLeft;
    public Waypoint NextWaypointRight;

    public GameObject TrafficTarget;
}
