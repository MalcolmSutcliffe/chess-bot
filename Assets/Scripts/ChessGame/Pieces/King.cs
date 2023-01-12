using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    public bool kingCastlingRights;
    public bool queenCastlingRights;
    public static int[][] KING_MOVE_DIRECTIONS = new int[][] {new int[] {-1, 0}, new int[] {1, 0}, new int[] {0, 1}, new int[] {0, -1}, 
                                                                        new int[] {-1, -1}, new int[] {-1, 1}, new int[] {1, -1}, new int[] {1, 1}};
    public King(int position, PlayerType playerType, PieceType pieceType) : base(position, playerType, pieceType)
    {
        kingCastlingRights = true;
        queenCastlingRights = true;
    }

    public override List<Move> GetPossibleMoves(ChessState chessState)
    {
        List<Move> possibleMoves = new List<Move>();
        
        foreach(var dir in KING_MOVE_DIRECTIONS)
        {
            if (!chessState.IsInBoard(position % 8 + dir[0], position/8 + dir[1])){
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
    public override List<Move> GetLegalMoves(ChessState chessState)
    {
        List<Move> possibleMoves = GetPossibleMoves(chessState);
        possibleMoves.AddRange(GetCastlingMoves(chessState));
        List<Move> legalMoves = new List<Move>();
        
        foreach (var move in possibleMoves)
        {
            ChessState virtualBoard = ChessState.DeepCopy(chessState);
            virtualBoard.MovePiece(move, true);
            if (virtualBoard.IsKingInCheck(this.playerType, virtualBoard.GetKingPosition(this.playerType)))
            {
                continue;
            }
            legalMoves.Add(move);
        }
        // filter moves if king is in check
        return legalMoves;
        
    }

    public List<Move> GetCastlingMoves(ChessState chessState)
    {
        List<Move> possibleMoves = new List<Move>();
        
        if (chessState.IsKingInCheck(this.playerType, this.position))
        {
            return possibleMoves;
        }

        // left castling
        if (CheckQueensideCastle(chessState))
        {
            possibleMoves.Add(new Move(pieceType, position, position - 2, false, false, pieceType));
        }

        // right castling
        if (CheckKingsideCastle(chessState))
        {
            possibleMoves.Add(new Move(pieceType, position, position + 2, false, false, pieceType));
        }

        return possibleMoves;
        
    }

    public bool CheckKingsideCastle(ChessState chessState)
    {
        // castling rights
        if (!kingCastlingRights)
        {
            return false;
        }
        
        // check empty squares
        for (int i = 1; i < 3; i++)
        {
            if (chessState.boardState[position + i].containsPiece)
            {
                return false;
            }
        }

        // check no checks
        for (int i = 1; i < 2; i++)
        {
            if (chessState.IsKingInCheck(this.playerType, position + i))
            {
                return false;
            }
        }
        
        return true;
    }

    public bool CheckQueensideCastle(ChessState chessState)
    {
        // castling rights
        if (!queenCastlingRights)
        {
            return false;
        }

        // check empty squares
        for (int i = 1; i < 3; i++)
        {
            if (chessState.boardState[position - i].containsPiece)
            {
                return false;
            }
        }

        // check no checks
        for (int i = 1; i < 2; i++)
        {
            if (chessState.IsKingInCheck(this.playerType, position - i))
            {
                return false;
            }
        }
        return true;
    }

    public override void Move(int position){
        base.Move(position);
        kingCastlingRights = false;
        queenCastlingRights = false;
    }

    public override Piece Copy(){
        King newKing =  new King(position , playerType, pieceType);
        newKing.kingCastlingRights = this.kingCastlingRights;
        newKing.queenCastlingRights = this.queenCastlingRights;
        return newKing;
    }

}