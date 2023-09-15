using LBS.Behaviours;
using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

[Obsolete("OLD")]
public class RoomsPalleteInspector<T,U> //: LBSInspector where T: TiledArea where U : LBSTile
{
    /*
    public Action<AreaTileMap<T>> OnSelectionChange;

    private VisualElement content;

    public RoomsPalleteInspector()
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("RoomsPalleteInspector");
        visualTree.CloneTree(this);

        // Content
        content = this.Q<VisualElement>("Content");

        //LBSNodeView<LBSNode>
    }

    public override void Init(List<IManipulatorLBS> lBSManipulators, MainView view, LBSLayer layer, LBSBehaviour behaviour)
    {
        var tiledArea = layer.GetModule<AreaTileMap<T>>();

        content.Clear();
        var areas = tiledArea.Areas;
        foreach (var area in areas)
        {
            var cont = new VisualElement();
            var label = new Label();
            var btn = new Button();
            cont.Add(btn);
            cont.Add(label);

            label.text = area.ID;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            btn.style.width = btn.style.height = 64;
            btn.style.backgroundColor = area.Color;

            btn.clicked += () => {
                foreach (var manipulator in lBSManipulators)
                {
                    var _area = area;
                    var mani = manipulator as ManipulateTiledArea<TiledArea, ConnectedTile>;
                    mani.areaToSet = _area;
                }
            };

            foreach (var manipulator in lBSManipulators)
            {
                var _area = area;
                var mani = manipulator as ManipulateTiledArea<TiledArea, ConnectedTile>;
                mani.areaToSet = _area;
            }

            content.Add(cont);
        }
    }

    public override void OnLayerChange(LBSLayer layer)
    {
        throw new NotImplementedException();
    }
    */
}
