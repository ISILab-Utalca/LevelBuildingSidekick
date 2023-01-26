using LBS.Components;
using LBS.Components.Teselation;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AddTileToTiledAreaAtPoint<T,U> : ManipulateTiledArea<T, U> where T : TiledArea<U> where U : LBSTile
{

    protected override void OnMouseDown(MouseDownEvent e)
    {
        OnManipulationStart?.Invoke();
        var pos = mainView.FixPos(e.localMousePosition);
        var area = Activator.CreateInstance(typeof(T)) as T;
        var tile = Activator.CreateInstance(typeof(U)) as U;
        tile.Position = mainView.ToTileCords(pos);
        area.AddTile(tile);
        module.AddArea(area);
        OnManipulationEnd?.Invoke();
    }

    protected override void OnMouseMove(MouseMoveEvent e)
    {
        //throw new NotImplementedException();
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        //throw new NotImplementedException();
    }

}
