using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileMapUtils
{
    /// <summary>
    /// Receives the position of 2 tile and returns the index to which that address corresponds.
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
