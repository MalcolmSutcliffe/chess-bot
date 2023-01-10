using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionalMove
{
    public bool containsMove {get; private set;}
    public Move move {get; private set;}

    public OptionalMove(Move move)
    {
        this.containsMove = true;
        this.move = move;
    }

    public OptionalMove()
    {
        this.containsMove = false;
    }
}
public abstract class ChessPlayer {

    public PlayerType playerType {get; private set;}

    public ChessPlayer(PlayerType playerType)
    {
        this.playerType = playerType;
    }
    public abstract OptionalMove GetMove(ChessState chessState);
}