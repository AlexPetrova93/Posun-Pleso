using DG.Tweening;
using UnityEngine;

public class LeakedWater : MonoBehaviour
{
    public Vector2Int GridSize;
    public SpriteRenderer WaterTile;
    public int LeakedCounter;

    private Vector2Int _lastCoords = Vector2Int.zero;

    public void Leak(int times)
    {
        var timeStep = Aquarium.TickLength / times;
        timeStep = Mathf.Min(timeStep, .3f);
        for (var i = 0; i < times; i++)
        {
            ++LeakedCounter;
            if (LeakedCounter > GridSize.x * GridSize.y) break;
            
            var water = Instantiate(WaterTile, transform);
            water.transform.localPosition = new Vector3(_lastCoords.x, _lastCoords.y, 0);
            var col = water.color;
            col.a = 0;
            water.color = col;
            water.DOFade(.75f, timeStep).SetDelay(i * timeStep);
            ++_lastCoords.x;
            if (_lastCoords.x >= GridSize.y)
            {
                _lastCoords.x = 0;
                ++_lastCoords.y;
            }
        }
    }
    
}
