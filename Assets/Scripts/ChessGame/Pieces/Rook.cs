using System.Collections;
using System.Collections.Generic;
public class Rook : SlidingPiece
{
    public static List<int[]> ROOK_MOVE_DIRECTIONS = new List<int[]> { new int[] {-1, 0}, new int[] {1, 0}, new int[] {0, 1}, new int[] {0, -1} };
    public bool castlingRights;
    public Rook(int[] position, PlayerType playerType, PieceType pieceType) : base(position, playerType, pieceType, ROOK_MOVE_DIRECTIONS)
    {
        castlingRights = true;
    }
    public override void Move(int[] position){
        castlingRights = false;
        base.Move(position);
    }

    public override Piece Copy(){
        Rook newRook = new Rook(new int[] {position[0], position[1]} , playerType, pieceType);
        newRook.castlingRights = this.castlingRights;
        return newRook;
    }
}