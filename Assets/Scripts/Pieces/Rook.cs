using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Rook : SlidingPiece
{
    public static List<Vector3Int> ROOK_MOVE_DIRECTIONS = new List<Vector3Int>{new Vector3Int(-1, 0, 0), new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0)};
    public bool castlingRights;
    public Rook(Vector3Int position, PlayerType player, PieceType piece, GameObject gameObject) : base(position, player, piece, ROOK_MOVE_DIRECTIONS, gameObject)
    {
        castlingRights = true;
    }
    public override void Move(Vector3Int position){
        castlingRights = false;
        base.Move(position);
    }
}