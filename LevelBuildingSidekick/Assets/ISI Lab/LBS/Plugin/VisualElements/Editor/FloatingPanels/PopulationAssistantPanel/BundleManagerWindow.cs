using System;
using System.Collections.Generic;
using ISILab.Commons.Utility.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements.Editor
{
    public class BundleManagerWindow : EditorWindow
    {
        private VisualElement _interiorColumn;
        private ListView _interiorList;
        
        private VisualElement _exteriorColumn;
        private ListView _exteriorList;
        
        private VisualElement _populationColumn;
        private ListView _populationList;

        [MenuItem("Window/ISILab/Bundle Manager")]
        public static void ShowWindow()
        {
            GetWindow<BundleManagerWindow>("Bundle Manager");
        }
        private void CreateGUI()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("BundleManagerWindow");
            visualTree.CloneTree(rootVisualElement);
            
            _interiorColumn = rootVisualElement.Q<VisualElement>("Interior");
            _interiorList = _interiorColumn.Q<ListView>();
            
            List<BundleManagerElement> items = new List<BundleManagerElement>();
            for (int i = 0; i < 5; i++)
            {
                BundleManagerElement b = new BundleManagerElement();
                b.bundleName.text = "olaa";
                items.Add(b);
            }
            
            // Provide the list view with an explicit height for every row
            // so it can calculate how many items to actually display
            const int itemHeight = 50;
            
            _interiorList.itemsSource = items;
            _interiorList.fixedItemHeight = itemHeight;
            _interiorList.bindItem = (e, i) => e = items[i];

            _interiorList.style.flexGrow = 1.0f;
            rootVisualElement.MarkDirtyRepaint();
        }
    }
}
