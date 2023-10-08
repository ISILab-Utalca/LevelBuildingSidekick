using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LBSTileExtensions
{
    
    public static Rect GetBounds(this IEnumerable<LBSTile> tiles)
    {
        if(tiles.Count() == 0)
            return new Rect();

        var x = tiles.Min(t => t.Position.x);
        var y = tiles.Min(t => t.Position.y);
        var width = tiles.Max(t => t.Position.x) - x + 1;
        var height = tiles.Max(t => t.Position.y) - y + 1;
        return new Rect(x, y, width, height);
    }
}
