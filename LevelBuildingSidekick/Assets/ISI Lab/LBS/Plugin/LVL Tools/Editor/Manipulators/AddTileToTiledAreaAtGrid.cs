using LBS.Components;
using LBS.Components.Teselation;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AddTileToTiledAreaAtGrid<T,U> : ManipulateTiledArea<T, U> where T : TiledArea where U : LBSTile
{
    private Vector2Int startPos;

    protected override void OnMouseDown(MouseDownEvent e)
    {
        OnManipulationStart?.Invoke();
        var fixPos = mainView.FixPos(e.localMousePosition);
        startPos = mainView.ToTileCords(fixPos);
    }

    protected override void OnMouseMove(MouseMoveEvent e)
    {
        // do nothing
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        var tPos1 = startPos;
        var fixPos = mainView.FixPos(e.localMousePosition);
        var tPos2 = mainView.ToTileCords(fixPos);

        for (int i = tPos1.y; i <= tPos2.y; i++)
        {
            for (int j = tPos1.x; j <= tPos2.x; j++)
            {
                var x = Activator.CreateInstance(typeof(T)) as T;
                var tile = Activator.CreateInstance(typeof(U)) as U; // (!) esto solo esta para 4 conectados
                tile.Position = new Vector2Int(i,j);
                areaToSet?.AddTile(tile);
            }
        }
        OnManipulationEnd?.Invoke();
    }
}
