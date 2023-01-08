using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Vector3Int position {get; protected set;}
    public PlayerType playerType {get; protected set;}
    public PieceType pieceType {get; protected set;}
    public GameObject gameObject;

    public Piece(Vector3Int position, PlayerType playerType, PieceType pieceType, GameObject gameObject){
        this.position = position;
        this.playerType = playerType;
        this.pieceType = pieceType;
        this.gameObject = gameObject;
        this.gameObject.transform.position = position;
    }
    public virtual List<Vector3Int> GetLegalMoves(GameLayout gameLayout){
        
        List<Vector3Int> possibleMoves = GetPossibleMoves(gameLayout);
        List<Vector3Int> legalMoves = new List<Vector3Int>();
        
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

    public abstract List<Vector3Int> GetPossibleMoves(GameLayout gameLayout);

    public virtual void Move(Vector3Int position){
        this.position = position;
        this.gameObject.transform.position = position;
    }
    
}
