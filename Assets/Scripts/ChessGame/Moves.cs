using System;
using System.Collections;
using System.Collections.Generic;

public class Move {

    public static Dictionary<char, PieceType> pieceChars = new Dictionary<char, PieceType>{
        {'R' , PieceType.Rook},
        {'N' , PieceType.Knight},
        {'B' , PieceType.Bishop},
        {'Q' , PieceType.Queen},
        {'K' , PieceType.King}, 
    };
    
    public PieceType pieceType {get; private set;}
    public int[] fromPos {get; private set;}
    public int[] toPos {get; private set;}
    public bool capturePiece {get; private set;}
    public bool promotePiece {get; private set;}
    public PieceType promotedTo {get; private set;}
    
    public Move(PieceType pieceType, int[] fromPos, int[] toPos, bool capturePiece, bool promotePiece, PieceType promotedTo)
    {
        this.pieceType = pieceType;
        this.fromPos = fromPos;
        this.toPos = toPos;
        this.capturePiece = capturePiece;
        this.promotePiece = promotePiece;
        this.promotedTo = promotedTo;
    }

    public static int[] chessNotationToPosition(string chessNotation)
    {
        if (chessNotation.Length != 2)
        {
            // throw error
            throw new ArgumentException("illegal position notation!");
        }

        char file = chessNotation[0];
        char rank = chessNotation[1];

        // ascii comparison
        if (file < 'a' || file > 'h')
        {
            // throw error
            throw new ArgumentException("illegal position notation!");
        }

        if (rank < '1' || rank > '8')
        {
            // throw error
            throw new ArgumentException("illegal position notation!");
        }

        return new int[]{file-'a',rank-'1'};
    }

}