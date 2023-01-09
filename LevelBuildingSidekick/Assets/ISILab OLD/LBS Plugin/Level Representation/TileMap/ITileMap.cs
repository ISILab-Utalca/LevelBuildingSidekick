using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileMap
{
    public float Subdivision { get; }
    public float TileSize { get; }
    public int MatrixWidth { get; }

    public Vector2Int ToTileCoords(Vector2 position); // (?) esto sobra aqui y en tods las implementaciones?
    public Vector2 FromTileCoords(Vector2 position);
}
