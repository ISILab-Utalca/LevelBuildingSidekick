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

    public override void CreateStamp(ContextualMenuPopulateEvent evt, GraphView view, StampPresset stamp)
    {
        var viewPos = new Vector2(view.viewTransform.position.x, view.viewTransform.position.y);
        var pos = (evt.localMousePosition - viewPos) / view.scale;

        pos = ToTileCoords(pos);

        var newStamp = new StampData(stamp.name, pos);
        data.AddStamp(newStamp);
        view.AddElement(new StampView(newStamp));
    }

    public Vector2 ToTileCoords(Vector2 position)
    {
        var size = UnitSize / Subdivision;
        int x = (int)((position.x / size) - (position.x % size));
        int y = (int)((position.y / size) - (position.y % size));

        return new Vector2(x, y);
    }
}
