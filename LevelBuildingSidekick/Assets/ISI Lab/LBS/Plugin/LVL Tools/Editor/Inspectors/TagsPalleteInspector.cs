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

public class TagsPalleteInspector<T> : LBSInspector where T : LBSTile
{
    //public Action<T> OnSelectionChange;

    private VisualElement content;
    private DropdownField dropdownBundles;

    private List<LBSTagsBundle> bundles;

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
        bundles = Utility.DirectoryTools.GetScriptables<LBSTagsBundle>();

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
        var bundle = bundles[i];

        foreach (var tag in bundle.tags)
        {
            var btn = new Button();
            btn.style.width = btn.style.height = 64;
            btn.text = tag.value;
            btn.style.color = new Color(1f - tag.color.r, 1f - tag.color.g, 1f - tag.color.b);
            btn.style.backgroundColor = tag.color;

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