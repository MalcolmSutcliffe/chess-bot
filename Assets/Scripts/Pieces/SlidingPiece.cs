using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlidingPiece : Piece
{
    protected List<Vector3Int> moveDirections;

    public SlidingPiece(Vector3Int position, PlayerType playerType, PieceType pieceType, List<Vector3Int> moveDirections) : base(position, playerType, pieceType)
    {
        this.moveDirections = moveDirections;
    }

    public override List<Vector3Int> GetPossibleMoves(GameLayout gameLayout)
    {
        List<Vector3Int> possibleMoves = new List<Vector3Int>();
        foreach (var dir in moveDirections)
        {
            possibleMoves.AddRange(GetPossibleMovesInDirection(gameLayout, dir));
        }
        return possibleMoves;
    }

    private List<Vector3Int> GetPossibleMovesInDirection(GameLayout gameLayout, Vector3Int direction)
    {
        List<Vector3Int> possibleMovesInDirection = new List<Vector3Int>();
        Vector3Int move = position;
        while(gameLayout.IsInBoard(move + direction))
        {
            move = move + direction;
            if (gameLayout.state[move.x, move.y].containsPiece)
            {
                if (gameLayout.state[move.x, move.y].piece.playerType == this.playerType)
                {
                    break;
                }
                possibleMovesInDirection.Add(move);
                break;
            }
            possibleMovesInDirection.Add(move);
        }
        return possibleMovesInDirection;
    }

}