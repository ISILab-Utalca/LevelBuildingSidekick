using LBS;
using LBS.Behaviours;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RemoveTileConnection : LBSManipulator
{
    private List<Vector2Int> Directions => global::Directions.Bidimencional.Edges;

    private SchemaBehaviour schema;
    private Vector2Int first;

    public RemoveTileConnection() : base()
    {
        feedback = new ConnectedLine();
        feedback.fixToTeselation = true;
    }

    public override void Init(LBSLayer layer, object behaviour)
    {
        schema = behaviour as SchemaBehaviour;
        feedback.TeselationSize = layer.TileSize;
        layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
    {
        first = schema.Owner.ToFixedPosition(position);
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int position, MouseMoveEvent e)
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

        var fDir = Directions.FindIndex(d => d.Equals(-new Vector2Int(dx, dy)));
        //var fDir = schema.Connections.FindIndex(d => d.Equals(-new Vector2Int(dx, dy)));

        if (fDir < 0 || fDir >= schema.Directions.Count)
            return;

        var t2 = schema.GetTile(pos);

        if (t2 == null)
        {
            schema.SetConnection(t1, fDir, "", true);
            return;
        }

        if (t1.Equals(t2))
            return;

        if (Mathf.Abs(dx) + Mathf.Abs(dy) > 1f)
            return;

        var tDir = schema.Directions.FindIndex(d => d.Equals(new Vector2Int(dx, dy)));

        schema.SetConnection(t1, fDir, "", true);
        schema.SetConnection(t2, tDir, "", true);
        schema.RecalculateWalls();
    }
}
