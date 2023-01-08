using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class UserInterface : MonoBehaviour
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

    public void OnClicked(GameLayout gameLayout, PlayerType playerMove)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = tileGrid.WorldToCell(worldPos);
        
        if (selectedPiece != null)
        {
            if (!gameLayout.IsInBoard(tilePos))
            {
                Unselect();
                return;
            }
            List<Vector3Int> legalMoves = selectedPiece.GetLegalMoves(gameLayout);
            if (legalMoves.Contains(tilePos))
            {
                EventManager.instance.OnMoveOccured(selectedPiece.position, tilePos);
                Unselect();
                return;
            }
            if (gameLayout.state[tilePos.x, tilePos.y].containsPiece && tilePos == selectedPiece.position)
            {
                Unselect();
                return;
            }
            if (gameLayout.state[tilePos.x, tilePos.y].containsPiece && gameLayout.state[tilePos.x, tilePos.y].piece.playerType == Game.instance.playerMove)
            {
                Unselect();
                Select(gameLayout, tilePos);
                return;
            }
            return;
        }

        if (!gameLayout.state[tilePos.x, tilePos.y].containsPiece)
        {
            Unselect();
            return;
        }
        
        if (!(gameLayout.state[tilePos.x, tilePos.y].piece.playerType == playerMove))
        {
            Unselect();
            return;
        }
        Select(gameLayout, tilePos);
    }

    private void HighlightPostions(GameLayout gameLayout, List<Vector3Int> positions)
    {
        foreach(var pos in positions)
        {
            if (gameLayout.state[pos.x, pos.y].containsPiece)
            {
                var newObject = Instantiate(attackingPieceHighlightPrefab, gameObject.transform);
                activeGameObjects.Add(newObject);
                newObject.transform.position = pos;
                continue;
            }
            else{
                var newObject = Instantiate(legalMovePrefab, gameObject.transform);
                activeGameObjects.Add(newObject);
                newObject.transform.position = pos;
            }
        }
    }

    private void Select(GameLayout gameLayout, Vector3Int tilePos)
    {
        if (!gameLayout.state[tilePos.x, tilePos.y].containsPiece)
        {
            Unselect();
            return;
        }
        selectedPiece = gameLayout.state[tilePos.x, tilePos.y].piece;
        var newObject = Instantiate(selectedHighlightPrefab, gameObject.transform);
        newObject.transform.position = tilePos;
        activeGameObjects.Add(newObject);
        HighlightPostions(gameLayout, selectedPiece.GetLegalMoves(gameLayout));
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