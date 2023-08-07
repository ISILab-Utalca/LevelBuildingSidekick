using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
[RequieredModule(typeof(LBSTileMap), typeof(ConnectedTileMapModule), typeof(SectorizedTileMapModule), typeof(ConnectedZonesModule))]
public class SchemaBehaviour : LBSBehaviour
{
    #region FIELDS
    [JsonIgnore]
    LBSTileMap tileMap;
    [JsonIgnore]
    ConnectedTileMapModule tileConnections;
    [JsonIgnore]
    SectorizedTileMapModule areas;
    [JsonIgnore]
    ConnectedZonesModule graph;

    public readonly List<string> Connections = new List<string>()
    {
        "Wall",
        "Door",
        "Empty"
    };

    public readonly List<Vector2> Directions = new List<Vector2>() 
    { 
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.up
    };

    #endregion

    #region PROEPRTIES
    public List<Zone> Areas
    {
        get
        {
            if(areas != null)
                return areas.Areas; //?
            return new List<Zone>();
        }
    }
    #endregion

    #region CONSTRUCTORS
    public SchemaBehaviour() : base()
    {

    }

    public SchemaBehaviour(Texture2D icon, string name) : base(icon, name)
    {

    }

    #endregion

    #region METHODS
    public override void Init(LBSLayer layer)
    {
        base.Init(layer);

        var tileMap = Owner.GetModule<LBSTileMap>();
        if (tileMap == null)
        {
            this.tileMap = new LBSTileMap();
            Owner.AddModule(this.tileMap);
        }
        else
        {
            this.tileMap = tileMap;
        } 

        var tileConnections = Owner.GetModule<ConnectedTileMapModule>();
        if (tileConnections == null)
        {
            this.tileConnections = new ConnectedTileMapModule();
            Owner.AddModule(this.tileConnections);
        }
        else
        {
            this.tileConnections = tileConnections;
        }

        var areas = Owner.GetModule<SectorizedTileMapModule>();
        if (areas == null)
        {
            this.areas = new SectorizedTileMapModule();
            Owner.AddModule(this.areas);
        }
        else
        {
            this.areas = areas;
        }


        var graph = Owner.GetModule<ConnectedZonesModule>();
        if (graph == null)
        {
            this.graph = new ConnectedZonesModule();
            Owner.AddModule(this.areas);
        }
        else
        {
            this.graph = graph;
        }
    }

    public void CheckModules()
    {
        var tileMap = Owner.GetModule<LBSTileMap>();
        var tileConnections = Owner.GetModule<ConnectedTileMapModule>();
        var areas = Owner.GetModule<SectorizedTileMapModule>();
        var graph = Owner.GetModule<ConnectedZonesModule>();

        if (tileMap == null)
        {
            Debug.LogError("[ISILab] TileMap has been removed from Layer");
            return;
        }
        if (tileConnections == null)
        {
            Debug.LogError("[ISILab] Connections Module has been removed from Layer");
            return;
        }
        if (areas == null)
        {
            Debug.LogError("[ISILab] Sectorized Tile Map Module has been removed from Layer");
            return;
        }
        if (graph == null)
        {
            Debug.LogError("[ISILab] Connected Zones Module has been removed from Layer");
            return;
        }

        if (!tileMap.Equals(this.tileMap))
        {
            this.tileMap = tileMap;
        }
        if (!tileConnections.Equals(this.tileConnections))
        {
            this.tileConnections = tileConnections;
        }
        if (!areas.Equals(this.areas))
        {
            this.areas = areas;
        }
        if (!graph.Equals(this.graph))
        {
            this.graph = graph;
        }
    }

    public void AddTile(Vector2Int position, Zone zone)
    {
        CheckModules();
        var tile = new LBSTile(position, "Tile: " + position.ToString());
        tileMap.AddTile(tile);
        areas.AddTile(tile, zone);

        //Calculate connections
        //tileConnections.AddTile(tile, connections);
    }

    public void AddZone(Vector2Int position)
    {
        CheckModules();
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
        CheckModules();
        var tile = tileMap.GetTile(position);
        tileMap.RemoveTile(tile);
        tileConnections.RemoveTile(tile);
        areas.RemoveTile(tile);
    }

    public void SetConnection(LBSTile tile, int direction, string connection)
    {
        CheckModules();
        var t = tileConnections.GetTile(tile);
        t.SetConnection(direction, connection);
    }

    public LBSTile GetTile(Vector2Int position)
    {
        CheckModules();
        return tileMap.GetTile(position);
    }

    public TileConnectionsPair GetConnections(LBSTile tile)
    {
        CheckModules();
        return tileConnections.GetTile(tile);
    }

    public Zone GetZone(LBSTile tile)
    {
        CheckModules();
        return areas.GetTile(tile).Zone;
    }
    public Zone GetZone(Vector2 position)
    {
        CheckModules();
        throw new System.NotImplementedException();
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
        return new SchemaBehaviour(this.icon, this.name);
    }
    #endregion
}
 