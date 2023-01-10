using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HumanPlayer : ChessPlayer {

    private OptionalMove moveChosen;
    private MoveSelector moveSelector;
    
    public HumanPlayer(PlayerType playerType, MoveSelector moveSelector) : base(playerType)
    {
        this.moveSelector = moveSelector;
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
        moveSelector.CheckUserInput(chessState);
        return moveChosen;
    }
}