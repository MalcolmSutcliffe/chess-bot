using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessState{
    public ChessSquare[] boardState {get; private set;}
    public int[] previousMove {get; private set;}
    public int halfClock {get; private set;} // time since last capture or pawn move used for draws
    public int fullMoveCount {get; private set;}
    public Dictionary<string, int> stateHistory {get; private set;}
    public string gameMoves {get; private set;}
    
    public Player playerWhite {get; private set;}
    public Player playerBlack {get; private set;}

    public Player activePlayer {get; private set;}
    
    public ChessState() 
    {
        this.boardState = new ChessSquare[64];

        this.playerWhite = new Player(PlayerType.White);
        this.playerBlack = new Player(PlayerType.Black);

        this.activePlayer = this.playerWhite;

        this.halfClock = 0;
        this.fullMoveCount = 1;

        this.stateHistory = new Dictionary<string, int>();
        this.gameMoves = "";
        
        for (int x = 0; x < 64; x++)
        {
            this.boardState[x] = new ChessSquare();
        }
    }

    public ChessState(string fenChessState)
    {
        this.boardState = new ChessSquare[64];

        this.playerWhite = new Player(PlayerType.White);
        this.playerBlack = new Player(PlayerType.Black);

        this.stateHistory = new Dictionary<string, int>();
        this.gameMoves = "";

        for (int x = 0; x < 64; x++)
        {
            this.boardState[x] = new ChessSquare();
        }

        int curFile = 0;
        int curRank = 7;
        
        // Add pieces
        foreach(char c in fenChessState)
        {
            // decode board state
            if (c == ' ')
            {
                break;
            }
            if (c == '/')
            {
                curRank--;
                curFile = 0;
                continue;
            }
            
            if ('1' <= c && c <= '8')
            {
                // add blank squares
                int numBlanks = c - '0';
                curFile = curFile + numBlanks;
                continue;
            }
            
            // add piece
            PlayerType playerType = PlayerType.White;
            if ('a' <= c && c <= 'z')
            {
                playerType = PlayerType.Black;
            }
            PieceType pieceType = Piece.charToPiece[Char.ToUpper(c)];
            int position = curFile + curRank* 8;

            this.AddPiece(position, playerType, pieceType);
            curFile++;
        }

        fenChessState = fenChessState.Remove(0, fenChessState.IndexOf(' ')+ 1);

        // decode active colour
        if (fenChessState[0] == 'w')
        {
            this.activePlayer = playerWhite;
        }
        else if (fenChessState[0] == 'b')
        {
            this.activePlayer = playerBlack;
        }
        else
        {
            throw new ArgumentException("invalid FEN string - current player move");
        }

        fenChessState = fenChessState.Remove(0, fenChessState.IndexOf(' ')+ 1);

        // decode castling rights
        bool whiteKingCastle= false;
        bool whiteQueenCastle = false;
        bool blackKingCastle = false;
        bool blackQueenCastle = false;
        foreach (char c in fenChessState)
        {
            if (c == ' ' || c == '-')
            {
                break;
            }
            else if (c == 'K')
            {
                whiteKingCastle = true;
            }
            else if (c == 'Q')
            {
                whiteQueenCastle = true;
            }
            else if (c == 'k')
            {
                blackKingCastle = true;
            }
            else if (c == 'q')
            {
                whiteQueenCastle = true;
            }
            else
            {
                throw new ArgumentException("invalid FEN string - castle rights");
            }
        }
        this.playerWhite.king.kingCastlingRights = whiteKingCastle;
        this.playerWhite.king.queenCastlingRights = whiteQueenCastle;
        this.playerBlack.king.kingCastlingRights = blackKingCastle;
        this.playerBlack.king.queenCastlingRights = blackQueenCastle;
        

        fenChessState = fenChessState.Remove(0, fenChessState.IndexOf(' ')+ 1);

        // decode enPassant
        if (fenChessState[0] != '-')
        {
            int enPassantPosition = Move.chessNotationToPosition(fenChessState.Substring(0,2));
            
            if (enPassantPosition / 8 == 2)
            {
                this.previousMove = new int[] {enPassantPosition - 8, enPassantPosition + 8};
            }
            else if (enPassantPosition/ 8 == 5)
            {
                this.previousMove = new int[] {enPassantPosition + 8, enPassantPosition - 8};
            }
            else
            {
                throw new ArgumentException("invalid FEN string - en passant targets");
            }
        }

        fenChessState = fenChessState.Remove(0, fenChessState.IndexOf(' ')+ 1);

        this.halfClock = Int32.Parse(fenChessState.Substring(0, fenChessState.IndexOf(' ')));

        fenChessState = fenChessState.Remove(0, fenChessState.IndexOf(' ')+ 1);

        this.fullMoveCount = Int32.Parse(fenChessState);
    }

    
    public void MovePiece(Move move)
    {
        int fromPos = move.fromPos;
        int toPos = move.toPos;
        bool promotePiece = move.promotePiece;
        PieceType promotedTo = move.promotedTo;

        string moveSAN = Move.EncodeMoveSAN(move, this);

        // check that position has a piece
        if (!this.boardState[fromPos].containsPiece)
        {
            return;
        }

        Piece piece = this.boardState[fromPos].piece;

        halfClock++;

        if (move.capturePiece || piece.pieceType == PieceType.Pawn)
        {
            halfClock = 0;
        }

        // EDGE CASE: castle move (only move where 2 pieces are moved)
        if (this.boardState[fromPos].piece.pieceType == PieceType.King && Math.Abs(fromPos-toPos) == 2)
        {
            int rookFromPos;
            int rookToPos;

            // left castle
            if ((fromPos - toPos) > 0)
            {
                rookFromPos = fromPos-4;
                rookToPos = toPos + 1;
            }
            //right castle
            else
            {
                rookFromPos = fromPos+3;
                rookToPos = toPos-1;
            }

            Piece rookPiece = this.boardState[rookFromPos].piece;
            
            rookPiece.Move(rookToPos);
            this.boardState[rookFromPos].RemovePiece();
            this.boardState[rookToPos].SetPiece(rookPiece);
        }

        // EDGE CASE: en passant
        if (piece.pieceType == PieceType.Pawn && fromPos % 8 != toPos % 8 && !this.boardState[toPos].containsPiece)
        {
            CapturePiece(toPos % 8 + fromPos / 8);
        }

        // if position moving to has piece, remove it
        if (this.boardState[toPos].containsPiece)
        {
             CapturePiece(toPos);
        }

        // EDGE CASE: pawn promotion
        if (promotePiece)
        {
            CapturePiece(fromPos);
            AddPiece(toPos, activePlayer.playerType, promotedTo);
        }
        else
        {
            // move piece
            piece.Move(toPos);
            this.boardState[fromPos].RemovePiece();
            this.boardState[toPos].SetPiece(piece);
        }
        
        this.previousMove = new int[]{fromPos, toPos};

        UpdateMoveString(moveSAN);
        EndTurn();
    }

    public void VirtualMovePiece(Move move)
    {
        int fromPos = move.fromPos;
        int toPos = move.toPos;
        bool promotePiece = move.promotePiece;
        PieceType promotedTo = move.promotedTo;

        // check that position has a piece
        if (!this.boardState[fromPos].containsPiece)
        {
            return;
        }

        Piece piece = this.boardState[fromPos].piece;

        halfClock++;

        if (move.capturePiece || piece.pieceType == PieceType.Pawn)
        {
            halfClock = 0;
        }

        // EDGE CASE: castle move (only move where 2 pieces are moved)
        if (move.pieceType == PieceType.King && Math.Abs(fromPos-toPos) == 2)
        {
            int rookFromPos;
            int rookToPos;

            // left castle
            if ((fromPos - toPos) > 0)
            {
                rookFromPos = fromPos-4;
                rookToPos = toPos + 1;
            }
            //right castle
            else
            {
                rookFromPos = fromPos+3;
                rookToPos = toPos-1;
            }

            Piece rookPiece = this.boardState[rookFromPos].piece;
            
            rookPiece.Move(rookToPos);
            this.boardState[rookFromPos].RemovePiece();
            this.boardState[rookToPos].SetPiece(rookPiece);
        }

        // EDGE CASE: en passant
        if (piece.pieceType == PieceType.Pawn && fromPos % 8 != toPos % 8 && !this.boardState[toPos].containsPiece)
        {
            CapturePiece(toPos % 8 + fromPos / 8);
        }

        // if position moving to has piece, remove it
        if (this.boardState[toPos].containsPiece)
        {
             CapturePiece(toPos);
        }

        // EDGE CASE: pawn promotion
        if (promotePiece)
        {
            CapturePiece(fromPos);
            AddPiece(toPos, activePlayer.playerType, promotedTo);
        }
        else
        {
            // move piece
            piece.Move(toPos);
            this.boardState[fromPos].RemovePiece();
            this.boardState[toPos].SetPiece(piece);
        }
        
        this.previousMove = new int[]{fromPos, toPos};

        EndTurn();
    }

    public void CapturePiece(int position)
    {
        if (!this.boardState[position].containsPiece)
        {
            return;
        }
        Piece piece = this.boardState[position].piece;
        if (piece.playerType == PlayerType.White)
        {
            playerWhite.RemovePiece(this.boardState[position].piece);
        }
        if (piece.playerType == PlayerType.Black)
        {
            playerBlack.RemovePiece(this.boardState[position].piece);
        }
        this.boardState[position].RemovePiece();
    }

    public int GetKingPosition(PlayerType playerType)
    {
        if (playerType == PlayerType.White)
        {
            return playerWhite.king.position;
        }
        if (playerType == PlayerType.Black)
        {
            return playerBlack.king.position;
        }
        return -1;
    }

    public bool IsKingInCheck(PlayerType playerType, int kingPosition = -1)
    {
        if (kingPosition == -1)
        {
            if (playerType == PlayerType.White)
            {
                kingPosition = playerWhite.king.position;
            }
            if (playerType == PlayerType.Black)
            {
                kingPosition = playerBlack.king.position;
            }
        }

        // use slider approach to avoid deep recursion

        // check pawns
        int direction = 1;
        if (playerType == PlayerType.Black)
        {
            direction = -1;
        }
        if (IsInBoard(kingPosition %8 -1, kingPosition/ 8 + direction))
        {
            int pawnPos = kingPosition + 8*direction-1;
            if (boardState[pawnPos].containsPiece && boardState[pawnPos].piece.playerType != playerType && boardState[pawnPos].piece.pieceType == PieceType.Pawn)
            {
                return true;
            }
        }
        if (IsInBoard(kingPosition %8 +1, kingPosition/ 8 + direction))
        {
            int pawnPos = kingPosition +8*direction+1;
            if (boardState[pawnPos].containsPiece && boardState[pawnPos].piece.playerType != playerType && boardState[pawnPos].piece.pieceType == PieceType.Pawn)
            {
                return true;
            }
        }

        // check knights
        foreach(var move in Knight.KNIGHT_MOVE_DIRECTIONS)
        {
            if (!IsInBoard(kingPosition % 8 + move[0], kingPosition /8 + move[1])){
                continue;
            }
            
            int nPos = kingPosition + move[0] + move[1]*8;
            
            if(boardState[nPos].containsPiece && boardState[nPos].piece.playerType != playerType && boardState[nPos].piece.pieceType == PieceType.Knight)
            {
                return true;
            }
        }

        // check king for redundancy
        foreach(var move in King.KING_MOVE_DIRECTIONS)
        {
            if (!IsInBoard(kingPosition % 8 + move[0], kingPosition /8 + move[1])){
                continue;
            }
            
            int kPos = kingPosition + move[0] + move[1]*8;
            
            if(boardState[kPos].containsPiece && boardState[kPos].piece.playerType != playerType && boardState[kPos].piece.pieceType == PieceType.King)
            {
                return true;
            }
        }

        // check rook-sliding pieces
        foreach(var move in Rook.ROOK_MOVE_DIRECTIONS)
        {
            int pos = kingPosition;
            
            while(IsInBoard(pos % 8 + move[0], pos/8 + move[1]))
            {
                pos = pos + move[0] + move[1]*8;
                
                if (boardState[pos].containsPiece)
                {
                    if (boardState[pos].piece.playerType == playerType)
                    {
                        break;
                    }
                    if (boardState[pos].piece.playerType != playerType && (boardState[pos].piece.pieceType == PieceType.Rook || boardState[pos].piece.pieceType == PieceType.Queen))
                    {
                        return true;
                    }
                    break;
                }
            
            }
        }

        // check bishop-sliding pieces
        foreach(var move in Bishop.BISHOP_MOVE_DIRECTIONS)
        {
            int pos = kingPosition;
            
            while(IsInBoard(pos % 8 + move[0], pos/8 + move[1]))
            {
                pos = pos + move[0] + move[1]*8;
                if (boardState[pos].containsPiece)
                {
                    if (boardState[pos].piece.playerType == playerType)
                    {
                        break;
                    }
                    else if (boardState[pos].piece.pieceType == PieceType.Bishop || boardState[pos].piece.pieceType == PieceType.Queen)
                    {
                        Debug.Log("h");
                        return true;
                    }
                    break;
                }
            
            }
        }

        return false;
    }

    public void UpdateMoveString(string moveSAN)
    {
        if (activePlayer.playerType == PlayerType.White)
        {
            this.gameMoves += this.fullMoveCount;
            this.gameMoves += '.';
            this.gameMoves += moveSAN;
            this.gameMoves += " ";
        }
        else if (activePlayer.playerType == PlayerType.Black)
        {
            this.gameMoves += moveSAN;
            this.gameMoves += " ";
        }
    }
    public void EndTurn()
    {
        if (activePlayer.playerType == PlayerType.White)
        {
            this.activePlayer = this.playerBlack;
        }
        else if (activePlayer.playerType == PlayerType.Black)
        {
            this.activePlayer = this.playerWhite;
            this.fullMoveCount++;
        }
    }


    // returns:
    // 0 if game is not over
    // 1 if white wins
    // 2 if black wins
    // 3 if draw by stalemate
    // 4 if draw by insufficient material
    // 5 if draw by move limit
    // 6 if draw by 3-fold repitition
    public int CheckEndGame()
    {
        // check draw by insufficient material
        if (!CheckSufficientMaterial())
        {
            return 4;
        }

        // check draw by move limit
        if (halfClock >= 100)
        {
            return 5;
        }

        // check draw by 3-fold repitition
        string currentState = BoardEncoder.EncodeBoardState(this.boardState);
        AddBoardStateToHistory(currentState);
        if (stateHistory[currentState] >= 3)
        {
            return 6;
        }
           
        // check for legal moves
        if (GetLegalMoves().Count > 0)
        {
            return 0;
        }
        
        if (IsKingInCheck(activePlayer.playerType))
        {
            if (activePlayer.playerType == PlayerType.Black)
                return 1;
            if (activePlayer.playerType == PlayerType.White)
                return 2;
        }
        return 3;
    }

    public List<Move> GetLegalMoves()
    {
        List<Move> legalMoves = new List<Move>();

        foreach(var piece in activePlayer.pieces)
        {
            legalMoves.AddRange(piece.GetLegalMoves(this));
        }

        return legalMoves;
    }

    public bool CheckSufficientMaterial()
    {
        if (playerWhite.pieces.Count >= 3 || playerBlack.pieces.Count >= 3)
        {
            return true;
        }
        foreach(var piece in playerWhite.pieces)
        {
            if (piece.pieceType == PieceType.Pawn || piece.pieceType == PieceType.Rook || piece.pieceType == PieceType.Queen)
            {
                return true;
            }
        }
        foreach(var piece in playerBlack.pieces)
        {
            if (piece.pieceType == PieceType.Pawn || piece.pieceType == PieceType.Rook || piece.pieceType == PieceType.Queen)
            {
                return true;
            }
        }
        return false;
    }

    private void AddBoardStateToHistory(string state)
    {
        if (stateHistory.ContainsKey(state))
        {
            stateHistory[state] = stateHistory[state] + 1;
        } else
        {
            stateHistory[state] = 1;
        }
    }

    public void AddPiece(int position, PlayerType playerType, PieceType pieceType)
    {
        Piece newPiece;
        switch (pieceType)
        {
            case PieceType.Pawn:
                newPiece = new Pawn(position, playerType, pieceType);
                break;
            case PieceType.Rook:
                newPiece = new Rook(position, playerType, pieceType);
                break;
            case PieceType.Knight:
                newPiece = new Knight(position, playerType, pieceType);
                break;
            case PieceType.Bishop:
                newPiece = new Bishop(position, playerType, pieceType);
                break;
            case PieceType.Queen:
                newPiece = new Queen(position, playerType, pieceType);
                break;
            case PieceType.King:
                newPiece = new King(position, playerType, pieceType);
                break;
            default:
                newPiece = new Pawn(position, playerType, pieceType);
                break;
        }
        
        this.boardState[position].SetPiece(newPiece);
        
        if (playerType == PlayerType.White)
        {
            playerWhite.AddPiece(newPiece);
        }

        if (playerType == PlayerType.Black)
        {
            playerBlack.AddPiece(newPiece);
        }
    }

    public bool IsInBoard(int position)
    {
        return position >= 0 && position < 64;
    }

    public bool IsInBoard(int positionX, int positionY)
    {
        return positionX >= 0 && positionX < 8 && positionY >= 0 && positionY < 8;
    }

    public static ChessState DeepCopy(ChessState chessState)
    {
        ChessState newChessState = new ChessState();
        newChessState.gameMoves = chessState.gameMoves;
        for (int x = 0; x < 64; x++)
        {
            newChessState.boardState[x] = new ChessSquare();
            if (!chessState.boardState[x].containsPiece)
            {
                continue;
            }
            newChessState.AddPiece(x, chessState.boardState[x].piece.playerType, chessState.boardState[x].piece.pieceType);
        }
        
        if (chessState.activePlayer.playerType == PlayerType.White)
        {
            newChessState.activePlayer = newChessState.playerWhite;
        } 
        else
        {
            newChessState.activePlayer = newChessState.playerBlack; 
        }
        newChessState.playerWhite.king.kingCastlingRights = chessState.playerWhite.king.kingCastlingRights;
        newChessState.playerWhite.king.queenCastlingRights = chessState.playerWhite.king.queenCastlingRights;
        newChessState.playerBlack.king.kingCastlingRights = chessState.playerBlack.king.kingCastlingRights;
        newChessState.playerBlack.king.queenCastlingRights = chessState.playerBlack.king.queenCastlingRights;
        return newChessState;
    }
}