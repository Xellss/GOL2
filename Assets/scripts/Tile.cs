using UnityEngine;
using System.Collections.Generic;

public enum CellState { Alive, Dead }

public class Tile
{
    public Vector2 Position;

    public CellState CurrentState = CellState.Dead;
    public CellState NextState = CellState.Dead;

    public List<Tile> Neighbors = new List<Tile>();

    public GameObject TileObject;

    public Tile(int xPos, int yPos, CellState state)
    {
        Position = new Vector2(xPos, yPos);
        CurrentState = state;
        NextState = state;
    }

    public Tile(Vector2 position, CellState state)
    {
        this.Position = position;
        CurrentState = state;
        NextState = state;
    }

    public int GetAliveCells()
    {
        int count = 0;

        foreach (var cell in Neighbors)
        {
            if (cell != null)
            {
                if (cell.CurrentState == CellState.Alive)
                {
                    count++;
                }
            }
        }

        return count;
    }
}
