using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HumanPlayer : ChessPlayer {

    private OptionalMove moveChosen;
    
    public HumanPlayer(PlayerType playerType) : base(playerType)
    {
        this.moveChosen = new OptionalMove();
        EventManager.instance.PlayerMoveOccurred += MoveChosen;
        EventManager.instance.TurnEnded += TurnEnded;
    }

    public void MoveChosen(Move move, PlayerType playerType)
    {
        if (playerType == this.playerType)
        {
            this.moveChosen = new OptionalMove(move);
        }
    }

    public void TurnEnded()
    {
        this.moveChosen = new OptionalMove();
    }

    public override OptionalMove GetMove(ChessState chessState)
    {
        MoveSelector.instance.CheckUserInput(chessState);
        return moveChosen;
    }
}