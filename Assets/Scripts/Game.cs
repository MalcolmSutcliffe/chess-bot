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

    // prefabs
    public GameObject legalMovePrefab;

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
        HighlightPostions(playerWhite.pieces[7].GetLegalMoves(this.gameLayout));
        // HighlightPostions(playerWhite.pieces[3].GetPossibleMoves(this.gameLayout));
    }

    private void InitializeGame(){
        playerWhite = new Player(PlayerType.White);
        playerBlack = new Player(PlayerType.Black);

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

    private void HighlightPostions(List<Vector3Int> positions)
    {
        foreach(var pos in positions)
        {
            var newObject = Instantiate(legalMovePrefab, gameObject.transform);
            newObject.transform.position = pos;
        }
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
