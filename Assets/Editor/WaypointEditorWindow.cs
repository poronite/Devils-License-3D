using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


//Reference: https://www.youtube.com/watch?v=MXCZ-n5VyJc

enum WaypointType
{
    Forward,
    Left,
    Right
}

public class WaypointEditorWindow : EditorWindow
{
    //Variables
    ///<summary>Game object where the waypoint will be created.</summary>
    public Transform WaypointParent;

    private List<Waypoint> waypoints = new List<Waypoint>();

    private Vector3 waypointColliderCenter = new Vector3(0, 0.1f, 0);
    private Vector3 waypointColliderSize = new Vector3(0.2f, 0.2f, 0.2f);
    
    //Functions
    //Open editor window
    [MenuItem("Tools/Waypoint Editor")]
    public static void Open()
    {
        GetWindow<WaypointEditorWindow>();
    }

    //Update editor window elements
    private void OnGUI()
    {
        SerializedObject window = new SerializedObject(this);

        EditorGUILayout.PropertyField(window.FindProperty("WaypointParent"));

        //only draw editor elements if there is a gameobject selected
        if (WaypointParent != null)
        {
            EditorGUILayout.BeginVertical("Box");
            DrawButtons();
            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.HelpBox("Waypoint Parent not assigned!", MessageType.Warning);
        }
            

        window.ApplyModifiedProperties();
    }


