using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move{
    public PieceType pieceType {get; private set;}
    public int fromPos {get; private set;}
    public int toPos {get; private set;}
    public bool capturePiece {get; private set;}
    public bool promotePiece {get; private set;}
    public PieceType promotedTo {get; private set;}
    private long hashID;
    
    public Move(PieceType pieceType, int fromPos, int toPos, bool capturePiece, bool promotePiece, PieceType promotedTo)
    {
        this.pieceType = pieceType;
        this.fromPos = fromPos;
        this.toPos = toPos;
        this.capturePiece = capturePiece;
        this.promotePiece = promotePiece;
        this.promotedTo = promotedTo;
        string hashCode = "";
        hashCode += fromPos;
        hashCode += toPos;
        hashCode += (int) Piece.pieceToChar[promotedTo];
        this.hashID = Int64.Parse(hashCode);
    }

    public static string EncodeMoveSAN(Move move, ChessState chessState)
    {
        string toReturn="";
        // Edge case king side castle
        if (move.pieceType == PieceType.King && (move.toPos - move.fromPos == 2))
        {
            return "0-0";
        }
        // Edge case queen side castle
        if (move.pieceType == PieceType.King && (move.toPos - move.fromPos == -2))
        {
            return "0-0-0";
        }
        if (move.pieceType != PieceType.Pawn)
        {
            toReturn += Piece.pieceToChar[move.pieceType];
        }

        bool disambiguateByFile = false;
        bool disambiguateByRank = false;

        // check if any disambiguations are required
        foreach (var piece in chessState.activePlayer.pieces)
        {
            // if not same type of piece, no need to worry
            if (piece.pieceType != move.pieceType)
            {
                continue;
            }
            
            // if same piece, dont worry
            if (piece.position == move.fromPos)
            {
                continue;
            }
            
            // get legal moves of other piece
            List<Move> legalMoves = piece.GetLegalMoves(chessState);
            
            foreach (var m in legalMoves)
            {
                // if can travel to same position, then disambiguation is required
                if (m.toPos == move.toPos)
                {
                    // if not in same file, disambiguate by file
                    if (piece.position % 8 != move.fromPos % 8)
                    {
                        disambiguateByFile = true;
                        continue;
                    }
                    // otherwise disambiguate by rank
                    disambiguateByRank = true;
                }
            }
        }

        if (move.capturePiece && move.pieceType == PieceType.Pawn)
        {
            disambiguateByFile = true;
        }

        if (disambiguateByFile)
        {
            toReturn += positionXToFile(move.fromPos); 
        }
        if (disambiguateByRank)
        {
            toReturn += positionYToRank(move.fromPos); 
        }

        // capture
        if (move.capturePiece)
        {
            toReturn += 'x';
        }

        // to position
        toReturn += positionToChessNotation(move.toPos);

        // promotion
        if (move.promotePiece)
        {
            toReturn += '=';
            toReturn += Piece.pieceToChar[move.promotedTo];
        }

        return toReturn;
        
    }

    // take a move in chess standard algebraic notation and convert to a move object
    // example string : "Ra4a5", "Ra4", "Rha2", "R3d3" b3b4, "c5", "0-0-0" , "h8=Q"
    // if no start position given, must be determined so the board state is required
    public static Move DecodeMoveSAN(string moveSAN, ChessState chessState)
    {
        // Edge case king side castle
        if (moveSAN.Equals("0-0") || moveSAN.Equals("O-O"))
        {
            if (chessState.activePlayer.playerType == PlayerType.White)
                return new Move(PieceType.King, 4, 6, false, false, PieceType.King);
            if (chessState.activePlayer.playerType == PlayerType.Black)
                return new Move(PieceType.King, 60, 62, false, false, PieceType.King);
        }
        // Edge case queen side castle
        if (moveSAN.Equals("0-0-0") || moveSAN.Equals("O-O-O"))
        {
            if (chessState.activePlayer.playerType == PlayerType.White)
                return new Move(PieceType.King, 4, 2, false, false, PieceType.King);
            if (chessState.activePlayer.playerType == PlayerType.Black)
                return new Move(PieceType.King, 60, 58, false, false, PieceType.King);
        }
        
        PieceType pieceType;
        int fromPos;
        int toPos;
        bool capturePiece;
        bool promotePiece;
        PieceType promotedTo;

        moveSAN.Replace("-", "");

        // encode piece 
        if (moveSAN[0] >= 'A' && moveSAN[0] <= 'Z')
        {
            pieceType = Piece.charToPiece[moveSAN[0]];
            // remove first char
            moveSAN.Remove(0, 1);
        }
        else
        {
            pieceType = PieceType.Pawn;
        }

        // check capture
        capturePiece  = moveSAN.Contains("x");
        moveSAN.Replace("x", "");

        // check promotion
        promotePiece = moveSAN.Contains("=");
        if (promotePiece)
        {
            promotedTo = Piece.charToPiece[moveSAN.Last()];
            moveSAN.Remove(moveSAN.Length-1, 2);
        }
        else
        {
            promotedTo = pieceType;
        }

        // decode positions

        // no original position information given, check all pieces and determine the move
        if (moveSAN.Length==2)
        {
            toPos = Move.chessNotationToPosition(moveSAN);
            fromPos = -1;
            foreach (var piece in chessState.activePlayer.pieces)
            {
                if (piece.pieceType != pieceType)
                {
                    continue;
                }
                
                foreach (var move in piece.GetLegalMoves(chessState))
                {
                    if (move.toPos == toPos)
                    {
                        fromPos = piece.position;
                        break;
                    }
                }
            }
            if (fromPos == -1)
            {
                // throw error
                throw new ArgumentException("could not find piece on board with requested legal move");
            }
        }

        // only one indicator given
        else if (moveSAN.Length==3)
        {
            toPos = Move.chessNotationToPosition(moveSAN.Substring(1,2));
            fromPos = -1;
            // given indicator is file
            if (moveSAN[0] >= 'a' && moveSAN[0] <= 'h')
            {
                int file = moveSAN[0] - 'a';
                
                foreach (var piece in chessState.activePlayer.pieces)
                {
                    if (piece.pieceType != pieceType)
                    {
                        continue;
                    }

                    if (piece.position % 8 != file)
                    {
                        continue;
                    }
                    foreach (var move in piece.GetLegalMoves(chessState))
                    {
                        if (move.toPos == toPos)
                        {
                            fromPos = piece.position;
                            break;
                        }
                    }
                }
                if (fromPos == -1)
                {
                    // throw error
                    throw new ArgumentException("could not find piece on board with requested legal move");
                }
            }
            
            // given indicator is rank
            else if (moveSAN[0] >= '1' && moveSAN[0] <= '8')
            {
                int rank = moveSAN[0] - '1';
                
                foreach (var piece in chessState.activePlayer.pieces)
                {
                    if (piece.pieceType != pieceType)
                    {
                        continue;
                    }

                    if (piece.position / 8 != rank)
                    {
                        continue;
                    }
                    foreach (var move in piece.GetLegalMoves(chessState))
                    {
                        if (move.toPos == toPos)
                        {
                            fromPos = piece.position;
                            break;
                        }
                    }
                }
                if (fromPos == -1)
                {
                    // throw error
                    throw new ArgumentException("could not find piece on board with requested legal move");
                }
            }
            
        }
        // full info given
        else if (moveSAN.Length==4)
        {
            fromPos = Move.chessNotationToPosition(moveSAN.Substring(0,2));
            toPos = Move.chessNotationToPosition(moveSAN.Substring(2,2));
        }
        else
        {
            // throw error
            throw new ArgumentException("illegal move notation!");
        }
        
        // check illegal promotion notation
        if (pieceType == PieceType.Pawn && (toPos >= 56 || toPos <= 7))
        {
            if (!promotePiece)
            {
                throw new ArgumentException("illegal move notation : cannot move pawn to end of board without specifying promotion piece");
            }
            
        }
        return new Move(pieceType, fromPos, toPos, capturePiece, promotePiece, promotedTo);
    }

    private static char positionXToFile(int position)
    {
        return (char) ('a' + position%8);
    }
    private static char positionYToRank(int position)
    {
        return (char) ('1' + position/8);
    }

    public static string positionToChessNotation(int position)
    {
        return "" + positionXToFile(position) + positionYToRank(position);
    }

    public static int chessNotationToPosition(string chessNotation)
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

        return (file-'a') + (rank-'1')*8;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Move))
        {
            return false;
        }
        Move other = (Move) obj;
        
        return (this.fromPos == other.fromPos) && (this.toPos == other.toPos) && (this.promotedTo == other.promotedTo);
    }

    public override int GetHashCode()
    {
        return (int) this.hashID;
    }
    
}