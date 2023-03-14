using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LBSTileMap : TileMapModule<LBSTile>
{
    public LBSTileMap() : base() { }

    public LBSTileMap (List<LBSTile> tiles, string key) : base(tiles, key) { }

    public override object Clone()
    {
        return new LBSTileMap(tiles.Select(t => t.Clone() as LBSTile).ToList(), key);
    }
}
