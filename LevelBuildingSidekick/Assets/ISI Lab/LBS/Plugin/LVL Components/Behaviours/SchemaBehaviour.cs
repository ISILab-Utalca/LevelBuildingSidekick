using LBS.Behaviours;
using LBS.Bundles;
using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
[RequieredModule(typeof(TileMapModule),
    typeof(ConnectedTileMapModule),
    typeof(SectorizedTileMapModule),
    typeof(ConnectedZonesModule))]
public class SchemaBehaviour : LBSBehaviour
{
    #region READONLY-FIELDS
    [JsonIgnore]
    private static List<string> connections = new List<string>() // esto puede ser remplazado despues (!)
    {
        "Wall", "Door", "Empty"
    };
    #endregion

    #region FIELDS
    [JsonIgnore]
    private TileMapModule tileMap => Owner.GetModule<TileMapModule>();
    [JsonIgnore]
    private ConnectedTileMapModule tileConnections => Owner.GetModule<ConnectedTileMapModule>();
    [JsonIgnore]
    private SectorizedTileMapModule areas => Owner.GetModule<SectorizedTileMapModule>();

    [SerializeField]
    private string pressetInsideStyle = "Castle_Wooden";
    [SerializeField]
    private string pressetOutsideStyle = "Castle_Brick";
    #endregion

    #region PROEPRTIES
    [JsonIgnore]
    public Bundle PressetInsideStyle
    {
        get => LBSAssetsStorage.Instance.Get<Bundle>().Find(b => b.Name == pressetInsideStyle);
        set => pressetInsideStyle = value.Name;
    }

    [JsonIgnore]
    public Bundle PressetOutsideStyle
    {
        get => LBSAssetsStorage.Instance.Get<Bundle>().Find(b => b.Name == pressetOutsideStyle);
        set => pressetOutsideStyle = value.Name;
    }

    [JsonIgnore]
    public List<Zone> Zones => areas.Zones; // esta clase deberia ser la que entrega las areas? (?)

    [JsonIgnore]
    public List<Zone> ZonesWhitTiles => areas.ZonesWithTiles;

    [JsonIgnore]
    public List<LBSTile> Tiles => tileMap.Tiles;

    public List<string> Connections => connections;

    [JsonIgnore]
    public List<Vector2Int> Directions => global::Directions.Bidimencional.Edges;
    #endregion

    #region CONSTRUCTORS
    public SchemaBehaviour(Texture2D icon, string name) : base(icon, name) { }
    #endregion

    #region METHODS
    public override void OnAttachLayer(LBSLayer layer)
    {
        Owner = layer;
    }

    public override void OnDetachLayer(LBSLayer layer)
    {
        throw new System.NotImplementedException();
    }

    public LBSTile AddTile(Vector2Int position, Zone zone)
    {
        var tile = new LBSTile(position);//, "Tile: " + position.ToString());
        tileMap.AddTile(tile);
        areas.AddTile(tile, zone);
        return tile;
    }

    public Zone AddZone()
    { 
        string prefix = "Zone: ";
        int counter = 0;
        string name = prefix + counter;
        IEnumerable<string> names = areas.Zones.Select(z => z.ID);
        while(names.Contains(name))
        {
            counter++;
            name = prefix + counter;
        }

        int r = (int)((Random.value * (256 - 32)) / 16);
        int g = (int)((Random.value * (256 - 32)) / 16);
        int b = (int)((Random.value * (256 - 32)) / 16);

        var c = new Color(r * 16 / 256f, g * 16 / 256f, b * 16 / 256f);

        var zone = new Zone(name, c);

        areas.AddZone(zone);
        return zone;
    }

    public void RemoveZone(Zone zone)
    {
        var tiles = areas.GetTiles(zone);
        tileMap.RemoveTiles(tiles);

        areas.RemoveZone(zone);
    }

    public void RemoveTile(Vector2Int position)
    {
        var tile = tileMap.GetTile(position);
        tileMap.RemoveTile(tile);
        tileConnections.RemoveTile(tile);
        areas.RemovePair(tile);
    }

    public void SetConnection(LBSTile tile, int direction, string connection, bool editedByIA)
    {
        var t = tileConnections.GetPair(tile);
        t.SetConnection(direction, connection,editedByIA);
    }

    public void AddConnections(LBSTile tile, List<string> connections, List<bool> editedByIA)
    {
        tileConnections.AddPair(tile, connections, editedByIA);
    }

    public LBSTile GetTile(Vector2Int position)
    {
        return tileMap.GetTile(position);
    }

    public List<LBSTile> GetTiles(Zone zone)
    {
        return areas.GetTiles(zone);
    }

    public Rect GetBound(Zone zone)
    {
        return areas.GetBounds(zone);
    }

    public List<string> GetConnections(LBSTile tile)
    {
        var pair = tileConnections.GetPair(tile);
        return pair.Connections;
    }

    public Zone GetZone(LBSTile tile)
    {
        var pair = areas.GetPairTile(tile);
        return pair.Zone;
    }

    public Zone GetZone(Vector2 position)
    {
        return GetZone(GetTile(position.ToInt()));
    }

    public List<LBSTile> GetTileNeighbors(LBSTile tile, List<Vector2Int> dirs)
    {
        var tor = new List<LBSTile>();
        foreach (var dir in dirs)
        {
            var t = GetTile(dir + tile.Position);
            tor.Add(t);
        }
        return tor;
    }

    public void RecalculateWalls()
    {
        foreach (var tile in Tiles)
        {
            var currZone = GetZone(tile);

            var currConnects = GetConnections(tile);
            var neigs = GetTileNeighbors(tile, Directions);

            var edt = tileConnections.GetPair(tile).EditedByIA;

            for (int i = 0; i < Directions.Count; i++)
            {
                if (!edt[i])
                    continue;

                if (neigs[i] == null)
                {
                    if (currConnects[i] != "Door")
                    {
                        SetConnection(tile, i, "Wall",true);
                    }
                    continue;
                }

                var otherZone = GetZone(neigs[i]);
                if (otherZone == currZone)
                {


                    SetConnection(tile, i, "Empty", true);
                }
                else
                {
                    if (currConnects[i] != "Door")
                    {
                        SetConnection(tile, i, "Wall", true);
                    }
                }
            }
        }
    }

    public override object Clone()
    {
        return new SchemaBehaviour(this.Icon, this.Name);
    }

    public override bool Equals(object obj)
    {
        var other = obj as SchemaBehaviour;

        if (other == null) return false;

        if(!this.Name.Equals(other.Name)) return false;

        return true;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion
}