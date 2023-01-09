using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticSharp.Domain.Chromosomes;

public abstract class ChromosomeBase2D<T> : ChromosomeBase<T>, ITileMap
{
    public float Subdivision => throw new System.NotImplementedException();

    public float TileSize => throw new System.NotImplementedException();
    public int MatrixWidth { get; set; }

    protected ChromosomeBase2D(int length, int matrixWidth) : base(length)
    {
        MatrixWidth = matrixWidth;
    }

    /// <summary>
    /// Converts a 2D position to an index in a 1D array.
    /// </summary>
    /// <param name="pos">The 2D position to convert.</param>
    public int ToIndex(Vector2 pos)
    {
        return (int)(pos.y * MatrixWidth + pos.x);
    }

    /// <summary>
    /// Converts an index in a 1D array to a 2D position.
    /// </summary>
    /// <param name="index">The index in the 1D array to convert.</param>
    public Vector2Int ToMatrixPosition(int index)
    {
        return new Vector2Int(index % MatrixWidth, index / MatrixWidth);
    }

    /// <summary>
    /// Converts a 2D position to tile coordinates.
    /// </summary>
    /// <param name="position">The 2D position to convert.</param>
    public Vector2Int ToTileCoords(Vector2 position)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Converts tile coordinates to a 2D position.
    /// </summary>
    /// <param name="position">The tile coordinates to convert.</param>
    public Vector2 FromTileCoords(Vector2 position)
    {
        throw new System.NotImplementedException();
    }
}
