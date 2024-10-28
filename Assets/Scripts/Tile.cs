using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;

    public Tile South;
    public Tile West;
    public Tile North;
    public Tile East;
    
    public Pipe PlacedPipe;

    public bool IsTarget;
    
    public bool Selectable = true;

    public int RecursionCounter;
    
    public bool IsInside(Vector2 pos)
    {
        return SpriteRenderer.bounds.Contains(pos);
    }

    public Tile GetTileIn(Direction direction)
    {
        return direction switch
        {
            Direction.S => South,
            Direction.W => West,
            Direction.N => North,
            Direction.E => East,
            _ => null
        };
    }

    public bool IsConnectedTo(Direction direction)
    {
        if (PlacedPipe == null || !PlacedPipe.HasExit(direction)) return false;
        var nextTile = GetTileIn(direction);
        return nextTile != null && nextTile.PlacedPipe != null && nextTile.PlacedPipe.HasExit(direction.GetOpposite());
    }

    public bool IsFlowAtExit(Direction direction)
    {
        if (PlacedPipe == null || !PlacedPipe.HasExit(direction)) return false;

        var nextTile = GetTileIn(direction);
        if (nextTile == null || nextTile.PlacedPipe == null || !nextTile.PlacedPipe.HasExit(direction.GetOpposite())) return false;
        
        return nextTile.PlacedPipe.Active;
    }
}
