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

    public override List<int[]> GetPossibleMoves(GameLayout gameLayout)
    {
        List<int[]> possibleMoves = new List<int[]>();
        // basic directions
        foreach(var direction in KING_MOVE_DIRECTIONS)
        {
            int[] move = new int[] {position[0] + direction[0], position[1] + direction[1]};
            
            if (!gameLayout.IsInBoard(move))
            {
                continue;
            }
            
            if (gameLayout.state[move[0], move[1]].containsPiece && gameLayout.state[move[0], move[1]].piece.playerType == this.playerType)
            {
                continue;
            }
            
            possibleMoves.Add(move);
        }

        return possibleMoves;
    }
    public override List<int[]> GetLegalMoves(GameLayout gameLayout)
    {
        List<int[]> possibleMoves = GetPossibleMoves(gameLayout);
        possibleMoves.AddRange(GetCastlingMoves(gameLayout));
        List<int[]> legalMoves = new List<int[]>();
        
        foreach (var move in possibleMoves)
        {
            GameLayout virtualBoard = gameLayout.DeepCopy();
            virtualBoard.MovePiece(position, move);
            if (virtualBoard.IsKingInCheck(this.playerType, virtualBoard.GetKingPosition(this.playerType)))
            {
                continue;
            }
            legalMoves.Add(move);
        }
        // filter moves if king is in check
        return legalMoves;
        
    }

    public List<int[]> GetCastlingMoves(GameLayout gameLayout)
    {
        List<int[]> possibleMoves = new List<int[]>();
        
        if (!castlingRights)
        {
            return possibleMoves;
        }

        if (gameLayout.IsKingInCheck(this.playerType, this.position))
        {
            return possibleMoves;
        }

        // left castling
        if (CheckLeftCastle(gameLayout))
        {
            possibleMoves.Add(new int[]{position[0] - 2, 0});
        }

        // right castling
        if (CheckRightCastle(gameLayout))
        {
            possibleMoves.Add(new int[]{position[0] + 2, 0});
        }

        return possibleMoves;
        
    }

    public bool CheckLeftCastle(GameLayout gameLayout)
    {
        if (!gameLayout.state[position[0] -4, position[1]].containsPiece)
        {
            return false;
        }

        if (!(gameLayout.state[position[0] -4, position[1]].piece.playerType == this.playerType))
        {
            return false;
        }
        
        if (!(gameLayout.state[position[0] -4, position[1]].piece.pieceType == PieceType.Rook))
        {
            return false;
        }

        Rook leftRook = (Rook) gameLayout.state[position[0] -4, position[1]].piece;
        
        if (!leftRook.castlingRights)
        {
            return false;
        }

        // check empty squares
        for (int i = 1; i < 4; i++)
        {
            if (gameLayout.state[position[0]-i, position[1]].containsPiece)
            {
                return false;
            }
        }

        // check no checks
        for (int i = 1; i < 3; i++)
        {
            if (gameLayout.IsKingInCheck(this.playerType, new int[] {position[0]-i, 0}))
            {
                return false;
            }
        }
        
        return true;
    }

    public bool CheckRightCastle(GameLayout gameLayout)
    {
        if (!gameLayout.state[position[0] +3, position[1]].containsPiece)
        {
            return false;
        }

        if (!(gameLayout.state[position[0] +3, position[1]].piece.playerType == this.playerType))
        {
            return false;
        }
        
        if (!(gameLayout.state[position[0] +3, position[1]].piece.pieceType == PieceType.Rook))
        {
            return false;
        }

        Rook leftRook = (Rook) gameLayout.state[position[0] +3, position[1]].piece;
        
        if (!leftRook.castlingRights)
        {
            return false;
        }

        // check empty squares
        for (int i = 1; i < 3; i++)
        {
            if (gameLayout.state[position[0] + i, position[1]].containsPiece)
            {
                return false;
            }
        }

        // check no checks
        for (int i = 1; i < 2; i++)
        {
            if (gameLayout.IsKingInCheck(this.playerType, new int[] {position[0] + i, 0}))
            {
                return false;
            }
        }
        
        return true;
    }

    public override void Move(int[] position){
        castlingRights = false;
        base.Move(position);
    }

    public override Piece Copy(){
        return new King(new int[] {position[0], position[1]} , playerType, pieceType);
    }

}