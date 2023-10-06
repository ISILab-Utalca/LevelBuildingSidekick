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
        var corners = this.exterior.Owner.ToFixedPosition(StartPosition, EndPosition);

        for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
        {
            for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
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
