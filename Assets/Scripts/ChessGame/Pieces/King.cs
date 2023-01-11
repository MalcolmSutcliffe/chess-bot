using System.Collections;
using System.Collections.Generic;

public class King : Piece
{
    public bool castlingRights;
    public static List<int[]> KING_MOVE_DIRECTIONS = new List<int[]>{new int[] {-1, 0}, new int[] {1, 0}, new int[] {0, 1}, new int[] {0, -1}, 
                                                                        new int[] {-1, -1}, new int[] {-1, 1}, new int[] {1, -1}, new int[] {1, 1}};
    public King(int[] position, PlayerType playerType, PieceType pieceType) : base(position, playerType, pieceType)
    {
        castlingRights = true;
    }

    public override List<Move> GetPossibleMoves(ChessState chessState)
    {
        List<Move> possibleMoves = new List<Move>();
        // basic directions
        foreach(var direction in KING_MOVE_DIRECTIONS)
        {
            int[] move = new int[] {position[0] + direction[0], position[1] + direction[1]};
            
            if (!chessState.IsInBoard(move))
            {
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
    public override List<Move> GetLegalMoves(ChessState chessState)
    {
        List<Move> possibleMoves = GetPossibleMoves(chessState);
        possibleMoves.AddRange(GetCastlingMoves(chessState));
        List<Move> legalMoves = new List<Move>();
        
        foreach (var move in possibleMoves)
        {
            ChessState virtualBoard = chessState.DeepCopy();
            virtualBoard.MovePiece(move);
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
        
        if (!castlingRights)
        {
            return possibleMoves;
        }

        if (chessState.IsKingInCheck(this.playerType, this.position))
        {
            return possibleMoves;
        }

        // left castling
        if (CheckQueensideCastle(chessState))
        {
            possibleMoves.Add(new Move(pieceType, position, new int[]{position[0] - 2, position[1]}, false, false, pieceType));
        }

        // right castling
        if (CheckKingsideCastle(chessState))
        {
            possibleMoves.Add(new Move(pieceType, position, new int[]{position[0] + 2, position[1]}, false, false, pieceType));
        }

        return possibleMoves;
        
    }

    public bool CheckKingsideCastle(ChessState chessState)
    {
        // castling rights
        if (!KingSideCastlingRights(chessState))
        {
            return false;
        }
        
        // check empty squares
        for (int i = 1; i < 3; i++)
        {
            if (chessState.boardState[position[0] + i, position[1]].containsPiece)
            {
                return false;
            }
        }

        // check no checks
        for (int i = 1; i < 2; i++)
        {
            if (chessState.IsKingInCheck(this.playerType, new int[] {position[0] + i, position[1]}))
            {
                return false;
            }
        }
        
        return true;
    }

    public bool CheckQueensideCastle(ChessState chessState)
    {
        // castling rights
        if (!QueenSideCastlingRights(chessState))
        {
            return false;
        }

        // check empty squares
        for (int i = 1; i < 4; i++)
        {
            if (chessState.boardState[position[0]-i, position[1]].containsPiece)
            {
                return false;
            }
        }

        // check no checks
        for (int i = 1; i < 3; i++)
        {
            if (chessState.IsKingInCheck(this.playerType, new int[] {position[0]-i, position[1]}))
            {
                return false;
            }
        }
        
        return true;
    }

    public bool KingSideCastlingRights(ChessState chessState)
    {
        if (!castlingRights)
        {
            return false;
        }
        // check rook
        if (!chessState.boardState[position[0] +3, position[1]].containsPiece)
        {
            return false;
        }

        if (!(chessState.boardState[position[0] +3, position[1]].piece.playerType == this.playerType))
        {
            return false;
        }
        
        if (!(chessState.boardState[position[0] +3, position[1]].piece.pieceType == PieceType.Rook))
        {
            return false;
        }

        Rook leftRook = (Rook) chessState.boardState[position[0] +3, position[1]].piece;
        
        if (!leftRook.castlingRights)
        {
            return false;
        }
        return true;
    }

    public bool QueenSideCastlingRights(ChessState chessState)
    {
        if (!castlingRights)
        {
            return false;
        }
        if (!chessState.boardState[position[0] -4, position[1]].containsPiece)
        {
            return false;
        }

        if (!(chessState.boardState[position[0] -4, position[1]].piece.playerType == this.playerType))
        {
            return false;
        }
        
        if (!(chessState.boardState[position[0] -4, position[1]].piece.pieceType == PieceType.Rook))
        {
            return false;
        }

        Rook leftRook = (Rook) chessState.boardState[position[0] -4, position[1]].piece;
        
        if (!leftRook.castlingRights)
        {
            return false;
        }
        return true;
    }

    public override void Move(int[] position){
        castlingRights = false;
        base.Move(position);
    }

    public override Piece Copy(){
        King newKing =  new King(new int[] {position[0], position[1]} , playerType, pieceType);
        newKing.castlingRights = this.castlingRights;
        return newKing;
    }

}