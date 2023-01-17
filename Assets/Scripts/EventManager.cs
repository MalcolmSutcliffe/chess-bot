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

    public event Action<Move> MoveOccurred;
    public event Action<int, int, bool> PawnPromotionStarted;
    public event Action PawnPromotionEnded;

    // more events added here

    public void OnMoveOccured(Move move)
    {
        MoveOccurred?.Invoke(move);
    }

    public void OnPawnPromotionStarted(int fromPos, int toPos, bool isCapture)
    {
        PawnPromotionStarted?.Invoke(fromPos, toPos, isCapture);
    }
    public void OnPawnPromotionEnded()
    {
        PawnPromotionEnded?.Invoke();
    }
}