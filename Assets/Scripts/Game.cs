using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    bool isGameActive;
    
    public Board board;
    public ChessState chessState;
    public int size = 8;
    private PieceManager pieceManager;
    private MoveSelector moveSelector;

    public GameObject isInCheckPrefab;
    private GameObject isInCheck;
    
    public GameObject previousMoveHighlightPrefab;
    private GameObject[] previousMoveHighlights;
    
    private void Awake()
    {
        moveSelector = GetComponentInChildren<MoveSelector>();
        pieceManager = GetComponentInChildren<PieceManager>();
    }

    private void Start()
    {
        Camera.main.transform.position = new Vector3(size / 2f, size/ 2f, -10);
        EventManager.instance.MoveOccured += MoveOccured;
        previousMoveHighlights = new GameObject[2];
        board.DrawGrid(size);
        chessState = new ChessState(size);

        InitializeGame();
    }

    void Update()
    {
        if (isGameActive)
            moveSelector.CheckUserInput(chessState, chessState.activePlayer.playerType);
    }

    private void InitializeGame(){
        isGameActive = true;

        // set board to default value
        chessState.AddPiece(new int[] {0, 0}, PlayerType.White, PieceType.Rook);
        chessState.AddPiece(new int[] {7, 0}, PlayerType.White, PieceType.Rook);
        chessState.AddPiece(new int[] {1, 0}, PlayerType.White, PieceType.Knight);
        chessState.AddPiece(new int[] {6, 0}, PlayerType.White, PieceType.Knight);
        chessState.AddPiece(new int[] {2, 0}, PlayerType.White, PieceType.Bishop);
        chessState.AddPiece(new int[] {5, 0}, PlayerType.White, PieceType.Bishop);
        chessState.AddPiece(new int[] {3, 0}, PlayerType.White, PieceType.Queen);
        chessState.AddPiece(new int[] {4, 0}, PlayerType.White, PieceType.King);
        for (int y = 0; y < size; y++)
            chessState.AddPiece(new int[] {y, 1}, PlayerType.White, PieceType.Pawn);
        
        chessState.AddPiece(new int[] {0, 7}, PlayerType.Black, PieceType.Rook);
        chessState.AddPiece(new int[] {7, 7}, PlayerType.Black, PieceType.Rook);
        chessState.AddPiece(new int[] {1, 7}, PlayerType.Black, PieceType.Knight);
        chessState.AddPiece(new int[] {6, 7}, PlayerType.Black, PieceType.Knight);
        chessState.AddPiece(new int[] {2, 7}, PlayerType.Black, PieceType.Bishop);
        chessState.AddPiece(new int[] {5, 7}, PlayerType.Black, PieceType.Bishop);
        chessState.AddPiece(new int[] {3, 7}, PlayerType.Black, PieceType.Queen);
        chessState.AddPiece(new int[] {4, 7}, PlayerType.Black, PieceType.King);
        for (int y = 0; y < size; y++)
            chessState.AddPiece(new int[] {y, 6}, PlayerType.Black, PieceType.Pawn);
        
        DrawBoard();
    }

    public void MoveOccured(Move move)
    {

        print(Move.EncodeMoveSAN(move, chessState));
        chessState.MovePiece(move);
        print(ChessState.EncodeChessStateFEN(chessState));

        DrawBoard();
        
        int moveOutcome = chessState.CheckEndGame();
        
        switch (moveOutcome)
        {
            case 0:
                break;
            case 1:
                print("white wins by checkmate!");
                isGameActive = false;
                break;
            case 2:
                print("black wins by checkmate!");
                isGameActive = false;
                break;
            case 3:
                print("draw by stalemate!");
                isGameActive = false;
                break;
            case 4:
                print("draw by insufficeint material");
                isGameActive = false;
                break;
            case 5:
                print("draw by move limit");
                isGameActive = false;
                break;
            case 6:
                print("draw by 3-fold repitition");
                isGameActive = false;
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

}
