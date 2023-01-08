using System.Collections;
using System.Collections.Generic;

public class Bishop : SlidingPiece
{
    public static List<int[]> BISHOP_MOVE_DIRECTIONS = new List<int[]>{new int[] {-1, -1}, new int[] {-1, 1}, new int[] {1, -1}, new int[] {1, 1}};
    public Bishop(int[] position, PlayerType playerType, PieceType pieceType) : base(position, playerType, pieceType, BISHOP_MOVE_DIRECTIONS)
    {
        
    }

    public override Piece Copy(){
        return new Bishop(new int[] {position[0], position[1]} , playerType, pieceType);
    }
}