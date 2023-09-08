using LBS.Components;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RemoveTileExterior : ManipulateTeselation
{
    private ExteriorBehaviour exterior;

    public override void Init(LBSLayer layer, object owner)
    {
        base.Init(layer, owner);

        exterior = owner as ExteriorBehaviour;
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
    {

    }

    protected override void OnMouseMove(VisualElement target, Vector2Int position,  MouseMoveEvent e)
    {

    }

    protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
    {
        var min = exterior.Owner.ToFixedPosition(Vector2Int.Min(StartPosition, EndPosition));
        var max = exterior.Owner.ToFixedPosition(Vector2Int.Max(StartPosition, EndPosition));

        for (int i = min.x; i <= max.x; i++)
        {
            for (int j = min.y; j <= max.y; j++)
            {
                var pos = new Vector2Int(i, j);
                var tile = exterior.GetTile(pos);
                if (tile == null)
                    continue;
                exterior.RemoveTile(tile);
            }
        }
    }
}
