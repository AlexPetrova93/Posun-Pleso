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

    public Tarn Tarn;
    public LeakedWater LeakedWater;
    public Guy Guy;
    public AnimatedMorena[] Morenas;

    public int CrackFrequency = 5;
    
    public UnityEvent OnTankFull { get; } = new();
    public UnityEvent OnWaterRanOut { get; } = new();
    public UnityEvent OnLeakedTooMuch { get; } = new();
    public UnityEvent OnLeakedAll { get; } = new();
    
    private Pipes _pipes;
    private Camera _cam;
    
    private GlassTile _lastClicked;
    private Queue<GlassTile> _toFixTiles = new ();
    
    private float _lastTickTime;
    private float _lastCrackTime;
    
    private int _lastTileFilled;
    private int _numCracks;

    public static float TickLength = 2;

    private int _totalWaterUnits;
    private int _currentWaterUnitsTotal;
    private int _currentWaterUnitsInAquarium;
    private float _totalWaterSafe;

    private int _leakedThreshold;

    public void Init(Pipes pipes, Camera cam)
    {
        _totalWaterUnits = AquariumSize.x * AquariumSize.y + LeakedWater.GridSize.x;
        _totalWaterSafe = _totalWaterUnits;
        _leakedThreshold = LeakedWater.GridSize.y * 2;
        _pipes = pipes;
        _cam = cam;
    }

    public void SetUnlimitedWater()
    {
        _totalWaterUnits = 999999;
        _leakedThreshold = 999999;
        TickLength = 1;
    }

    public void UpdateProgress()
    {
        if (Time.time - _lastTickTime > TickLength)
        {
            _lastTickTime = Time.time;
            
            Tarn.Empty(_pipes.WaterPressure/_totalWaterSafe);
            
            // If water from more then 1 pipe -> crack
            if (_pipes.WaterPressure > 1 && _lastTileFilled > 0 && Time.time - _lastCrackTime > TickLength * CrackFrequency)
            {
                _lastCrackTime = Time.time;
                var numNewCracks = _pipes.WaterPressure - 1;
                // crack random filled tiles
                for (var i = 0; i < numNewCracks; i++)
                {
                    var rnd = Random.Range(0, _lastTileFilled - 1);

                    if (Tiles[rnd].Cracked) continue;
                    
                    Tiles[rnd].Crack();
                    ++_numCracks;
                }
            }
            
            var diff = _pipes.WaterPressure - _numCracks;
            
            if (diff > 0 && _currentWaterUnitsTotal < _totalWaterUnits)
            {
                // End of water reached?
                if (_currentWaterUnitsTotal + diff > _totalWaterUnits)  
                {
                    diff = _totalWaterUnits - _currentWaterUnitsTotal;
                }
                _currentWaterUnitsInAquarium += diff;
                Fill(diff);
            }
            if (diff < 0)
            {
                _currentWaterUnitsInAquarium += diff;
                Empty(-diff);
            }

            if (_numCracks > 0)
            {
                LeakedWater.Leak(_numCracks);
            }
            
            if (_currentWaterUnitsTotal <= _totalWaterUnits)
            {
                _currentWaterUnitsTotal += diff;  // how much water passed through pipes
            }
            else
            {
                OnWaterRanOut.Invoke();
            }
            
            if (LeakedWater.LeakedCounter > _leakedThreshold) OnLeakedTooMuch.Invoke();
            if (LeakedWater.LeakedCounter > LeakedWater.GridSize.x * LeakedWater.GridSize.y) OnLeakedAll.Invoke();
        }

        if (Guy.IsFixing) return;

        if (_toFixTiles.Count > 0)
        {
            var nextCracked = _toFixTiles.Dequeue();
            StartCoroutine(Guy.GoFix(nextCracked, () => --_numCracks));
        }
    }

    private void Fill(int times)
    {
        var nextFilled = _lastTileFilled + times;

        var timeStep = TickLength / times - .0999f;
        timeStep = Mathf.Min(timeStep, .3f);
        nextFilled = Mathf.Min(nextFilled, Tiles.Length);
        var totalDelay = (nextFilled - _lastTileFilled) * timeStep + timeStep;
        for (var i = _lastTileFilled; i < nextFilled; i++)
        {
            Tiles[i].Fill(timeStep, timeStep*(i-_lastTileFilled));
        }

        _lastTileFilled = nextFilled;
        if (_lastTileFilled >= Tiles.Length)
        {
            StartCoroutine(InvokeFullAfterTime(totalDelay));
        }
        foreach (var morena in Morenas)
        {
            if (morena.RequiredTile <= _lastTileFilled) morena.Appear();
        }
    }

    private IEnumerator InvokeFullAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        OnTankFull.Invoke();
    }

    private void Empty(int times)
    {
        var timeStep = TickLength / times - .0999f;
        timeStep = Mathf.Min(timeStep, .3f);
        var nextFilled = _lastTileFilled - times;
        nextFilled = Mathf.Max(0, nextFilled);
        var j = 0;
        for (var i = _lastTileFilled; i >= nextFilled; i--)
        {
            Tiles[i].Empty(timeStep, timeStep*j);
            ++j;
        }
        
        _lastTileFilled = nextFilled;
        foreach (var morena in Morenas)
        {
            if (morena.RequiredTile > _lastTileFilled) morena.Disappear();
        }
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
