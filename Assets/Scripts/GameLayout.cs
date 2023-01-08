using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayout{
    public Cell[,] state {get; private set;}
    public int size {get; private set;}
    public Vector3Int[] previousMove {get; private set;} 
    public GameLayout(int size) 
    {
        this.size = size;
        this.state = new Cell[size,size];
        this.previousMove = new Vector3Int[2];
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                this.state[x,y] = new Cell();
            }
        }
    }

    public void AddPiece(Vector3Int position, Piece piece)
    {
        this.state[position.x, position.y].SetPiece(piece);
    }

    public void MovePiece(Vector3Int fromPos, Vector3Int toPos)
    {

        // check that position has a piece
        if (!this.state[fromPos.x, fromPos.y].containsPiece)
        {
            return;
        }

        Piece piece = this.state[fromPos.x, fromPos.y].piece;

        // EDGE CASE: castle move (only move where 2 pieces are moved)
        if (this.state[fromPos.x, fromPos.y].piece.pieceType == PieceType.King && Vector3.Distance(fromPos, toPos) >= 2)
        {
            Vector3Int rookFromPos;
            Vector3Int rookToPos;

            // left castle
            if ((fromPos - toPos).x > 0)
            {
                rookFromPos = new Vector3Int(0, fromPos.y, 0);
                rookToPos = new Vector3Int(3, fromPos.y, 0);
            }
            //right castle
            else
            {
                rookFromPos = new Vector3Int(7, fromPos.y, 0);
                rookToPos = new Vector3Int(5, fromPos.y, 0);
            }

            Piece rookPiece = this.state[rookFromPos.x, rookFromPos.y].piece;
            
            rookPiece.Move(rookToPos);
            this.state[rookFromPos.x, rookFromPos.y].RemovePiece();
            this.state[rookToPos.x, rookToPos.y].SetPiece(rookPiece);
        }

        // EDGE CASE: en passant
        if (this.state[fromPos.x, fromPos.y].piece.pieceType == PieceType.Pawn && fromPos.x != toPos.x && !this.state[toPos.x, toPos.y].containsPiece)
        {
            state[toPos.x, fromPos.y].RemovePiece();
        }

        // if position moving to has piece, remove it
        if (this.state[toPos.x, toPos.y].containsPiece)
        {
            this.state[toPos.x, toPos.y].RemovePiece();
        }
        
        // move piece
        piece.Move(toPos);
        this.state[fromPos.x, fromPos.y].RemovePiece();
        this.state[toPos.x, toPos.y].SetPiece(piece);

        this.previousMove = new Vector3Int[]{fromPos, toPos};
    }

    public bool IsInBoard(Vector3Int position){
        return position.x >= 0 && position.x < size && position.y >= 0 && position.y < size;
    }

    public Vector3Int GetKingPosition(PlayerType playerType)
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (state[x,y].containsPiece && state[x,y].piece.playerType == playerType && state[x,y].piece.pieceType == PieceType.King)
                {
                    return new Vector3Int(x, y, 0);
                }
            }
        }
        return new Vector3Int(0, 0, 0);
    }

    public bool IsKingInCheck(PlayerType playerType, Vector3Int kingPosition)
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
            if (piece.GetPossibleMoves(this).Contains(kingPosition))
                return true;
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