using System;
using UnityEngine;

public class EventManager: MonoBehaviour
{
    public static EventManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this);
        }
    }

    public event Action<int[], int[]> MoveOccured;

    // more events added here

    public void OnMoveOccured(int[] fromPos, int[] toPos)
    {
        MoveOccured?.Invoke(fromPos, toPos);
    }
}