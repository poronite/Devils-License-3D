using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad()]
public class WaypointGizmo
{
    //properties of the waypoint gizmo
    private static float sphereRadius = 0.2f;
    private static Color sphereColor = Color.yellow;
    private static Color lineColor = Color.green;

 
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(Waypoint waypoint, GizmoType gizmoType)
    {
        //draw the gizmo with the color depending if selected or not
        if ((gizmoType & GizmoType.Selected) != 0)
        {
            Gizmos.color = sphereColor;
        }
        else
        {
            Gizmos.color = sphereColor * 0.5f;
        }

        Gizmos.DrawSphere(waypoint.transform.position + new Vector3(0, sphereRadius, 0), sphereRadius);

        //draw a line between the waypoints
        Gizmos.color = lineColor;

        //forward
        if (waypoint.NextWaypointForward != null)
        {
            Gizmos.DrawLine(waypoint.transform.position, waypoint.NextWaypointForward.transform.position);
        }

        //left
        if (waypoint.NextWaypointLeft != null)
        {
            Gizmos.DrawLine(waypoint.transform.position, waypoint.NextWaypointLeft.transform.position);
        }

        //right
        if (waypoint.NextWaypointRight != null)
        {
            Gizmos.DrawLine(waypoint.transform.position, waypoint.NextWaypointRight.transform.position);
        }
    }
}
