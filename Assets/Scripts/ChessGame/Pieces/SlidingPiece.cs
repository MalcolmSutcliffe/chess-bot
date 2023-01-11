using System.Collections;
using System.Collections.Generic;

public abstract class SlidingPiece : Piece
{
    protected int[][] moveDirections;

    public SlidingPiece(int position, PlayerType playerType, PieceType pieceType, int[][] moveDirections) : base(position, playerType, pieceType)
    {
        this.moveDirections = moveDirections;
    }

    public override List<Move> GetPossibleMoves(ChessState chessState)
    {
        List<Move> possibleMoves = new List<Move>();
        foreach (var direction in moveDirections)
        {
            possibleMoves.AddRange(GetPossibleMovesInDirection(chessState, direction));
        }
        return possibleMoves;
    }

    private List<Move> GetPossibleMovesInDirection(ChessState chessState, int[] direction)
    {
        List<Move> possibleMovesInDirection = new List<Move>();
        int move = position;
        while(chessState.IsInBoard(move % 8 + direction[0], move/8 + direction[1]))
        {
            move = move + direction[0] + direction[1]*8;
            if (chessState.boardState[move].containsPiece)
            {
                if (chessState.boardState[move].piece.playerType == this.playerType)
                {
                    break;
                }
                possibleMovesInDirection.Add(new Move(pieceType, position, move, true, false, pieceType));
                break;
            }
            possibleMovesInDirection.Add(new Move(pieceType, position, move, false, false, pieceType));
        }
        return possibleMovesInDirection;
    }

}