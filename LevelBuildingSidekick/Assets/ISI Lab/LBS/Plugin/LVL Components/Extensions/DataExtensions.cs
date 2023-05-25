using LBS.Components;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DataExtensions
{
    public static void RemoveEmptyLayers(this LBSLevelData data)
    {
        var x = 0;
        var layers = data.Layers;
        for (int i = layers.Count - 1; i >= 0; i--)
        {
            if (layers[i] == null)
            {
                x++;
                data.RemoveAt(i);
            }
        }
        Debug.Log("[ISI Lab]: Remove");
    }


    public static bool Compare(this string[] a, string[] b)
    {
        for (int i = 0; i < a.Length; i++)
        {
            for (int j = 0; j < b.Length; j++)
            {
                if (a[i] != b[i] && !string.IsNullOrEmpty(a[i]) && !string.IsNullOrEmpty(a[i]))
                    return false;
            }
        }
        return true;
    }

    public static Tuple<WFCBundle,int> Get(this List<WFCBundle> tiles, string[] conections)
    {
        foreach (var tile in tiles)
        {
            for (int i = 0; i < 4; i++)
            {
                var cs = tile.GetConnection(i);
                if(conections.Compare(cs))
                {
                    return new Tuple<WFCBundle, int>(tile,i);
                }
            }
        }
        return null;
    }

    public static RectInt GetRect<T>(this AreaTileMap<T> schema) where T : TiledArea
    {
        var r = schema.GetBounds();
        return new RectInt((int)r.x, (int)r.y, (int)r.width, (int)r.height);
    }
    
    public static Vector2Int RecalculateTilePos<T>(this AreaTileMap<T> schema) where T : TiledArea
    { 
        var rect = schema.GetBounds();
        var m = rect.min;
        for (int i = 0; i < schema.Areas.Count; i++)
        {
            schema.GetArea(i).Move(-new Vector2Int((int)m.x, (int)m.y));
        }
        return -new Vector2Int((int)m.x, (int)m.y);
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
