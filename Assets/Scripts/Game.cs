using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game instance {get; private set;}
    public Board board;
    public GameLayout gameLayout;
    public int size = 8;

    public Player playerWhite {get; private set;}
    public Player playerBlack {get; private set;}

    public PlayerType playerMove;

    private GameObject isInCheck;
    public GameObject isInCheckPrefab;

    // piece prefabs
    public GameObject whitePawnPrefab;
    public GameObject whiteRookPrefab;
    public GameObject whiteKnightPrefab;
    public GameObject whiteBishopPrefab;
    public GameObject whiteQueenPrefab;
    public GameObject whiteKingPrefab;
    
    public GameObject blackPawnPrefab;
    public GameObject blackRookPrefab;
    public GameObject blackKnightPrefab;
    public GameObject blackBishopPrefab;
    public GameObject blackQueenPrefab;
    public GameObject blackKingPrefab;

    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        Camera.main.transform.position = new Vector3(size / 2f, size/ 2f, -10);
        board.DrawGrid(size);
        gameLayout = new GameLayout(size);
        InitializeGame();
    }

    private void InitializeGame(){
        playerWhite = new Player(PlayerType.White);
        playerBlack = new Player(PlayerType.Black);

        playerMove = PlayerType.White;

        // set board to default value
        InitPiece(new Vector3Int(0, 0), PlayerType.White, PieceType.Rook);
        InitPiece(new Vector3Int(7, 0), PlayerType.White, PieceType.Rook);
        InitPiece(new Vector3Int(1, 0), PlayerType.White, PieceType.Knight);
        InitPiece(new Vector3Int(6, 0), PlayerType.White, PieceType.Knight);
        InitPiece(new Vector3Int(2, 0), PlayerType.White, PieceType.Bishop);
        InitPiece(new Vector3Int(5, 0), PlayerType.White, PieceType.Bishop);
        InitPiece(new Vector3Int(3, 0), PlayerType.White, PieceType.Queen);
        InitPiece(new Vector3Int(4, 0), PlayerType.White, PieceType.King);
        for (int y = 0; y < size; y++)
            InitPiece(new Vector3Int(y, 1), PlayerType.White, PieceType.Pawn);
        
        InitPiece(new Vector3Int(0, 7), PlayerType.Black, PieceType.Rook);
        InitPiece(new Vector3Int(7, 7), PlayerType.Black, PieceType.Rook);
        InitPiece(new Vector3Int(1, 7), PlayerType.Black, PieceType.Knight);
        InitPiece(new Vector3Int(6, 7), PlayerType.Black, PieceType.Knight);
        InitPiece(new Vector3Int(2, 7), PlayerType.Black, PieceType.Bishop);
        InitPiece(new Vector3Int(5, 7), PlayerType.Black, PieceType.Bishop);
        InitPiece(new Vector3Int(3, 7), PlayerType.Black, PieceType.Queen);
        InitPiece(new Vector3Int(4, 7), PlayerType.Black, PieceType.King);
        for (int y = 0; y < size; y++)
            InitPiece(new Vector3Int(y, 6), PlayerType.Black, PieceType.Pawn);
    }

    public void Move(Vector3Int fromPos, Vector3Int toPos)
    {
        if (!gameLayout.state[fromPos.x, fromPos.y].containsPiece)
        {
            return;
        }
        EventManager.instance.OnMoveOccured();
        Piece piece = gameLayout.state[fromPos.x, fromPos.y].piece;
        if (gameLayout.state[toPos.x, toPos.y].containsPiece)
        {
            Destroy(gameLayout.state[toPos.x, toPos.y].piece.gameObject);
        }
        
        // check edge case if castle move (only move where 2 pieces are moved)
        if (piece.pieceType == PieceType.King && Vector3.Distance(fromPos, toPos) >= 2)
        {
            Vector3Int rookFromPos;
            Vector3Int rookToPos;

            // left castle
            if ((fromPos - toPos).x > 0)
            {
                rookFromPos = new Vector3Int(0, fromPos.y, 0);
                rookToPos = new Vector3Int(3, fromPos.y, 0);
            }
            //right castle
            else
            {
                rookFromPos = new Vector3Int(7, fromPos.y, 0);
                rookToPos = new Vector3Int(5, fromPos.y, 0);
            }
            gameLayout.state[rookFromPos.x, rookFromPos.y].piece.Move(rookToPos);
            gameLayout.MovePiece(rookFromPos, rookToPos);
        }

        // en passant
        if (piece.pieceType == PieceType.Pawn && fromPos.x != toPos.x && !gameLayout.state[toPos.x, toPos.y].containsPiece)
        {
            Destroy(gameLayout.state[toPos.x, fromPos.y].piece.gameObject);
            gameLayout.state[toPos.x, fromPos.y].RemovePiece();
        }

        // pawn promotion
        // TODO : dont auto promote to queen
        if (piece.pieceType == PieceType.Pawn && (toPos.y == 7  || toPos.y == 0))
        {
            Destroy(piece.gameObject);
            gameLayout.state[toPos.x, toPos.y].RemovePiece();
            gameLayout.state[fromPos.x, fromPos.y].RemovePiece();
            if (toPos.y == 7)
            {
                InitPiece(new Vector3Int(toPos.x, toPos.y), PlayerType.White, PieceType.Queen);
                EndTurn();
                return;
            }
            if (toPos.y == 0)
            {
                InitPiece(new Vector3Int(toPos.x, toPos.y), PlayerType.Black, PieceType.Queen);
                EndTurn();
                return;
            }
        }
        gameLayout.MovePiece(fromPos, toPos);
        piece.Move(toPos);
        EndTurn();
    }

    public void EndTurn()
    {
        if (playerMove == PlayerType.White)
        {
            playerMove = PlayerType.Black;
        }
        else if (playerMove == PlayerType.Black)
        {
            playerMove = PlayerType.White;
        }
        DisplayCheck();
        CheckEndGame();
    }
    
    public void DisplayCheck()
    {
        Destroy(isInCheck);
        Vector3Int kingPosition = gameLayout.GetKingPosition(playerMove);
        if (gameLayout.IsKingInCheck(playerMove, kingPosition))
        {
            isInCheck = Instantiate(isInCheckPrefab, gameObject.transform);
            isInCheck.transform.position = kingPosition;
        }
    }

    public bool CheckEndGame()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (gameLayout.state[x,y].containsPiece && gameLayout.state[x,y].piece.playerType == playerMove)
                {
                    if (gameLayout.state[x,y].piece.GetLegalMoves(gameLayout).Count > 0)
                    {
                        return false;
                    }
                }
            }
        }
        if (gameLayout.IsKingInCheck(playerMove, gameLayout.GetKingPosition(playerMove)))
        {
            print(playerMove.ToString() + " wins by checkmate!");
            return true;
        }
        print("stalemate!");
        return true;

        // check if draw by insufficient material

        // check if draw by move limit

        // check if draw by 3-fold repitition
    }

    private void InitPiece(Vector3Int position, PlayerType playerType, PieceType pieceType)
    {
        Piece newPiece;
        GameObject prefab = GetPrefab(playerType, pieceType);
        var newObject = Instantiate(prefab, gameObject.transform);
        switch (pieceType)
        {
            case PieceType.Pawn:
                newPiece = new Pawn(position, playerType, pieceType, newObject);
                break;
            case PieceType.Rook:
                newPiece = new Rook(position, playerType, pieceType, newObject);
                break;
            case PieceType.Knight:
                newPiece = new Knight(position, playerType, pieceType, newObject);
                break;
            case PieceType.Bishop:
                newPiece = new Bishop(position, playerType, pieceType, newObject);
                break;
            case PieceType.Queen:
                newPiece = new Queen(position, playerType, pieceType, newObject);
                break;
            case PieceType.King:
                newPiece = new King(position, playerType, pieceType, newObject);
                break;
            default:
                newPiece = new Pawn(position, playerType, pieceType, newObject);
                break;
        }
        gameLayout.AddPiece(position, newPiece);
        if (playerType == PlayerType.White)
        {
            playerWhite.AddPiece(newPiece);
        }
        else if (playerType == PlayerType.Black)
        {
            playerBlack.AddPiece(newPiece);
        }
    }

    public GameObject GetPrefab(PlayerType playerType, PieceType piece)
    {
        if (playerType == PlayerType.White)
        {
            switch (piece)
            {
                case PieceType.Pawn:
                    return whitePawnPrefab;
                case PieceType.Rook:
                    return whiteRookPrefab;
                case PieceType.Knight:
                    return whiteKnightPrefab;
                case PieceType.Bishop:
                    return whiteBishopPrefab;
                case PieceType.Queen:
                    return whiteQueenPrefab;
                case PieceType.King:
                    return whiteKingPrefab;
                default:
                    return whitePawnPrefab;
            }
        }
        if (playerType == PlayerType.Black)
        {
            switch (piece)
            {
                case PieceType.Pawn:
                    return blackPawnPrefab;
                case PieceType.Rook:
                    return blackRookPrefab;
                case PieceType.Knight:
                    return blackKnightPrefab;
                case PieceType.Bishop:
                    return blackBishopPrefab;
                case PieceType.Queen:
                    return blackQueenPrefab;
                case PieceType.King:
                    return blackKingPrefab;
                default:
                    return blackPawnPrefab;
            }
        }

        return whitePawnPrefab;
    }

}
