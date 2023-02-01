using LBS.Components;
using LBS.Components.Teselation;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AddTileToTiledAreaAtFree<T,U> : ManipulateTiledArea<T, U> where T : TiledArea where U : LBSTile
{
    private List<Vector2Int> tiles;

    protected override void OnMouseDown(MouseDownEvent e)
    {
        OnManipulationStart?.Invoke();
        tiles = new List<Vector2Int>();
    }

    protected override void OnMouseMove(MouseMoveEvent e)
    {
        var pos = mainView.FixPos(e.localMousePosition);
        var tPos = mainView.ToTileCords(pos);

        if (!tiles.Contains(tPos))
            tiles.Add(tPos);
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        foreach (var tPos in tiles)
        {
            var tile = Activator.CreateInstance(typeof(U)) as U;
            tile.Position = tPos;
            areaToSet?.AddTile(tile);
        }
        OnManipulationEnd?.Invoke();
    }

}
