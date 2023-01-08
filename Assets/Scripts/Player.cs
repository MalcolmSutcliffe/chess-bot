using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType {
    White,
    Black
}
public class Player
{
    public PlayerType playerType;
    public List<Piece> pieces {get; private set;}

    public Player(PlayerType playerType)
    {
        this.playerType = playerType;
        pieces = new List<Piece>();
    }

    public void AddPiece(Piece piece)
    {
        this.pieces.Add(piece);
    }

}