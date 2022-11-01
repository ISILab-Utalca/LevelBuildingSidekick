using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class MapWFC : MonoBehaviour
{
    [SerializeField]
    private float tileSize = 1;

    [SerializeField]
    public int m_extraBorder = 5;

    private List<TileWFC> tiles = new List<TileWFC>();

    public float TileSize { get => tileSize; }

    public List<TileWFC> Tiles { get => tiles; }

    public Vector2Int GetMin()
    {
        if (Tiles.Count <= 0)
            return new Vector2Int(0, 0);

        var minX = int.MaxValue;
        var minY = int.MaxValue;
        foreach (var tile in Tiles)
        {
            if (tile.position.x < minX)
                minX = tile.position.x;
            if (tile.position.y < minY)
                minY = tile.position.y;
        }
        return new Vector2Int(minX, minY);
    }

    public Vector2Int GetMax()
    {
        if (Tiles.Count <= 0)
            return new Vector2Int(0, 0);

        var maxX = int.MinValue;
        var maxY = int.MinValue;
        foreach (var tile in Tiles)
        {
            if (tile.position.x > maxX)
                maxX = tile.position.x;
            if (tile.position.y > maxY)
                maxY = tile.position.y;
        }
        return new Vector2Int(maxX, maxY);
    }

    public void Add(TileWFC tile)
    {
        if(tiles.Any(t => t.position == tile.position))
        {
            Debug.LogWarning("the mosaic cannot be added because that space is already occupied");
            return;
        }
        tiles.Add(tile);
    }

    public void Replace(TileWFC tile)
    {
        try
        {
            var last = tiles.First(t => t.position == tile.position);
            tiles.Remove(last);
        }
        catch (System.Exception)
        {
            //throw;
        }
        tiles.Add(tile);
    }
}

public struct TileWFC
{
    public TileConnectWFC data;
    public Vector2Int position;
    public bool locked;

    public TileWFC(TileConnectWFC data, Vector2Int position, bool locked)
    {
        this.data = data;
        this.position = position;
        this.locked = locked;
    }
}


