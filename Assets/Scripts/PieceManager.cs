using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
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

    private List<GameObject> activeGameObjects;

    void Start()
    {
        activeGameObjects = new List<GameObject>();
    }

    public void ClearBoard()
    {
        for(int i = 0; i < activeGameObjects.Count; i++)
            {
                Destroy(activeGameObjects[i]);
            }
        activeGameObjects = new List<GameObject>();
    }
        
    public void DrawPiece(Piece piece, Vector3Int position)
    {
        GameObject prefab = GetPrefab(piece);
        var newObject = Instantiate(prefab, gameObject.transform);
        activeGameObjects.Add(newObject);
        newObject.transform.position = position;
    }

    private GameObject GetPrefab(Piece piece)
    {
        if (piece.playerType == PlayerType.White)
        {
            switch (piece.pieceType)
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
        if (piece.playerType == PlayerType.Black)
        {
            switch (piece.pieceType)
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