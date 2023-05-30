using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AddEmptyTile<T> : ManipulateTeselation<T> where T : LBSTile
{
    protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
    {
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int position, MouseMoveEvent e)
    {
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
    {
        var min = this.module.Owner.ToFixedPosition(Vector2Int.Min(StartPosition, EndPosition));
        var max = this.module.Owner.ToFixedPosition(Vector2Int.Max(StartPosition, EndPosition));

        for (int i = min.x; i <= max.x; i++)
        {
            for (int j = min.y; j <= max.y; j++)
            {
                var tile = Activator.CreateInstance(typeof(T)) as T;
                tile.Position = new Vector2Int(i, j);
                (tile as ConnectedTile).SetConnections("", "", "", "");
                module.AddTile(tile);
            }
        }
    }
}
