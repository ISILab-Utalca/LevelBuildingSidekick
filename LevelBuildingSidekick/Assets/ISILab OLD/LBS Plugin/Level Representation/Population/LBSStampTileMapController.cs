using LBS.ElementView;
using LBS.Representation;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using System;

// public class LBSPopulationController : LBSRepController<LBStile>, ITileMap
public class LBSStampTileMapController : LBSStampController, ITileMap, IChromosomable
{
    public static int UnitSize = 100; //esto no dbeería estar aca(!!!)

    public float Subdivision { get; set; }
    public int MatrixWidth {
        get {
            var dist = data.GetStamps().Max(s => s.Position.x) - data.GetStamps().Min(s => s.Position.x);
            dist = (int)(dist - (dist % TileSize));
            return dist;
        } 
    }

    public LBSStampTileMapController(LBSGraphView view, LBSStampGroupData data) : base(view, data)
    {
        Subdivision = 1;
    }
    public float TileSize { get { return UnitSize / Subdivision; } }


    public override void OnContextualBuid(MainView view, ContextualMenuPopulateEvent cmpe)
    {
        base.OnContextualBuid(view, cmpe);
    }

    public void Remove(TileData tile)
    {
        //data.RemoveStamp();
    }

    public override void PopulateView(MainView view)
    {
        var stamps = data.GetStamps();
        foreach (var stamp in stamps)
        {
            var pos = stamp.Position;
            var sv = CreateStampView(stamp, pos, new Vector2(TileSize, TileSize));
            elements.Add(sv);
            view.AddElement(sv);
        }

    }

    public LBSStampView CreateStampView(StampData data, Vector2Int pos, Vector2 size)
    {
        var sv = new LBSStampView(data,view);
        sv.SetPosition(new Rect(pos * size, size));
        return sv;
    }

    public override void CreateStamp(Vector2 pos, GraphView view, StampPresset stamp)
    {
        var viewPos = new Vector2(view.viewTransform.position.x, view.viewTransform.position.y);
        pos = (pos - viewPos) / view.scale;

        var tPos = ToTileCoords(pos);
        var cPos = FromTileCoords(pos);

        var newStamp = new StampData(stamp.name, tPos);
        data.AddStamp(newStamp);
        var v = new LBSStampView(newStamp, this.view);
        view.AddElement(v);
    }

    public Vector2Int ToTileCoords(Vector2 position)
    {
        int x = (int)((position.x - (position.x % TileSize))/UnitSize);
        int y = (int)((position.y - (position.y % TileSize))/UnitSize);

        return new Vector2Int(x, y);
    }

    public Vector2 FromTileCoords(Vector2 position)
    {
        return position * TileSize;
    }

    public IChromosome ToChromosome()
    {
        return new StampTileMapChromosome(this);
    }

    public void FromChromosome(IChromosome chromosome)
    {
        if(!(chromosome is StampTileMapChromosome))
        {
            return;
        }

        data.Clear();

        var genome = chromosome.GetGenes<int>();
        var chrom = (chromosome as StampTileMapChromosome);

        for(int i = 0; i < genome.Length; i++)
        {
            if (genome[i] == -1)
                continue;
            var pos = chrom.ToMatrixPosition(i);
            var stamp = new StampData();
            stamp.Position = pos;
            stamp.Label = chrom.stamps[genome[i]].Label;
            data.AddStamp(stamp);
        }
    }
}

