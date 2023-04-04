using LBS.Components.TileMap;
using LBS.Tools.Transformer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DeleteTile<T, U> : ManipulateTiledArea<T, U> where T : TiledArea where U : LBSTile
{
    protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
    {
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int MovePosition, MouseMoveEvent e)
    {
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        var min = MainView.ToTileCords(Vector2Int.Min(StartPosition, EndPosition));
        var max = MainView.ToTileCords(Vector2Int.Max(StartPosition, EndPosition));

        for (int i = min.x; i <= max.x; i++)
        {
            for (int j = min.y; j <= max.y; j++)
            {
                var pos = new Vector2Int(i, j);
                var area = module.GetArea(pos);

                if (area == null)
                    continue;

                var tile = area.GetTile(pos);

                if (tile == null)
                    continue;

                area.RemoveTile(tile);
            }
        }

        CalculateConnections.Operate(module);
    }
}
