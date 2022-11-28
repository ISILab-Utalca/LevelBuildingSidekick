using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// (!) la clase TileMapUtils y MapUtilities podrian mesclarce en una sola ya que cumplen el mismo proposito.

public static class TileMapUtils
{
    /// <summary>
    /// Receives the position of 2 tile and returns the index to which that address corresponds in 4 connected.
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <returns></returns>
    public static int CalcDir4Connected(Vector2Int pos1, Vector2Int pos2)
    {
        var dir = pos1 - pos2;

        if (dir.Equals(Vector2Int.up))
            return 0;
        if (dir.Equals(Vector2Int.right))
            return 1;
        if (dir.Equals(Vector2Int.down))
            return 2;
        if (dir.Equals(Vector2Int.left))
            return 3;
        return 0;
    }

    /// <summary>
    /// Receives the position of 2 tile and returns the index to which that address corresponds in 8 conected.
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <returns></returns>
    public static int CalcDir8Connected(Vector2Int pos1, Vector2Int pos2)
    {
        var dir = pos1 - pos2;

        if (dir.Equals(Vector2Int.up))
            return 0;
        if (dir.Equals(Vector2Int.up + Vector2Int.right))
            return 1;
        if (dir.Equals(Vector2Int.right))
            return 2;
        if (dir.Equals(Vector2Int.right + Vector2Int.down))
            return 3;
        if (dir.Equals(Vector2Int.down))
            return 4;
        if (dir.Equals(Vector2Int.down + Vector2Int.left))
            return 5;
        if (dir.Equals(Vector2Int.left))
            return 6;
        if (dir.Equals(Vector2Int.left + Vector2Int.up))
            return 7;
        return 0;
    }

    public static int CalcDir6Connected(Vector2Int pos1, Vector2Int pos2)
    {
        throw new NotImplementedException();
    }
}

public static class MapUtilities
{
    private readonly static Vector2Int[] sidedirs = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };
    private readonly static Vector2Int[] diagdirs = { Vector2Int.right + Vector2Int.up, Vector2Int.up + Vector2Int.left, Vector2Int.left + Vector2Int.down, Vector2Int.down + Vector2Int.right };

    public static List<Vector2Int> GetNeigthborsPosition(int neightAmount, Vector2Int tilePos)
    {
        var toReturn = new List<Vector2Int>();
        switch (neightAmount)
        {
            case 3:
                toReturn = Get3Conected(tilePos);
                break;
            case 4:
                toReturn = Get4Conected(tilePos);
                break;
            case 6:
                toReturn = Get6Connected(tilePos);
                break;
            case 8:
                toReturn = Get8Conected(tilePos);
                break;
            default:
                Debug.LogError("There is no map configuration for " + neightAmount + " number of neighbors per tile.");
                break;
        }
        return toReturn;
    }

    private static List<Vector2Int> Get3Conected(Vector2Int tilePos)
    {
        throw new NotImplementedException();
    }

    private static List<Vector2Int> Get4Conected(Vector2Int tilePos)
    {
        var dirs = new List<Vector2Int>(sidedirs);
        dirs.ForEach(d => d += tilePos);
        return dirs;
    }

    private static List<Vector2Int> Get8Conected(Vector2Int tilePos)
    {
        var dirs = new List<Vector2Int>(sidedirs).Concat(diagdirs).ToList();
        dirs.ForEach(d => d += tilePos);
        return dirs;
    }

    private static List<Vector2Int> Get6Connected(Vector2Int tilePos)
    {
        if (tilePos.x % 2 == 0) // Pair
        {
            var dirs = new List<Vector2Int>()
            {

            };
            return dirs;
        }
        else // Odd
        {
            var dirs = new List<Vector2Int>()
            {
                new Vector2Int(1,0),
                new Vector2Int(0,-1),
                new Vector2Int(-1,-1),
            };
            return dirs;
        }
    }
}