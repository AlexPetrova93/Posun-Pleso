using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[Serializable]
public struct TileRow
{
    public Tile[] Row;
}

public class Pipes : MonoBehaviour
{
    public Transform Grid;
    public Transform PipeSpot;
    public Vector2Int GridSize;
    
    public Pipe CrossPrefab;
    public Pipe CurvePrefab;
    public Pipe StraightPrefab;
    public Pipe ThreePrefab;
    public Tile TilePrefab;

    public Tile Source;
    public Tile[] Targets;

    public TileRow[] GridRows;

    private SoundManager _soundManager;
    private Camera _cam;
    private Tile _lastClicked;
    private Dictionary<PipeType, Pipe> _pipePrefabs;

    private Pipe _lastPipe;

    private int _lastPipeType;

    public int WaterPressure;
    public int MaxWaterPressure => Targets.Length;

    //TODO pipe destroyer
    
    public void Init(SoundManager soundManager, Camera cam)
    {
        _soundManager = soundManager;
        _cam = cam;
        _pipePrefabs = new Dictionary<PipeType, Pipe>()
        {
            { PipeType.Cross, CrossPrefab },
            { PipeType.Curve, CurvePrefab },
            { PipeType.Straight, StraightPrefab },
            { PipeType.Three, ThreePrefab }
        };
        Source.PlacedPipe.Activate();
        foreach (var target in Targets)
        {
            target.PlacedPipe.Init();
        }
    }
    
    public void StartGame()
    {
        SpawnNewPipe();
    }

    // TODO spawn random + only have a certain amount of each type - just a shuffled array of all the entire available stack of pipes
    private void SpawnNewPipe()
    {
        var prefab = _pipePrefabs[(PipeType)_lastPipeType];
        _lastPipe = Instantiate(prefab, PipeSpot.transform.position, Quaternion.identity, transform);
        _lastPipe.Init();

        ++_lastPipeType;
        _lastPipeType %= 4;
    }

    public IEnumerator FillAllWithPipes()
    {
        foreach (var gridRow in GridRows)
        {
            foreach (var gridTile in gridRow.Row)
            {
                if (gridTile.PlacedPipe != null)
                {
                    Destroy(gridTile.PlacedPipe.gameObject);
                }
                var newPipe = _pipePrefabs[PipeType.Cross];
                newPipe = Instantiate(newPipe, gridTile.transform.position, Quaternion.identity, transform);
                newPipe.Init();
                newPipe.transform.SetParent(gridTile.transform);
                newPipe.transform.localPosition = Vector3.zero;
                gridTile.PlacedPipe = newPipe;
                gridTile.PlacedPipe.Activate();
                
                var southTile = gridTile.GetTileIn(Direction.S);
                if (southTile != null && southTile.IsTarget)
                {
                    ++WaterPressure;
                    southTile.PlacedPipe.Activate();
                }
                _soundManager.PlaySound(AudioRef.PipePlaceTurn);
                
                yield return new WaitForSeconds(.15f);
            }
        }
    }

    public void HandleInput(InputStruct input)
    {
        var worldPos = _cam.ScreenToWorldPoint(input.Position);
        if (input.Began)
        {
            foreach (var gridRow in GridRows)
            {
                foreach (var gridTile in gridRow.Row)
                {
                    if (gridTile.Selectable && gridTile.IsInside(worldPos))
                    {
                        _lastClicked = gridTile;
                        break;
                    }
                }
            }
        }
        if (input.Ended)
        {
            foreach (var gridRow in GridRows)
            {
                foreach (var gridTile in gridRow.Row)
                {
                    if (gridTile.Selectable && gridTile.IsInside(worldPos) && _lastClicked == gridTile)
                    {
                        if (gridTile.PlacedPipe == null)
                        {
                            gridTile.PlacedPipe = _lastPipe;
                            _lastPipe.transform.SetParent(gridTile.transform);
                            _lastPipe.transform.localPosition = Vector3.zero;
                            SpawnNewPipe();
                        }
                        else
                        {
                            gridTile.PlacedPipe.Rotate();
                        }
                        _soundManager.PlaySound(AudioRef.PipePlaceTurn);

                        Check();

                        break;
                    }
                }
            }

            _lastClicked = null;
        }
    }

    private void Check()
    {
        foreach (var gridRow in GridRows)
        {
            foreach (var tile in gridRow.Row)
            {
                tile.RecursionCounter = 0;
                if (tile.PlacedPipe != null) tile.PlacedPipe.Deactivate();
            }
        }

        Source.RecursionCounter = 0;
        foreach (var target in Targets)
        {
            target.RecursionCounter = 0;
            target.PlacedPipe.Deactivate();
        }
        
        CheckRec(Source);

        var pressure = 0;
        foreach (var target in Targets)
        {
            if (target.PlacedPipe.Active) ++pressure;
        }

        WaterPressure = pressure;
    }

    private void CheckRec(Tile cur)
    {
        if (cur.PlacedPipe == null || cur.RecursionCounter > GridSize.x * GridSize.y) return;

        ++cur.RecursionCounter;
        CheckDirection(cur, Direction.S);
        CheckDirection(cur, Direction.W);
        CheckDirection(cur, Direction.N);
        CheckDirection(cur, Direction.E);
    }

    private void CheckDirection(Tile cur, Direction direction)
    {
        var opDir = direction.GetOpposite();
        var nextTile = cur.GetTileIn(direction);
        if (cur.PlacedPipe.HasExit(direction) && nextTile != null && nextTile.PlacedPipe != null &&
            nextTile.PlacedPipe.HasExit(opDir))
        {
            nextTile.PlacedPipe.Activate();
            CheckRec(nextTile);
        }
    }
    
    [ContextMenu("Build")]
    public void BuildGrid()
    {
        GridRows = new TileRow[GridSize.x];

        for (var i = 0; i < GridSize.x; i++)
        {
            GridRows[i].Row = new Tile[GridSize.y];
            for (var j = 0; j < GridSize.y; j++)
            {
                var tile = Instantiate(TilePrefab, Grid);
                tile.transform.localPosition = new Vector3(j, i, 0);
                tile.name = "tile (" + i + ", " + j + ")";
                GridRows[i].Row[j] = tile;
            }
        }
        
        for (var i = 0; i < GridSize.x; i++)
        {
            var row = GridRows[i].Row;
            for (var j = 0; j < GridSize.y; j++)
            {
                var tile = GridRows[i].Row[j];
                if (i > 0)
                {
                    var southRow = GridRows[i-1].Row;
                    tile.South = southRow[j];
                }
                if (i < GridSize.x-1)
                {
                    var northRow = GridRows[i+1].Row;
                    tile.North = northRow[j];
                }

                if (j > 0)
                {
                    var westTile = row[j - 1];
                    tile.West = westTile;
                }
                if (j < GridSize.y-1)
                {
                    var eastTile = row[j + 1];
                    tile.East = eastTile;
                }
            }
        }
    }
    
}
