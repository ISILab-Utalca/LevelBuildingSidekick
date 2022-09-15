using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticSharp.Domain.Chromosomes;

public abstract class ChromosomeBase2D<T> : ChromosomeBase<T>
{
    public int MatrixWidth { get; set; }

    protected ChromosomeBase2D(int length, int matrixWidth) : base(length)
    {
        MatrixWidth = matrixWidth;
    }

    public int ToIndex(Vector2 pos)
    {
        return (int)(pos.y * MatrixWidth + pos.x);
    }

    public Vector2 ToMatrixPosition(int index)
    {
        return new Vector2(index % MatrixWidth, index / MatrixWidth);
    }
}
