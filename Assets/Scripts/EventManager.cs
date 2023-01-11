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

    public event Action<Move, PlayerType> PlayerMoveOccurred;
    public event Action TurnEnded;
    public event Action<int, int, bool> PawnPromotionStarted;
    public event Action PawnPromotionEnded;

    // more events added here

    public void OnPlayerMoveOccured(Move move, PlayerType playerType)
    {
        PlayerMoveOccurred?.Invoke(move, playerType);
    }

    public void OnTurnEnded()
    {
        TurnEnded?.Invoke();
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