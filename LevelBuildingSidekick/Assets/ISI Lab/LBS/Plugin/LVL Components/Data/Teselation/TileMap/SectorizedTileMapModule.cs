using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SectorizedTileMapModule : LBSModule, ISelectable
{
    #region FIELDS
    [SerializeField, JsonRequired, SerializeReference]
    private List<Zone> zones = new List<Zone>();

    [SerializeField, JsonRequired, SerializeReference]
    private List<TileZonePair> pairs = new List<TileZonePair>();
    #endregion

    #region PROPERTIES
    [JsonIgnore]
    public List<TileZonePair> PairTiles => new List<TileZonePair>(pairs);

    [JsonIgnore]
    public List<Zone> Zones => zones;

    [JsonIgnore]
    public List<Zone> ZonesWithTiles => pairs.Select(t => t.Zone).Distinct().ToList();

    [JsonIgnore]
    private List<Vector2Int> Dirs => Directions.Bidimencional.Edges;
    
    [JsonIgnore]
    private List<Vector2Int> DirsDiag => Directions.Bidimencional.Diagonals;
    #endregion

    #region CONSTRUCTORS
    public SectorizedTileMapModule()
    {

    }

    public SectorizedTileMapModule(List<Zone> zones, List<TileZonePair> tiles, string id = "TilesToAreaModule") : base(id)
    {
        foreach (var zone in zones)
        {
            AddZone(zone);
        }

        foreach(var t in tiles)
        {
            AddTile(t);
        }
    }
    #endregion

    #region METHODS
    public void MoveArea(Zone zone, Vector2Int dir)
    {
        var tiles = GetTiles(zone);

        var poss = new List<Vector2Int>();
        foreach (var t in tiles)
        {
            t.Position += new Vector2Int(dir.x, dir.y);
            poss.Add(t.Position + dir);
        }

        var tor = new List<LBSTile>();
        foreach (var otherZone in zones)
        {
            if (zone.ID != otherZone.ID)
            {
                var otherTiles = GetTiles(otherZone);

                foreach (var t in otherTiles)
                {   
                    if(poss.Contains(t.Position))
                    {
                        tor.Add(t);
                    }
                }
            }
        }

        foreach (var t in tor)
        {
            RemoveTile(t);
        }
    }

    public void AddTile(TileZonePair tile)
    {
        var t = GetPairTile(tile.Tile.Position);
        if (t != null)
        {
            pairs.Remove(t);
        }
        pairs.Add(tile);
    }

    public void AddTile(LBSTile tile, Zone zone)
    {
        AddTile(new TileZonePair(tile, zone));
    }

    public void AddZone(Zone zone)
    {
        zones.Add(zone);
    }

    public TileZonePair GetPairTile(LBSTile tile)
    {
        if (pairs.Count <= 0)
            return null;

        foreach (var pair in pairs)
        {
            if (pair.Tile == tile)
                return pair;
        }
        return null;
        //return pairs.Find(t => t.Tile.Equals(tile));
    }

    private TileZonePair GetPairTile(Vector2Int pos)
    {
        return pairs.Find(t => t.Tile.Position == pos);
    }

    public void RemoveZone(Zone zone)
    {
        zones.Remove(zone);

        var toRemove = new List<TileZonePair>();
        foreach (var pair in pairs)
        {
            if (pair.Zone == zone)
                toRemove.Add(pair);
        }

        foreach (var pair in toRemove)
        {
            pairs.Remove(pair);
        }
    }

    public void RemoveTile(LBSTile tile)
    {
        var t = GetPairTile(tile);
        pairs.Remove(t);
    }

    public void RemoveTile(int index)
    {
        pairs.RemoveAt(index);
    }

    public bool Contains(LBSTile tile)
    {
        if (pairs.Count <= 0)
            return false;
        return pairs.Any(t => t.Tile.Equals(tile));
    }

    public Zone GetZone(LBSTile tile)
    {
        var p = GetPairTile(tile);
        if (p == null)
            return null;
        return p.Zone;
    }

    public List<LBSTile> GetTiles(Zone zone)
    {
        var tiles = new List<LBSTile>();
        foreach (var pair in pairs)
        {
            if(pair.Zone == zone)
            {
                tiles.Add(pair.Tile);
            }
        }
        return tiles;
    }

    public Rect GetBounds(Zone zone)
    {
        return GetTiles(zone).GetBounds();
    }

    public Vector2 ZoneCentroid(Zone zone)
    {
        return GetBounds(zone).center;
    }

    private List<bool> CheckNeighborhood(Vector2Int position, List<Vector2> directions)
    {
        var neighborhood = new List<bool>();
        for (int i = 0; i < directions.Count; i++)
        {
            var otherPos = position + directions[i];
            neighborhood.Add(GetPairTile(otherPos.ToInt()) != null);
        }
        return neighborhood;
    }

    private List<Zone> CheckZonesInNeighborhood(Vector2Int position, List<Vector2Int> directions)
    {
        var neighborhood = new List<Zone>();
        for (int i = 0; i < directions.Count; i++)
        {
            var otherPos = position + directions[i];
            var t = GetPairTile(otherPos);
            if (t == null)
                neighborhood.Add(null);
            else
                neighborhood.Add(t.Zone);
        }
        return neighborhood;
    }

    public override Rect GetBounds()
    {
        if (pairs == null || pairs.Count == 0)
        {
            //Debug.LogWarning("Esta tilemap no tiene tiles!!!");
            return new Rect(Vector2.zero, Vector2.zero);
        }
        return pairs.Select(t => t.Tile).GetBounds();
    }

    public override bool IsEmpty()
    {
        return pairs.Count <= 0;
    }

    public override void Clear()
    {
        pairs.Clear();
    }

    public override object Clone()
    {
        var zones = this.zones.Select(t => t.Clone()).Cast<Zone>().ToList();
        var pairs = this.pairs.Select(t => t.Clone()).Cast<TileZonePair>().ToList();

        var clone = new SectorizedTileMapModule(zones, pairs, this.id);

        return clone;
    }

    public override void Rewrite(LBSModule module)
    {
        throw new System.NotImplementedException();
    }

    public override void OnAttach(LBSLayer layer)
    {
    }

    public override void OnDetach(LBSLayer layer)
    {

    }

    public override void Reload(LBSLayer layer)
    {
        Owner = layer;
    }

    public override void Print()
    {
        throw new System.NotImplementedException();
    }

    private int NeighborhoodValue(Vector2Int position, List<Vector2Int> directions) // (!) el nombre es malisimo mejorar, esta tambien es de la clase de las tablas del gabo
    {
        var value = 0;
        var t = GetPairTile(position);
        if (t == null)
            return -1;
        var zones = CheckZonesInNeighborhood(position, directions);
        for (int i = 0; i < directions.Count; i++)
        {
            var otherPos = position + directions[i];
            if (zones[i] == null || !zones[i].Equals(t.Zone))
            {
                value += Mathf.RoundToInt(Mathf.Pow(2, i));
            }
        }

        return value;
    }

    public bool IsConvexCorner(Vector2 pos, List<Vector2Int> directions)
    {
        var s = NeighborhoodValue(pos.ToInt(), directions);
        if (s != 0)
        {
            if (s % 3 == 0 || s == 7 || s == 11 || s == 13 || s == 14)
                return true;
        }
        return false;
    }

    public bool IsConcaveCorner(Vector2 pos, List<Vector2Int> directions)
    {
        var s = NeighborhoodValue(pos.ToInt(), directions);
        if (s == 1 || s == 2 || s == 4 || s == 8)
            return true;
        return false;
    }

    public bool IsWall(Vector2 pos, List<Vector2Int> directions)
    {
        var s = NeighborhoodValue(pos.ToInt(), directions);
        if (s == 1 || s == 2 || s == 4 || s == 8)
            return true;
        return false;

    }

    internal List<LBSTile> GetConvexCorners(Zone zone) // (??)  esto solo funciona para "4 conected", deberia estar en una clase aparte?, si en la clase de las tablas del gabo
    {
        var corners = new List<LBSTile>();
        foreach (var t in pairs)
        {
            if (t.Zone != zone)
                continue;

            if (IsConvexCorner(t.Tile.Position, Dirs))
            {
                corners.Add(t.Tile);
                //corners.Add(t.Clone() as LBSTile);
            }
        }
        return corners;
    }

    internal List<LBSTile> GetConcaveCorners(Zone zone) // (!) Tambien es de la clase de las tablas del gabo 
    {

        var corners = new List<LBSTile>();

        foreach (var t in pairs)
        {
            if (t.Zone != zone)
                continue;

            if (!IsConcaveCorner(t.Tile.Position, DirsDiag))
                continue;

            for (int i = 0; i < Dirs.Count; i++)
            {
                var other = GetPairTile(t.Tile.Position + Dirs[i]);
                if (other == null)
                    continue;
                if (IsWall(other.Tile.Position, Dirs))
                {
                    corners.Add(other.Tile);
                    //corners.Add(other.Clone() as LBSTile);
                }
            }
        }
        return corners;
    }

    internal List<WallData> GetVerticalWalls(Zone zone) // (!) Tambien es de la clase de las tablas del gabo 
    {
        var walls = new List<WallData>();

        var convexCorners = GetConvexCorners(zone);
        var allCorners = GetConcaveCorners(zone);
        allCorners.AddRange(convexCorners);

        foreach (var current in convexCorners)
        {
            LBSTile other = null;
            int lessDist = int.MaxValue;
            foreach (var candidate in allCorners)
            {
                if (current == candidate)
                    continue;

                var tile = current;
                if (tile.Position.x - candidate.Position.x != 0)
                    continue;

                var dist = Mathf.Abs(tile.Position.y - candidate.Position.y);
                if (dist < lessDist)
                {
                    lessDist = dist;
                    other = candidate;
                }
            }

            if (other == null)
                other = current;

            if (walls.Any(w => (w.First == other.Position) && (w.Last == current.Position)))
                continue;

            var wallTiles = new List<Vector2Int>();
            var oth = other.Position;
            var end = Mathf.Max(current.Position.y, oth.y);
            var start = Mathf.Min(current.Position.y, oth.y);
            for (int i = 0; i <= end - start; i++)
            {
                wallTiles.Add(new Vector2Int(current.Position.x, start + i));
            }
            var dir = (current.Position.x >= ZoneCentroid(GetZone(current)).x) ? Vector2Int.right : Vector2Int.left;

            var wall = new WallData(this.id, dir, wallTiles);
            walls.Add(wall);
        }
        return walls;
    }

    internal List<WallData> GetHorizontalWalls(Zone zone)
    {
        var walls = new List<WallData>();

        var convexCorners = GetConvexCorners(zone);
        var allCorners = GetConcaveCorners(zone);
        allCorners.AddRange(convexCorners);

        foreach (var current in convexCorners)
        {
            LBSTile other = null;
            int lessDist = int.MaxValue;
            foreach (var candidate in allCorners)
            {
                if (current == candidate)
                    continue;

                var tile = current;
                if (tile.Position.y - candidate.Position.y != 0)
                    continue;

                var dist = Mathf.Abs(tile.Position.x - candidate.Position.x);
                if (dist < lessDist)
                {
                    lessDist = dist;
                    other = candidate;
                }
            }

            if (other == null)
                other = current;

            if (walls.Any(w => (w.First == other.Position) && (w.Last == current.Position)))
                continue;

            var wallTiles = new List<Vector2Int>();
            var oth = other.Position;
            var end = Mathf.Max(current.Position.x, oth.x);
            var start = Mathf.Min(current.Position.x, oth.x);
            for (int i = 0; i <= end - start; i++)
            {
                wallTiles.Add(new Vector2Int(start + i, current.Position.y));
            }
            var dir = (current.Position.y >= ZoneCentroid(GetZone(current)).y) ? Vector2Int.up : Vector2Int.down;
            var wall = new WallData(this.id, dir, wallTiles);
            walls.Add(wall);
        }
        return walls;
    }

    public float GetRoomDistance(Zone r1, Zone r2) // O2 - manhattan
    {
        var lessDist = float.MaxValue;

        var tiles1 = GetTiles(r1);
        var tiles2 = GetTiles(r2);

        //var tileWalls1 = room1.GetWalls().SelectMany(x => x.Tiles).ToList();
        //var tileWalls2 = room2.GetWalls().SelectMany(x => x.Tiles).ToList();

        for (int i = 0; i < tiles1.Count; i++)
        {
            for (int j = 0; j < tiles2.Count; j++)
            {
                var dist = Vector2Int.Distance(tiles1[i].Position, tiles2[j].Position);
                if (dist <= lessDist)
                {
                    lessDist = dist;
                }
            }
        }

        return lessDist;
    }

    public List<WallData> GetWalls(Zone zone)
    {
        var horizontal = GetHorizontalWalls(zone);
        var vertical = GetVerticalWalls(zone);

        return horizontal.Concat(vertical).ToList();
    }

    public Zone GetZone(string name)
    {
        foreach (var zone in zones)
        {
            if(zone.ID == name)
                return zone;
        }
        return null;
    }

    public Zone GetZone(Vector2Int position)
    {
        foreach(var pair in pairs)
        {
            if(pair.Tile.Position == position)
            {
                return pair.Zone;
            }
        }

        return null;
    }

    public List<object> GetSelected(Vector2Int position)
    {
        var r = new List<object>();
        var zone = GetZone(position);

        if (zone != null)
        {
            r.Add(zone);
        }

        return r;
    }

    #endregion
}

[System.Serializable]
public class TileZonePair : ICloneable
{
    #region FIELDS
    [SerializeField, SerializeReference, JsonRequired]
    private LBSTile tile;

    [SerializeField, SerializeReference, JsonRequired]
    private Zone zone;
    #endregion

    #region PROEPRTIES
    [JsonIgnore]
    public LBSTile Tile => tile;

    [JsonIgnore]
    public Zone Zone
    {
        get => zone;
        set => zone = value;
    }
    #endregion

    #region CONSTRUCTORS
    public TileZonePair(LBSTile tile, Zone zone)
    {
        this.tile = tile;
        this.zone = zone;
    }
    #endregion

    #region METHODS
    public object Clone()
    {
        var cTile = CloneRefs.Get(tile) as LBSTile;
        var cZone = CloneRefs.Get(zone) as Zone;

        return new TileZonePair(cTile, cZone);
    }
    #endregion

}
