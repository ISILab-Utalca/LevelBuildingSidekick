using LBS;
using LBS.Behaviours;
using LBS.Bundles;
using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Obsolete("OLD")]
public abstract class ManipulateTaggedTileMap : LBSManipulator
{
    public Bundle bundleToSet; 
    protected TaggedTileMap module;

    public ManipulateTaggedTileMap() : base()
    {
        feedback = new AreaFeedback();
        feedback.fixToTeselation = true;
    }

    public override void Init(LBSLayer layer, object owner)
    {
        this.module = layer.GetModule<TaggedTileMap>();
        feedback.TeselationSize = layer.TileSize;
        layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
    }
}
