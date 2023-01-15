using System.Collections;
using System.Collections.Generic;
public class Rook : SlidingPiece
{
    public static int[][] ROOK_MOVE_DIRECTIONS = new int[][] { new int[] {-1, 0}, new int[] {1, 0}, new int[] {0, 1}, new int[] {0, -1} };
    public Rook(int position, PlayerType playerType, PieceType pieceType) : base(position, playerType, pieceType, ROOK_MOVE_DIRECTIONS)
    {
    }
    
    public override Piece Copy(){
        return new Rook(position , playerType, pieceType);
    }
}