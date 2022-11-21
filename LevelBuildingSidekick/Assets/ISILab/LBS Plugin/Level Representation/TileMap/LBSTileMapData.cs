using LBS.Representation;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBSTileMapData : LBSRepesentationData, ICloneable
{
    // Fields
    [SerializeField, JsonRequired, SerializeReference]
    private List<TileData> tiles = new List<TileData>(); // (??) cambiar tileData a ITile
    [SerializeField, JsonRequired]
    private Vector2Int maxMapSize;
    [SerializeField, JsonRequired]
    private float tileSize = 1f;
    [SerializeField, JsonRequired]
    private int tileSideAmount = 4;

    // Properties
    [HideInInspector, JsonIgnore]
    public List<TileData> Tiles => new List<TileData>(tiles); // (??) cambiar tileData a ITile
    [HideInInspector, JsonIgnore]
    public int Width => GetRect().width;

    [HideInInspector, JsonIgnore]
    public int Height => GetRect().height;

    [HideInInspector, JsonIgnore]
    public Vector2Int Size => GetRect().size;


    // Constructors
    public LBSTileMapData() { }

    // Methods
    public virtual void RecalculateTilePos() // (!) find a better name 
    {
        var m = GetRect().min;
        foreach (var tile in tiles)
        {
            tile.Position = tile.Position - m;
        }
    }

    public RectInt GetRect()
    {
        Vector2Int max = new Vector2Int(int.MinValue, int.MinValue);
        Vector2Int min = new Vector2Int(int.MaxValue, int.MaxValue);
        foreach (var t in tiles)
        {
            var pos = t.Position;
            if (pos.x > max.x)
                max.x = pos.x;
            if (pos.y > max.y)
                max.y = pos.y;

            if (pos.x < min.x)
                min.x = pos.x;
            if (pos.y < min.y)
                min.y = pos.y;
        }
        var rect = new RectInt(min, max - min + new Vector2Int(1, 1));
        return rect;
    }
    
    internal TileData GetTile(Vector2Int pos)
    {
        foreach (var tile in this.tiles)
        {
            if (tile.Position.Equals(pos))
            {
                return tile;
            }
        }
        return null;
    }

    public void SetTiles(List<TileData> others)
    {
        foreach (var other in others)
        {
            if (tiles.Contains(other))
            {
                tiles[tiles.IndexOf(other)] = other;
            }
            else
            {
                tiles.Add(other);
            }
        }
    }

    public virtual void AddTile(TileData tile, string roomId)
    {
        SetTiles(new List<TileData>() { tile });
    }

    public virtual void AddTiles(List<TileData> tiles)
    {
        SetTiles(tiles);
    }

    public virtual void RemoveTiles(List<Vector2Int> tiles)
    {
        var toRemove = new List<TileData>();
        foreach (var t in this.tiles)
        {
            foreach (var ot in tiles)
            {
                if (t.Position == ot)
                    toRemove.Add(t);
            }
        }
        RemoveTiles(toRemove);
    }

    public virtual void RemoveTiles(List<TileData> tiles)
    {
        foreach (var t in tiles)
        {
            RemoveTile(t);
        }
    }

    public virtual void RemoveTile(TileData tile)
    {
        if (this.tiles.Contains(tile))
        {
            this.tiles.Remove(tile);
        }
    }

    public override void Clear()
    {
        tiles.Clear();
    }

    public virtual object Clone()
    {
        var clone = new LBSTileMapData();
        throw new System.NotImplementedException();
    }

    public TileData[,] ToMatrix()
    {
        var rect = GetRect();
        var matrixIDs = new TileData[rect.width, rect.height];
        foreach (var tile in tiles)
        {
            var p = tile.Position;
            var pos = p - rect.min;
            matrixIDs[pos.x, pos.y] = tile;
        }
        return matrixIDs;
    }

    public override void Print()
    {
        var msg = "";
        var mtx = this.ToMatrix();

        for (int j = 0; j < mtx.GetLength(1); j++)
        {
            for (int i = 0; i < mtx.GetLength(0); i++)
            {
                if (mtx[i, j] != null)
                {
                    var tile = mtx[i, j];
                    msg += "#";
                }
                else
                {
                    msg += "O";
                }
            }
            msg += "\n";
        }
        Debug.Log(msg);
    }
}
