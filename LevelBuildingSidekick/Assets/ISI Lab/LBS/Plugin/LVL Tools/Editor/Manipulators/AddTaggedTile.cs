using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AddTaggedTile : ManipulateTaggedTileMap
{
    protected override void OnMouseDown(MouseDownEvent e)
    {
        OnManipulationStart?.Invoke();

        var pos = mainView.ToTileCords(mainView.FixPos(e.localMousePosition));
        var t = new LBSTile(pos, "Tile: " + pos);
        module.AddTile(t, bundleToSet);

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
