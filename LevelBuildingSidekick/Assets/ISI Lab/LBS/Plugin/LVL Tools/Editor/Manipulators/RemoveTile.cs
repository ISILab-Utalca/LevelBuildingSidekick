using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RemoveTile : ManipulateTileMap<LBSTile>
{
    protected override void OnMouseDown(MouseDownEvent e)
    {
        OnManipulationStart?.Invoke();
        var view = e.target as TileView;

        if (view == null)
        {
            return;
        }
        var tile = view.Data;

        module.RemoveTile(tile);
        Debug.Log("M: " + module);

        OnManipulationEnd?.Invoke();
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
