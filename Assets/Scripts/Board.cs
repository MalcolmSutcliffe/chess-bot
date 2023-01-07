using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[RequireComponent(typeof(Tilemap))]
public class Board : MonoBehaviour
{
    public Tilemap tileGrid {get; private set;}
    public Tile tileBlack;
    public Tile tileWhite;

    void Awake()
    {
        tileGrid = GetComponent<Tilemap>();
    }

    public void DrawGrid(int size)
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if ((x+y) % 2 == 0)
                    tileGrid.SetTile(new Vector3Int(x, y, 0), tileBlack);
                else
                    tileGrid.SetTile(new Vector3Int(x, y, 0), tileWhite);
            }
        }
    }
}
