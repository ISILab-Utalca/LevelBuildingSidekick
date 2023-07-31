using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Newtonsoft.Json;
using LBS.Components.TileMap;

[System.Serializable]
public class TiledArea : ICloneable
{
    #region FIELDS
    [SerializeField, JsonRequired]
    protected string id = "Area";
    [SerializeField, JsonRequired, JsonConverter(typeof(ColorConverter))]
    protected Color color;
    [SerializeField, JsonRequired, JsonConverter(typeof(ColorConverter))]
    protected List<LBSTile> tiles;
    #endregion

    #region PROPERTIES
    [JsonIgnore]
    public string ID => id;

    [JsonIgnore]
    public Color Color => color;

    [JsonIgnore]
    public Vector2 Origin
    {
        get => GetBounds().position; //GetBounds().center;
        set
        {
            var offset = value - GetBounds().position;
            foreach (var t in tiles)
            {
                t.Position += offset.ToInt();
            }
        }
    }

    [JsonIgnore]
    public Vector2 Centroid
    {
        get => GetBounds().center;
        set
        {
            foreach (var t in tiles)
            {
                t.Position += new Vector2Int((int)value.x, (int)value.y);
            }
        }
    }

    [JsonIgnore]
    public Vector2 Size => GetBounds().size;

    [JsonIgnore]
    public int Width => (int)Size.x;

    [JsonIgnore]
    public int Height => (int)Size.y;

    [JsonIgnore]
    public int TileCount => tiles.Count;

    [JsonIgnore]
    public List<LBSTile> Tiles => new List<LBSTile>(tiles);

    #endregion

    #region CONSTRUCTORS

    public TiledArea() : base() 
    { 
    }

    public TiledArea(IEnumerable<LBSTile> tiles, string id, Color color)
    {
        this.color = color;
        this.id = id;
        foreach (var t in tiles)
            AddTile(t);
    }

    #endregion

    #region METHODS

    public List<LBSTile> GetCorners()
    {
        var x = GetConcaveCorners();
        var xx = GetConvexCorners();
        x.AddRange(xx);
        return x;
    }

    public virtual bool AddTile(LBSTile tile)
    {
        var t = GetTile(tile.Position);
        if (t != null)
            tiles.Remove(t);

        tiles.Add(tile);
        return true;
    }
    public LBSTile GetTile(Vector2Int pos)
    {
        var tile = tiles.Find(t => t.Position == pos);
        return tile;
    }

    public LBSTile GetTile(int index)
    {
        return tiles[index];
    }

    public bool RemoveTile(LBSTile tile)
    {
        if (tiles.Remove(tile))
        {
            return true;
        }
        return false;
    }

    public LBSTile RemoveAt(int index)
    {
        var t = tiles[index];
        tiles.Remove(t);
        return t;
    }

    public LBSTile RemoveAt(Vector2Int position)
    {
        var tile = GetTile(position);
        if (tile != null)
        {
            tiles.Remove(tile);
        }
        return tile;
    }

    public int GetDistance(Vector2 pos)
    {
        return  (int)tiles.Min(t => (t.Position - pos).Distance(DistanceType.CONNECT_4));
    }

    public bool IsEmpty()
    {
        return (tiles.Count <= 0);
    }

    public Rect GetBounds()
    {
        if (tiles == null || tiles.Count == 0)
        {
            //Debug.LogWarning("Esta tilemap no tiene tiles!!!");
            return new Rect(Vector2.zero, Vector2.zero);
        }

        var x = tiles.Min(t => t.Position.x);
        var y = tiles.Min(t => t.Position.y);
        var width = tiles.Max(t => t.Position.x) - x + 1;
        var height = tiles.Max(t => t.Position.y) - y + 1;
        return new Rect(x, y, width, height);
    }

    private List<bool> CheckNeighbors(Vector2Int position, List<Vector2> directions)
    {
        var neighborhood = new List<bool>();
        for (int i = 0; i < directions.Count; i++)
        {
            var otherPos = position + directions[i];
            neighborhood.Add(GetTile(otherPos.ToInt()) != null);
        }
        return neighborhood;
    }

    private int NeighborhoodValue(Vector2Int position, List<Vector2> directions) // (!) el nombre es malisimo mejorar, esta tambien es de la clase de las tablas del gabo
    {
        var value = 0;
        for (int i = 0; i < directions.Count; i++)
        {
            var otherPos = position + directions[i];
            if (GetTile(otherPos.ToInt()) == null)
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
            if (IsConvexCorner(t.Position, sideDir))
            {
                //corners.Add(t);
                corners.Add(t.Clone() as LBSTile);
            }
        }
        return corners;
    }

    public List<LBSTile> GetConcaveCorners() // (!) Tambien es de la clase de las tablas del gabo 
    {
        var diagDir = new List<Vector2>() { Vector2.right + Vector2.up, Vector2.up + Vector2.left, Vector2.left + Vector2.down, Vector2.down + Vector2.right };
        var sideDir = new List<Vector2>() { Vector2.right, Vector2.up, Vector2.left, Vector2.down };

        var corners = new List<LBSTile>();

        foreach (var t in tiles)
        {
            if (IsConcaveCorner(t.Position, diagDir))
            {
                corners.Add(t.Clone() as LBSTile);
            }
        }
        return corners;
    }

    /*
    internal List<LBSTile> GetConcaveCorners() // (!) Tambien es de la clase de las tablas del gabo 
    {
        var diagDir = new List<Vector2>() { Vector2.right + Vector2.up, Vector2.up + Vector2.left, Vector2.left + Vector2.down, Vector2.down + Vector2.right };
        var sideDir = new List<Vector2>() { Vector2.right, Vector2.up, Vector2.left, Vector2.down };

        var corners = new List<LBSTile>();

        foreach (var t in tiles)
        {
            if (!IsConcaveCorner(t.Position, diagDir))
                continue;

            for (int i = 0; i < sideDir.Count; i++)
            {
                var other = GetTile((t.Position + sideDir[i]).ToInt());
                if (other == null)
                    continue;
                if (IsWall(other.Position, sideDir))
                {
                    //corners.Add(other);
                    corners.Add(other.Clone() as LBSTile);
                }
            }
        }
        return corners;
    }
    */

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
            var dir = (current.Position.x >= Centroid.x) ? Vector2Int.right : Vector2Int.left;

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
            var dir = (current.Position.y >= Centroid.y) ? Vector2Int.up : Vector2Int.down;
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
/*
    public object Clone()
    {
        return new TiledArea(tiles.Select(t => t.Clone()).Cast<LBSTile>(), this.ID, this.color);
    }
*/
    public object Clone()
    {
        return new TiledArea(tiles.Select(t => t.Clone()).Cast<LBSTile>(), this.ID, this.color);
    }

    #endregion
}

