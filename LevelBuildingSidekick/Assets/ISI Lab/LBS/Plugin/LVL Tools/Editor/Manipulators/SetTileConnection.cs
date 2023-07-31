using LBS.Components;
using LBS.Components.TileMap;
using LBS.Tools.Transformer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SetTileConnection : LBSManipulator
{
    string toSet;
    SchemaBehaviour schema;
    Vector2Int first;

    public SetTileConnection() : base()
    {
        feedback = new ConnectedLine();
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
        first = schema.Owner.ToFixedPosition(position);
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e)
    {
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
    {
        var t1 = schema.GetTile(first);
        if (t1 == null)
            return;

        var pos = schema.Owner.ToFixedPosition(position);

        var dx = (t1.Position.x - pos.x);
        var dy = (t1.Position.y - pos.y);

        var fDir = schema.Directions.FindIndex(d => d.Equals(-new Vector2Int(dx, dy)));

        if (fDir < 0 || fDir >= schema.Directions.Count)
            return;

        var t2 = schema.GetTile(pos);

        if (t2 == null)
        {
            schema.SetConnection(t1, fDir, toSet);
            return;
        }

        if (t1.Equals(t2))
            return;

        if (Mathf.Abs(dx) + Mathf.Abs(dy) > 1f)
            return;

        var tDir = schema.Directions.FindIndex(d => d.Equals(new Vector2Int(dx, dy)));

        schema.SetConnection(t1, fDir, toSet);
        schema.SetConnection(t2, tDir, toSet);
    }
}

