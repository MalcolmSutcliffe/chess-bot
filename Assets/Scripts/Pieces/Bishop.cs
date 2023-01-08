using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : SlidingPiece
{
    public static List<Vector3Int> BISHOP_MOVE_DIRECTIONS = new List<Vector3Int>{new Vector3Int(-1, -1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(1, -1, 0), new Vector3Int(1, 1, 0)};
    public Bishop(Vector3Int position, PlayerType playerType, PieceType pieceType) : base(position, playerType, pieceType, BISHOP_MOVE_DIRECTIONS)
    {
        
    }

    public override Piece Copy(){
        return new Bishop(position, playerType, pieceType);
    }
}