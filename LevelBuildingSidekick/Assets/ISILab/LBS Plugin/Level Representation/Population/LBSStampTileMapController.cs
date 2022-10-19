using LBS.ElementView;
using LBS.Representation;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;

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

    public override void CreateStamp(Vector2 pos, GraphView view, StampPresset stamp)
    {
        var viewPos = new Vector2(view.viewTransform.position.x, view.viewTransform.position.y);
        pos = (pos - viewPos) / view.scale;

        pos = ToTileCoords(pos);
        Debug.Log(pos);
        pos = FromTileCoords(pos);
        Debug.Log(pos);

        var newStamp = new StampData(stamp.name, pos);
        data.AddStamp(newStamp);
        view.AddElement(new LBSStampView(newStamp,this.view));
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
}

