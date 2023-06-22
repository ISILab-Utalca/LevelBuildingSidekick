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
        var c = Utility.DirectoryTools.GetScriptables<LBSIdentifierBundle>();
        idBundles = c.Where(b => b.type == LBSIdentifierBundle.TagType.Element).ToList();

        dropdownBundles.choices = idBundles.Select(b => b.name).ToList();
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
            var cont = new VisualElement();
            var label = new Label();
            var btn = new Button();
            cont.Add(btn);
            cont.Add(label);

            label.text = b.name;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;

            btn.style.width = btn.style.height = 64;
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
            content.Add(cont);
        }

    }
}
