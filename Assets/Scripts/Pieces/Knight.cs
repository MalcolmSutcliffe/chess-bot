using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public static List<Vector3Int> KNIGHT_MOVE_DIRECTIONS = new List<Vector3Int>{new Vector3Int(1, 2, 0), new Vector3Int(1, -2, 0), new Vector3Int(-1, 2, 0), new Vector3Int(-1, -2, 0), new Vector3Int(2, 1, 0), new Vector3Int(2, -1, 0), new Vector3Int(-2, 1, 0), new Vector3Int(-2, -1, 0)};
    public Knight(Vector3Int position, PlayerType playerType, PieceType pieceType) : base(position, playerType, pieceType)
    {
        
    }

    public override List<Vector3Int> GetPossibleMoves(GameLayout gameLayout)
    {
        List<Vector3Int> possibleMoves = new List<Vector3Int>();
        foreach (var dir in KNIGHT_MOVE_DIRECTIONS)
        {
            Vector3Int move = position + dir;
            if (!gameLayout.IsInBoard(move)){
                continue;
            }
            if (gameLayout.state[move.x, move.y].containsPiece && gameLayout.state[move.x, move.y].piece.playerType == this.playerType)
            {
                continue;
            }
            possibleMoves.Add(move);
        }
        return possibleMoves;
    }

    public override Piece Copy(){
        return new Knight(position, playerType, pieceType);
    }

}