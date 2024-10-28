using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Aquarium : MonoBehaviour
{
    public Vector2Int AquariumSize;

    public Transform GlassGrid;
    public GlassTile GlassTilePrefab;
    public GlassTile[] Tiles;

    public UnityEvent OnTankFull { get; } = new();
    
    private Pipes _pipes;
    private Camera _cam;
    
    private GlassTile _lastClicked;
    
    private float _lastTickTime;
    
    private int _lastTileFilled;
    private int _numCracks;

    private const float TickLength = 2;

    public void Init(Pipes pipes, Camera cam)
    {
        _pipes = pipes;
        _cam = cam;
    }

    public void UpdateProgress()
    {
        if (Time.time - _lastTickTime > TickLength)
        {
            _lastTickTime = Time.time;
            var diff = _pipes.WaterPressure - _numCracks;
            if (diff > 0) Fill(diff);
            else if (diff < 0) Empty(diff);
        }
    }

    private void Fill(int times)
    {
        var nextFilled = _lastTileFilled + times;
        if (nextFilled > Tiles.Length - 1)
        {
            OnTankFull.Invoke();
        }

        nextFilled = Mathf.Min(nextFilled, Tiles.Length - 1);
        for (var i = _lastTileFilled; i < nextFilled; i++)
        {
            Tiles[i].Fill();
        }

        _lastTileFilled = nextFilled;
    }

    private void Empty(int times)
    {
        var nextFilled = _lastTileFilled - times;
        nextFilled = Mathf.Max(nextFilled, 0);
        for (var i = _lastTileFilled; i < nextFilled; i--)
        {
            Tiles[i].Empty();
            
            // TODO leak outside !!
        }
        
        _lastTileFilled = nextFilled;
    }
    
    public void HandleInput(InputStruct input)
    {
        var worldPos = _cam.ScreenToWorldPoint(input.Position);
        if (input.Began)
        {
            foreach (var glass in Tiles)
            {
                if (glass.Cracked && glass.IsInside(worldPos))
                {
                    _lastClicked = glass;
                    break;
                }
            }
        }
        if (input.Ended)
        {
            foreach (var glass in Tiles)
            {
                if (glass.Cracked && glass.IsInside(worldPos) && _lastClicked == glass)
                {
                    glass.Fix(); // TODO wait for anim
                    break;
                }
            }

            _lastClicked = null;
        }
    }
    
    [ContextMenu("Build")]
    public void Build()
    {
        Tiles = new GlassTile[AquariumSize.x * AquariumSize.y];
        for (var i = 0; i < AquariumSize.x; i++)
        {
            for (var j = 0; j < AquariumSize.y; j++)
            {
                var tile = Instantiate(GlassTilePrefab, GlassGrid);
                tile.transform.localPosition = new Vector3(j, i, 0);
                tile.Coords = new Vector2Int(i, j);
                tile.name = "glass tile " + i + ", " + j + ")";
                Tiles[i * AquariumSize.y + j] = tile;
            }
        }
    }
}
