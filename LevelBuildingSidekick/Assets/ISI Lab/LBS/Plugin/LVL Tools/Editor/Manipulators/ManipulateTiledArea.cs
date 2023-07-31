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

    public ManipulateTiledArea() : base()
    {
        feedback = new AreaFeedback();
        feedback.fixToTeselation = true;
    }

    public override void Init(MainView view, LBSLayer layer, LBSBehaviour behaviour)
    {
        this.module = layer.GetModule<AreaTileMap<T>>();
        this.MainView = view;
    }

}
