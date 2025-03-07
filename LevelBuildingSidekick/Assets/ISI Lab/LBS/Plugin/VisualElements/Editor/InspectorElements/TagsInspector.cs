using ISILab.Commons.Utility.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [UxmlElement]
    public partial class TagsInspector : VisualElement
    {
    //    public new class UxmlFactory : UxmlFactory<TagsInspector, UxmlTraits> { }

        private List<BundleTagView> bundleViews = new List<BundleTagView>();

        public TagsInspector()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("BundleTagView");
            visualTree.CloneTree(this);

            // Init tags bundle
            var bundles = DirectoryTools.GetScriptablesByType<LBSIdentifierBundle>().ToList();

            // Content
            var content = this.Q<VisualElement>("Content");
            foreach (var bundle in bundles)
            {
                var v = new BundleTagView(bundle);
                bundleViews.Add(v);
                content.Add(v);
            }

            // AddField
            var addField = this.Q<TextField>("NewField");

            // AddTagButton
            var addButton = this.Q<Button>("AddButton");
            addButton.clicked += () =>
            {
                var v = addField.text;
                if (v == null || v == "")
                    return;

                AddBundle(v);
            };


        }

        private void AddBundle(string name)
        {
            var so = ScriptableObject.CreateInstance<LBSIdentifierBundle>();

            string path = "Assets/" + name + "_Tag" + ".asset";
            AssetDatabase.CreateAsset(so, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = so;
        }
    }
}