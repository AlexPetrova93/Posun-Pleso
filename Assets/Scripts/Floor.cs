using UnityEngine;

public class Floor : MonoBehaviour
{
    public int Lenght;
    
    public Transform FloorGrid;
    public FloorTile FloorTilePrefab;
    
    public FloorTile[] Tiles;

    [ContextMenu("Build")]
    public void Build()
    {
        Tiles = new FloorTile[Lenght];
        for (var i = 0; i < Lenght; i++)
        {
            var tile = Instantiate(FloorTilePrefab, FloorGrid);
            tile.transform.localPosition = new Vector3(i, 0, 0);
            tile.Coords = new Vector2Int(i, 0);
            tile.name = "floor tile " + i;
            Tiles[i] = tile;
            if (i > 0)
            {
                tile.Prev = Tiles[i - 1];
                Tiles[i - 1].Next = tile;
            }
        }
    }
}
