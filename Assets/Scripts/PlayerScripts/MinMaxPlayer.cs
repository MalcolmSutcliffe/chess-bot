using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class MinMaxPlayer : ChessPlayer {

    private int maxDepth;

    public MinMaxPlayer(PlayerType playerType, int maxDepth) : base(playerType)
    {
        this.maxDepth = maxDepth;
    }
    
    public override OptionalMove GetMove(ChessState chessState)
    {
        bool maximize = this.playerType == PlayerType.White;

        List<Move> legalMoves = chessState.GetLegalMoves();

        // shuffle moves
        legalMoves.Shuffle();
        
        float bestScore;
        Move bestMove = null;

        float alpha = -999;
        float beta = 999;
        
        if (maximize)
        {
            bestScore = -999;
        }
        
        else
        {
            bestScore = 999;
        }
        
        foreach (var move in legalMoves)
        {
            // apply move to virtual board
            ChessState virtualBoard = chessState.DeepCopy();
            virtualBoard.MovePiece(move);
            float moveScore = this.MinMax(virtualBoard, !maximize, 0, alpha, beta);
            if (maximize && moveScore > bestScore)
            {
                bestMove = move;
                alpha = moveScore;
            }
            else if (!maximize && moveScore < bestScore)
            {
                bestMove = move;
                beta = moveScore;
            }
        }
        return new OptionalMove(bestMove);
    }

    // min max algo with alpha-beta pruning
    private float MinMax(ChessState chessState, bool maximize=true, int currentDepth=0, float alpha=-999, float beta=999)
    {   
        // Check Endgame
        int outcome = chessState.CheckEndGame();
        
        if (outcome == 1)
        {
            return 100;
        }
        else if (outcome == 2)
        {
            return -100;
        }
        else if (outcome > 0)
        {
            return 0;
        }

        // if max depth reached
        if (currentDepth >= maxDepth)
        {
            return BoardHeuristic(chessState);
        }

        // recurse through legal moves
        List<Move> legalMoves = chessState.GetLegalMoves();

        foreach (var move in legalMoves)
        {
            // apply move to virtual board
            ChessState virtualBoard = chessState.DeepCopy();
            virtualBoard.MovePiece(move);
            float moveScore = this.MinMax(virtualBoard, !maximize, currentDepth+1, alpha, beta);
            if (maximize)
            {
                if (moveScore > alpha)
                {
                    alpha = moveScore;
                }
                if (alpha >= beta)
                {
                    return beta;
                }
            }
            else
            {
                if (moveScore < beta)
                {
                    beta = moveScore;
                }
                if (alpha >= beta)
                {
                    return alpha;
                }
            }
        }

        if (maximize)
            return alpha;
            
        return beta;
    
    }
    
    public abstract float BoardHeuristic(ChessState chessState);
}