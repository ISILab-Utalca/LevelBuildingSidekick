using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LBSPopulationTileMap : TileMapModule<LBSPopulationTile>
{
    public LBSPopulationTileMap() : base() { }

    public LBSPopulationTileMap (List<LBSPopulationTile> tiles, string key) : base(tiles, key) { }

    public override object Clone()
    {
        return new LBSPopulationTileMap(tiles.Select(t => t as LBSPopulationTile).ToList(), key);
    }
}
