using UnityEngine;

public class LeakedWater : MonoBehaviour
{
    public Vector2Int GridSize;
    public GameObject WaterTile;
    public int LeakedCounter;

    private Vector2Int _lastCoords = Vector2Int.zero;

    public void Leak(int times)
    {
        LeakedCounter += times;
        for (var i = 0; i < times; i++)
        {
            var water = Instantiate(WaterTile, transform);
            water.transform.localPosition = new Vector3(_lastCoords.x, _lastCoords.y, 0);
            ++_lastCoords.x;
            if (_lastCoords.x >= GridSize.y)
            {
                _lastCoords.x = 0;
                ++_lastCoords.y;
            }
        }
    }
    
}
