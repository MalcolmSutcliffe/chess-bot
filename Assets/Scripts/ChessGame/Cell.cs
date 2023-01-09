using System.Collections;
using System.Collections.Generic;


public class ChessSquare
{
    public bool containsPiece;
    public Piece piece;

    public ChessSquare()
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

    public ChessSquare DeepCopy()
    {
        ChessSquare newCell = new ChessSquare();
        if(this.containsPiece)
        {
            newCell.SetPiece(piece.Copy());
        }
        return newCell;
    }
}