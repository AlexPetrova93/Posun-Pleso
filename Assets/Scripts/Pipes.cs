using System;
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
    public Tile Target;

    public TileRow[] GridRows;

    private Camera _cam;
    private Tile _lastClicked;
    private Dictionary<PipeType, Pipe> _pipePrefabs;

    private Pipe _lastPipe;

    private int _lastPipeType;

    public void Init(Camera cam)
    {
        _cam = cam;
        _pipePrefabs = new Dictionary<PipeType, Pipe>()
        {
            { PipeType.Cross, CrossPrefab },
            { PipeType.Curve, CurvePrefab },
            { PipeType.Straight, StraightPrefab },
            { PipeType.Three, ThreePrefab }
        };
        Source.PlacedPipe.Activate();
        Target.PlacedPipe.Deactivate();
    }

    public void RestartGame()
    {
        // TODO
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void StartGame()
    {
        SpawnNewPipe();
    }

    private void SpawnNewPipe()
    {
        var prefab = _pipePrefabs[(PipeType)_lastPipeType];
        _lastPipe = Instantiate(prefab, PipeSpot.transform.position, Quaternion.identity, transform);
        _lastPipe.Init();

        ++_lastPipeType;
        _lastPipeType %= 4;
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

                        Check();

                        break;
                    }
                }
            }

            _lastClicked = null;
        }
    }

    private void CheckTile(Tile tile)
    {
        if (tile.PlacedPipe != null)
        {
            if (tile.PlacedPipe.Active)
            {
                
            }
            else
            {
                
            }
        }
    }

    private void FillFrom(Tile tile, Direction direction)
    {
        
    }

    private bool _chekedDebug;
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
        Target.RecursionCounter = 0;
        Target.PlacedPipe.Deactivate();
        
        CheckRec(Source);
    }

    // only check east and north for now
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
            if (nextTile == Target) nextTile.PlacedPipe.ActivateSpecial();
            else nextTile.PlacedPipe.Activate();
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
