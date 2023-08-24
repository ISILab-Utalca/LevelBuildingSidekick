using LBS.Behaviours;
using LBS.Components;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RemoveAreaConnection : LBSManipulator
{
    SchemaBehaviour schema;

    public RemoveAreaConnection() : base()
    {
        //feedback = new ConnectedLine();
        //feedback.fixToTeselation = true;
    }

    public override void Init(LBSLayer layer, object owner)
    {
        schema = owner as SchemaBehaviour;
        feedback.TeselationSize = layer.TileSize;
        layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
    {
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int position, MouseMoveEvent e)
    {

    }

    protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
    {
        //schema.RemoveZoneConnection(position, 15);
    }


}
