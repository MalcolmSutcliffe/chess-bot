using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIAgent : ChessPlayer {
    private bool hasMove;
    public AIAgent(PlayerType playerType) : base(playerType)
    {
        hasMove = true;
    }

    public override void Update(ChessState chessState)
    {
        if (hasMove)
        {
            hasMove = false;
            Move m = GetMove(chessState);
            EventManager.instance.OnMoveOccured(m);
            hasMove = true;
        }
    }

    public abstract Move GetMove(ChessState chessState);
}