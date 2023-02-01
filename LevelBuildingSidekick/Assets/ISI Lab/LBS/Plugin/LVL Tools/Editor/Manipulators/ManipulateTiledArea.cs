using LBS.Components;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class ManipulateTiledArea<T, U> : LBSManipulator where T : TiledArea where U : LBSTile
{
    public TiledArea areaToSet;
    protected AreaTileMap<T> module;
    protected MainView mainView;

    public override void Init(ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        this.module = layer.GetModule<AreaTileMap<T>>();
        this.mainView = view;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        target.RegisterCallback<MouseUpEvent>(OnMouseUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
    }

    protected abstract void OnMouseDown(MouseDownEvent e);

    protected abstract void OnMouseMove(MouseMoveEvent e);

    protected abstract void OnMouseUp(MouseUpEvent e);
}
