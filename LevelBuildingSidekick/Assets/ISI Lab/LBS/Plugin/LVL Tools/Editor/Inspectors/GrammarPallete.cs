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
public class GrammarPallete //: LBSInspector
{
    /*
    private VisualElement content;
    private DropdownField dropdownBundles;

    private List<LBSGrammar> grammars;

    public GrammarPallete()
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("TagsPalleteInspector");
        visualTree.CloneTree(this);

        // Content
        content = this.Q<VisualElement>("Content");

        dropdownBundles = this.Q<DropdownField>("DropdownBundles");
    }

    public override void Init(List<IManipulatorLBS> lBSManipulators, MainView view, LBSLayer layer, LBSBehaviour behaviour)
    {
        grammars = Utility.DirectoryTools.GetScriptables<LBSGrammar>();

        dropdownBundles.choices = grammars.Select(b => b.name).ToList();
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
        content.Clear();
        var grammarActions = grammars[i].GrammarTree.GetActions();

        foreach (var action in grammarActions)
        {
            var btn = new Button();
            btn.style.width = btn.style.height = 64;
            btn.text = action.ID;
            var color = new Color(0.1f, 0.1f,0.1f);
            btn.style.color = new Color(1f - color.r, 1f - color.g, 1f - color.b);
            btn.style.backgroundColor = color;

            btn.clicked += () => {
                foreach (var manipulator in lBSManipulators)
                {
                    var mani = manipulator as ManipulateGrammarGraph;
                    mani.actionToSet = action;
                }
            };

            foreach (var manipulator in lBSManipulators)
            {
                var mani = manipulator as ManipulateGrammarGraph;
                mani.actionToSet = action;
            }
            content.Add(btn);
        }

    }
    */
}
