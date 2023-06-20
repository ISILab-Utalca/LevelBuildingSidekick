using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Directions
{
    private static readonly List<Vector3Int> centers = new List<Vector3Int>()
    {
        new Vector3Int(1,0,0),
        new Vector3Int(0,1,0),
        new Vector3Int(0,0,1),
        new Vector3Int(-1,0,0),
        new Vector3Int(0,-1,0),
        new Vector3Int(0,0,-1),
    };

    private static readonly List<Vector3Int> diagonals = new List<Vector3Int>()
    {
        new Vector3Int(1, 1, 1),
        new Vector3Int(-1, 1, 1),
        new Vector3Int(1, -1, 1),
        new Vector3Int(1, 1, -1),
        new Vector3Int(-1, -1, 1),
        new Vector3Int(-1, 1, -1),
        new Vector3Int(1, -1, -1),
        new Vector3Int(-1, -1, -1)
    };

    private static readonly List<Vector3Int> edges = new List<Vector3Int>()
    {
        new Vector3Int(1, 1, 0),
        new Vector3Int(1, -1, 0),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(-1, -1, 0),
        new Vector3Int(1, 0, 1),
        new Vector3Int(1, 0, -1),
        new Vector3Int(-1, 0, 1),
        new Vector3Int(-1, 0, -1),
        new Vector3Int(0, 1, 1),
        new Vector3Int(0, 1, -1),
        new Vector3Int(0, -1, 1),
        new Vector3Int(0, -1, -1)
    };

    public static List<Vector3Int> Centers => centers;
    public static List<Vector3Int> Edges => edges;
    public static List<Vector3Int> Diagonals => diagonals;

    /// <summary>
    /// Finds the perpendicular directions to a given axis.
    /// </summary>
    /// <param name="axis">The axis for which to find the perpendicular directions.</param>
    /// <returns>Perpendicular directions to the axis.</returns>
    public static List<Vector3Int> GetPerpendicularDirs(Vector3Int axis, bool whitDiagonals = false)
    {
        var toR = new List<Vector3Int>();

        foreach (var direction in Centers)
        {
            if (Vector3.Dot(axis, direction) == 0)
            {
                toR.Add(direction);
            }
        }

        if (!whitDiagonals)
            return toR;

        foreach (var direction in Edges)
        {
            if (Vector3.Dot(axis, direction) == 0)
            {
                toR.Add(direction);
            }
        }

        return toR;
    }
}
