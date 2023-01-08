using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    public bool castlingRights;
    public static List<Vector3Int> KING_MOVE_DIRECTIONS = new List<Vector3Int>{new Vector3Int(-1, 0, 0), new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0), new Vector3Int(-1, -1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(1, -1, 0), new Vector3Int(1, 1, 0)};
    public King(Vector3Int position, PlayerType player, PieceType piece, GameObject gameObject) : base(position, player, piece, gameObject)
    {
        castlingRights = true;
    }

    public override List<Vector3Int> GetPossibleMoves(GameLayout gameLayout)
    {
        List<Vector3Int> possibleMoves = new List<Vector3Int>();
        // basic directions
        foreach(var dir in KING_MOVE_DIRECTIONS)
        {
            Vector3Int move = position + dir;
            if (!gameLayout.IsInBoard(move))
            {
                continue;
            }
            if (gameLayout.state[move.x, move.y].containsPiece && gameLayout.state[move.x, move.y].piece.playerType == this.playerType)
            {
                continue;
            }
            possibleMoves.Add(move);
        }

        return possibleMoves;
    }
    public override List<Vector3Int> GetLegalMoves(GameLayout gameLayout)
    {
        List<Vector3Int> possibleMoves = GetPossibleMoves(gameLayout);
        possibleMoves.AddRange(GetCastlingMoves(gameLayout));
        List<Vector3Int> legalMoves = new List<Vector3Int>();
        
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

    public List<Vector3Int> GetCastlingMoves(GameLayout gameLayout)
    {
        List<Vector3Int> possibleMoves = new List<Vector3Int>();
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
            possibleMoves.Add(position + new Vector3Int(-2, 0, 0));
        }

        // left castling
        if (CheckRightCastle(gameLayout))
        {
            possibleMoves.Add(position + new Vector3Int(2, 0, 0));
        }

        return possibleMoves;
        
    }

    public bool CheckLeftCastle(GameLayout gameLayout)
    {
        if (!gameLayout.state[position.x -4, position.y].containsPiece)
        {
            return false;
        }

        if (!(gameLayout.state[position.x -4, position.y].piece.playerType == this.playerType))
        {
            return false;
        }
        
        if (!(gameLayout.state[position.x -4, position.y].piece.pieceType == PieceType.Rook))
        {
            return false;
        }

        Rook leftRook = (Rook) gameLayout.state[position.x -4, position.y].piece;
        
        if (!leftRook.castlingRights)
        {
            return false;
        }

        // check empty squares
        for (int i = 1; i < 4; i++)
        {
            if (gameLayout.state[position.x-i, position.y].containsPiece)
            {
                return false;
            }
        }

        // check no checks
        for (int i = 1; i < 3; i++)
        {
            if (gameLayout.IsKingInCheck(this.playerType, position + new Vector3Int(-i, 0, 0)))
            {
                return false;
            }
        }
        
        return true;
    }

    public bool CheckRightCastle(GameLayout gameLayout)
    {
        if (!gameLayout.state[position.x +3, position.y].containsPiece)
        {
            return false;
        }

        if (!(gameLayout.state[position.x +3, position.y].piece.playerType == this.playerType))
        {
            return false;
        }
        
        if (!(gameLayout.state[position.x +3, position.y].piece.pieceType == PieceType.Rook))
        {
            return false;
        }

        Rook leftRook = (Rook) gameLayout.state[position.x +3, position.y].piece;
        
        if (!leftRook.castlingRights)
        {
            return false;
        }

        // check empty squares
        for (int i = 1; i < 3; i++)
        {
            if (gameLayout.state[position.x + i, position.y].containsPiece)
            {
                return false;
            }
        }

        // check no checks
        for (int i = 1; i < 2; i++)
        {
            if (gameLayout.IsKingInCheck(this.playerType, position+new Vector3Int(i, 0, 0)))
            {
                return false;
            }
        }
        
        return true;
    }

    public override void Move(Vector3Int position){
        castlingRights = false;
        base.Move(position);
    }

}