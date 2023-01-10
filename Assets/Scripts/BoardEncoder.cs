using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static class BoardEncoder
{
    public static string EncodeChessStateToFEN(ChessState chessState)
    {
        string toReturn = "";

        // encode board state
        toReturn += EncodeBoardState(chessState.boardState);

        toReturn += " ";

        // encode player turn
        if (chessState.activePlayer.playerType == PlayerType.White)
        {
            toReturn += "w";
        }
        else if (chessState.activePlayer.playerType == PlayerType.Black)
        {
            toReturn += "b";
        }

        toReturn += " ";
        
        // encode castling rights
        bool hasEncodedCastlingRights = false;
        if (chessState.playerWhite.king.KingSideCastlingRights(chessState))
        {
            toReturn += "K";
            hasEncodedCastlingRights = true;
        }
        if (chessState.playerWhite.king.QueenSideCastlingRights(chessState))
        {
            toReturn += "Q";
            hasEncodedCastlingRights = true;
        }
        if (chessState.playerBlack.king.KingSideCastlingRights(chessState))
        {
            toReturn += "k";
            hasEncodedCastlingRights = true;
        }
        if (chessState.playerBlack.king.QueenSideCastlingRights(chessState))
        {
            toReturn += "q";
            hasEncodedCastlingRights = true;
        }
        if (!hasEncodedCastlingRights)
        {
            toReturn += "-";
        }

        toReturn += " ";

        // encode enpassant
        toReturn += GetEnPasantSquare(chessState);

        toReturn += " ";

        toReturn += chessState.halfClock.ToString();
        
        toReturn += " ";

        toReturn += chessState.fullMoveCount.ToString();

        return toReturn;
    }

    public static string EncodeBoardState(ChessSquare[,] boardState)
    {
        string toReturn="";
        for (int y = 7; y >= 0; y--)
        {
            toReturn += EncodeRank(boardState, y);
            if (y > 0)
            {
                toReturn += "/";
            }
        }
        return toReturn;
    }

    private static string EncodeRank(ChessSquare[,] boardState, int rank)
    {
        string toReturn = "";
        int runningEmptySpaces=0;
        for (int x = 0; x < 8; x++)
        {
            if (!boardState[x, rank].containsPiece)
            {
                runningEmptySpaces++;
                continue;
            }
            if (runningEmptySpaces > 0)
            {
                toReturn += (char) ('0' + runningEmptySpaces);
            }
            char pieceSymbol = Piece.pieceToChar[boardState[x, rank].piece.pieceType];
            
            if (boardState[x, rank].piece.playerType == PlayerType.Black)
            {
                pieceSymbol = Char.ToLower(pieceSymbol);
            }
            
            toReturn += pieceSymbol;
            runningEmptySpaces=0;
        }
        if (runningEmptySpaces > 0)
        {
            toReturn += (char) ('0' + runningEmptySpaces);
        }
        return toReturn;
    }

    private static string GetEnPasantSquare(ChessState chessState)
    {
        int[] previousMoveFrom = chessState.previousMove[0];
        int[] previousMoveTo = chessState.previousMove[1];
        if (!chessState.boardState[previousMoveTo[0], previousMoveTo[1]].containsPiece)
        {
            return "-";
        }
        if (chessState.boardState[previousMoveTo[0], previousMoveTo[1]].piece.pieceType != PieceType.Pawn)
        {
            return "-";
        }
        if (Math.Abs(previousMoveFrom[1] - previousMoveTo[1]) < 2)
        {
            return "-";
        }
        return Move.positionToChessNotation(new int[] {previousMoveTo[0], (int) ((previousMoveFrom[1] + previousMoveTo[1])/2)});
    }
}