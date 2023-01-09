using System;
using System.Collections;
using System.Collections.Generic;

public class ChessState{
    public ChessSquare[,] boardState {get; private set;}
    public int size {get; private set;}
    public int[][] previousMove {get; private set;}
    
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
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                this.boardState[x,y] = new ChessSquare();
            }
        }
    }

    public void AddPiece(int[] position, Piece piece)
    {
        this.boardState[position[0], position[1]].SetPiece(piece);
        
        if (piece.playerType == PlayerType.White)
        {
            playerWhite.AddPiece(piece);
        }

        if (piece.playerType == PlayerType.Black)
        {
            playerBlack.AddPiece(piece);
        }
    }

    public void MovePiece(int[] fromPos, int[] toPos, PieceType promotedTo=PieceType.Queen)
    {

        // check that position has a piece
        if (!this.boardState[fromPos[0], fromPos[1]].containsPiece)
        {
            return;
        }

        Piece piece = this.boardState[fromPos[0], fromPos[1]].piece;

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

        // EDGE CASE: pawn promotion

        // if position moving to has piece, remove it
        if (this.boardState[toPos[0], toPos[1]].containsPiece)
        {
             CapturePiece(boardState[toPos[0], toPos[1]].piece);
        }
        
        // move piece
        piece.Move(toPos);
        this.boardState[fromPos[0], fromPos[1]].RemovePiece();
        this.boardState[toPos[0], toPos[1]].SetPiece(piece);

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
                if (possibleMove[0] == kingPosition[0] && possibleMove[1] == kingPosition[1])
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
    //  0 if game is not over
    //  1 if white wins
    //  2 if black wins
    //  3 if draw by stalemate
    public int CheckEndGame()
    {
        // check draw by insufficient material

        // check draw by move limit

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