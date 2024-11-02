using System;
using System.Collections.Generic;
using DG.Tweening;
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
    
    public void Init()
    {
        PipeExits.Init();
        WaterSprite.color = new Color(1, 1, 1, 0);
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
        WaterSprite.DOKill();
        WaterSprite.DOFade(1, .2f);
    }
    
    public void Deactivate()
    {
        Active = false;
        WaterSprite.DOKill();
        WaterSprite.DOFade(0, .2f);
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
