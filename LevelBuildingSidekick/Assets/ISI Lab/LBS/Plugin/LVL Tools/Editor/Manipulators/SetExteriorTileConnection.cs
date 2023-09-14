using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Behaviours;
using LBS.Components;
using LBS.Components.TileMap;
using LBS.Tools.Transformer;
using UnityEngine.UIElements;
using LBS;

public class SetExteriorTileConnection : LBSManipulator
{
    private List<Vector2Int> Directions => global::Directions.Bidimencional.Edges;

    private LBSIdentifier toSet;
    private ExteriorBehaviour exterior;
    private Vector2Int first;

    public LBSIdentifier ToSet
    {
        get => toSet;
        set => toSet = value;
    }

    public SetExteriorTileConnection() : base()
    {
        feedback = new ConnectedLine();
        feedback.fixToTeselation = true;
    }

    public override void Init(LBSLayer layer, object behaviour)
    {
        exterior = behaviour as ExteriorBehaviour;
        feedback.TeselationSize = layer.TileSize;
        layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
    {
        first = exterior.Owner.ToFixedPosition(position);
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e)
    {
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
    {
        // Get tile in first position
        var t1 = exterior.GetTile(first);

        // Cheack if valid
        if (t1 == null)
            return;

        // Get second fixed position
        var pos = exterior.Owner.ToFixedPosition(position);

        // Get vector direction
        var dx = (t1.Position.x - pos.x);
        var dy = (t1.Position.y - pos.y);

        // Get index of direction
        var fDir = Directions.FindIndex(d => d.Equals(-new Vector2Int(dx, dy)));

        if (fDir < 0 || fDir >= exterior.Directions.Count)
            return;

        var t2 = exterior.GetTile(pos);

        if (t2 == null)
        {
            exterior.SetConnection(t1, fDir, toSet.Label, false);
            return;
        }

        if (t1.Equals(t2))
            return;

        if (Mathf.Abs(dx) + Mathf.Abs(dy) > 1f)
            return;

        var tDir = exterior.Directions.FindIndex(d => d.Equals(new Vector2Int(dx, dy)));

        exterior.SetConnection(t1, fDir, toSet.Label, false);
        exterior.SetConnection(t2, tDir, toSet.Label, false);
    }
}

