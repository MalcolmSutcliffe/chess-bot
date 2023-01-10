using System;
using System.Collections;
using System.Collections.Generic;
// using UnityEngine;
public class ChessState{
    public ChessSquare[,] boardState {get; private set;}
    public int size {get; private set;}
    public int[][] previousMove {get; private set;}
    public int halfClock {get; private set;} // time since last capture or pawn move used for draws
    public int fullMoveCount {get; private set;}
    public Dictionary<string, int> stateHistory {get; private set;}
    
    public Player playerWhite {get; private set;}
    public Player playerBlack {get; private set;}

    public Player activePlayer {get; private set;}
    
    public ChessState(int size) 
    {
        this.size = size;
        this.boardState = new ChessSquare[size,size];

        this.playerWhite = new Player(PlayerType.White);
        this.playerBlack = new Player(PlayerType.Black);

        this.activePlayer = this.playerWhite;

        this.halfClock = 0;
        this.fullMoveCount = 0;

        this.stateHistory = new Dictionary<string, int>();
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                this.boardState[x,y] = new ChessSquare();
            }
        }
    }
    
    public void MovePiece(Move move)
    {
        int[] fromPos = move.fromPos;
        int[] toPos = move.toPos;
        bool promotePiece = move.promotePiece;
        PieceType promotedTo = move.promotedTo;

        // check that position has a piece
        if (!this.boardState[fromPos[0], fromPos[1]].containsPiece)
        {
            return;
        }

        Piece piece = this.boardState[fromPos[0], fromPos[1]].piece;

        halfClock++;

        if (move.capturePiece || piece.pieceType == PieceType.Pawn)
        {
            halfClock = 0;
        }

        // EDGE CASE: castle move (only move where 2 pieces are moved)
        if (this.boardState[fromPos[0], fromPos[1]].piece.pieceType == PieceType.King && Math.Abs(fromPos[0] - toPos[0]) >= 2)
        {
            int[] rookFromPos;
            int[] rookToPos;

            // left castle
            if ((fromPos[0] - toPos[0]) > 0)
            {
                rookFromPos = new int[] {0, fromPos[1]};
                rookToPos = new int[]{ 3, fromPos[1]};
            }
            //right castle
            else
            {
                rookFromPos = new int[] {7, fromPos[1]} ;
                rookToPos = new int[] {5, fromPos[1]};
            }

            Piece rookPiece = this.boardState[rookFromPos[0], rookFromPos[1]].piece;
            
            rookPiece.Move(rookToPos);
            this.boardState[rookFromPos[0], rookFromPos[1]].RemovePiece();
            this.boardState[rookToPos[0], rookToPos[1]].SetPiece(rookPiece);
        }

        // EDGE CASE: en passant
        if (this.boardState[fromPos[0], fromPos[1]].piece.pieceType == PieceType.Pawn && fromPos[0] != toPos[0] && !this.boardState[toPos[0], toPos[1]].containsPiece)
        {
            CapturePiece(boardState[toPos[0], fromPos[1]].piece);
        }

        // if position moving to has piece, remove it
        if (this.boardState[toPos[0], toPos[1]].containsPiece)
        {
             CapturePiece(boardState[toPos[0], toPos[1]].piece);
        }

        // EDGE CASE: pawn promotion
        if (promotePiece)
        {
            CapturePiece(boardState[fromPos[0], fromPos[1]].piece);
            AddPiece(toPos, activePlayer.playerType, promotedTo);
        }
        else
        {
            // move piece
            piece.Move(toPos);
            this.boardState[fromPos[0], fromPos[1]].RemovePiece();
            this.boardState[toPos[0], toPos[1]].SetPiece(piece);
        }
        
        this.previousMove = new int[][]{fromPos, toPos};

        EndTurn();
    }

    public void CapturePiece(Piece piece)
    {
        this.boardState[piece.position[0], piece.position[1]].RemovePiece();
        if (piece.playerType == PlayerType.White)
        {
            playerWhite.RemovePiece(piece);
        }
        if (piece.playerType == PlayerType.Black)
        {
            playerBlack.RemovePiece(piece);
        }
    }

    public int[] GetKingPosition(PlayerType playerType)
    {
        if (playerType == PlayerType.White)
        {
            return playerWhite.king.position;
        }
        if (playerType == PlayerType.Black)
        {
            return playerBlack.king.position;
        }
        return new int[] {0, 0};
    }

    public bool IsKingInCheck(PlayerType playerType, int[] kingPosition = null)
    {
        List<Piece> piecesToCheck = new List<Piece>();
        
        if (playerType == PlayerType.White)
        {
            piecesToCheck = playerBlack.pieces;
        }
        if (playerType == PlayerType.Black)
        {
            piecesToCheck = playerWhite.pieces;
        }

        if (kingPosition == null)
        {
            kingPosition = GetKingPosition(playerType);
        }
        
        foreach (var piece in piecesToCheck)
        {
            foreach (var possibleMove in piece.GetPossibleMoves(this))
            {
                if (possibleMove.toPos[0] == kingPosition[0] && possibleMove.toPos[1] == kingPosition[1])
                    {
                        return true;
                    }
                }
        }
        return false;
    }

    public void EndTurn()
    {
        if (activePlayer.playerType == PlayerType.White)
        {
            this.activePlayer = this.playerBlack;
        }
        else if (activePlayer.playerType == PlayerType.Black)
        {
            this.activePlayer = this.playerWhite;
            this.fullMoveCount++;
        }
    }


    // returns:
    // 0 if game is not over
    // 1 if white wins
    // 2 if black wins
    // 3 if draw by stalemate
    // 4 if draw by insufficient material
    // 5 if draw by move limit
    // 6 if draw by 3-fold repitition
    public int CheckEndGame()
    {
        // check draw by insufficient material
        if (!CheckSufficientMaterial())
        {
            return 4;
        }

        // check draw by move limit
        if (halfClock >= 100)
        {
            return 5;
        }

        // check draw by 3-fold repitition
        string currentState = ChessState.EncodeBoardState(this.boardState);
        AddBoardStateToHistory(currentState);
        if (stateHistory[currentState] >= 3)
        {
            return 6;
        }
           
        // check for legal moves
        foreach(var piece in activePlayer.pieces)
        {
            if (piece.GetLegalMoves(this).Count > 0)
            {
                return 0;
            }
        }
        
        if (IsKingInCheck(activePlayer.playerType))
        {
            if (activePlayer.playerType == PlayerType.Black)
                return 1;
            if (activePlayer.playerType == PlayerType.White)
                return 2;
        }
        return 3;
    }

    public bool CheckSufficientMaterial()
    {
        if (playerWhite.pieces.Count >= 3 || playerBlack.pieces.Count >= 3)
        {
            return true;
        }
        foreach(var piece in playerWhite.pieces)
        {
            if (piece.pieceType == PieceType.Pawn || piece.pieceType == PieceType.Rook || piece.pieceType == PieceType.Queen)
            {
                return true;
            }
        }
        foreach(var piece in playerBlack.pieces)
        {
            if (piece.pieceType == PieceType.Pawn || piece.pieceType == PieceType.Rook || piece.pieceType == PieceType.Queen)
            {
                return true;
            }
        }
        return false;
    }

    private void AddBoardStateToHistory(string state)
    {
        if (stateHistory.ContainsKey(state))
        {
            stateHistory[state] = stateHistory[state] + 1;
        } else
        {
            stateHistory[state] = 1;
        }

    }

    public static string EncodeChessStateFEN(ChessState chessState)
    {
        string toReturn = "";

        // encode board state
        toReturn += EncodeBoardState(chessState.boardState);

        toReturn += " ";

        // encode player turn
        if (chessState.activePlayer.playerType == PlayerType.White)
        {
            toReturn += "w";
        }
        else if (chessState.activePlayer.playerType == PlayerType.Black)
        {
            toReturn += "b";
        }

        toReturn += " ";
        
        // encode castling rights
        bool hasEncodedCastlingRights = false;
        if (chessState.playerWhite.king.KingSideCastlingRights(chessState))
        {
            toReturn += "K";
            hasEncodedCastlingRights = true;
        }
        if (chessState.playerWhite.king.QueenSideCastlingRights(chessState))
        {
            toReturn += "Q";
            hasEncodedCastlingRights = true;
        }
        if (chessState.playerBlack.king.KingSideCastlingRights(chessState))
        {
            toReturn += "k";
            hasEncodedCastlingRights = true;
        }
        if (chessState.playerBlack.king.QueenSideCastlingRights(chessState))
        {
            toReturn += "q";
            hasEncodedCastlingRights = true;
        }
        if (!hasEncodedCastlingRights)
        {
            toReturn += "-";
        }

        toReturn += " ";

        // encode enpassant
        toReturn += GetEnPasantSquare(chessState);

        toReturn += " ";

        toReturn += chessState.halfClock.ToString();
        
        toReturn += " ";

        toReturn += chessState.fullMoveCount.ToString();

        return toReturn;
    }

    public static string EncodeBoardState(ChessSquare[,] boardState)
    {
        string toReturn="";
        for (int y = 7; y >= 0; y--)
        {
            toReturn += EncodeRank(boardState, y);
            if (y > 0)
            {
                toReturn += "/";
            }
        }
        return toReturn;
    }

    private static string EncodeRank(ChessSquare[,] boardState, int rank)
    {
        string toReturn = "";
        int runningEmptySpaces=0;
        for (int x = 0; x < 8; x++)
        {
            if (!boardState[x, rank].containsPiece)
            {
                runningEmptySpaces++;
                continue;
            }
            if (runningEmptySpaces > 0)
            {
                toReturn += (char) ('0' + runningEmptySpaces);
            }
            char pieceSymbol = Piece.pieceToChar[boardState[x, rank].piece.pieceType];
            
            if (boardState[x, rank].piece.playerType == PlayerType.Black)
            {
                pieceSymbol = Char.ToLower(pieceSymbol);
            }
            
            toReturn += pieceSymbol;
            runningEmptySpaces=0;
        }
        if (runningEmptySpaces > 0)
        {
            toReturn += (char) ('0' + runningEmptySpaces);
        }
        return toReturn;
    }

    private static string GetEnPasantSquare(ChessState chessState)
    {
        int[] previousMoveFrom = chessState.previousMove[0];
        int[] previousMoveTo = chessState.previousMove[1];
        if (!chessState.boardState[previousMoveTo[0], previousMoveTo[1]].containsPiece)
        {
            return "-";
        }
        if (chessState.boardState[previousMoveTo[0], previousMoveTo[1]].piece.pieceType != PieceType.Pawn)
        {
            return "-";
        }
        if (Math.Abs(previousMoveFrom[1] - previousMoveTo[1]) < 2)
        {
            return "-";
        }
        return Move.positionToChessNotation(new int[] {previousMoveTo[0], (int) ((previousMoveFrom[1] + previousMoveTo[1])/2)});
    }


    public void AddPiece(int[] position, PlayerType playerType, PieceType pieceType)
    {
        Piece newPiece;
        switch (pieceType)
        {
            case PieceType.Pawn:
                newPiece = new Pawn(position, playerType, pieceType);
                break;
            case PieceType.Rook:
                newPiece = new Rook(position, playerType, pieceType);
                break;
            case PieceType.Knight:
                newPiece = new Knight(position, playerType, pieceType);
                break;
            case PieceType.Bishop:
                newPiece = new Bishop(position, playerType, pieceType);
                break;
            case PieceType.Queen:
                newPiece = new Queen(position, playerType, pieceType);
                break;
            case PieceType.King:
                newPiece = new King(position, playerType, pieceType);
                break;
            default:
                newPiece = new Pawn(position, playerType, pieceType);
                break;
        }
        this.boardState[position[0], position[1]].SetPiece(newPiece);
        
        if (playerType == PlayerType.White)
        {
            playerWhite.AddPiece(newPiece);
        }

        if (playerType == PlayerType.Black)
        {
            playerBlack.AddPiece(newPiece);
        }
    }

    public bool IsInBoard(int[] position){
        return position[0] >= 0 && position[0] < size && position[1] >= 0 && position[1] < size;
    }

    public ChessState DeepCopy()
    {
        ChessState newChessState = new ChessState(size);
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                newChessState.boardState[x,y] = this.boardState[x,y].DeepCopy();
            }
        }
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (!newChessState.boardState[x,y].containsPiece)
                {
                    continue;
                }
                Piece piece = newChessState.boardState[x,y].piece;
                if (piece.playerType == PlayerType.White)
                {
                    newChessState.playerWhite.AddPiece(piece);
                }
                if (piece.playerType == PlayerType.Black)
                {
                    newChessState.playerBlack.AddPiece(piece);
                }
            }
        }
        return newChessState;
    }
}