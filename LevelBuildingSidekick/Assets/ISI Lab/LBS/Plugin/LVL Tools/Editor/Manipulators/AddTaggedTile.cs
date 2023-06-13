using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AddTaggedTile : ManipulateTaggedTileMap
{
    protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
    {
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e)
    {
        //throw new System.NotImplementedException();
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        var min = this.module.Owner.ToFixedPosition(Vector2Int.Min(StartPosition, EndPosition));
        var max = this.module.Owner.ToFixedPosition(Vector2Int.Max(StartPosition, EndPosition));

        for (int i = min.x; i <= max.x; i++)
        {
            for (int j = min.y; j <= max.y; j++)
            {
                var pos = new Vector2Int(i, j);
                var t = new LBSTile(pos, "Tile: " + pos);
                module.AddTile(t, bundleToSet);
                /*var tile = Activator.CreateInstance(typeof(T)) as T;
                tile.Position = new Vector2Int(i, j);
                (tile as ConnectedTile).SetConnections("", "", "", "");
                module.AddTile(tile);*/
            }
        }
    }
}
