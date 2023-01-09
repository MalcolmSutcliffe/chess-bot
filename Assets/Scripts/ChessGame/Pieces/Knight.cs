using System.Collections;
using System.Collections.Generic;

public class Knight : Piece
{
    public static List<int[]> KNIGHT_MOVE_DIRECTIONS = new List<int[]>{new int[] {1, 2}, new int[] {1, -2}, new int[] {-1, 2}, new int[] {-1, -2}, 
                                                                        new int[] {2, 1}, new int[] {2, -1}, new int[] {-2, 1}, new int[] {-2, -1}};
    public Knight(int[] position, PlayerType playerType, PieceType pieceType) : base(position, playerType, pieceType)
    {
        
    }

    public override List<Move> GetPossibleMoves(ChessState chessState)
    {
        List<Move> possibleMoves = new List<Move>();
        foreach (var dir in KNIGHT_MOVE_DIRECTIONS)
        {
            int[] move = new int[] {position[0] + dir[0], position[1] + dir[1]};
            if (!chessState.IsInBoard(move)){
                continue;
            }
            if (chessState.boardState[move[0], move[1]].containsPiece && chessState.boardState[move[0], move[1]].piece.playerType == this.playerType)
            {
                continue;
            }
            possibleMoves.Add(new Move(pieceType, position, move, chessState.boardState[move[0], move[1]].containsPiece, false, pieceType));
        }
        return possibleMoves;
    }

    public override Piece Copy(){
        return new Knight(new int[] {position[0], position[1]} , playerType, pieceType);
    }

}