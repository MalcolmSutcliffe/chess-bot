using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : SlidingPiece
{
    public static List<Vector3Int> BISHOP_MOVE_DIRECTIONS = new List<Vector3Int>{new Vector3Int(-1, -1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(1, -1, 0), new Vector3Int(1, 1, 0)};
    public Bishop(Vector3Int position, PlayerType player, PieceType piece, GameObject gameObject) : base(position, player, piece, BISHOP_MOVE_DIRECTIONS, gameObject)
    {
        
    }

}