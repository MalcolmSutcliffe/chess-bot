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

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClicked();
        }
    }

    void OnClicked()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = tileGrid.WorldToCell(worldPos);
        
        if (selectedPiece != null)
        {
            if (!Game.instance.gameLayout.IsInBoard(tilePos))
            {
                Unselect();
                return;
            }
            List<Vector3Int> legalMoves = selectedPiece.GetLegalMoves(Game.instance.gameLayout);
            if (legalMoves.Contains(tilePos))
            {
                Game.instance.Move(selectedPiece.position, tilePos);
                Unselect();
                return;
            }
            if (Game.instance.gameLayout.state[tilePos.x, tilePos.y].containsPiece && tilePos == selectedPiece.position)
            {
                Unselect();
                return;
            }
            if (Game.instance.gameLayout.state[tilePos.x, tilePos.y].containsPiece && Game.instance.gameLayout.state[tilePos.x, tilePos.y].piece.playerType == Game.instance.playerMove)
            {
                Unselect();
                Select(tilePos);
                return;
            }
            return;
        }

        if (!Game.instance.gameLayout.state[tilePos.x, tilePos.y].containsPiece)
        {
            Unselect();
            return;
        }
        
        if (!(Game.instance.gameLayout.state[tilePos.x, tilePos.y].piece.playerType == Game.instance.playerMove))
        {
            Unselect();
            return;
        }
        Select(tilePos);
    }

    private void HighlightPostions(List<Vector3Int> positions)
    {
        foreach(var pos in positions)
        {
            if (Game.instance.gameLayout.state[pos.x, pos.y].containsPiece)
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

    private void Select(Vector3Int tilePos)
    {
        selectedPiece = Game.instance.gameLayout.state[tilePos.x, tilePos.y].piece;
        var newObject = Instantiate(selectedHighlightPrefab, gameObject.transform);
        newObject.transform.position = tilePos;
        activeGameObjects.Add(newObject);
        HighlightPostions(selectedPiece.GetLegalMoves(Game.instance.gameLayout));
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