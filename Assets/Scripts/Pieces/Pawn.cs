using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    private int travelDirection;
    private int startingYPosition;
    public Pawn(Vector3Int position, PlayerType playerType, PieceType pieceType) : base(position, playerType, pieceType)
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

    public override List<Vector3Int> GetPossibleMoves(GameLayout gameLayout)
    {
        List<Vector3Int> possibleMoves = new List<Vector3Int>();

        Vector3Int moveForward1 = position + new Vector3Int(0, travelDirection, 0);

        if (!gameLayout.IsInBoard(moveForward1))
        {
            return possibleMoves;
        }

        // move forward 1
        if (!gameLayout.state[moveForward1.x, moveForward1.y].containsPiece)
        {
            possibleMoves.Add(moveForward1);
        }

        // move forward 2
        Vector3Int moveForward2 = position + new Vector3Int(0, 2*travelDirection, 0);
        
        if (this.position.y == startingYPosition && !gameLayout.state[moveForward2.x, moveForward2.y].containsPiece)
        {
            possibleMoves.Add(moveForward2);
        }

        // capture
        Vector3Int captureLeft = position + new Vector3Int(-1, travelDirection, 0);
        Vector3Int captureRight = position + new Vector3Int(1, travelDirection, 0);
        
        if (CheckCapture(gameLayout, -1))
        {
            possibleMoves.Add(captureLeft);
        }
        if (CheckCapture(gameLayout, 1))
        {
            possibleMoves.Add(captureRight);
        }
        
        // en passant
        Vector3Int enPassantLeft = position + new Vector3Int(-1, travelDirection, 0);
        Vector3Int enPassantRight = position + new Vector3Int(1, travelDirection, 0);
        
        if (CheckEnPasant(gameLayout, -1))
        {
            possibleMoves.Add(enPassantLeft);
        }

        if (CheckEnPasant(gameLayout, 1))
        {
            possibleMoves.Add(enPassantRight);
        }

        return possibleMoves;
    }

    public bool CheckCapture(GameLayout gameLayout, int direction)
    {
        if (!gameLayout.IsInBoard(position + new Vector3Int(direction, 0, 0))){
            return false;
        }
        if (!gameLayout.state[position.x + direction, position.y + travelDirection].containsPiece)
        {
            return false;
        } 
        if (gameLayout.state[position.x + direction, position.y + travelDirection].piece.playerType == this.playerType)
        {
            return false;
        }

        return true;
    }

    public bool CheckEnPasant(GameLayout gameLayout, int direction)
    {
        if (!gameLayout.IsInBoard(position + new Vector3Int(direction, 0, 0))){
            return false;
        }
        if (!gameLayout.state[position.x + direction, position.y].containsPiece)
        {
            return false;
        } 
        if (gameLayout.state[position.x + direction, position.y].piece.playerType == playerType)
        {
            return false;
        }
        if (gameLayout.state[position.x + direction, position.y].piece.pieceType != PieceType.Pawn)
        {
            return false;
        }
        Pawn pawnAdj = (Pawn) gameLayout.state[position.x + direction, position.y].piece;
        if (!pawnAdj.enPassantable(gameLayout))
        {
            return false;
        }
        
        return true;
    }

    // method to check if this pawn is enPassantable
    public bool enPassantable(GameLayout gameLayout)
    {
        if (gameLayout.previousMove == null)
        {
            return false;
        }
        
        if (position.y != startingYPosition + 2*travelDirection){
            return false;
        }

        if (gameLayout.previousMove[1] != position)
        {
            return false;
        }

        return true;
    }

    public override Piece Copy(){
        return new Pawn(position, playerType, pieceType);
    }
}