using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MaterialisticMinMaxPlayer : MinMaxPlayer {

    public static Dictionary<PieceType, int> materialValue = new Dictionary<PieceType, int> {
        {PieceType.Pawn, 1},
        {PieceType.Knight, 3},
        {PieceType.Bishop, 3},
        {PieceType.Rook, 5},
        {PieceType.Queen, 9},
        {PieceType.King, 0},
    };

    public MaterialisticMinMaxPlayer(PlayerType playerType, int maxDepth) : base(playerType, maxDepth)
    {

    }
    public override float BoardHeuristic(ChessState chessState)
    {
        int materialCount = 0;
        foreach (var piece in chessState.playerWhite.pieces)
        {
            materialCount += materialValue[piece.pieceType];
        }
        foreach (var piece in chessState.playerBlack.pieces)
        {
            materialCount -= materialValue[piece.pieceType];
        }
        return materialCount;
    }
}