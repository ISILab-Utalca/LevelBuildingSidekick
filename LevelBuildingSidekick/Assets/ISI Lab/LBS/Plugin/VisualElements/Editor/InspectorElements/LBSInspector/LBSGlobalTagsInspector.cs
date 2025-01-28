using ISILab.LBS.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Components;
using ISILab.Commons.Utility;
using ISILab.Commons.Utility.Editor;
using ISILab.Extensions;
using ISILab.LBS.Internal;
using ISILab.LBS.Components;

namespace ISILab.LBS.VisualElements
{
    public class LBSGlobalTagsInspector : LBSInspector
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<LBSGlobalTagsInspector, UxmlTraits> { }
        #endregion

        #region FIELDS
        private LBSIdentifierBundle selected;
        #endregion

        #region VIEW FIELDS
        private ListView list;
        private Button AddBtn;
        private Button RemoveBtn;
        private TagInfo tagInfo;
        #endregion

        #region PROPERTIES
        private List<LBSIdentifierBundle> TagsBundles
        {
            get
            {
                var storage = LBSAssetsStorage.Instance;
                var tagsBundles = storage.Get<LBSIdentifierBundle>();
                return tagsBundles;
            }
        }
        #endregion

        #region CONSTRUCTORS
        public LBSGlobalTagsInspector()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LBSGlobalTagsInspector");
            visualTree.CloneTree(this);

            // Add Button
            AddBtn = this.Q<Button>("Plus");
            AddBtn.clicked += CreateBundle;

            // Remove Button
            RemoveBtn = this.Q<Button>("Less");
            RemoveBtn.clicked += DeleteBundle;

            // BundleList
            list = this.Q<ListView>();
            list.makeItem = MakeItem;
            list.bindItem = BindItem;
            list.itemsChosen += OnItemChosen;
            list.selectionChanged += OnSelectionChange;
            list.style.flexGrow = 1.0f;
            list.itemsSource = TagsBundles;

            // TagInfo
            tagInfo = this.Q<TagInfo>();
            tagInfo.style.display = DisplayStyle.None;
        }
        #endregion

        #region METHODS
        private VisualElement MakeItem()
        {
            var tbv = new TagBundleView();
            tbv.OnSelectionChange += (tbv) => SelectedTagChange(tbv.selected);
            tbv.OnRemoveTag += (tbv) => tagInfo.SetDisplay(false);

            return tbv;
        }

        private void BindItem(VisualElement ve, int index)
        {
            if (index >= TagsBundles.Count())
                return;

            var view = ve as TagBundleView;
            view.SetInfo(TagsBundles[index]);

            if (index == TagsBundles.Count - 1)
            {
                view.Children().ToList()[0].style.borderBottomLeftRadius = 3;
                view.Children().ToList()[0].style.borderBottomRightRadius = 3;
                view.Children().ToList()[0].SetBorder(Color.red, 0);
            }
        }

        private void DeleteBundle()
        {
            if (selected == null)
                return;

            var path = AssetDatabase.GetAssetPath(selected);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();

            list.itemsSource = TagsBundles;

            list.Rebuild();
        }

        private void CreateBundle()
        {
            var nSO = ScriptableObject.CreateInstance<LBSIdentifierBundle>();

            var settings = LBSSettings.Instance;

            var name = Format.CheckNameFormat(TagsBundles.Select(b => b.name), "tagBundle");

            AssetDatabase.CreateAsset(nSO, settings.paths.tagFolderPath + "/" + name + ".asset");
            AssetDatabase.SaveAssets();

            list.itemsSource = TagsBundles;

            list.Rebuild();

        }

        private void OnItemChosen(IEnumerable<object> objects)
        {

        }

        private void OnSelectionChange(IEnumerable<object> objects)
        {
            selected = objects.ToList()[0] as LBSIdentifierBundle;
        }

        private void SelectedTagChange(LBSTag tag)
        {
            tagInfo.SetInfo(tag);
        }

        public override void SetTarget(LBSLayer layer)
        {
            //Debug.Log("Actualizacion de layer Global/Tags inspector");
        }
        #endregion
    }
}