using LBS.Behaviours;
using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
[RequieredModule(typeof(TileMapModule), typeof(ConnectedTileMapModule), typeof(SectorizedTileMapModule), typeof(ConnectedZonesModule))]
public class SchemaBehaviour : LBSBehaviour
{
    #region READONLY-FIELD
    [JsonIgnore]
    public readonly List<string> Connections = new()
    {
        "Wall", "Door", "Empty"
    };

    [JsonIgnore]
    public readonly List<Vector2> Directions = new() // esto deberia sacarse de la clase estatica de Directions (!)
    {
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.up
    };
    #endregion

    #region FIELDS
    [JsonIgnore]
    private TileMapModule tileMap;
    [JsonIgnore]
    private ConnectedTileMapModule tileConnections;
    [JsonIgnore]
    private SectorizedTileMapModule areas;
    [JsonIgnore]
    private ConnectedZonesModule graph;
    #endregion

    #region PROEPRTIES
    [JsonIgnore]
    public List<Zone> Areas // esta clase deberia ser la que entrega las areas? (?)
    {
        get
        {
            if (areas != null)
                return areas.Areas; //?
            return new List<Zone>();
        }
    }

    public List<LBSTile> Tiles => tileMap.Tiles;
    #endregion

    #region CONSTRUCTORS
    public SchemaBehaviour(Texture2D icon, string name) : base(icon, name) { }
    #endregion

    #region METHODS
    public override void OnAdd(LBSLayer layer)
    {
        Owner = layer;

        tileMap = Owner.GetModule<TileMapModule>();
        tileConnections = Owner.GetModule<ConnectedTileMapModule>();
        areas = Owner.GetModule<SectorizedTileMapModule>();
        graph = Owner.GetModule<ConnectedZonesModule>();
    }

    public void AddTile(Vector2Int position, Zone zone)
    {
        var tile = new LBSTile(position, "Tile: " + position.ToString());
        tileMap.AddTile(tile);
        areas.AddTile(tile, zone);
    }

    public void AddZone(Vector2Int position)
    {
        string prefix = "Zone: ";
        int counter = 0;
        string name = prefix + counter;
        IEnumerable<string> names = areas.Areas.Select(z => z.ID);
        while(names.Contains(name))
        {
            counter++;
            name = prefix + counter;
        }

        int r = (int)((Random.value * (256 - 32)) / 16);
        int g = (int)((Random.value * (256 - 32)) / 16);
        int b = (int)((Random.value * (256 - 32)) / 16);

        var c = new Color(r*16,g*16,b*16);

        var zone = new Zone(name, c);

        AddTile(position, zone);
    }

    public void RemoveTile(Vector2Int position)
    {
        var tile = tileMap.GetTile(position);
        tileMap.RemoveTile(tile);
        tileConnections.RemoveTile(tile);
        areas.RemoveTile(tile);
    }

    public void SetConnection(LBSTile tile, int direction, string connection)
    {
        var t = tileConnections.GetTile(tile);
        t.SetConnection(direction, connection);
    }

    public LBSTile GetTile(Vector2Int position)
    {
        return tileMap.GetTile(position);
    }

    public TileConnectionsPair GetConnections(LBSTile tile)
    {
        return tileConnections.GetTile(tile);
    }

    public TileZonePair GetZone(LBSTile tile)
    {
        return areas.GetTile(tile);
    }

    public TileZonePair GetZone(Vector2 position)
    {
        return GetZone(GetTile(position.ToInt()));
    }

    public void ConnectZones(Zone first, Zone second)
    {
        graph.AddEdge(first, second);
    }

    public void RemoveZoneConnection(Vector2Int position, float delta)
    {
        ZoneEdge edge= graph.GetEdge(position, delta);
        graph.RemoveEdge(edge);
        throw new System.NotImplementedException();
    }

    public void DisconnectZones(Zone first, Zone second)
    {
        graph.RemoveEdge(first, second);
    }

    public override object Clone()
    {
        return new SchemaBehaviour(this.Icon, this.Name);
    }
    #endregion
}
 