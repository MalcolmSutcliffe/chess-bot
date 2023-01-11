using System.Collections;
using System.Collections.Generic;

public class Knight : Piece
{
    public static int[][] KNIGHT_MOVE_DIRECTIONS = new int[][]{new int[] {1, 2}, new int[] {1, -2}, new int[] {-1, 2}, new int[] {-1, -2}, 
                                                                        new int[] {2, 1}, new int[] {2, -1}, new int[] {-2, 1}, new int[] {-2, -1}};
    public Knight(int position, PlayerType playerType, PieceType pieceType) : base(position, playerType, pieceType)
    {
        
    }

    public override List<Move> GetPossibleMoves(ChessState chessState)
    {
        List<Move> possibleMoves = new List<Move>();
        foreach (var dir in KNIGHT_MOVE_DIRECTIONS)
        {
            if (!chessState.IsInBoard(position % 8 + dir[0], position /8 + dir[1])){
                continue;
            }
            
            int move = position + dir[0] + dir[1] *8;
            
            if (chessState.boardState[move].containsPiece && chessState.boardState[move].piece.playerType == this.playerType)
            {
                continue;
            }
            possibleMoves.Add(new Move(pieceType, position, move, chessState.boardState[move].containsPiece, false, pieceType));
        }
        return possibleMoves;
    }

    public override Piece Copy(){
        return new Knight(position , playerType, pieceType);
    }

}