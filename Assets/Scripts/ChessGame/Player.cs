using System.Collections;
using System.Collections.Generic;

public enum PlayerType{
    White,
    Black
}

public class Player{
    public PlayerType playerType {get; private set;}
    public List<Piece> pieces {get; private set;}
    public Piece king {get; private set;}

    public Player(PlayerType playerType)
    {
        this.playerType = playerType;
        this.pieces = new List<Piece>();
    }

    public void AddPiece(Piece piece)
    {
        if (piece.playerType != playerType)
        {
            return;
        }
        
        if (piece.pieceType == PieceType.King)
        {
            if (this.king == null)
            {
                this.king = piece;
            }
            return;
        }
        this.pieces.Add(piece);
    }

    public void RemovePiece(Piece piece)
    {
        if (piece.playerType != playerType)
        {
            return;
        }
        
        if (piece.pieceType == PieceType.King)
        {
            // should not happen
            return;
        }
        this.pieces.Remove(piece);
    }
}