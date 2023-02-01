using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class PopulationPalleteInspector<T> : LBSInspector where T : TiledArea
{
    public Action<AreaTileMap<T>> OnSelectionChange;
    private VisualElement content;

    public PopulationPalleteInspector()
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("PopulationPalleteInspector");
        visualTree.CloneTree(this);

        content = this.Q<VisualElement>("Content");
    }

    public override void Init(List<IManipulatorLBS> lBSManipulators, ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        
        var tiledArea = layer.GetModule<PopulationTileMap<T>>();
        if (tiledArea == null)
        {
            Debug.LogError("The specified module could not be found in the layer.");
            return;
        }

        content.Clear();
        var areas = tiledArea.Areas;
        foreach (var area in areas)
        {
            var btn = new Button();
            btn.style.width = btn.style.height = 64;
            btn.text = area.ID;
            btn.style.backgroundColor = area.Color;

            btn.clicked += () => {
                foreach (var manipulator in lBSManipulators)
                {
                    var _area = area;
                    if (manipulator is PopulationTiledArea)
                    {
                        var mani = manipulator as PopulationTiledArea;
                        mani.areaToSet = _area;
                    }
                }
                OnSelectionChange?.Invoke(tiledArea);
            };

            content.Add(btn);
        }
    }

    public void SelectElement(int index)
    {
        var btns = content.Children().ToList();
        foreach (var btn in btns)
        {
            btn.RemoveFromClassList("selected");
        }
        btns[index].AddToClassList("selected");
    }
}
