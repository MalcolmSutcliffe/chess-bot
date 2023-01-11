using System.Collections;
using System.Collections.Generic;

public class Queen : SlidingPiece
{
    public static int[][] QUEEN_MOVE_DIRECTIONS = new int[][] {new int[] {-1, 0}, new int[] {1, 0}, new int[] {0, 1}, new int[] {0, -1}, 
                                                                        new int[] {-1, -1}, new int[] {-1, 1}, new int[] {1, -1}, new int[] {1, 1}};
    public Queen(int position, PlayerType playerType, PieceType pieceType) : base(position, playerType, pieceType, QUEEN_MOVE_DIRECTIONS)
    {
        
    }

    public override Piece Copy(){
        return new Queen(position , playerType, pieceType);
    }

}