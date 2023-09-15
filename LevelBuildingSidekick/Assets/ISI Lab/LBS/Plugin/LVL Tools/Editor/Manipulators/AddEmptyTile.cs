using LBS.Components;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AddEmptyTile : ManipulateTeselation
{
    private ExteriorBehaviour exterior;

    public override void Init(LBSLayer layer, object owner)
    {
        base.Init(layer, owner);

        exterior = owner as ExteriorBehaviour;
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
    {

    }

    protected override void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e)
    {

    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        var corners = exterior.Owner.ToFixedPosition(StartPosition, EndPosition);

        for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
        {
            for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
            {
                var pos = new Vector2Int(i, j);
                var tile = new LBSTile(pos);
                exterior.AddTile(tile);
            }
        }
    }

}
