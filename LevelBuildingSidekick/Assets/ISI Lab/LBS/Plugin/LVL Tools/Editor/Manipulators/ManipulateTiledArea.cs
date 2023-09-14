using LBS;
using LBS.Behaviours;
using LBS.Components;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Obsolete("OLD")]
public abstract class ManipulateTiledArea<T, U> : LBSManipulator where T : TiledArea where U : LBSTile
{
    public TiledArea areaToSet;

    protected AreaTileMap<T> module;

    public ManipulateTiledArea() : base()
    {
        feedback = new AreaFeedback();
        feedback.fixToTeselation = true;
    }

    public override void Init(LBSLayer layer, object behaviour)
    {
        this.module = layer.GetModule<AreaTileMap<T>>();
    }

}
