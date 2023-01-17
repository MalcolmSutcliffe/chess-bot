using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HumanPlayer : ChessPlayer {
    
    public HumanPlayer(PlayerType playerType) : base(playerType)
    {
    }

    public override void Update(ChessState chessState)
    {
        MoveSelector.instance.HandleUserInput(chessState, playerType);
    }
}