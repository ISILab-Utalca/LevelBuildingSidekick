using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AddEmptyTile<T> : ManipulateTileMap<T> where T : LBSTile
{
    protected override void OnMouseDown(MouseDownEvent e)
    {
        OnManipulationStart?.Invoke();

        var pos = mainView.FixPos(e.localMousePosition);
        var tile = Activator.CreateInstance(typeof(T)) as T;
        tile.Position = mainView.ToTileCords(pos);
        (tile as ConnectedTile).SetConnections("", "", "", "");
        module.AddTile(tile);

        this.OnManipulationEnd?.Invoke();
    }

    protected override void OnMouseMove(MouseMoveEvent e)
    {
        //throw new System.NotImplementedException();
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        //throw new System.NotImplementedException();
    }
}
