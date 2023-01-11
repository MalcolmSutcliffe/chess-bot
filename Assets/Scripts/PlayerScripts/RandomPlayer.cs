using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RandomPlayer : ChessPlayer {

    public RandomPlayer(PlayerType playerType) : base(playerType)
    {
        
    }
    public override OptionalMove GetMove(ChessState chessState)
    {
        List<Move> legalMoves = chessState.GetLegalMoves();
        Move randomMove = legalMoves[Random.Range(0, legalMoves.Count-1)];
        return new OptionalMove(randomMove);
    }
}