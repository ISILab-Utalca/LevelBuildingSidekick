using LBS.Behaviours;
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

[Obsolete("OLD")]
public class TagsPalleteInspector //: LBSInspector
{
    /*
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
    }

    public override void Init(List<IManipulatorLBS> lBSManipulators, MainView view, LBSLayer layer, LBSBehaviour behaviour)
    {
        var c = Utility.DirectoryTools.GetScriptables<LBSIdentifierBundle>();
        bundles = c.Where(b => b.type == LBSIdentifierBundle.TagType.Aesthetic).ToList();

        dropdownBundles.choices = bundles.Select(b => b.name).ToList();
        dropdownBundles.index = 0;
        dropdownBundles.RegisterCallback<ChangeEvent<string>>(e => {
            var manis = lBSManipulators;
            RefreshPallete(dropdownBundles.index, manis);
        });

        RefreshPallete(dropdownBundles.index, lBSManipulators);
    }
    
    public override void OnLayerChange(LBSLayer layer)
    {

    }

    public void RefreshPallete(int i, List<IManipulatorLBS> lBSManipulators)
    {
        / *
        content.Clear();
        var bundle = bundles[i].Tags;

        foreach (var tag in bundle)
        {
            var cont = new VisualElement();
            var label = new Label();
            var btn = new Button();
            cont.Add(btn);
            cont.Add(label);

            label.text = tag.Label;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;

            btn.style.width = btn.style.height = 64;
            btn.style.color = new Color(1f - tag.Color.r, 1f - tag.Color.g, 1f - tag.Color.b);
            btn.style.backgroundColor = tag.Color;
            if(tag.Icon != null)
            btn.style.backgroundImage = tag.Icon;

            btn.clicked += () => {
                foreach (var manipulator in lBSManipulators)
                {
                    var mani = manipulator as AddConnection<ConnectedTile>;
                    mani.tagToSet = tag;
                }
            };

            foreach (var manipulator in lBSManipulators)
            {
                var mani = manipulator as AddConnection<ConnectedTile>;
                mani.tagToSet = tag;
            }
            content.Add(cont);
        }
        * /

    }
    */

}
