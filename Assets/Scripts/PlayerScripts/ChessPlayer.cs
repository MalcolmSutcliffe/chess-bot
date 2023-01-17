using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChessPlayer {

    public PlayerType playerType {get; private set;}

    public ChessPlayer(PlayerType playerType)
    {
        this.playerType = playerType;
    }
    public abstract void Update(ChessState chessState);

}