using LBS.Components;
using LBS.Components.Teselation;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.PackageManager.UI;
using LBS.Tools.Transformer;
using LBS.Behaviours;

public class AddAreaConnection : LBSManipulator
{
    SchemaBehaviour schema;
    Vector2Int first;

    public AddAreaConnection() : base()
    {
        feedback = new ConnectedLine();
        feedback.fixToTeselation = false;
    }

    public override void Init(LBSLayer layer, LBSBehaviour behaviour)
    {
        schema = behaviour as SchemaBehaviour;
        feedback.TeselationSize = layer.TileSize;
        layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
    {
        first = position;
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int position, MouseMoveEvent e)
    {
        throw new NotImplementedException();
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
    {
        var z1 = schema.GetZone(first);
        if (z1 == null)
        {
            var pos = schema.Owner.ToFixedPosition(first);
            var t = schema.GetTile(pos);
            z1 = schema.GetZone(t);
        }
        if (z1 == null)
            return;

        var z2 = schema.GetZone(position);
        if (z2 == null)
        {
            var pos = schema.Owner.ToFixedPosition(position);
            var t = schema.GetTile(pos);
            z2 = schema.GetZone(t);
        }
        if (z2 == null)
            return;

        if (z1.Equals(z2))
            return;

        schema.ConnectZones(z1.Zone,z2.Zone);
    }
}
