using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public Pawn(Vector3Int position, PlayerType playerType, PieceType pieceType) : base(position, playerType, pieceType)
    {
    }

    public override List<Vector3Int> GetPossibleMoves(GameLayout gameLayout)
    {
        List<Vector3Int> possibleMoves = new List<Vector3Int>();

        // logic for white pawn
        if (this.playerType == PlayerType.White)
        {
            // move forward (dont need to worry about out of board as it will be a new piece by then)
            Vector3Int moveForward1 = position + new Vector3Int(0, 1, 0);
            if (!gameLayout.state[moveForward1.x, moveForward1.y].containsPiece)
            {
                possibleMoves.Add(moveForward1);
            }

            // move forward 2
            Vector3Int moveForward2 = position + new Vector3Int(0, 2, 0);
            if (this.position.y == 1 && !gameLayout.state[moveForward2.x, moveForward2.y].containsPiece)
            {
                possibleMoves.Add(moveForward2);
            }

            // capture
            Vector3Int captureLeft = position + new Vector3Int(-1, 1, 0);
            Vector3Int captureRight = position + new Vector3Int(1, 1, 0);
            if (gameLayout.IsInBoard(captureLeft) && gameLayout.state[captureLeft.x, captureLeft.y].containsPiece && gameLayout.state[captureLeft.x, captureLeft.y].piece.playerType != this.playerType)
            {
                possibleMoves.Add(captureLeft);
            }
            if (gameLayout.IsInBoard(captureRight) && gameLayout.state[captureRight.x, captureRight.y].containsPiece && gameLayout.state[captureRight.x, captureRight.y].piece.playerType != this.playerType)
            {
                possibleMoves.Add(captureRight);
            }
            
            // en passant
            Vector3Int enPassantLeft = position + new Vector3Int(-1, 1, 0);
            Vector3Int enPassantRight = position + new Vector3Int(1, 1, 0);
            if (gameLayout.IsInBoard(enPassantLeft))
            {    if (gameLayout.state[position.x - 1, position.y].containsPiece && gameLayout.state[position.x - 1, position.y].piece.playerType != playerType && gameLayout.state[position.x - 1, position.y].piece.pieceType == PieceType.Pawn)
                {
                    Pawn pawnLeft = (Pawn) gameLayout.state[position.x - 1, position.y].piece;
                    if (pawnLeft.enPassantable(gameLayout))
                        possibleMoves.Add(enPassantLeft);
                }
            }
            if (gameLayout.IsInBoard(enPassantRight))
            {
                if (gameLayout.state[position.x + 1, position.y].containsPiece && gameLayout.state[position.x + 1, position.y].piece.playerType != playerType && gameLayout.state[position.x + 1, position.y].piece.pieceType == PieceType.Pawn)
                {
                    Pawn pawnRight = (Pawn) gameLayout.state[position.x + 1, position.y].piece;
                    if (pawnRight.enPassantable(gameLayout))
                        possibleMoves.Add(enPassantRight);
                }
            }
            return possibleMoves;
        }
        
        // logic for black pawn
        if (this.playerType == PlayerType.Black)
        {
            // move forward (dont need to worry about out of board as it will be a new piece by then)
            Vector3Int moveForward1 = position + new Vector3Int(0, -1, 0);
            if (!gameLayout.state[moveForward1.x, moveForward1.y].containsPiece)
            {
                possibleMoves.Add(moveForward1);
            }

            // move forward 2
            Vector3Int moveForward2 = position + new Vector3Int(0, -2, 0);
            if (this.position.y == 6 && !gameLayout.state[moveForward2.x, moveForward2.y].containsPiece)
            {
                possibleMoves.Add(moveForward2);
            }

            // capture
            Vector3Int captureLeft = position + new Vector3Int(-1, -1, 0);
            Vector3Int captureRight = position + new Vector3Int(1, -1, 0);
            if (gameLayout.IsInBoard(captureLeft) && gameLayout.state[captureLeft.x, captureLeft.y].containsPiece && gameLayout.state[captureLeft.x, captureLeft.y].piece.playerType != this.playerType)
            {
                possibleMoves.Add(captureLeft);
            }
            if (gameLayout.IsInBoard(captureRight) && gameLayout.state[captureRight.x, captureRight.y].containsPiece && gameLayout.state[captureRight.x, captureRight.y].piece.playerType != this.playerType)
            {
                possibleMoves.Add(captureRight);
            }
            
            // en passant
            Vector3Int enPassantLeft = position + new Vector3Int(-1, -1, 0);
            Vector3Int enPassantRight = position + new Vector3Int(1, -1, 0);
            if (gameLayout.IsInBoard(enPassantLeft))
            {    if (gameLayout.state[position.x - 1, position.y].containsPiece && gameLayout.state[position.x - 1, position.y].piece.playerType != playerType && gameLayout.state[position.x - 1, position.y].piece.pieceType == PieceType.Pawn)
                {
                    Pawn pawnLeft = (Pawn) gameLayout.state[position.x - 1, position.y].piece;
                    if (pawnLeft.enPassantable(gameLayout))
                        possibleMoves.Add(enPassantLeft);
                }
            }
            if (gameLayout.IsInBoard(enPassantRight))
            {
                if (gameLayout.state[position.x + 1, position.y].containsPiece && gameLayout.state[position.x + 1, position.y].piece.playerType != playerType && gameLayout.state[position.x + 1, position.y].piece.pieceType == PieceType.Pawn)
                {
                    Pawn pawnRight = (Pawn) gameLayout.state[position.x + 1, position.y].piece;
                    if (pawnRight.enPassantable(gameLayout))
                        possibleMoves.Add(enPassantRight);
                }
            }
            return possibleMoves;
        }
        return possibleMoves;
    }

    public bool enPassantable(GameLayout gameLayout)
    {
        if (gameLayout.previousMove == null)
        {
            return false;
        }
        if (playerType == PlayerType.White)
        {
            if (position.y != 3){
                return false;
            }
            // if previous move was moving this pawn forward
            return (gameLayout.previousMove == new Vector3Int[]{new Vector3Int(position.x, 1, 0), new Vector3Int(position.x, 3, 0)});
        }
        if (playerType == PlayerType.Black)
        {
            if (position.y != 6){
                return false;
            }
            // if previous move was moving this pawn forward
            return (gameLayout.previousMove == new Vector3Int[]{new Vector3Int(position.x, 6, 0), new Vector3Int(position.x, 4, 0)});
        }
        return false;
    }

    public override Piece Copy(){
        return new Pawn(position, playerType, pieceType);
    }
}