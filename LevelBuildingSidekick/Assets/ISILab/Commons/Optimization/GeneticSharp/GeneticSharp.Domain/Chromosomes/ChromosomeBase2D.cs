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

    public int ToIndex(Vector2 pos)
    {
        return (int)(pos.y * MatrixWidth + pos.x);
    }

    public Vector2Int ToMatrixPosition(int index)
    {
        return new Vector2Int(index % MatrixWidth, index / MatrixWidth);
    }

    public Vector2Int ToTileCoords(Vector2 position)
    {
        throw new System.NotImplementedException();
    }

    public Vector2 FromTileCoords(Vector2 position)
    {
        throw new System.NotImplementedException();
    }
}
