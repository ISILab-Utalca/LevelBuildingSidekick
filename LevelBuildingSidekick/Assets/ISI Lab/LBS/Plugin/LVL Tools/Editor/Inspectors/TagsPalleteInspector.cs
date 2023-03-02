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

public class TagsPalleteInspector : LBSInspector
{
    //public Action<T> OnSelectionChange;

    private VisualElement content;
    private DropdownField dropdownBundles;

    private List<LBSIdentifierBundle> bundles;

    public TagsPalleteInspector()
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("TagsPalleteInspector");
        visualTree.CloneTree(this);

        // Content
        content = this.Q<VisualElement>("Content");

        dropdownBundles = this.Q<DropdownField>("DropdownBundles");

        //LBSNodeView<LBSNode>
    }

    public override void Init(List<IManipulatorLBS> lBSManipulators, ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        bundles = Utility.DirectoryTools.GetScriptables<LBSIdentifierBundle>();

        dropdownBundles.choices = bundles.Select(b => b.name).ToList();
        dropdownBundles.index = 0;
        dropdownBundles.RegisterCallback<ChangeEvent<string>>(e => {
            var manis = lBSManipulators;
            RefreshPallete(dropdownBundles.index, manis);
        });

        RefreshPallete(dropdownBundles.index, lBSManipulators);
    }

    public void RefreshPallete(int i, List<IManipulatorLBS> lBSManipulators)
    {
        content.Clear();
        var bundle = bundles[i].Tags;

        foreach (var tag in bundle)
        {
            var btn = new Button();
            btn.style.width = btn.style.height = 64;
            btn.text = tag.Label;
            btn.style.color = new Color(1f - tag.Color.r, 1f - tag.Color.g, 1f - tag.Color.b);
            btn.style.backgroundColor = tag.Color;

            btn.clicked += () => {
                foreach (var manipulator in lBSManipulators)
                {
                    var mani = manipulator as ManipulateTileMap<ConnectedTile>;
                    mani.tagToSet = tag;
                }
            };

            foreach (var manipulator in lBSManipulators)
            {
                var mani = manipulator as ManipulateTileMap<ConnectedTile>;
                mani.tagToSet = tag;
            }
            content.Add(btn);
        }

    }

}
