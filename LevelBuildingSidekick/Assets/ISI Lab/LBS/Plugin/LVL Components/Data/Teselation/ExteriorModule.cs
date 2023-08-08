using LBS.Components.TileMap;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ExteriorModule : TileMapModule
{
    public ExteriorModule() : base() { }

    public ExteriorModule(List<ConnectedTile> tiles, string key) : base(tiles, key)
    {
    }

    public override object Clone()
    {
        return new ExteriorModule(this.tiles.Select(t => t.Clone() as ConnectedTile).ToList(), this.id);
    }
}