using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileMap
{
    public float Subdivision { get; }
    public float TileSize { get; }

    public Vector2Int ToTileCoords(Vector2 position);
    public Vector2 FromTileCoords(Vector2 position);
}
