using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New Driver", menuName = "Driver", order = 51)]
public class DriverTemplate : ScriptableObject
{
    [SerializeField]
    private string driverName;

    [SerializeField]
    private int numPossibleMistakes = 0;

    [SerializeField]
    private float reactionTime = 0.0f;


    public string DriverName
    {
        get
        {
            return driverName;
        }
    }

    public int NumPossibleMistakes
    {
        get
        {
            return numPossibleMistakes;
        }
    }

    public float ReactionTime
    {
        get
        {
            return reactionTime;
        }
    }

    
    //Reaction times:
    //Liv: 0.5s
    //Nikolai: 0.15s
}
