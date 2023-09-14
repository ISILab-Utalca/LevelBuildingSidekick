using LBS;
using LBS.Behaviours;
using LBS.Components;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AddConnection : LBSManipulator
{
    public LBSIdentifier tagToSet;
    public ConnectedTileMapModule module;

    private List<Vector2Int> dirs = new List<Vector2Int>() // (!) esto deberia estar en un lugar general
    {
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.up
    };

    private TileConnectionsPair first;

    public AddConnection() : base()
    {
        feedback = new ConnectedLine();
        feedback.fixToTeselation = true;
    }

    public override void Init(LBSLayer layer, object owner)
    {
        this.module = layer.GetModule<ConnectedTileMapModule>();
        feedback.TeselationSize = layer.TileSize;
        layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
    {
        //OnManipulationStart?.Invoke();
        var pos = module.Owner.ToFixedPosition(position);
        var tile = module.Pairs.Find(t => t.Tile.Position == pos);

        //var tile = e.target as ExteriorTileView;
        if (tile == null)
            return;

        first = tile;
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int position, MouseMoveEvent e)
    {
        //throw new System.NotImplementedException();
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
    {
        if (first == null)
            return;

        var pos = module.Owner.ToFixedPosition(position);

        var dx = (first.Tile.Position.x - pos.x);
        var dy = (first.Tile.Position.y - pos.y);
        var fDir = dirs.FindIndex(d => d.Equals(-new Vector2Int(dx, dy)));

        if (fDir < 0 || fDir >= dirs.Count)
            return;

        var tile = module.Pairs.Find(t => t.Tile.Position == pos);

        if (tile == null)
        {
            first.SetConnection(fDir, tagToSet.Label, false);
            return;
        }

        if (first == tile)
            return;

        if (Mathf.Abs(dx) + Mathf.Abs(dy) > 1f)
            return;

        var tDir = dirs.FindIndex(d => d.Equals(new Vector2Int(dx, dy)));

        first.SetConnection(fDir, tagToSet.Label, false);
        tile.SetConnection(tDir, tagToSet.Label, false);

    }
}
