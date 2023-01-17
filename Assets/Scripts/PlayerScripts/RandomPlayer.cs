using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RandomPlayer : AIAgent {

    public RandomPlayer(PlayerType playerType) : base(playerType)
    {
        
    }
    public override Move GetMove(ChessState chessState)
    {
        List<Move> legalMoves = chessState.GetLegalMoves();
        Move randomMove = legalMoves[Random.Range(0, legalMoves.Count-1)];
        return randomMove;
    }
}