using LBS.Components;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class ManipulateTeselation : LBSManipulator
{

    protected TileMapModule module;

    public ManipulateTeselation() : base() 
    {
        feedback = new AreaFeedback();
        feedback.fixToTeselation = true;
    }

    public override void Init(MainView view, LBSLayer layer, LBSBehaviour behaviour)
    {
        this.module = layer.GetModule<TileMapModule>();
        feedback.TeselationSize = layer.TileSize;
        layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
        this.MainView = view;
    }

}
