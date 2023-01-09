using LBS.Representation;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class MapData : LBSTileMapData, ICloneable
{

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
                var concts = tile.Tile.Connections; // (?) profundidad inesesaria??
                var x = new TileData(i, j, tile.Rotation, concts);
                map.AddTile(x);
            }
        }
        return map;
    }
}


