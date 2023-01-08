using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayout{
    public Cell[,] state;
    public int size {get; private set;}
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

    public void AddPiece(Vector3Int position, Piece piece)
    {
        this.state[position.x, position.y].SetPiece(piece);
    }

    public void MovePiece(Vector3Int fromPosition, Vector3Int toPosition)
    {
        if (!this.state[fromPosition.x, fromPosition.y].containsPiece)
        {
            return;
        }
        if (this.state[toPosition.x, toPosition.y].containsPiece)
        {
            this.state[toPosition.x, toPosition.y].RemovePiece();
        }
        Piece piece = this.state[fromPosition.x, fromPosition.y].piece;
        this.state[fromPosition.x, fromPosition.y].RemovePiece();
        this.state[toPosition.x, toPosition.y].SetPiece(piece);
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
            newCell.SetPiece(this.piece);
        }
        return newCell;
    }
}