using System.Collections;
using System.Collections.Generic;

public class Pawn : Piece
{
    private int travelDirection;
    private int startingYPosition;
    public Pawn(int position, PlayerType playerType, PieceType pieceType) : base(position, playerType, pieceType)
    {
        if (playerType == PlayerType.White)
        {
            travelDirection = 8;
            startingYPosition = 1;
        }
        else if (playerType == PlayerType.Black)
        {
            travelDirection = -8;
            startingYPosition = 6;
        }
    }

    public override List<Move> GetPossibleMoves(ChessState chessState)
    {
        List<Move> possibleMoves = new List<Move>();

        int moveForward1 = position + travelDirection;

        if (!chessState.IsInBoard(moveForward1))
        {
            return possibleMoves;
        }

        // move forward 1
        if (!chessState.boardState[moveForward1].containsPiece)
        {
            // check promotion
            if (IsPromoting(moveForward1))
            {
                possibleMoves.Add(new Move(pieceType, position, moveForward1, false, true, PieceType.Queen));
                possibleMoves.Add(new Move(pieceType, position, moveForward1, false, true, PieceType.Knight));
                possibleMoves.Add(new Move(pieceType, position, moveForward1, false, true, PieceType.Rook));
                possibleMoves.Add(new Move(pieceType, position, moveForward1, false, true, PieceType.Bishop));
            }
            else
            {
                possibleMoves.Add(new Move(pieceType, position, moveForward1, false, false, pieceType));
            }
        }

        // move forward 2
        int moveForward2 = position + 2*travelDirection;
        
        if (this.position / 8 == startingYPosition && !chessState.boardState[moveForward1].containsPiece && !chessState.boardState[moveForward2].containsPiece)
        {
            possibleMoves.Add(new Move(pieceType, position, moveForward2, false, false, pieceType));
        }

        // capture
        int captureLeft = position + travelDirection-1;
        int captureRight = position + travelDirection+1;
        
        if (CheckCapture(chessState, -1))
        {
            // check promotion
            if (IsPromoting(captureLeft))
            {
                possibleMoves.Add(new Move(pieceType, position, captureLeft, true, true, PieceType.Queen));
                possibleMoves.Add(new Move(pieceType, position, captureLeft, true, true, PieceType.Knight));
                possibleMoves.Add(new Move(pieceType, position, captureLeft, true, true, PieceType.Rook));
                possibleMoves.Add(new Move(pieceType, position, captureLeft, true, true, PieceType.Bishop));
            }
            else
            {
                possibleMoves.Add(new Move(pieceType, position, captureLeft, true, false, pieceType));
            }
        }
        if (CheckCapture(chessState, 1))
        {
            // check promotion
            if (IsPromoting(captureRight))
            {
                possibleMoves.Add(new Move(pieceType, position, captureRight, true, true, PieceType.Queen));
                possibleMoves.Add(new Move(pieceType, position, captureRight, true, true, PieceType.Knight));
                possibleMoves.Add(new Move(pieceType, position, captureRight, true, true, PieceType.Rook));
                possibleMoves.Add(new Move(pieceType, position, captureRight, true, true, PieceType.Bishop));
            }
            else
            {
                possibleMoves.Add(new Move(pieceType, position, captureRight, true, false, pieceType));
            }
        }
        
        // en passant
        if (CheckEnPasant(chessState, -1))
        {
            possibleMoves.Add(new Move(pieceType, position, captureLeft, true, false, pieceType));
        }

        if (CheckEnPasant(chessState, 1))
        {
            possibleMoves.Add(new Move(pieceType, position, captureRight, true, false, pieceType));
        }

        return possibleMoves;
    }

    private bool IsPromoting(int toPos)
    {
        return (travelDirection == 8 && toPos / 8 == 7) || (travelDirection == -8 && toPos / 8 == 0);
    }

    private bool CheckCapture(ChessState chessState, int direction)
    {
        if (!chessState.IsInBoard(position % 8+ direction, position/8))
        {
            return false;
        }

        if (!chessState.boardState[position + travelDirection + direction].containsPiece)
        {
            return false;
        } 

        if (chessState.boardState[position + travelDirection + direction].piece.playerType == this.playerType)
        {
            return false;
        }

        return true;
    }

    private bool CheckEnPasant(ChessState chessState, int direction)
    {
        if (!chessState.IsInBoard(position % 8+ direction, position/8))
        {
            return false;
        }

        if (!chessState.boardState[position + direction].containsPiece)
        {
            return false;
        } 

        if (chessState.boardState[position + direction].piece.playerType == playerType)
        {
            return false;
        }

        if (chessState.boardState[position + direction].piece.pieceType != PieceType.Pawn)
        {
            return false;
        }

        if (chessState.previousMove == null)
        {
            return false;
        }
        
        if (chessState.previousMove[0] != 7 - startingYPosition){
            return false;
        }

        if (chessState.previousMove[1] != 7 - startingYPosition + 2 * (-travelDirection/8)){
            return false;
        }
        
        return true;
    }

    public override Piece Copy(){
        return new Pawn(position, playerType, pieceType);
    }
}