using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SectorizedTileMapModule : LBSModule
{
    #region FIELDS

    [SerializeField, JsonRequired]
    protected List<TileZonePair> tiles = new List<TileZonePair>();

    #endregion

    #region PROPERTIES

    [JsonIgnore]
    public List<TileZonePair> Tiles => new List<TileZonePair>(tiles);

    [JsonIgnore]
    public List<Zone> Areas => tiles.Select(t => t.Zone).Distinct().ToList();

    #endregion

    #region CONSTRUCTORS

    public SectorizedTileMapModule()
    {

    }

    public SectorizedTileMapModule(IEnumerable<TileZonePair> tiles, string id = "TilesToAreaModule") : base(id)
    {
        foreach(var t in tiles)
        {
            AddTile(t);
        }
    }

    #endregion

    #region METHODS

    public void AddTile(TileZonePair tile)
    {
        var t = GetTile(tile.Tile);
        if (t != null)
        {
            tiles.Remove(t);
        }
        tiles.Add(tile);

    }

    public void AddTile(LBSTile tile, Zone zone)
    {
        AddTile(new TileZonePair(tile, zone));
    }

    public TileZonePair GetTile(LBSTile tile)
    {
        if (tiles.Count <= 0)
            return null;
        return tiles.Find(t => t.Tile.Equals(tile));

    }

    private TileZonePair GetTile(Vector2Int pos)
    {
        return tiles.Find(t => t.Tile.Position == pos);
    }

    public void RemoveTile(LBSTile tile)
    {
        var t = GetTile(tile);
        tiles.Remove(t);
    }

    public void RemoveTile(int index)
    {
        tiles.RemoveAt(index);
    }

    public bool Contains(LBSTile tile)
    {
        if (tiles.Count <= 0)
            return false;
        return tiles.Any(t => t.Tile.Equals(tile));
    }

    public Zone InZone(LBSTile tile)
    {
        var t = GetTile(tile);
        if (t == null)
            return null;
        return t.Zone;
    }

    public List<LBSTile> GetZoneTiles(Zone zone)
    {
        var tiles = this.tiles.Where(t => t.Zone.Equals(zone));
        if(tiles.Count() > 0)
        {
            return tiles.Select(t => t.Tile).ToList();
        }
        return new List<LBSTile>();
    }

    public Rect GetZoneBounds(Zone zone)
    {
        return GetZoneTiles(zone).GetBounds();
    }

    public Vector2 ZoneCentroid(Zone zone)
    {
        return GetZoneBounds(zone).center;
    }

    private List<bool> CheckNeighborhood(Vector2Int position, List<Vector2> directions)
    {
        var neighborhood = new List<bool>();
        for (int i = 0; i < directions.Count; i++)
        {
            var otherPos = position + directions[i];
            neighborhood.Add(GetTile(otherPos.ToInt()) != null);
        }
        return neighborhood;
    }

    private List<Zone> CheckZonesInNeighborhood(Vector2Int position, List<Vector2> directions)
    {
        var neighborhood = new List<Zone>();
        for (int i = 0; i < directions.Count; i++)
        {
            var otherPos = position + directions[i];
            var t = GetTile(otherPos.ToInt());
            if (t == null)
                neighborhood.Add(null);
            else
                neighborhood.Add(t.Zone);
        }
        return neighborhood;
    }

    public override Rect GetBounds()
    {
        if (tiles == null || tiles.Count == 0)
        {
            //Debug.LogWarning("Esta tilemap no tiene tiles!!!");
            return new Rect(Vector2.zero, Vector2.zero);
        }
        return tiles.Select(t => t.Tile).GetBounds();
    }

    public override bool IsEmpty()
    {
        return tiles.Count <= 0;
    }

    public override void Clear()
    {
        tiles.Clear();
    }

    public override object Clone()
    {
        return new SectorizedTileMapModule(tiles.Select(t => t.Clone()).Cast<TileZonePair>(), ID);
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
        throw new System.NotImplementedException();
    }

    public override void Reload(LBSLayer layer)
    {
        Owner = layer;
    }

    public override void Print()
    {
        throw new System.NotImplementedException();
    }

    private int NeighborhoodValue(Vector2Int position, List<Vector2> directions) // (!) el nombre es malisimo mejorar, esta tambien es de la clase de las tablas del gabo
    {
        var value = 0;
        var t = GetTile(position);
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

    public bool IsConvexCorner(Vector2 pos, List<Vector2> directions)
    {
        var s = NeighborhoodValue(pos.ToInt(), directions);
        if (s != 0)
        {
            if (s % 3 == 0 || s == 7 || s == 11 || s == 13 || s == 14)
                return true;
        }
        return false;
    }

    public bool IsConcaveCorner(Vector2 pos, List<Vector2> directions)
    {
        var s = NeighborhoodValue(pos.ToInt(), directions);
        if (s == 1 || s == 2 || s == 4 || s == 8)
            return true;
        return false;
    }

    public bool IsWall(Vector2 pos, List<Vector2> directions)
    {
        var s = NeighborhoodValue(pos.ToInt(), directions);
        if (s == 1 || s == 2 || s == 4 || s == 8)
            return true;
        return false;

    }

    internal List<LBSTile> GetConvexCorners() // (??)  esto solo funciona para "4 conected", deberia estar en una clase aparte?, si en la clase de las tablas del gabo
    {
        var sideDir = new List<Vector2>() { Vector2.right, Vector2.up, Vector2.left, Vector2.down };
        var corners = new List<LBSTile>();
        foreach (var t in tiles)
        {
            if (IsConvexCorner(t.Tile.Position, sideDir))
            {
                //corners.Add(t);
                corners.Add(t.Clone() as LBSTile);
            }
        }
        return corners;
    }

    internal List<LBSTile> GetConcaveCorners() // (!) Tambien es de la clase de las tablas del gabo 
    {
        var diagDir = new List<Vector2>() { Vector2.right + Vector2.up, Vector2.up + Vector2.left, Vector2.left + Vector2.down, Vector2.down + Vector2.right };
        var sideDir = new List<Vector2>() { Vector2.right, Vector2.up, Vector2.left, Vector2.down };

        var corners = new List<LBSTile>();

        foreach (var t in tiles)
        {
            if (!IsConcaveCorner(t.Tile.Position, diagDir))
                continue;

            for (int i = 0; i < sideDir.Count; i++)
            {
                var other = GetTile((t.Tile.Position + sideDir[i]).ToInt());
                if (other == null)
                    continue;
                if (IsWall(other.Tile.Position, sideDir))
                {
                    //corners.Add(other);
                    corners.Add(other.Clone() as LBSTile);
                }
            }
        }
        return corners;
    }

    internal List<WallData> GetVerticalWalls() // (!) Tambien es de la clase de las tablas del gabo 
    {
        var walls = new List<WallData>();

        var convexCorners = GetConvexCorners();
        var allCorners = GetConcaveCorners();
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
            var dir = (current.Position.x >= ZoneCentroid(InZone(current)).x) ? Vector2Int.right : Vector2Int.left;

            var wall = new WallData(this.id, dir, wallTiles);
            walls.Add(wall);
        }
        return walls;
    }

    internal List<WallData> GetHorizontalWalls()
    {
        var walls = new List<WallData>();

        var convexCorners = GetConvexCorners();
        var allCorners = GetConcaveCorners();
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
            var dir = (current.Position.y >= ZoneCentroid(InZone(current)).y) ? Vector2Int.up : Vector2Int.down;
            var wall = new WallData(this.id, dir, wallTiles);
            walls.Add(wall);
        }
        return walls;
    }

    public List<WallData> GetWalls()
    {
        var horizontal = GetHorizontalWalls();
        var vertical = GetVerticalWalls();

        return horizontal.Concat(vertical).ToList();
    }

    #endregion
}

public class TileZonePair : ICloneable
{
    #region FIELDS

    [SerializeField, JsonRequired, SerializeReference]
    LBSTile tile;
    [SerializeField, JsonRequired, SerializeReference]
    Zone zone;

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
        return new TileZonePair(tile, zone);
    }

    #endregion

}
