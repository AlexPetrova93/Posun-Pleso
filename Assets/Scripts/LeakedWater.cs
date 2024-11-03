using System.Collections;
using DG.Tweening;
using UnityEngine;

public class LeakedWater : MonoBehaviour
{
    public Vector2Int GridSize;  // x = vyska, y = sirka
    public WaterTile WaterTile;
    public int LeakedCounter;

    private WaterTile[] Grid;
    private Vector2Int _lastCoords = Vector2Int.zero;

    public void Init()
    {
        Grid = new WaterTile[GridSize.x * GridSize.y];
    }
    
    public void Leak(int times)
    {
        StartCoroutine(LeakRoutine(times));
    }

    private IEnumerator LeakRoutine(int times)
    {
        var timeStep = Aquarium.TickLength / times;
        timeStep = Mathf.Min(timeStep, .3f);
        for (var i = 0; i < times; i++)
        {
            ++LeakedCounter;
            if (LeakedCounter > GridSize.x * GridSize.y) break;
            
            var water = Instantiate(WaterTile, transform);
            water.transform.localPosition = new Vector3(_lastCoords.x, _lastCoords.y, 0);
            if (_lastCoords.x < GridSize.y - 1) water.FadeIn(timeStep);
            else water.FilledNext(timeStep);
            Grid[_lastCoords.y * GridSize.y + _lastCoords.x] = water;
            
            // switch tiles
            if (_lastCoords.x > 0)
            {
                Grid[_lastCoords.y * GridSize.y + (_lastCoords.x - 1)].FilledNext(timeStep);
            }
            if (_lastCoords.y > 0)
            {
                Grid[(_lastCoords.y - 1) * GridSize.y + _lastCoords.x].FilledTop(timeStep);
            }
            
            ++_lastCoords.x;
            if (_lastCoords.x >= GridSize.y)
            {
                _lastCoords.x = 0;
                ++_lastCoords.y;
            }

            yield return new WaitForSeconds(timeStep);
        }
        
    }
    
}
