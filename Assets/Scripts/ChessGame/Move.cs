using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Move {
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

    public static string EncodeMoveSAN(Move move, ChessState chessState)
    {
        string toReturn="";
        // Edge case king side castle
        if (move.pieceType == PieceType.King && (move.toPos[0] - move.fromPos[0] == 2))
        {
            return "0-0";
        }
        // Edge case queen side castle
        if (move.pieceType == PieceType.King && (move.toPos[0] - move.fromPos[0] == -2))
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
            if (piece.position[0] == move.fromPos[0] && piece.position[1] == move.fromPos[1])
            {
                continue;
            }
            
            // get legal moves of other piece
            List<Move> legalMoves = piece.GetLegalMoves(chessState);
            
            foreach (var m in legalMoves)
            {
                // if can travel to same position, then disambiguation is required
                if (m.toPos[0] == move.toPos[0] && m.toPos[1] == move.toPos[1])
                {
                    // if not in same file, disambiguate by file
                    if (piece.position[0] != move.fromPos[0])
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
            toReturn += positionXToFile(move.fromPos[0]); 
        }
        if (disambiguateByRank)
        {
            toReturn += positionYToRank(move.fromPos[1]); 
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
                return new Move(PieceType.King, new int[] {4, 0}, new int[] {6, 0}, false, false, PieceType.King);
            if (chessState.activePlayer.playerType == PlayerType.Black)
                return new Move(PieceType.King, new int[] {4, 7}, new int[] {6, 7}, false, false, PieceType.King);
        }
        // Edge case queen side castle
        if (moveSAN.Equals("0-0-0") || moveSAN.Equals("O-O-O"))
        {
            if (chessState.activePlayer.playerType == PlayerType.White)
                return new Move(PieceType.King, new int[] {4, 0}, new int[] {2, 0}, false, false, PieceType.King);
            if (chessState.activePlayer.playerType == PlayerType.Black)
                return new Move(PieceType.King, new int[] {4, 7}, new int[] {2, 7}, false, false, PieceType.King);
        }
        
        PieceType pieceType;
        int[] fromPos;
        int[] toPos;
        bool capturePiece;
        bool promotePiece;
        PieceType promotedTo;

        moveSAN.Replace("-", "");

        // encode piece 
        if (moveSAN[0] >= 65 && moveSAN[0] <= 90)
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
            fromPos = new int[] {-1, -1};
            foreach (var piece in chessState.activePlayer.pieces)
            {
                if (piece.pieceType != pieceType)
                {
                    continue;
                }
                
                foreach (var move in piece.GetLegalMoves(chessState))
                {
                    if (move.toPos[0] == toPos[0] && move.toPos[1] == toPos[1])
                    {
                        fromPos = piece.position;
                        break;
                    }
                }
            }
            if (fromPos[0] == -1)
            {
                // throw error
                throw new ArgumentException("could not find piece on board with requested legal move");
            }
        }

        // only one indicator given
        else if (moveSAN.Length==3)
        {
            toPos = Move.chessNotationToPosition(moveSAN.Substring(1,2));
            fromPos = new int[] {-1, -1};
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

                    if (piece.position[0] != file)
                    {
                        continue;
                    }
                    foreach (var move in piece.GetLegalMoves(chessState))
                    {
                        if (move.toPos[0] == toPos[0] && move.toPos[1] == toPos[1])
                        {
                            fromPos = piece.position;
                            break;
                        }
                    }
                }
                if (fromPos[0] == -1)
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

                    if (piece.position[1] != rank)
                    {
                        continue;
                    }
                    foreach (var move in piece.GetLegalMoves(chessState))
                    {
                        if (move.toPos[0] == toPos[0] && move.toPos[1] == toPos[1])
                        {
                            fromPos = piece.position;
                            break;
                        }
                    }
                }
                if (fromPos[0] == -1)
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
        if (pieceType == PieceType.Pawn && (toPos[1] == 7 || toPos[1] == 0))
        {
            if (!promotePiece)
            {
                throw new ArgumentException("illegal move notation : cannot move pawn to end of board without specifying promotion piece");
            }
            
        }
        return new Move(pieceType, fromPos, toPos, capturePiece, promotePiece, promotedTo);
    }

    public static string positionToChessNotation(int[] position)
    {
        return "" + positionXToFile(position[0]) + positionYToRank(position[1]);
    }

    private static char positionXToFile(int x)
    {
        if (x < 0 || x > 7)
        {
            throw new ArgumentException("position out of bounds");
        }
        return (char) ('a' + x);
    }
    private static char positionYToRank(int y)
    {
        if (y < 0 || y > 7)
        {
            throw new ArgumentException("position out of bounds");
        }
        return (char) ('1' + y);
    }

    private static int[] chessNotationToPosition(string chessNotation)
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