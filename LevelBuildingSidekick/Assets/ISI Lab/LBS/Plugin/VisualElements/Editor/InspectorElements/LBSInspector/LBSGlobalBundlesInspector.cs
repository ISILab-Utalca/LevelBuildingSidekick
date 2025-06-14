using ISILab.Commons.Utility;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Internal;
using LBS.Bundles;
using LBS.Components;
using ISILab.LBS.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Template;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [Obsolete("OLD")]
    public class LBSGlobalBundlesInspector : LBSInspector
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<LBSGlobalBundlesInspector, UxmlTraits> { }
        #endregion

        #region FIELDS VIEWS
        // bundles panel
        private ListView list;
        private DropdownField typeField;
        private Button addRoot;
        private Button addChild;
        private Button removeBtn;

        // General panel
        private GeneralBundlesPanel generalPanel;

        // Characteristic panel
        private CharacteristicsPanel characteristicsPanel;
        #endregion

        #region FIELDS
        // Internal
        private Bundle pressetSelected;
        private Bundle selected;
        private List<Tuple<Bundle, int>> targets;
        #endregion

        #region CONSTRUCTORS
        public LBSGlobalBundlesInspector()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LBSGlobalBundlesInspector");
            visualTree.CloneTree(this);

            // Bundle list
            var allBUndles = DirectoryTools.GetScriptables<Bundle>().ToList();
            var presetsBundles = allBUndles.Where(b => b.IsRoot()).ToList();
            var bundles = allBUndles.ToList();

            targets = OrderList(bundles, 0, new List<Tuple<Bundle, int>>());

            list = this.Q<ListView>("BundleList");
            list.itemsSource = targets;
            list.makeItem = MakeItem;
            list.bindItem = BindItem;
            list.selectionChanged += OnSelectionChange;
            list.style.flexGrow = 1.0f;

            // select type to add
            typeField = this.Q<DropdownField>("TypeToAdd");
            typeField.choices = presetsBundles.Select(b => b.name).ToList();
            typeField.RegisterCallback<ChangeEvent<string>>(e =>
            {
                pressetSelected = presetsBundles.Find(b => b.name.Equals(e.newValue));
            });
            typeField.value = presetsBundles[0].name;
            pressetSelected = presetsBundles[0];

            // Add button
            addRoot = this.Q<Button>("AddBrother");
            addRoot.clicked += () => CreateBundle(null);
            addChild = this.Q<Button>("AddChild");
            addChild.clicked += () => CreateBundle(selected);

            // remove button
            removeBtn = this.Q<Button>("RemoveBtn");
            removeBtn.clicked += DeleteBundle;

            generalPanel = this.Q<GeneralBundlesPanel>("GeneralPanel");
            characteristicsPanel = this.Q<CharacteristicsPanel>("CharacteristicsPanel");

            if (selected == null)
            {
                generalPanel.style.display = DisplayStyle.None;
                characteristicsPanel.style.display = DisplayStyle.None;
            }
        }
        #endregion

        #region METHODS
        private List<Tuple<Bundle, int>> OrderList(List<Bundle> bundles, int currentValue, List<Tuple<Bundle, int>> closed)
        {
            var roots = GetRoots(bundles);

            foreach (var root in roots)
            {
                if (root == null)
                    continue;

                if (closed.Select(t => t.Item1).Contains(root))
                {
                    continue;
                }

                closed.Add(new Tuple<Bundle, int>(root, currentValue));

                if (root.ChildsBundles.Count() > 0)
                {
                    var nextValue = currentValue + 1;
                    var tempClosed = OrderList(root.ChildsBundles, nextValue, closed);
                }
            }

            return closed;
        }

        private List<Bundle> GetRoots(List<Bundle> bundles)
        {
            var toR = new List<Bundle>(bundles);

            foreach (var b in bundles)
            {
                if (b == null)
                    continue;

                if (b.IsLeaf)
                {
                    b.ChildsBundles.ForEach(c => toR.Remove(c));
                }
            }

            return toR;
        }

        private VisualElement MakeItem()
        {
            return new BundleAssetView();
        }

        private void BindItem(VisualElement ve, int index)
        {
            if (index >= targets.Count())
                return;

            var view = ve as BundleAssetView;
            view.SetInfo(targets[index].Item1, targets[index].Item2);
        }

        private void OnSelectionChange(IEnumerable<object> objects)
        {
            selected = (objects.ToList()[0] as Tuple<Bundle, int>).Item1;

            generalPanel.style.display = DisplayStyle.Flex;
            characteristicsPanel.style.display = DisplayStyle.Flex;
            generalPanel.SetInfo(selected);
            characteristicsPanel.SetInfo(selected);

            Selection.SetActiveObjectWithContext(selected, selected);
        }

        private void CreateBundle(Bundle parent)
        {
            var settings = LBSSettings.Instance;
            var storage = LBSAssetsStorage.Instance;

            var clone = pressetSelected.Clone() as Bundle;
            var name = Format.CheckNameFormat(targets.Select(b => b.Item1.name), pressetSelected.name);

            AssetDatabase.CreateAsset(clone, settings.paths.bundleFolderPath + "/" + name + ".asset");
            AssetDatabase.SaveAssets();

            var all = storage.Get<Bundle>().ToList();
            targets = OrderList(all, 0, new List<Tuple<Bundle, int>>());
            list.itemsSource = targets;

            list.Rebuild();
        }

        private void DeleteBundle()
        {
            if (selected == null)
                return;

            var storage = LBSAssetsStorage.Instance;

            var path = AssetDatabase.GetAssetPath(selected);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();

            var all = storage.Get<Bundle>().ToList();
            targets = OrderList(all, 0, new List<Tuple<Bundle, int>>());
            list.itemsSource = targets;

            list.Rebuild();
        }

        public override void InitCustomEditors(ref List<LBSLayer> layers)
        {
            throw new NotImplementedException();
        }

        public override void SetTarget(LBSLayer layer)
        {
        }
        #endregion
    }
}