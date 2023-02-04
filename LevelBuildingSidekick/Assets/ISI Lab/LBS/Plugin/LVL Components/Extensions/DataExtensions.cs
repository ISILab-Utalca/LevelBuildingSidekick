using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DataExtensions
{

    public static RectInt GetRect<T>(this AreaTileMap<T> schema) where T : TiledArea
    {
        var x = (int) schema.Areas.Min(a => a.Rect.min.x);
        var y = (int) schema.Areas.Min(a => a.Rect.min.y);
        var width = (int) schema.Areas.Min(a => a.Rect.max.x) - x;
        var height = (int) schema.Areas.Min(a => a.Rect.max.y) - y;
        return new RectInt(x, y, width, height);
    }
    

    public static Vector2Int RecalculateTilePos<T>(this AreaTileMap<T> schema) where T : TiledArea
    { 
        var rect = schema.GetRect();
        var m = rect.min;
        for (int i = 0; i < schema.Areas.Count; i++)
        {
            schema.GetArea(i).Move(-m);
        }
        return -m;
    }

    public static bool CheckTilesRooms<T,U>(this AreaTileMap<T> schema) where T : TiledArea
    {
        foreach (var area in schema.Areas)
        {
            if (area.TileCount <= 0)
            {
                return true;
            }
        }

        return false;
    }

    public static List<T> GetRoomsWithoutTiles<T>(this AreaTileMap<T> schema) where T : TiledArea
    {
        var rwt = new List<T>();

        foreach (var areas in schema.Areas)
        {
            if (areas.TileCount <= 0)
                rwt.Add(areas);
        }

        return rwt;
    }

    public static void Move(this TiledArea area, Vector2Int value)
    {
        for (int i = 0; i < area.Tiles.Count; i++)
        {
            area.Tiles[i].Position += value;
        }
    }

    public static Vector2Int GetRandomTilePosFromCenter(this TiledArea area)
    {
        int auxNum = (area.Size == new Vector2Int(1, 1)) ? 1 : 2;

        return new Vector2Int((int)(area.Centroid.x + UnityEngine.Random.Range(-area.Width / auxNum, area.Width / auxNum)),
                              (int)(area.Centroid.y + UnityEngine.Random.Range(-area.Height / auxNum, area.Height / auxNum)));

    }
}
