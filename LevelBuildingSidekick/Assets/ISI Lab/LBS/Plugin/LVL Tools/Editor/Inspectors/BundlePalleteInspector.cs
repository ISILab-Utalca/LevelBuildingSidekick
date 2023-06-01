using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class BundlePalleteInspector : LBSInspector
{

    private VisualElement content;
    private DropdownField dropdownBundles;

    private List<LBSIdentifierBundle> idBundles;

    public BundlePalleteInspector()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("TagsPalleteInspector");
        visualTree.CloneTree(this);


        // Content
        content = this.Q<VisualElement>("Content");

        dropdownBundles = this.Q<DropdownField>("DropdownBundles");
    }

    public override void Init(List<IManipulatorLBS> lBSManipulators, ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        // Una mejor opción podría buscar identifier bundles y sacar los bundles que tengan esos identifiers.
        idBundles = Utility.DirectoryTools.GetScriptables<LBSIdentifierBundle>();
        
        dropdownBundles.choices = idBundles.Select(b => b.name).ToList();
        dropdownBundles.index = 0;
        dropdownBundles.RegisterCallback<ChangeEvent<string>>(e => {
            var manis = lBSManipulators;
            RefreshPallete(dropdownBundles.index, manis);
        });

        RefreshPallete(dropdownBundles.index, lBSManipulators);
    }

    public void RefreshPallete(int index, List<IManipulatorLBS> lBSManipulators)
    {
        content.Clear();
        var storage = LBSAssetsStorage.Instance;
        var bundles = storage.Get<Bundle>();
        var tags = idBundles[index].Tags;
        var ids = tags.Select(id => id?.Label).ToList();
        bundles = bundles.Where(b => ids.Contains(b.ID?.Label)).ToList();

        foreach (var b in bundles)
        {
            var btn = new Button();
            btn.style.width = btn.style.height = 64;
            btn.text = b.name;
            btn.style.color = b.ID.Color.Inverse();// + new Color(0.2f, 0.2f, 0.2f); //new Color(1f - b.ID.Color.r, 1f - b.ID.Color.g, 1f - b.ID.Color.b);
            btn.style.backgroundColor = b.ID.Color;
            if (b.ID.Icon != null)
                btn.style.backgroundImage = b.ID.Icon;

            btn.clicked += () => {
                foreach (var manipulator in lBSManipulators)
                {
                    var mani = manipulator as ManipulateTaggedTileMap;
                    mani.bundleToSet = b;
                }
            };

            foreach (var manipulator in lBSManipulators)
            {
                var mani = manipulator as ManipulateTaggedTileMap;
                mani.bundleToSet = b;
            }
            content.Add(btn);
        }

    }
}
