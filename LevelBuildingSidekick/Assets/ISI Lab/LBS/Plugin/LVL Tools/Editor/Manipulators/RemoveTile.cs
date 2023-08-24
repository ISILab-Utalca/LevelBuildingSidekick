using LBS.Components;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RemoveTile : ManipulateTeselation
{
    public override void Init(LBSLayer layer, object behaviour)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
    {
        
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int position, MouseMoveEvent e)
    {
        //throw new System.NotImplementedException();
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
    {
        var min = this.module.Owner.ToFixedPosition(Vector2Int.Min(StartPosition, EndPosition));
        var max = this.module.Owner.ToFixedPosition(Vector2Int.Max(StartPosition, EndPosition));

        for (int i = min.x; i <= max.x; i++)
        {
            for (int j = min.y; j <= max.y; j++)
            {
                var pos = new Vector2Int(i, j);

                var tile = module.GetTile(pos);

                if (tile == null)
                    continue;

                module.RemoveTile(tile);
            }
        }
    }
}
