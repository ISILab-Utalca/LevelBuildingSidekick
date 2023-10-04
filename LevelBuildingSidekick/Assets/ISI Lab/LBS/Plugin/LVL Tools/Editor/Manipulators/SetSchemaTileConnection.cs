using LBS;
using LBS.Behaviours;
using LBS.Components;
using LBS.Components.TileMap;
using LBS.Tools.Transformer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SetSchemaTileConnection : LBSManipulator
{
    private List<Vector2Int> Directions => global::Directions.Bidimencional.Edges; 

    private string toSet;
    private SchemaBehaviour schema;
    private Vector2Int first;

    public string ToSet
    {
        get => toSet;
        set => toSet = value;
    }

    public SetSchemaTileConnection() : base()
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

    protected override void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e)
    {
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
    {
        if (toSet == null)
        {
            Debug.LogWarning("No tienens ninguna connecion seleccionada para colocar.");
            return;
        }

        // Get tile in first position
        var t1 = schema.GetTile(first);

        // Cheack if valid
        if (t1 == null)
            return;

        // Get second fixed position
        var pos = schema.Owner.ToFixedPosition(position);

        // Get vector direction
        var dx = (t1.Position.x - pos.x);
        var dy = (t1.Position.y - pos.y);

        // Get index of direction
        var fDir = Directions.FindIndex(d => d.Equals(-new Vector2Int(dx, dy)));

        // Check if index is validate
        if (fDir < 0 || fDir >= schema.Directions.Count)
            return;

        // Get tile in second position
        var t2 = schema.GetTile(pos);

        if (t2 == null)
        {
            schema.SetConnection(t1, fDir, toSet, false);
            return;
        }

        if (t1.Equals(t2))
            return;

        if (Mathf.Abs(dx) + Mathf.Abs(dy) > 1f)
            return;

        var tDir = schema.Directions.FindIndex(d => d.Equals(new Vector2Int(dx, dy)));

        schema.SetConnection(t1, fDir, toSet, false);
        schema.SetConnection(t2, tDir, toSet, false);
    }
}

