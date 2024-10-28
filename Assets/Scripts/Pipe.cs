using System;
using System.Collections.Generic;
using UnityEngine;

public enum PipeType
{
    Straight, Curve, Cross, Three  // TODO mozne vylepseni - krizovatka neni krizovatka ale dve straight pres sebe
}

public enum Direction
{
    S, W, N, E
}

[Serializable]
public class PipeExit
{
    public Direction ExitDirection;
    public bool Active;
}

[Serializable]
public class PipeExits
{
    public PipeExit South;
    public PipeExit West;
    public PipeExit North;
    public PipeExit East;

    private Dictionary<Direction, PipeExit> _exits;

    public void Init()
    {
        _exits = new Dictionary<Direction, PipeExit>()
        {
            { Direction.S, South },
            { Direction.W, West },
            { Direction.N, North },
            { Direction.E, East }
        };
    }

    public bool HasExit(Direction direction) => _exits[direction].Active;
    
    // rotate CW - i.e. S -> W, W -> N, etc.
    public void Rotate()
    {
        var lastDirA = East.Active;
        for (int i = 0; i < 4; i++)
        {
            var dir = (Direction)i;
            (_exits[dir].Active, lastDirA) = (lastDirA, _exits[dir].Active);
        }
    }
}

public class Pipe : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public PipeType PipeType;
    //public Direction PipeRotation;
    public SpriteRenderer WaterSprite;

    public PipeExits PipeExits;
    
    public bool Active { get; private set; }

    public bool WasFilled;
    
    public void Init()
    {
        PipeExits.Init();
    }

    // rotate CW
    public void Rotate()
    {
        //var rot = (int)PipeRotation;
        //++rot;
        //rot %= 4;
        //PipeRotation = (Direction)rot;
        transform.Rotate(Vector3.forward, -90);
        PipeExits.Rotate();
    }

    public void Activate()
    {
        Active = true;
        WasFilled = true;  // TODO for effects
        SpriteRenderer.color = new Color(0f, 0.49f, 0.44f);
        if (WaterSprite != null) WaterSprite.color = new Color(0.02f, 0.93f, 0.82f); // TODO changes with new assets - separate water and pipe sprite for each pipe
    }
    
    public void ActivateSpecial()
    {
        Active = true;
        SpriteRenderer.color = new Color(0.13f, 0.13f, 0.13f);
        WaterSprite.color = new Color(0.02f, 0.93f, 0.82f);
    }
    
    public void Deactivate()
    {
        Active = false;
        SpriteRenderer.color = Color.white;
        if (WaterSprite != null) WaterSprite.color = Color.clear; 
    }

    public bool HasExit(Direction direction)
    {
        switch (direction)
        {
            case Direction.S:
                return PipeExits.South.Active;
            case Direction.W:
                return PipeExits.West.Active;
            case Direction.N:
                return PipeExits.North.Active;
            case Direction.E:
                return PipeExits.East.Active;
        }

        return false;
    }
}
