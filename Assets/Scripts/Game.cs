using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static string STARTING_POSITION = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    public static Dictionary<PlayerType, PlayerType> otherPlayer = new Dictionary<PlayerType, PlayerType>{
        {PlayerType.White, PlayerType.Black},
        {PlayerType.Black, PlayerType.White}
    };
    
    bool isGameActive;
    
    public Board board;
    public ChessState chessState;
    public int size = 8;
    private PieceManager pieceManager;

    private ChessPlayer playerWhite;
    private ChessPlayer playerBlack;
    
    public GameObject isInCheckPrefab;
    private GameObject isInCheck;
    
    public GameObject previousMoveHighlightPrefab;
    private GameObject[] previousMoveHighlights;
    
    private void Awake()
    {
        pieceManager = GetComponentInChildren<PieceManager>();
    }

    private void Start()
    {
        Camera.main.transform.position = new Vector3(size / 2f, size/ 2f, -10);
        previousMoveHighlights = new GameObject[2];
        board.DrawGrid(size);
        chessState = new ChessState(STARTING_POSITION);

        InitializeGame();
    }

    private void InitializeGame(){
        playerWhite = new HumanPlayer(PlayerType.White);
        playerBlack = new MaterialisticMinMaxPlayer(PlayerType.Black, 2);
        DrawBoard();
        isGameActive = true;
    }

    private void Update()
    {
        if (isGameActive)
        {
            OptionalMove move;
            if (chessState.activePlayer.playerType == PlayerType.White)
            {
                move = playerWhite.GetMove(ChessState.DeepCopy(chessState));
            }
            else
            {
                move = playerBlack.GetMove(ChessState.DeepCopy(chessState));
            }
            // check if move was returned
            if (!move.containsMove)
            {
                return;
            }
            if (!chessState.GetLegalMoves().Contains(move.move))
            {
                return;
            }
            MoveOccured(move.move);
            EventManager.instance.OnTurnEnded();
            print(chessState.gameMoves);
        }
    }

    public void MoveOccured(Move move)
    {  
        chessState.MovePiece(move);
        
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
        
        for (int pos = 0; pos < 64; pos++)
        {
            if (chessState.boardState[pos].containsPiece)
            {
                pieceManager.DrawPiece(chessState.boardState[pos].piece, new Vector3Int(pos%8, pos/8, 0));
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
        previousMoveHighlights[0].transform.position = new Vector3Int(chessState.previousMove[0] % 8, chessState.previousMove[0] / 8, 0);
        previousMoveHighlights[1].transform.position = new Vector3Int(chessState.previousMove[1] % 8, chessState.previousMove[1] / 8, 0);
    }
    
    public void DisplayCheck()
    {
        Destroy(isInCheck);
        int kingPosition = chessState.GetKingPosition(chessState.activePlayer.playerType);
        if (chessState.IsKingInCheck(chessState.activePlayer.playerType))
        {
            isInCheck = Instantiate(isInCheckPrefab, gameObject.transform);
            isInCheck.transform.position = new Vector3Int(kingPosition % 8, kingPosition / 8, 0);
        }
    }

}
