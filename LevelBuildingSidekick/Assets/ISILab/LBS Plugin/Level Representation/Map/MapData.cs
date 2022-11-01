using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapData : LBSRepesentationData, ICloneable
{
    [SerializeField, JsonRequired, SerializeReference]
    public List<TileData_2> tiles = new List<TileData_2>();

    public override void Clear()
    {
        throw new NotImplementedException();
    }

    public object Clone()
    {
        throw new NotImplementedException();
    }

    public override void Print()
    {
        throw new NotImplementedException();
    }
}

public static class GridToMapData
{
    public static MapData Generate(TileConnectWFC[,] grid)
    {
        var map = new MapData();
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                var tile = grid[i,j];
                var x = new TileData_2(i, j, tile.Rotation);
                map.tiles.Add(x);
            }
        }
        return map;
    }
}


public static class MapUtilities 
{
    private readonly static Vector2Int[] sidedirs = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };
    private readonly static Vector2Int[] diagdirs = { Vector2Int.right + Vector2Int.up, Vector2Int.up + Vector2Int.left, Vector2Int.left + Vector2Int.down, Vector2Int.down + Vector2Int.right };

    public static List<Vector2Int> GetNeigthborsPosition(int neightAmount, Vector2Int tilePos)
    {
        var toReturn = new List<Vector2Int>();
        switch(neightAmount)
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
        if (tilePos.x %2 == 0) // Pair
        {
            var dirs = new List<Vector2Int>() { 

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