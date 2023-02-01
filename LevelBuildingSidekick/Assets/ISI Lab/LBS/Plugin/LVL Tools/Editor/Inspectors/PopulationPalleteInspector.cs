using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class PopulationPalleteInspector<T> : LBSInspector where T : PopulationTiledArea 
{
    public Action<PopulationTileMap<T>> OnSelectionChange;

    private VisualElement content;

    public PopulationPalleteInspector()
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("RoomsPalleteInspector");
        visualTree.CloneTree(this);

        content = this.Q<VisualElement>("Content");
    }

    public override void Init(List<IManipulatorLBS> lBSManipulators, ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        var tiledArea = layer.GetModule<PopulationTileMap<T>>();
        content.Clear();

        var areas = tiledArea.Areas;
        foreach (var area in areas)
        {
            var btn = new Button();
            btn.style.width = btn.style.height = 64;

            var label = area.Label;
            var icon = area.Icon;

            var populationLabel = new Label(label);
            populationLabel.style.backgroundColor = Color.grey;
            btn.Add(populationLabel);

            var populationIcon = new Image { image = icon };
            btn.Add(populationIcon);

            content.Add(btn);
        }
    }
}