using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType{
    White,
    Black
}

public class Game : MonoBehaviour
{
    public static Game instance {get; private set;}
    public Board board;
    public GameLayout gameLayout;
    public int size = 8;
    private UserInterface userInterface;
    private PieceManager pieceManager;

    public PlayerType playerMove;

    public GameObject isInCheckPrefab;
    public GameObject isInCheck;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        userInterface = GetComponentInChildren<UserInterface>();
        pieceManager = GetComponentInChildren<PieceManager>();
    }

    private void Start()
    {
        Camera.main.transform.position = new Vector3(size / 2f, size/ 2f, -10);
        EventManager.instance.MoveOccured += Move;
        board.DrawGrid(size);
        gameLayout = new GameLayout(size);
        
        InitializeGame();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            userInterface.OnClicked(gameLayout, playerMove);
        }
    }

    private void InitializeGame(){
        
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
        
        DrawBoard();
    }

    public void Move(Vector3Int fromPos, Vector3Int toPos)
    {
        gameLayout.MovePiece(fromPos, toPos);
        EndTurn();
        DrawBoard();
        
    }

    public void DrawBoard()
    {
        pieceManager.ClearBoard();
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (gameLayout.state[x,y].containsPiece)
                {
                    pieceManager.DrawPiece(gameLayout.state[x,y].piece, new Vector3Int(x, y, 0));
                }
            }
        }
        
        DisplayCheck();
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
            if (playerMove == PlayerType.White)
                print("black wins by checkmate!");
            if (playerMove == PlayerType.Black)
                print("white wins by checkmate!");
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
        gameLayout.AddPiece(position, newPiece);
    }

}
