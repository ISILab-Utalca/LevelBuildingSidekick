using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RemoveDoor<T, U> : ManipulateTiledArea<T, U> where T : TiledArea where U : LBSTile
{
    private readonly List<Vector2Int> dirs = new List<Vector2Int>() // (!) esto deberia estar en un lugar general
    {
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.up
    };

    private ConnectedTile first;

    public RemoveDoor() : base()
    {
        feedback = new ConectedLine();
        feedback.fixToTeselation = true;
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
    {
        var tile = e.target as SchemaTileView;
        if (tile == null)
            return;

        first = tile.Data;
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int MovePosition, MouseMoveEvent e)
    {
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        if (first == null)
            return;


        var r1 = module.GetArea(first.Position);

        var pos = module.Owner.ToFixedPosition(endPosition);

        var dx = (first.Position.x - pos.x);
        var dy = (first.Position.y - pos.y);
        var fDir = dirs.FindIndex(d => d.Equals(-new Vector2Int(dx, dy)));

        if (fDir < 0 || fDir >= dirs.Count)
            return;

        if (e.target is MainView)
        {
            first.SetConnection("Wall", fDir);
            return;
        }

        var tile = e.target as SchemaTileView;

        if (tile == null)
            return;

        var second = tile.Data;

        var r2 = module.GetArea(second.Position);
        if (r1.Equals(r2))
            return;


        if (Mathf.Abs(dx) + Mathf.Abs(dy) > 1f)
            return;

        var tDir = dirs.FindIndex(d => d.Equals(new Vector2Int(dx, dy)));

        first.SetConnection("Wall", fDir);
        second.SetConnection("Wall", tDir);
    }
}
