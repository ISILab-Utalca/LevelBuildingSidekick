using LBS.Behaviours;
using LBS.Components;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AddSchemaTile : LBSManipulator
{
    Zone toSet;
    SchemaBehaviour schema;

    public AddSchemaTile() : base()
    {
        feedback = new AreaFeedback();
        feedback.fixToTeselation = true;
    }

    public override void Init(MainView view, LBSLayer layer, LBSBehaviour behaviour)
    {
        schema = behaviour as SchemaBehaviour;
        feedback.TeselationSize = layer.TileSize;
        layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
        this.MainView = view;
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
    {
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int position, MouseMoveEvent e)
    {
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
    {
        var min = schema.Owner.ToFixedPosition(Vector2Int.Min(StartPosition, EndPosition));
        var max = schema.Owner.ToFixedPosition(Vector2Int.Max(StartPosition, EndPosition));

        for (int i = min.x; i <= max.x; i++)
        {
            for (int j = min.y; j <= max.y; j++)
            {
                schema.AddTile(new Vector2Int(i, j), toSet);
            }
        }
    }
}
