using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

public abstract class MinMaxPlayer : ChessPlayer {

    private int maxDepth;
    private Dictionary<string, (int, float)> searchCache;

    public MinMaxPlayer(PlayerType playerType, int maxDepth) : base(playerType)
    {
        this.maxDepth = maxDepth;
        this.searchCache = new Dictionary<string, (int, float)>();
    }
    
    public override OptionalMove GetMove(ChessState chessState)
    {
        // time move
        Stopwatch stopwatch = Stopwatch.StartNew(); 

        bool maximize = this.playerType == PlayerType.White;

        List<Move> legalMoves = chessState.GetLegalMoves();

        // shuffle moves
        legalMoves.Shuffle();
        
        Move bestMove = null;

        float alpha = -999;
        float beta = 999;
        
        foreach (var move in legalMoves)
        {
            // apply move to virtual board
            ChessState virtualBoard = ChessState.DeepCopy(chessState);
            virtualBoard.MovePiece(move);
            float moveScore = this.MinMax(virtualBoard, !maximize, 0, alpha, beta);
            if (maximize && (moveScore > alpha))
            {
                bestMove = move;
                alpha = moveScore;
            }
            else if (!maximize && (moveScore < beta))
            {
                bestMove = move;
                beta = moveScore;
            }
        }

        stopwatch.Stop();
        UnityEngine.Debug.Log("Depth: " + maxDepth + " time: " + stopwatch.ElapsedMilliseconds);
        return new OptionalMove(bestMove);
    }

    // min max algo with alpha-beta pruning, search caching
    private float MinMax(ChessState chessState, bool maximize=true, int currentDepth=0, float alpha=-999, float beta=999)
    {
        // check if state has been cached at same or higher depth
        string encodedBoard = BoardEncoder.EncodeChessStateToFEN(chessState);
        
        if (searchCache.ContainsKey(encodedBoard) && searchCache[encodedBoard].Item1 >= maxDepth - currentDepth)
        {
            return searchCache[encodedBoard].Item2;
        }
        
        float toReturn = 0;
        // Check Endgame
        int outcome = chessState.CheckEndGame();
        
        if (outcome == 1)
        {
            searchCache[encodedBoard] = (maxDepth-currentDepth, 100);
            return 100;
        }
        else if (outcome == 2)
        {
            searchCache[encodedBoard] = (maxDepth-currentDepth, -100);
            return -100;
        }
        else if (outcome > 0)
        {
            searchCache[encodedBoard] = (maxDepth-currentDepth, 0);
            return 0;
        }

        // if max depth reached
        if (currentDepth >= maxDepth)
        {
            return BoardHeuristic(chessState);
        }

        // get legal moves
        List<Move> legalMoves = chessState.GetLegalMoves();

        // sort by heuristic by best
        legalMoves.Sort((move1, move2) => BoardHeuristicFromMove(chessState, move2).CompareTo(BoardHeuristicFromMove(chessState, move1)));

        foreach (var move in legalMoves)
        {
            // apply move to virtual board
            ChessState virtualBoard = ChessState.DeepCopy(chessState);
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
                    toReturn = beta;
                    break;
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
                    toReturn = alpha;
                    break;
                }
            }
        }

        if (maximize)
            return alpha;
            
        if (!maximize)
            toReturn = beta;

        searchCache[encodedBoard] = (maxDepth-currentDepth, toReturn);
        return toReturn;
    }

    private float BoardHeuristicFromMove(ChessState chessState, Move move)
    {
        ChessState virtualBoard = ChessState.DeepCopy(chessState);
        virtualBoard.MovePiece(move);
        return BoardHeuristic(virtualBoard);
    }
    
    public abstract float BoardHeuristic(ChessState chessState);
}