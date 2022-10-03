using LBS.ElementView;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSStampTileMapController : LBSStampController, ITileMap
{
    public static int UnitSize = 100; //esto no dbeería estar aca(!!!)

    public float Subdivision { get; set; }

    public LBSStampTileMapController(LBSGraphView view, LBSStampGroupData data) : base(view, data)
    {
        Subdivision = 1;
    }
    public float TileSize { get { return UnitSize / Subdivision; } }

    public override void OnContextualBuid(MainView view, ContextualMenuPopulateEvent cmpe)
    {
        base.OnContextualBuid(view, cmpe);


    }

    public override void CreateStamp(Vector2 pos, GraphView view, StampPresset stamp)
    {
        var viewPos = new Vector2(view.viewTransform.position.x, view.viewTransform.position.y);
        pos = (pos - viewPos) / view.scale;

        Debug.Log("Before: " + pos);
        pos = ToTileCoords(pos);
        Debug.Log("After: " + pos);

        var newStamp = new StampData(stamp.name, pos);
        data.AddStamp(newStamp);
        view.AddElement(new StampView(newStamp));
    }

    public Vector2 ToTileCoords(Vector2 position)
    {
        int x = (int)(position.x - (position.x % TileSize));
        int y = (int)(position.y - (position.y % TileSize));

        return new Vector2(x, y);
    }

    public Vector2 FromTileCoords(Vector2 position)
    {
        return position * TileSize;
    }
}

