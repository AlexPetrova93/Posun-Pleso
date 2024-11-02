using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Aquarium : MonoBehaviour
{
    public Vector2Int AquariumSize;

    public Transform GlassGrid;
    public GlassTile GlassTilePrefab;
    public GlassTile[] Tiles;

    public LeakedWater LeakedWater;
    public Guy Guy;
    
    public UnityEvent OnTankFull { get; } = new();
    public UnityEvent OnWaterRanOut { get; } = new();
    public UnityEvent OnLeakedTooMuch { get; } = new();
    public UnityEvent OnLeakedAll { get; } = new();
    
    private Pipes _pipes;
    private Camera _cam;
    
    private GlassTile _lastClicked;
    private Queue<GlassTile> _toFixTiles = new ();
    
    private float _lastTickTime;
    
    private int _lastTileFilled;
    private int _numCracks;

    private const float TickLength = 2;

    private int _totalWaterUnits;
    private int _currentWaterUnitsTotal;
    private int _currentWaterUnitsInAquarium;

    private int _leakedThreshold;

    public void Init(Pipes pipes, Camera cam)
    {
        _totalWaterUnits = AquariumSize.x * AquariumSize.y + LeakedWater.GridSize.x;
        _leakedThreshold = LeakedWater.GridSize.y * 2;
        _pipes = pipes;
        _cam = cam;
    }

    public void SetUnlimitedWater()
    {
        _totalWaterUnits = 999999;
        _leakedThreshold = 999999;
    }

    public void UpdateProgress()
    {
        if (Time.time - _lastTickTime > TickLength)
        {
            _lastTickTime = Time.time;
            
            var diff = _pipes.WaterPressure - _numCracks;
            if (_currentWaterUnitsTotal < _totalWaterUnits && diff > 0)
            {
                // End of water reached?
                if (_currentWaterUnitsTotal + diff > _totalWaterUnits)  
                {
                    diff = _totalWaterUnits - _currentWaterUnitsTotal;
                }
                _currentWaterUnitsInAquarium += diff;
                Fill(diff);
            }
            else if (diff < 0)
            {
                _currentWaterUnitsInAquarium -= diff;
                Empty(diff);
                LeakedWater.Leak(diff);
            }
            
            if (_currentWaterUnitsTotal <= _totalWaterUnits)
            {
                _currentWaterUnitsTotal += diff;  // how much water passed through pipes
            }
            else
            {
                OnWaterRanOut.Invoke();
            }
            
            // If water from more then 1 pipe -> crack
            if (_pipes.WaterPressure > 1)
            {
                var numNewCracks = _pipes.WaterPressure - 1;
                // crack random filled tiles
                for (var i = 0; i < numNewCracks; i++)
                {
                    var rnd = Random.Range(0, _lastTileFilled - 1);
                    Tiles[rnd].Crack();
                }
                Empty(numNewCracks);
                LeakedWater.Leak(diff);
            }
            
            if (LeakedWater.LeakedCounter > _leakedThreshold) OnLeakedTooMuch.Invoke();
            if (LeakedWater.LeakedCounter > LeakedWater.GridSize.x * LeakedWater.GridSize.y) OnLeakedAll.Invoke();
        }

        if (Guy.IsFixing) return;

        if (_toFixTiles.Count > 0)
        {
            var nextCracked = _toFixTiles.Dequeue();
            StartCoroutine(Guy.GoFix(nextCracked));
        }
    }

    private void Fill(int times)
    {
        var nextFilled = _lastTileFilled + times;
        if (nextFilled >= Tiles.Length)
        {
            OnTankFull.Invoke();
        }

        nextFilled = Mathf.Min(nextFilled, Tiles.Length);
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
                    QueueFix(glass);
                    break;
                }
            }

            _lastClicked = null;
        }
    }

    private void QueueFix(GlassTile tile)
    {
        StartCoroutine(Guy.Acknowledge());
        _toFixTiles.Enqueue(tile);
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
                tile.name = "glass tile (" + i + ", " + j + ")";
                Tiles[i * AquariumSize.y + j] = tile;
            }
        }
    }
}
