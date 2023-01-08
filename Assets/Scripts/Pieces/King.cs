using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    public bool castlingRights;
    public static List<Vector3Int> KING_MOVE_DIRECTIONS = new List<Vector3Int>{new Vector3Int(-1, 0, 0), new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0), new Vector3Int(-1, -1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(1, -1, 0), new Vector3Int(1, 1, 0)};
    public King(Vector3Int position, PlayerType player, PieceType piece, GameObject gameObject) : base(position, player, piece, gameObject)
    {
        castlingRights = true;
    }

    public override List<Vector3Int> GetPossibleMoves(GameLayout gameLayout)
    {
        List<Vector3Int> possibleMoves = new List<Vector3Int>();
        // basic directions
        foreach(var dir in KING_MOVE_DIRECTIONS)
        {
            Vector3Int move = position + dir;
            if (!gameLayout.IsInBoard(move))
            {
                continue;
            }
            if (gameLayout.state[move.x, move.y].containsPiece && gameLayout.state[move.x, move.y].piece.playerType == this.playerType)
            {
                continue;
            }
            possibleMoves.Add(move);
        }
        // TODO: castling
        return possibleMoves;
    }

    public void Move(Vector3Int position){
        base.Move(position);
        castlingRights = false;
    }

}