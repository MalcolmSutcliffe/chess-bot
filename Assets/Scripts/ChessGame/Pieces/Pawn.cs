using System.Collections;
using System.Collections.Generic;

public class Pawn : Piece
{
    private int travelDirection;
    private int startingYPosition;
    public Pawn(int[] position, PlayerType playerType, PieceType pieceType) : base(position, playerType, pieceType)
    {
        if (playerType == PlayerType.White)
        {
            travelDirection = 1;
            startingYPosition = 1;
        }
        else if (playerType == PlayerType.Black)
        {
            travelDirection = -1;
            startingYPosition = 6;
        }
    }

    public override List<int[]> GetPossibleMoves(ChessState chessState)
    {
        List<int[]> possibleMoves = new List<int[]>();

        int[] moveForward1 = new int[] {position[0], position[1] + travelDirection};

        if (!chessState.IsInBoard(moveForward1))
        {
            return possibleMoves;
        }

        // move forward 1
        if (!chessState.boardState[moveForward1[0], moveForward1[1]].containsPiece)
        {
            possibleMoves.Add(moveForward1);
        }

        // move forward 2
        int[] moveForward2 = new int[] {position[0], position[1] + 2*travelDirection};
        
        if (this.position[1] == startingYPosition && !chessState.boardState[moveForward1[0], moveForward1[1]].containsPiece && !chessState.boardState[moveForward2[0], moveForward2[1]].containsPiece)
        {
            possibleMoves.Add(moveForward2);
        }

        // capture
        int[] captureLeft = new int[] {position[0] - 1, position[1] + travelDirection};
        int[] captureRight = new int[] {position[0] + 1, position[1] + travelDirection};
        
        if (CheckCapture(chessState, -1))
        {
            possibleMoves.Add(captureLeft);
        }
        if (CheckCapture(chessState, 1))
        {
            possibleMoves.Add(captureRight);
        }
        
        // en passant
        if (CheckEnPasant(chessState, -1))
        {
            possibleMoves.Add(captureLeft);
        }

        if (CheckEnPasant(chessState, 1))
        {
            possibleMoves.Add(captureRight);
        }

        return possibleMoves;
    }

    public bool CheckCapture(ChessState chessState, int direction)
    {
        if (!chessState.IsInBoard(new int[] {position[0] + direction, position[1]}))
        {
            return false;
        }

        if (!chessState.boardState[position[0] + direction, position[1] + travelDirection].containsPiece)
        {
            return false;
        } 

        if (chessState.boardState[position[0] + direction, position[1] + travelDirection].piece.playerType == this.playerType)
        {
            return false;
        }

        return true;
    }

    public bool CheckEnPasant(ChessState chessState, int direction)
    {
        if (!chessState.IsInBoard(new int[] {position[0] + direction, position[1]}))
        {
            return false;
        }

        if (!chessState.boardState[position[0] + direction, position[1]].containsPiece)
        {
            return false;
        } 

        if (chessState.boardState[position[0] + direction, position[1]].piece.playerType == playerType)
        {
            return false;
        }

        if (chessState.boardState[position[0] + direction, position[1]].piece.pieceType != PieceType.Pawn)
        {
            return false;
        }

        Pawn pawnAdj = (Pawn) chessState.boardState[position[0]+direction, position[1]].piece;
        
        if (!pawnAdj.enPassantable(chessState))
        {
            return false;
        }
        
        return true;
    }

    // method to check if this pawn is enPassantable
    public bool enPassantable(ChessState chessState)
    {
        if (chessState.previousMove == null)
        {
            return false;
        }
        
        if (position[1] != startingYPosition + 2*travelDirection){
            return false;
        }

        if (chessState.previousMove[1][0] != position[0] || chessState.previousMove[1][1] != position[1] )
        {
            return false;
        }

        return true;
    }

    public override Piece Copy(){
        return new Pawn(new int[]{position[0], position[1]}, playerType, pieceType);
    }
}