using LBS.Components.TileMap;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Exterior : TileMapModule<ConnectedTile>
{
    public Exterior() : base() { }

    public Exterior(List<ConnectedTile> tiles, string key) : base(tiles, key)
    {
    }

    public override object Clone()
    {
        return new Exterior(this.tiles.Select(t => t.Clone() as ConnectedTile).ToList(), this.key);
    }
}