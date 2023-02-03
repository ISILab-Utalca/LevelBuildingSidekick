using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RemoveConnection<T> : ManipulateTileMap<T> where T : LBSTile
{
    private List<Vector2Int> dirs = new List<Vector2Int>() // (!) esto deberia estar en un lugar general
    {
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.up
    };

    private ConnectedTile first;

    protected override void OnMouseDown(MouseDownEvent e)
    {
        OnManipulationStart?.Invoke();

        var tile = e.target as ExteriorTileView;
        if (tile == null)
            return;

        first = tile.Data;
    }

    protected override void OnMouseMove(MouseMoveEvent e)
    {
        //throw new System.NotImplementedException();
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        if (first == null)
            return;

        var tile = e.target as ExteriorTileView;
        if (tile == null)
            return;

        var second = tile.Data;

        var dx = (first.Position.x - second.Position.x);
        var dy = (first.Position.y - second.Position.y);

        if (Mathf.Abs(dx) + Mathf.Abs(dy) > 1f)
            return;

        var fDir = dirs.FindIndex(d => d.Equals(-new Vector2Int(dx, dy)));
        var tDir = dirs.FindIndex(d => d.Equals(new Vector2Int(dx, dy)));

        first.SetConnection("", fDir);
        second.SetConnection("", tDir);

        OnManipulationEnd?.Invoke();
    }


}
