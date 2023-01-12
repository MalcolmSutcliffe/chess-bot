using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCCPPlayer : ChessPlayer {

    public CCCPPlayer(PlayerType playerType) : base(playerType)
    {
        
    }
    public override OptionalMove GetMove(ChessState chessState)
    {
        List<Move> legalMoves = chessState.GetLegalMoves();
        Move currentBestMove = null;
        int moveRating = 0;
        foreach (var legalMove in legalMoves)
        {
            ChessState virtualBoard = ChessState.DeepCopy(chessState);
            virtualBoard.MovePiece(legalMove, true);
            int outcome = virtualBoard.CheckEndGame();
            
            // Checkmate
            if (playerType == PlayerType.White && outcome == 1)
            {
                return new OptionalMove(legalMove);
            }
            if (playerType == PlayerType.Black && outcome == 2)
            {
                return new OptionalMove(legalMove);
            }

            // Check
            if (virtualBoard.IsKingInCheck(Game.otherPlayer[playerType]) && moveRating < 2)
            {
                currentBestMove = legalMove;
                moveRating = 2;
            }

            // Capture
            if (legalMove.capturePiece && moveRating < 1)
            {
                currentBestMove = legalMove;
                moveRating = 1;
            }
        }

        // if found move, return it
        if (currentBestMove != null)
        {
            return new OptionalMove(currentBestMove);
        }

        // else return random
        Move randomMove = legalMoves[Random.Range(0, legalMoves.Count-1)];
        return new OptionalMove(randomMove);
    }
}