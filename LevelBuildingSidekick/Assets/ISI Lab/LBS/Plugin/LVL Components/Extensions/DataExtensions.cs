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

    public static void Move(this TiledArea area, Vector2Int value)
    {
        for (int i = 0; i < area.Tiles.Count; i++)
        {
            area.Tiles[i].Position += value;
        }
    }

}
