using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
// using UnityEngine;
public class ChessState{
    public ChessSquare[,] boardState {get; private set;}
    public int size {get; private set;}
    public int[][] previousMove {get; private set;}
    public int halfClock; // time since last capture or pawn move used for draws
    public Dictionary<string, int> stateHistory;
    
    public Player playerWhite;
    public Player playerBlack;

    public Player activePlayer;
    
    public ChessState(int size) 
    {
        this.size = size;
        this.boardState = new ChessSquare[size,size];

        this.playerWhite = new Player(PlayerType.White);
        this.playerBlack = new Player(PlayerType.Black);

        this.activePlayer = this.playerWhite;

        this.halfClock = 0;
        
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

    public string EncodeMoveSAN(Move move)
    {
        return "";
    }

    // take a move in chess standard algebraic notation and convert to a move object
    // example string : "Ra4a5", "Ra4", "Rha2", "R3d3" b3b4, "c5", "0-0-0" , "h8=Q"
    // if no start position given, must be determined so the board state is required
    public Move DecodeMoveSAN(string moveSAN)
    {
        // Edge case king side castle
        if (moveSAN.Equals("0-0") || moveSAN.Equals("O-O"))
        {
            if (activePlayer.playerType == PlayerType.White)
                return new Move(PieceType.King, new int[] {4, 0}, new int[] {6, 0}, false, false, PieceType.King);
            if (activePlayer.playerType == PlayerType.Black)
                return new Move(PieceType.King, new int[] {4, 7}, new int[] {6, 7}, false, false, PieceType.King);
        }
        // Edge case queen side castle
        if (moveSAN.Equals("0-0-0") || moveSAN.Equals("O-O-O"))
        {
            if (activePlayer.playerType == PlayerType.White)
                return new Move(PieceType.King, new int[] {4, 0}, new int[] {2, 0}, false, false, PieceType.King);
            if (activePlayer.playerType == PlayerType.Black)
                return new Move(PieceType.King, new int[] {4, 7}, new int[] {2, 7}, false, false, PieceType.King);
        }
        
        PieceType pieceType;
        int[] fromPos;
        int[] toPos;
        bool capturePiece;
        bool promotePiece;
        PieceType promotedTo;

        moveSAN.Replace("-", "");

        // encode piece 
        if (moveSAN[0] >= 65 && moveSAN[0] <= 90)
        {
            pieceType = Move.pieceChars[moveSAN[0]];
            // remove first char
            moveSAN.Remove(0, 1);
        }
        else
        {
            pieceType = PieceType.Pawn;
        }

        // check capture
        capturePiece  = moveSAN.Contains("x");
        moveSAN.Replace("x", "");

        // check promotion
        promotePiece = moveSAN.Contains("=");
        if (promotePiece)
        {
            promotedTo = Move.pieceChars[moveSAN.Last()];
            moveSAN.Remove(moveSAN.Length-1, 2);
        }
        else
        {
            promotedTo = pieceType;
        }

        // decode positions

        // no original position information given, check all pieces and determine the move
        if (moveSAN.Length==2)
        {
            toPos = Move.chessNotationToPosition(moveSAN);
            fromPos = new int[] {-1, -1};
            foreach (var piece in activePlayer.pieces)
            {
                if (piece.pieceType != pieceType)
                {
                    continue;
                }
                
                foreach (var move in piece.GetLegalMoves(this))
                {
                    if (move.toPos[0] == toPos[0] && move.toPos[1] == toPos[1])
                    {
                        fromPos = piece.position;
                        break;
                    }
                }
            }
            if (fromPos[0] == -1)
            {
                // throw error
                throw new ArgumentException("could not find piece on board with requested legal move");
            }
        }

        // only one indicator given
        else if (moveSAN.Length==3)
        {
            toPos = Move.chessNotationToPosition(moveSAN.Substring(1,2));
            fromPos = new int[] {-1, -1};
            // given indicator is file
            if (moveSAN[0] >= 'a' && moveSAN[0] <= 'h')
            {
                int file = moveSAN[0] - 'a';
                
                foreach (var piece in activePlayer.pieces)
                {
                    if (piece.pieceType != pieceType)
                    {
                        continue;
                    }

                    if (piece.position[0] != file)
                    {
                        continue;
                    }
                    foreach (var move in piece.GetLegalMoves(this))
                    {
                        if (move.toPos[0] == toPos[0] && move.toPos[1] == toPos[1])
                        {
                            fromPos = piece.position;
                            break;
                        }
                    }
                }
                if (fromPos[0] == -1)
            {
                // throw error
                throw new ArgumentException("could not find piece on board with requested legal move");
            }
            
            }
            
            // given indicator is rank
            else if (moveSAN[0] >= '1' && moveSAN[0] <= '8')
            {
                int rank = moveSAN[0] - '1';
                
                foreach (var piece in activePlayer.pieces)
                {
                    if (piece.pieceType != pieceType)
                    {
                        continue;
                    }

                    if (piece.position[1] != rank)
                    {
                        continue;
                    }
                    foreach (var move in piece.GetLegalMoves(this))
                    {
                        if (move.toPos[0] == toPos[0] && move.toPos[1] == toPos[1])
                        {
                            fromPos = piece.position;
                            break;
                        }
                    }
                }
                if (fromPos[0] == -1)
            {
                // throw error
                throw new ArgumentException("could not find piece on board with requested legal move");
            }
            }
            
        }
        // full info given
        else if (moveSAN.Length==4)
        {
            fromPos = Move.chessNotationToPosition(moveSAN.Substring(0,2));
            toPos = Move.chessNotationToPosition(moveSAN.Substring(2,2));
        }
        else
        {
            // throw error
            throw new ArgumentException("illegal move notation!");
        }
        
        // check illegal promotion notation
        if (pieceType == PieceType.Pawn && (toPos[1] == 7 || toPos[1] == 0))
        {
            if (!promotePiece)
            {
                throw new ArgumentException("illegal move notation : cannot move pawn to end of board without specifying promotion piece");
            }
            
        }
        return new Move(pieceType, fromPos, toPos, capturePiece, promotePiece, promotedTo);
    }

    public static string EncodeBoardState()
    {
        return "";
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