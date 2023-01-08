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

    public event Action<Vector3Int, Vector3Int> MoveOccured;

    // more events added here

    public void OnMoveOccured(Vector3Int fromPos, Vector3Int toPos)
    {
        MoveOccured?.Invoke(fromPos, toPos);
    }
}