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

    public event Action<int[], int[], bool, PieceType> MoveOccured;

    // more events added here

    public void OnMoveOccured(int[] fromPos, int[] toPos, bool doPromotion, PieceType promoteTo)
    {
        MoveOccured?.Invoke(fromPos, toPos, doPromotion, promoteTo);
    }
}