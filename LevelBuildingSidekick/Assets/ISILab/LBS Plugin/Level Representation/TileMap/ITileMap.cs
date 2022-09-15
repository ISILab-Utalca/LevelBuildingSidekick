using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileMap
{
    public float Subdivision { get; }

    public Vector2 ToTileCoords(Vector2 position);
}
