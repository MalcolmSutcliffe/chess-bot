using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game instance {get; private set;}
    public Board board;
    public ChessState chessState;
    public int size = 8;
    private UserInterface userInterface;
    private PieceManager pieceManager;

    public GameObject isInCheckPrefab;
    private GameObject isInCheck;
    
    public GameObject previousMoveHighlightPrefab;
    private GameObject[] previousMoveHighlights;

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
        previousMoveHighlights = new GameObject[2];
        board.DrawGrid(size);
        chessState = new ChessState(size);
        
        InitializeGame();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            userInterface.SelectOnBoard(chessState, chessState.activePlayer.playerType);
        }
    }

    private void InitializeGame(){
        // set board to default value
        InitPiece(new int[] {0, 0}, PlayerType.White, PieceType.Rook);
        InitPiece(new int[] {7, 0}, PlayerType.White, PieceType.Rook);
        InitPiece(new int[] {1, 0}, PlayerType.White, PieceType.Knight);
        InitPiece(new int[] {6, 0}, PlayerType.White, PieceType.Knight);
        InitPiece(new int[] {2, 0}, PlayerType.White, PieceType.Bishop);
        InitPiece(new int[] {5, 0}, PlayerType.White, PieceType.Bishop);
        InitPiece(new int[] {3, 0}, PlayerType.White, PieceType.Queen);
        InitPiece(new int[] {4, 0}, PlayerType.White, PieceType.King);
        for (int y = 0; y < size; y++)
            InitPiece(new int[] {y, 1}, PlayerType.White, PieceType.Pawn);
        
        InitPiece(new int[] {0, 7}, PlayerType.Black, PieceType.Rook);
        InitPiece(new int[] {7, 7}, PlayerType.Black, PieceType.Rook);
        InitPiece(new int[] {1, 7}, PlayerType.Black, PieceType.Knight);
        InitPiece(new int[] {6, 7}, PlayerType.Black, PieceType.Knight);
        InitPiece(new int[] {2, 7}, PlayerType.Black, PieceType.Bishop);
        InitPiece(new int[] {5, 7}, PlayerType.Black, PieceType.Bishop);
        InitPiece(new int[] {3, 7}, PlayerType.Black, PieceType.Queen);
        InitPiece(new int[] {4, 7}, PlayerType.Black, PieceType.King);
        for (int y = 0; y < size; y++)
            InitPiece(new int[] {y, 6}, PlayerType.Black, PieceType.Pawn);
        
        DrawBoard();
    }

    public void Move(int[] fromPos, int[] toPos)
    {
        if (!chessState.boardState[fromPos[0], fromPos[1]].containsPiece)
        {
            return;
        }
        
        // check for pawn promotion
        PieceType promotedTo = PieceType.Queen;
        
        if (chessState.boardState[fromPos[0], fromPos[1]].piece.pieceType == PieceType.Pawn)
        {
            // white promoted
            if (chessState.boardState[fromPos[0], fromPos[1]].piece.playerType == PlayerType.White && toPos[1] == 7)
            {
                promotedTo = userInterface.GetPromotionPiece(PlayerType.White);
            }
            // black promoted
            if (chessState.boardState[fromPos[0], fromPos[1]].piece.playerType == PlayerType.Black && toPos[1] == 0)
            {
                promotedTo = userInterface.GetPromotionPiece(PlayerType.Black);
            }
        }
        
        chessState.MovePiece(fromPos, toPos, promotedTo);
        
        DrawBoard();
        
        int moveOutcome = chessState.CheckEndGame();
        
        switch (moveOutcome)
        {
            case 0:
                break;
            case 1:
                print("white wins by checkmate!");
                break;
            case 2:
                print("black wins by checkmate!");
                break;
            case 3:
                print("draw by stalemate!");
                break;
        }
    }

    public void DrawBoard()
    {
        pieceManager.ClearBoard();
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (chessState.boardState[x,y].containsPiece)
                {
                    pieceManager.DrawPiece(chessState.boardState[x,y].piece, new Vector3Int(x, y, 0));
                }
            }
        }
        DrawPreviousMove();
        DisplayCheck();
    }

    public void DrawPreviousMove()
    {
        if (chessState.previousMove==null)
        {
            return;
        }
        Destroy(previousMoveHighlights[0]);
        Destroy(previousMoveHighlights[1]);
        previousMoveHighlights[0] = Instantiate(previousMoveHighlightPrefab, gameObject.transform);
        previousMoveHighlights[1] = Instantiate(previousMoveHighlightPrefab, gameObject.transform);
        previousMoveHighlights[0].transform.position = new Vector3Int(chessState.previousMove[0][0], chessState.previousMove[0][1], 0);
        previousMoveHighlights[1].transform.position = new Vector3Int(chessState.previousMove[1][0], chessState.previousMove[1][1], 0);
    }
    
    public void DisplayCheck()
    {
        Destroy(isInCheck);
        int[] kingPosition = chessState.GetKingPosition(chessState.activePlayer.playerType);
        if (chessState.IsKingInCheck(chessState.activePlayer.playerType))
        {
            isInCheck = Instantiate(isInCheckPrefab, gameObject.transform);
            isInCheck.transform.position = new Vector3Int(kingPosition[0], kingPosition[1], 0);
        }
    }

    private void InitPiece(int[] position, PlayerType playerType, PieceType pieceType)
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
        chessState.AddPiece(position, newPiece);
    }

}
