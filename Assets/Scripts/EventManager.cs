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

    public event Action MoveOccured;

    // more events added here

    public void OnMoveOccured()
    {
        MoveOccured?.Invoke();
    }
}