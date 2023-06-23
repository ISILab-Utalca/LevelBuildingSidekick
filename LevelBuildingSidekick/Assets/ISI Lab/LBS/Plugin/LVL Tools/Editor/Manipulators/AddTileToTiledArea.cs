using LBS.Components;
using LBS.Components.Teselation;
using LBS.Components.TileMap;
using LBS.Tools.Transformer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AddTileToTiledArea<T,U> : ManipulateTiledArea<T, U> where T : TiledArea where U : LBSTile
{
    protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
    {

    }

    protected override void OnMouseMove(VisualElement target, Vector2Int MovePosition, MouseMoveEvent e)
    {
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        var min = this.module.Owner.ToFixedPosition(Vector2Int.Min(StartPosition, EndPosition));
        var max = this.module.Owner.ToFixedPosition(Vector2Int.Max(StartPosition, EndPosition));


        for (int i = min.x; i <= max.x; i++)
        {
            for (int j = min.y; j <= max.y; j++)
            {
                var tile = Activator.CreateInstance(typeof(U)) as U;
                tile.Position = new Vector2Int(i, j);
                tile.ID = areaToSet.ID;
                this.module.AddTile(areaToSet.ID,tile);
                (tile as ConnectedTile).SetConnections("Wall", "Wall", "Wall", "Wall");

            }
        }

        var module = this.module;
        CalculateConnections.Operate(module);

    }
}
