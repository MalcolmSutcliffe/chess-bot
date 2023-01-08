using System;
using System.Collections;
using System.Collections.Generic;

public class GameLayout{
    public Cell[,] state {get; private set;}
    public int size {get; private set;}
    public int[][] previousMove {get; private set;} 
    public GameLayout(int size) 
    {
        this.size = size;
        this.state = new Cell[size,size];
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                this.state[x,y] = new Cell();
            }
        }
    }

    public void AddPiece(int[] position, Piece piece)
    {
        this.state[position[0], position[1]].SetPiece(piece);
    }

    public void MovePiece(int[] fromPos, int[] toPos)
    {

        // check that position has a piece
        if (!this.state[fromPos[0], fromPos[1]].containsPiece)
        {
            return;
        }

        Piece piece = this.state[fromPos[0], fromPos[1]].piece;

        // EDGE CASE: castle move (only move where 2 pieces are moved)
        if (this.state[fromPos[0], fromPos[1]].piece.pieceType == PieceType.King && Math.Abs(fromPos[0] - toPos[0]) >= 2)
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

            Piece rookPiece = this.state[rookFromPos[0], rookFromPos[1]].piece;
            
            rookPiece.Move(rookToPos);
            this.state[rookFromPos[0], rookFromPos[1]].RemovePiece();
            this.state[rookToPos[0], rookToPos[1]].SetPiece(rookPiece);
        }

        // EDGE CASE: en passant
        if (this.state[fromPos[0], fromPos[1]].piece.pieceType == PieceType.Pawn && fromPos[0] != toPos[0] && !this.state[toPos[0], toPos[1]].containsPiece)
        {
            state[toPos[0], fromPos[1]].RemovePiece();
        }

        // if position moving to has piece, remove it
        if (this.state[toPos[0], toPos[1]].containsPiece)
        {
            this.state[toPos[0], toPos[1]].RemovePiece();
        }
        
        // move piece
        piece.Move(toPos);
        this.state[fromPos[0], fromPos[1]].RemovePiece();
        this.state[toPos[0], toPos[1]].SetPiece(piece);

        this.previousMove = new int[][]{fromPos, toPos};
    }

    public bool IsInBoard(int[] position){
        return position[0] >= 0 && position[0] < size && position[1] >= 0 && position[1] < size;
    }

    public int[] GetKingPosition(PlayerType playerType)
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (state[x,y].containsPiece && state[x,y].piece.playerType == playerType && state[x,y].piece.pieceType == PieceType.King)
                {
                    return new int[] {x, y};
                }
            }
        }
        return new int[] {0, 0};
    }

    public bool IsKingInCheck(PlayerType playerType, int[] kingPosition)
    {
        List<Piece> piecesToCheck = new List<Piece>();
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (state[x,y].containsPiece && state[x,y].piece.playerType != playerType)
                {
                    piecesToCheck.Add(state[x,y].piece);
                }
            }
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

    public GameLayout DeepCopy()
    {
        GameLayout newGameLayout = new GameLayout(size);
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                newGameLayout.state[x,y] = this.state[x,y].DeepCopy();
            }
        }
        return newGameLayout;
    }
}

public class Cell
{
    public bool containsPiece;
    public Piece piece;

    public Cell()
    {
        this.containsPiece = false;
    }

    public void RemovePiece()
    {
        this.containsPiece = false;
        this.piece = null;
    }

    public void SetPiece(Piece piece)
    {
        this.containsPiece = true;
        this.piece = piece;
    }

    public Cell DeepCopy()
    {
        Cell newCell = new Cell();
        if(this.containsPiece)
        {
            newCell.SetPiece(piece.Copy());
        }
        return newCell;
    }
}