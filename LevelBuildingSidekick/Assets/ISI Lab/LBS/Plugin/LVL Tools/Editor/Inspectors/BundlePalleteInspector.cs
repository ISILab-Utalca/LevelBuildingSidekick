using LBS.Behaviours;
using LBS.Bundles;
using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[Obsolete("OLD")]
public class BundlePalleteInspector //: LBSInspector
{
    /*
    private VisualElement content;
    private DropdownField dropdownBundles;

    private List<LBSIdentifierBundle> currentBundles;

    public BundlePalleteInspector()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("TagsPalleteInspector");
        visualTree.CloneTree(this);

        // Content
        content = this.Q<VisualElement>("Content");

        dropdownBundles = this.Q<DropdownField>("DropdownBundles");
    }

    public override void Init(List<IManipulatorLBS> lBSManipulators, MainView view, LBSLayer layer, LBSBehaviour behaviour)
    {
        // Una mejor opción podría buscar identifier bundles y sacar los bundles que tengan esos identifiers.
        var allBundles = Utility.DirectoryTools.GetScriptables<LBSIdentifierBundle>();
        currentBundles = allBundles.Where(b => b.type == LBSIdentifierBundle.TagType.Element).ToList();

        dropdownBundles.choices = currentBundles.Select(b => b.name).ToList();
        dropdownBundles.index = 0;
        dropdownBundles.RegisterCallback<ChangeEvent<string>>(e => {
            //var manis = lBSManipulators;
            RefreshPallete(dropdownBundles.index, lBSManipulators);
        });

        RefreshPallete(dropdownBundles.index, lBSManipulators);
    }

    public override void OnLayerChange(LBSLayer layer)
    {

    }

    public void RefreshPallete(int index, List<IManipulatorLBS> lBSManipulators)
    {
        content.Clear();
        var tags = currentBundles[index].Tags;

        foreach (var tag in tags)
        {
            var ve = CreateElement(tag, lBSManipulators);
            content.Add(ve);
        }
    }

    private VisualElement CreateElement(LBSIdentifier identifier, List<IManipulatorLBS> manipulators)
    {
        var cont = new VisualElement();
        var label = new Label();
        var btn = new Button();
        cont.Add(btn);
        cont.Add(label);

        label.text = identifier.name;
        label.style.unityTextAlign = TextAnchor.MiddleCenter;

        btn.style.width = btn.style.height = 64;
        btn.style.backgroundColor = identifier.Color;
        if (identifier.Icon != null)
            btn.style.backgroundImage = identifier.Icon;

        btn.clicked += () => {
            foreach (var manipulator in manipulators)
            {
                SetManipulator(manipulator,identifier);
            }
        };

        foreach (var manipulator in manipulators)
        {
            SetManipulator(manipulator,identifier);
        }
        return cont;
    }

    private void SetManipulator(IManipulatorLBS manipulator, LBSIdentifier identifier)
    {
        var mani = manipulator as ManipulateTaggedTileMap;
        mani.bundleToSet = SearchBundleByIdentifier(identifier);
    }

    private Bundle SearchBundleByIdentifier(LBSIdentifier identifier)
    {
       var bundles = LBSAssetsStorage.Instance.Get<Bundle>();
        var label = identifier.Label;

        foreach (var bundle in bundles)
        {
            var id = bundle.Name;

            if (d.Equals(label))
                return bundle;
        }
        return null;
    }
    */
}