    //buttons for the window that allow to create and delete waypoints
    private void DrawButtons()
    {
        if (!IsWaypointSelected())
        {
            //create waypoint after the last waypoint (Forward/Left/Right possible)
            EditorGUILayout.BeginHorizontal("Box");
            if (GUILayout.Button("Create Waypoint Left"))
            {
                CreateWaypoint(WaypointType.Left);
            }

            if (GUILayout.Button("Create Waypoint Forward"))
            {
                CreateWaypoint(WaypointType.Forward);
            }

            if (GUILayout.Button("Create Waypoint Right"))
            {
                CreateWaypoint(WaypointType.Right);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        if (IsWaypointSelected())
        {
            //create waypoint before the selected waypoint (only Forward)
            if (GUILayout.Button("Create Waypoint Before"))
            {
                CreateWaypointBefore();
            }

            //create waypoint after the selected waypoint (Forward/Left/Right possible)
            EditorGUILayout.BeginHorizontal("Box");
            if (GUILayout.Button("Create Waypoint Left After"))
            {
                CreateWaypointAfter(WaypointType.Left);
            }

            if (GUILayout.Button("Create Waypoint Forward After"))
            {
                CreateWaypointAfter(WaypointType.Forward);
            }            

            if (GUILayout.Button("Create Waypoint Right After"))
            {
                CreateWaypointAfter(WaypointType.Right);
            }
            EditorGUILayout.EndHorizontal();

            //delete waypoint
            if (GUILayout.Button("Delete Waypoint"))
            {
                DeleteWaypoint();
            }

            //if first or last waypoint of a path is selected give the option to close the path in a loop
            if (IsLooseEndSelected())
            {
                if (GUILayout.Button("Close Path"))
                {
                    ClosePath();
                }
            }
        }
    }

    private bool IsWaypointSelected()
    {
        return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Waypoint>();
    }

    //is the first or last waypoints of the path selected?
    private bool IsLooseEndSelected()
    {
        bool looseEndSelected = false;

        waypoints = new List<Waypoint>(WaypointParent.GetComponentsInChildren<Waypoint>());

        Waypoint selectedWaypoint = null;

        //to avoid editor errors
        if (IsWaypointSelected())
            selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        if (selectedWaypoint == null && (selectedWaypoint == waypoints[0] || selectedWaypoint == waypoints[waypoints.Count - 1]))
        {
            looseEndSelected = true;
        }

        return looseEndSelected;
    }


    //just to not repeat the same code thrice
    private Waypoint NewWaypoint()
    {
        GameObject waypointObject = new GameObject("Waypoint", typeof(Waypoint));
        waypointObject.transform.SetParent(WaypointParent, false);

        //add box trigger collider to waypoint
        BoxCollider box = waypointObject.AddComponent<BoxCollider>();
        box.center = waypointColliderCenter;
        box.size = waypointColliderSize;
        box.isTrigger = true;
        

        return waypointObject.GetComponent<Waypoint>();
    }

    //create a waypoint at the last position of the parent hierarchy
    private void CreateWaypoint(WaypointType type)
    {
        Waypoint newWaypoint = NewWaypoint();

        //if there is a child waypoint already created
        if (WaypointParent.childCount > 1)
        {
            Waypoint previousWaypoint = WaypointParent.GetChild(WaypointParent.childCount - 2).GetComponent<Waypoint>();

            newWaypoint.transform.position = previousWaypoint.transform.position;
            newWaypoint.transform.forward = previousWaypoint.transform.forward;

            switch (type)
            {
                case WaypointType.Forward:
                    previousWaypoint.NextWaypointForward = newWaypoint;
                    break;
                case WaypointType.Left:
                    previousWaypoint.NextWaypointLeft = newWaypoint;
                    break;
                case WaypointType.Right:
                    previousWaypoint.NextWaypointRight = newWaypoint;
                    break;
                default:
                    break;
            }

            newWaypoint.PreviousWaypoint = previousWaypoint;
        }

        RenameAllWaypointsByOrder();

        Selection.activeGameObject = newWaypoint.gameObject;
    }

    //create an waypoint before the one selected and reassign previous and next waypoints
    private void CreateWaypointBefore()
    {
        Waypoint newWaypoint = NewWaypoint();

        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        newWaypoint.transform.position = selectedWaypoint.transform.position;
        newWaypoint.transform.forward = selectedWaypoint.transform.forward;

        //set connections to the new waypoint
        Waypoint previousWaypoint = selectedWaypoint.PreviousWaypoint;
        if (previousWaypoint != null)
        {
            newWaypoint.PreviousWaypoint = previousWaypoint;

            if (selectedWaypoint == previousWaypoint.NextWaypointForward)
            {
                previousWaypoint.NextWaypointForward = newWaypoint;
            }
            else if (selectedWaypoint == previousWaypoint.NextWaypointLeft)
            {
                previousWaypoint.NextWaypointLeft = newWaypoint;
            }
            else if (selectedWaypoint == previousWaypoint.NextWaypointRight)
            {
                previousWaypoint.NextWaypointRight = newWaypoint;
            }
        }
        newWaypoint.NextWaypointForward = selectedWaypoint;
        selectedWaypoint.PreviousWaypoint = newWaypoint;

        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

        RenameAllWaypointsByOrder();

        Selection.activeGameObject = newWaypoint.gameObject;
    }

    //create an waypoint after the one selected and reassign previous and next waypoints
    //always assigns the new waypoint to the selected object's NextWaypointForward variable
    private void CreateWaypointAfter(WaypointType type)
    {
        Waypoint newWaypoint = NewWaypoint();

        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        newWaypoint.transform.position = selectedWaypoint.transform.position;
        

        switch (type)
        {
            case WaypointType.Forward:
                Waypoint nextWaypointForward = selectedWaypoint.NextWaypointForward;

                //newWaypoint.transform.rotation = selectedWaypoint.transform.rotation;
                newWaypoint.transform.forward = selectedWaypoint.transform.forward;

                if (nextWaypointForward != null)
                {
                    newWaypoint.NextWaypointForward = nextWaypointForward;
                    nextWaypointForward.PreviousWaypoint = newWaypoint;
                }

                selectedWaypoint.NextWaypointForward = newWaypoint;
                break;
            case WaypointType.Left:
                Waypoint nextWaypointLeft = selectedWaypoint.NextWaypointLeft;

                //newWaypoint.transform.rotation = Quaternion.Euler(new Vector3(selectedWaypoint.transform.rotation.x, selectedWaypoint.transform.rotation.y - 90, selectedWaypoint.transform.rotation.x));
                newWaypoint.transform.forward = -selectedWaypoint.transform.right;

                if (nextWaypointLeft != null)
                {
                    newWaypoint.NextWaypointLeft = nextWaypointLeft;
                    nextWaypointLeft.PreviousWaypoint = newWaypoint;
                }

                selectedWaypoint.NextWaypointLeft = newWaypoint;
                break;
            case WaypointType.Right:
                Waypoint nextWaypointRight = selectedWaypoint.NextWaypointRight;

                //newWaypoint.transform.rotation = Quaternion.Euler(new Vector3(selectedWaypoint.transform.rotation.x, selectedWaypoint.transform.rotation.y + 90, selectedWaypoint.transform.rotation.x));
                newWaypoint.transform.forward = selectedWaypoint.transform.right;

                if (nextWaypointRight != null)
                {
                    newWaypoint.NextWaypointRight = nextWaypointRight;
                    nextWaypointRight.PreviousWaypoint = newWaypoint;
                }

                selectedWaypoint.NextWaypointRight = newWaypoint;
                break;
            default:
                break;
        }

        newWaypoint.PreviousWaypoint = selectedWaypoint;

        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex() + 1);

        RenameAllWaypointsByOrder();

        Selection.activeGameObject = newWaypoint.gameObject;
    }

    //reassign the previous and next waypoints of the chosen waypoint and delete it 
    private void DeleteWaypoint()
    {
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        Waypoint previousWaypoint = selectedWaypoint.PreviousWaypoint;

        if (previousWaypoint != null)
        {
            //connect previous waypoint nextWaypoint based on if selected waypoint is Forward/Left/Right waypoint of previous waypoint
            if (selectedWaypoint == previousWaypoint.NextWaypointForward)
            {
                if (selectedWaypoint.NextWaypointForward != null)
                    previousWaypoint.NextWaypointForward = selectedWaypoint.NextWaypointForward;
                else
                    previousWaypoint.NextWaypointForward = null;
            }
            else if (selectedWaypoint == previousWaypoint.NextWaypointLeft)
            {
                if (selectedWaypoint.NextWaypointLeft != null)
                    previousWaypoint.NextWaypointLeft = selectedWaypoint.NextWaypointLeft;
                else
                    previousWaypoint.NextWaypointLeft = null;
            }
            else if (selectedWaypoint == previousWaypoint.NextWaypointRight)
            {
                if (selectedWaypoint.NextWaypointRight != null)
                    previousWaypoint.NextWaypointRight = selectedWaypoint.NextWaypointRight;
                else
                    previousWaypoint.NextWaypointRight = null;
            }

            //if selected waypoint has nextWaypoints, connect those nextWaypoints to previous waypoint
            if (selectedWaypoint.NextWaypointForward != null)
            {
                selectedWaypoint.NextWaypointForward.PreviousWaypoint = previousWaypoint;
                previousWaypoint.NextWaypointForward = selectedWaypoint.NextWaypointForward;
            }
            if (selectedWaypoint.NextWaypointLeft != null)
            {
                selectedWaypoint.NextWaypointLeft.PreviousWaypoint = previousWaypoint;
                previousWaypoint.NextWaypointLeft = selectedWaypoint.NextWaypointLeft;
            }
            if (selectedWaypoint.NextWaypointRight != null)
            {
                selectedWaypoint.NextWaypointRight.PreviousWaypoint = previousWaypoint;
                previousWaypoint.NextWaypointRight = selectedWaypoint.NextWaypointRight;
            }

            Selection.activeGameObject = previousWaypoint.gameObject;
        }
        else //prevent editor errors when deleting an waypoint that doesn't have a previous waypoint
        {
            //if waypoint to be deleted has nextWaypoints, remove the previous waypoint reference from those waypoints
            if (selectedWaypoint.NextWaypointForward != null)
                selectedWaypoint.NextWaypointForward.PreviousWaypoint = null;

            if (selectedWaypoint.NextWaypointLeft != null)
                selectedWaypoint.NextWaypointLeft.PreviousWaypoint = null;

            if (selectedWaypoint.NextWaypointRight != null)
                selectedWaypoint.NextWaypointRight.PreviousWaypoint = null;


            //choose next selected waypoint based on if current selected waypoint has nextWaypoints
            if (selectedWaypoint.NextWaypointForward != null)
            {
                Selection.activeGameObject = selectedWaypoint.NextWaypointForward.gameObject;
            }
            else if(selectedWaypoint.NextWaypointLeft != null)
            {
                Selection.activeGameObject = selectedWaypoint.NextWaypointLeft.gameObject;
            }
            else if (selectedWaypoint.NextWaypointRight != null)
            {
                Selection.activeGameObject = selectedWaypoint.NextWaypointRight.gameObject;
            }
            else
            {
                Selection.activeGameObject = null;
            }
        }

        //normal Destroy doesn't work in edit mode
        DestroyImmediate(selectedWaypoint.gameObject);

        RenameAllWaypointsByOrder();
    }

    //go through all the children of the waypoint parent and rename them by order
    private void RenameAllWaypointsByOrder()
    {
        waypoints = new List<Waypoint>(WaypointParent.GetComponentsInChildren<Waypoint>());

        for (int i = 0; i < waypoints.Count; i++)
        {
            waypoints[i].name = $"{WaypointParent.name} Waypoint {i + 1}";
        }
    }


    //Close path if first or last waypoint of a path is selected
    private void ClosePath()
    {
        waypoints = new List<Waypoint>(WaypointParent.GetComponentsInChildren<Waypoint>());

        waypoints[waypoints.Count - 1].NextWaypointForward = waypoints[0];
        waypoints[0].PreviousWaypoint = waypoints[waypoints.Count - 1];
    }
}
