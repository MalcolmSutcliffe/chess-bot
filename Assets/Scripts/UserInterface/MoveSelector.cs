using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class MoveSelector : MonoBehaviour
{
    public static MoveSelector instance;
    public Piece selectedPiece;
    public Tilemap tileGrid {get; private set;}
    public GameObject legalMovePrefab;
    public GameObject selectedHighlightPrefab;
    public GameObject attackingPieceHighlightPrefab;
    private List<GameObject> activeGameObjects = new List<GameObject>();

    [SerializeField] private GameObject pawnPromotionMenu;
    private bool isPromoting;
    private int promoteFromPos;
    private int promoteToPos;
    private bool isPromotionCapture;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        tileGrid = GetComponentInChildren<Tilemap>();
        EventManager.instance.PawnPromotionStarted += StartPawnPromotion;
        EventManager.instance.PawnPromotionEnded += StopPawnPromotion;
    }

    private void StartPawnPromotion(int fromPos, int toPos, bool isCapture)
    {
        promoteFromPos = fromPos;
        promoteToPos = toPos;
        isPromotionCapture = isCapture;
        isPromoting = true;
        pawnPromotionMenu.SetActive(true);
    }

    private void StopPawnPromotion()
    {
        promoteFromPos = -1;
        promoteToPos = -1;
        isPromotionCapture = false;
        isPromoting = false;
        pawnPromotionMenu.SetActive(false);
    }

    public void CheckUserInput(ChessState chessState, PlayerType playerType)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = tileGrid.WorldToCell(worldPos);

        if (isPromoting)
        {
            pawnPromotionMenu.GetComponent<PawnPromotionSelector>().CheckPromotion(promoteFromPos, promoteToPos, isPromotionCapture, chessState.activePlayer.playerType);
            return;
        }

        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }
        if(!(tilePos.x >= 0 && tilePos.x < 8) || !(tilePos.y >= 0 && tilePos.y < 8))
        {
            Unselect();
            return;
        }
        if (selectedPiece != null)
        {
            List<Move> legalMoves = selectedPiece.GetLegalMoves(chessState);
            
            foreach (var m in legalMoves)
            {
                if (m.toPos % 8 == tilePos.x && m.toPos / 8 == tilePos.y)
                {
                    // check for pawn promotion
                    if (m.promotePiece)
                    {
                        EventManager.instance.OnPawnPromotionStarted(m.fromPos, m.toPos, m.capturePiece);
                    }
                    else
                    {
                        EventManager.instance.OnPlayerMoveOccured(m, playerType);
                    }
                    Unselect();
                    return;
                }
            }
            
            if (chessState.boardState[tilePos.x +  tilePos.y * 8].containsPiece && tilePos.x == selectedPiece.position % 8 && tilePos.y == selectedPiece.position / 8)
            {
                    Unselect();
                    return;
            }
            if (chessState.boardState[tilePos.x +  tilePos.y * 8].containsPiece && chessState.boardState[tilePos.x +  tilePos.y * 8].piece.playerType == chessState.activePlayer.playerType)
            {
                    Unselect();
                    Select(chessState, tilePos);
                    return;
            }
        }

        if (!chessState.boardState[tilePos.x +  tilePos.y * 8].containsPiece)
        {
            Unselect();
            return;
        }
        
        if (!(chessState.boardState[tilePos.x +  tilePos.y * 8].piece.playerType == playerType))
        {
            print("d");
            Unselect();
            return;
        }
        Select(chessState, tilePos);
    }

    private void HighlightPostions(ChessState chessState, List<int> positions)
    {
        foreach(var pos in positions)
        {
            if (chessState.boardState[pos].containsPiece)
            {
                var newObject = Instantiate(attackingPieceHighlightPrefab, gameObject.transform);
                activeGameObjects.Add(newObject);
                newObject.transform.position = new Vector3Int(pos%8, pos/8, 0);
                continue;
            }
            else{
                var newObject = Instantiate(legalMovePrefab, gameObject.transform);
                activeGameObjects.Add(newObject);
                newObject.transform.position = new Vector3Int(pos%8, pos/8, 0);
            }
        }
    }

    private void Select(ChessState chessState, Vector3Int tilePos)
    {
        if (!chessState.boardState[tilePos.x +  tilePos.y * 8].containsPiece)
        {
            Unselect();
            return;
        }
        selectedPiece = chessState.boardState[tilePos.x +  tilePos.y * 8].piece;
        var newObject = Instantiate(selectedHighlightPrefab, gameObject.transform);
        newObject.transform.position = tilePos;
        activeGameObjects.Add(newObject);
        List<int> positions = new List<int>();
        foreach(Move m in selectedPiece.GetLegalMoves(chessState))
        {
            positions.Add(m.toPos);
        }
        HighlightPostions(chessState, positions);
    }

    private void Unselect()
    {
        selectedPiece = null;
        for(int i = 0; i < activeGameObjects.Count; i++)
        {
            Destroy(activeGameObjects[i]);
        }
        activeGameObjects = new List<GameObject>();
    }

}