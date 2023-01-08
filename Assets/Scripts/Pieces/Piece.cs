using System.Collections;
using System.Collections.Generic;

public enum PieceType {
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King
}

public abstract class Piece
{
    public int[] position {get; protected set;}
    public PlayerType playerType {get; protected set;}
    public PieceType pieceType {get; protected set;}

    public Piece(int[] position, PlayerType playerType, PieceType pieceType){
        this.position = position;
        this.playerType = playerType;
        this.pieceType = pieceType;
    }
    public virtual List<int[]> GetLegalMoves(GameLayout gameLayout){
        
        List<int[]> possibleMoves = GetPossibleMoves(gameLayout);
        List<int[]> legalMoves = new List<int[]>();
        
        foreach (var move in possibleMoves)
        {
            GameLayout virtualBoard = gameLayout.DeepCopy();
            virtualBoard.MovePiece(position, move);
            if (virtualBoard.IsKingInCheck(this.playerType, virtualBoard.GetKingPosition(this.playerType)))
            {
                continue;
            }
            legalMoves.Add(move);
        }
        // filter moves if king is in check
        return legalMoves;
    }

    public abstract List<int[]> GetPossibleMoves(GameLayout gameLayout);

    public virtual void Move(int[] position){
        this.position = position;
    }

    public abstract Piece Copy();
}
