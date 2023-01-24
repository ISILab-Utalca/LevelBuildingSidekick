using LBS.Components;
using LBS.Components.Teselation;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AddTileToTiledAreaAtPoint<T,U> : LBSManipulator where T : TiledArea<U> where U : LBSTile
{
    private AreaTileMap<T,U> module;
    private MainView mainView;

    public override void Init(ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        this.module = layer.GetModule<AreaTileMap<T,U>>();
        this.mainView = view;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown); 
    }

    private void OnMouseDown(MouseDownEvent e)
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
}
