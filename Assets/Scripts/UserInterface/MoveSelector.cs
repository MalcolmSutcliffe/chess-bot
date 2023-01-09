using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class MoveSelector : MonoBehaviour
{
    public Piece selectedPiece;
    public Tilemap tileGrid {get; private set;}
    public GameObject legalMovePrefab;
    public GameObject selectedHighlightPrefab;
    public GameObject attackingPieceHighlightPrefab;
    private List<GameObject> activeGameObjects = new List<GameObject>();

    void Awake()
    {
        tileGrid = GetComponentInChildren<Tilemap>();
    }

    public void SelectOnBoard(ChessState chessState, PlayerType playerMove)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = tileGrid.WorldToCell(worldPos);
        
        if (selectedPiece != null)
        {
            if (!chessState.IsInBoard(new int[] {tilePos.x, tilePos.y}))
            {
                Unselect();
                return;
            }
            
            List<int[]> legalMoves = selectedPiece.GetLegalMoves(chessState);
            
            List<Vector3Int> legalMovesV3 = new List<Vector3Int>();
            foreach (var m in legalMoves)
            {
                legalMovesV3.Add(new Vector3Int(m[0], m[1], 0));
            }

            if (legalMovesV3.Contains(tilePos))
            {
                // check for pawn promotion
                if (selectedPiece.pieceType == PieceType.Pawn && selectedPiece.playerType == PlayerType.White && tilePos.y == 7)
                {
                    PieceType promotedTo = PieceType.Queen;
                    EventManager.instance.OnMoveOccured(selectedPiece.position, new int[]{tilePos.x, tilePos.y}, true, promotedTo);
                }
                else if (selectedPiece.pieceType == PieceType.Pawn && selectedPiece.playerType == PlayerType.Black && tilePos.y == 0)
                {
                    PieceType promotedTo = PieceType.Queen;
                    EventManager.instance.OnMoveOccured(selectedPiece.position, new int[]{tilePos.x, tilePos.y}, true, promotedTo);
                }
                else
                {
                    EventManager.instance.OnMoveOccured(selectedPiece.position, new int[]{tilePos.x, tilePos.y}, false, PieceType.Pawn);
                }
                Unselect();
                return;
            }
            if (chessState.boardState[tilePos.x, tilePos.y].containsPiece && tilePos.x == selectedPiece.position[0] && tilePos.y == selectedPiece.position[1])
            {
                Unselect();
                return;
            }
            if (chessState.boardState[tilePos.x, tilePos.y].containsPiece && chessState.boardState[tilePos.x, tilePos.y].piece.playerType == chessState.activePlayer.playerType)
            {
                Unselect();
                Select(chessState, tilePos);
                return;
            }
            return;
        }

        if (!chessState.boardState[tilePos.x, tilePos.y].containsPiece)
        {
            Unselect();
            return;
        }
        
        if (!(chessState.boardState[tilePos.x, tilePos.y].piece.playerType == playerMove))
        {
            Unselect();
            return;
        }
        Select(chessState, tilePos);
    }

    private void HighlightPostions(ChessState chessState, List<int[]> positions)
    {
        foreach(var pos in positions)
        {
            if (chessState.boardState[pos[0], pos[1]].containsPiece)
            {
                var newObject = Instantiate(attackingPieceHighlightPrefab, gameObject.transform);
                activeGameObjects.Add(newObject);
                newObject.transform.position = new Vector3Int(pos[0], pos[1], 0);
                continue;
            }
            else{
                var newObject = Instantiate(legalMovePrefab, gameObject.transform);
                activeGameObjects.Add(newObject);
                newObject.transform.position = new Vector3Int(pos[0], pos[1], 0);
            }
        }
    }

    private void Select(ChessState chessState, Vector3Int tilePos)
    {
        if (!chessState.boardState[tilePos.x, tilePos.y].containsPiece)
        {
            Unselect();
            return;
        }
        selectedPiece = chessState.boardState[tilePos.x, tilePos.y].piece;
        var newObject = Instantiate(selectedHighlightPrefab, gameObject.transform);
        newObject.transform.position = tilePos;
        activeGameObjects.Add(newObject);
        HighlightPostions(chessState, selectedPiece.GetLegalMoves(chessState));
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