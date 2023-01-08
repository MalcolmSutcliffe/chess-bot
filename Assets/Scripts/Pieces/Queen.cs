using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : SlidingPiece
{
    public static List<Vector3Int> QUEEN_MOVE_DIRECTIONS = new List<Vector3Int>{new Vector3Int(-1, 0, 0), new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0), new Vector3Int(-1, -1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(1, -1, 0), new Vector3Int(1, 1, 0)};
    public Queen(Vector3Int position, PlayerType player, PieceType piece, GameObject gameObject) : base(position, player, piece, QUEEN_MOVE_DIRECTIONS, gameObject)
    {
        
    }

}