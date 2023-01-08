using System.Collections;
using System.Collections.Generic;

public abstract class SlidingPiece : Piece
{
    protected List<int[]> moveDirections;

    public SlidingPiece(int[] position, PlayerType playerType, PieceType pieceType, List<int[]> moveDirections) : base(position, playerType, pieceType)
    {
        this.moveDirections = moveDirections;
    }

    public override List<int[]> GetPossibleMoves(GameLayout gameLayout)
    {
        List<int[]> possibleMoves = new List<int[]>();
        foreach (var direction in moveDirections)
        {
            possibleMoves.AddRange(GetPossibleMovesInDirection(gameLayout, direction));
        }
        return possibleMoves;
    }

    private List<int[]> GetPossibleMovesInDirection(GameLayout gameLayout, int[] direction)
    {
        List<int[]> possibleMovesInDirection = new List<int[]>();
        int[] move = position;
        while(gameLayout.IsInBoard(new int[] {move[0] + direction[0], move[1] + direction[1]} ))
        {
            move = new int[] {move[0] + direction[0], move[1] + direction[1]};
            if (gameLayout.state[move[0], move[1]].containsPiece)
            {
                if (gameLayout.state[move[0], move[1]].piece.playerType == this.playerType)
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