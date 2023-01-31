using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class RoomsPalleteInspector<T,U> : LBSInspector where T: TiledArea<U> where U : LBSTile
{

    public Action<AreaTileMap<T,U>> OnSelectionChange;

    private VisualElement content;

    public RoomsPalleteInspector()
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("RoomsPalleteInspector");
        visualTree.CloneTree(this);

        // Content
        content = this.Q<VisualElement>("Content");

        //LBSNodeView<LBSNode>
    }

    public override void Init(List<IManipulatorLBS> lBSManipulators, ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        var tiledArea = layer.GetModule<AreaTileMap<T,U>>();

        content.Clear();
        var areas = tiledArea.Areas;
        foreach (var area in areas)
        {
            var btn = new Button();
            btn.style.width = btn.style.height = 64;
            btn.text = area.ID;
            btn.style.backgroundColor = area.Color;
            content.Add(btn);
        }
    }
}
