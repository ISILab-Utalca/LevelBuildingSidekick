using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ISILab.Commons.Utility.Editor;
using LBS.Bundles;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements.Editor
{
    public class BundleManagerWindow : EditorWindow
    {   
        // Bundle lists
        private readonly List<Bundle> _allBundles = new List<Bundle>();
        private readonly List<MasterBundleContainer> _masterBundles = new List<MasterBundleContainer>();
        
        private readonly List<MasterBundleContainer> _interiorBundles = new List<MasterBundleContainer>();
        private readonly List<MasterBundleContainer> _exteriorBundles = new List<MasterBundleContainer>();
        private readonly List<MasterBundleContainer> _populationBundles = new List<MasterBundleContainer>();
        private readonly List<MasterBundleContainer> _unassignedBundles = new List<MasterBundleContainer>();
        
        private List<Bundle> _subBundles = new List<Bundle>();
        private readonly List<Bundle> _orphanBundles = new List<Bundle>();
        
        // Visual Elements
        private ListView _interiorList;
        private ListView _exteriorList;
        private ListView _populationList;
        private ListView _unassignedList;

        private ListView _subBundleList;
        private ListView _orphanList;


        [MenuItem("Window/ISILab/Bundle Manager")]
        public static void ShowWindow()
        {
            GetWindow<BundleManagerWindow>("Bundle Manager");
        }

        private void CreateGUI()
        {
            // Collect all bundles in project
            SearchAllBundles();
            
            // Create window
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("BundleManagerWindow");
            visualTree.CloneTree(rootVisualElement);
            
            // Explicit height for every row so ListView can calculate how many items to actually display
            const int itemHeight = 50;
            
            // Setting interior list
            SetMasterBundleViewSettings(out _interiorList, "Interior", itemHeight, _interiorBundles);
            
            // Setting exterior list
            SetMasterBundleViewSettings(out _exteriorList, "Exterior", itemHeight, _exteriorBundles);
            
            // Setting population list
            SetMasterBundleViewSettings(out _populationList, "Population", itemHeight, _populationBundles);
            
            // Setting unassigned list
            SetMasterBundleViewSettings(out _unassignedList, "Unassigned", itemHeight, _unassignedBundles);
            
            // Setting sub-bundle list
            //SetBundleViewSettings(out _subBundleList, "SubBundles", itemHeight, _subBundles);
            _subBundleList = rootVisualElement.Q<VisualElement>("SubBundles").Q<ListView>();
            
            _subBundleList.itemsSource = _subBundles;
            _subBundleList.fixedItemHeight = itemHeight;
            _subBundleList.makeItem = () => new BundleManagerElement();
            _subBundleList.bindItem = (e, i) => ((BundleManagerElement)e).bundleName.text = _subBundles[i].Name;
            _subBundleList.selectedIndicesChanged += objects =>
            {
                if (objects.Count() <= 0)
                {
                    return;
                }
                ClearSelectionInOtherLists("SubBundles");
                Selection.activeObject = _subBundles[objects.First()];
            };
            
            // Setting orphan list
            SetBundleViewSettings(out _orphanList, "OrphanBundles", itemHeight, _orphanBundles);
        }
        
        void SearchAllBundles()
        {
            string[] getGUIDs = AssetDatabase.FindAssets("t:Bundle");
            foreach (string guid in getGUIDs)
            {
                _allBundles.Add((Bundle)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Bundle)));
            }
            
            for(int i = 0; i < _allBundles.Count; i++)
            {
                if (_allBundles[i].ChildsBundles.Count > 0)
                {
                    MasterBundleContainer mBundle = new MasterBundleContainer(_allBundles[i]);
                    foreach (var bundle in _allBundles[i].ChildsBundles)
                    {
                        mBundle.AddSubBundle(bundle);
                    }
                    _masterBundles.Add(mBundle);
                }
                
                if (_allBundles[i].ChildsBundles.Count <= 0 && _allBundles[i].Parent() == null)
                {
                    _orphanBundles.Add(_allBundles[i]);
                }
            }

            foreach (var mBundle in _masterBundles)
            {
                switch (mBundle.GetMasterBundle().LayerContentFlags)
                {
                    case BundleFlags.Exterior:
                        _exteriorBundles.Add(mBundle);
                        break;
                    case BundleFlags.Interior:
                        _interiorBundles.Add(mBundle);
                        break;
                    case BundleFlags.Population:
                        _populationBundles.Add(mBundle);
                        break;
                    default:
                        _unassignedBundles.Add(mBundle);
                        break;
                }
            }
        }

        void ClearSelectionInOtherLists(string noClear)
        {
            if (!noClear.Equals("Interior"))
            {
                _interiorList.ClearSelection();
            }
            if (!noClear.Equals("Exterior"))
            {
                _exteriorList.ClearSelection();
            }
            if (!noClear.Equals("Population"))
            {
                _populationList.ClearSelection();
            }
            if (!noClear.Equals("Unassigned"))
            {
                _unassignedList.ClearSelection();
            }
            if (!noClear.Equals("SubBundles"))
            {
                _subBundleList.ClearSelection();
            }
            if (!noClear.Equals("OrphanBundles"))
            {
                _orphanList.ClearSelection();
            }
        }

        void SetMasterBundleViewSettings(out ListView listView, string columnName, int itemHeight, List<MasterBundleContainer> masterBundles)
        {
            listView = rootVisualElement.Q<VisualElement>(columnName).Q<ListView>();
            
            listView.itemsSource = masterBundles;
            listView.fixedItemHeight = itemHeight;
            listView.makeItem = () => new BundleManagerElement();
            listView.bindItem = (e, i) => ((BundleManagerElement)e).bundleName.text = masterBundles[i].GetMasterBundle().Name;

            listView.selectedIndicesChanged += objects =>
            {
                //Skip when there's no selection
                if (objects.Count() <= 0)
                {
                    return;
                }
                
                ClearSelectionInOtherLists(columnName);
                
                _subBundleList.itemsSource = masterBundles[objects.First()].GetSubBundles();
                _subBundleList.RefreshItems();
                
                Selection.activeObject = masterBundles[objects.First()].GetMasterBundle();
            };
        }

        void SetBundleViewSettings(out ListView listView, string columnName, int itemHeight, List<Bundle> bundles)
        {
            listView = rootVisualElement.Q<VisualElement>(columnName).Q<ListView>();
            
            listView.itemsSource = bundles;
            listView.fixedItemHeight = itemHeight;
            listView.makeItem = () => new BundleManagerElement();
            listView.bindItem = (e, i) => ((BundleManagerElement)e).bundleName.text = bundles[i].Name;
            
            listView.selectedIndicesChanged += objects =>
            {
                if (objects.Count() <= 0)
                {
                    return;
                }
                ClearSelectionInOtherLists(columnName);
                Selection.activeObject = bundles[objects.First()];
            };
        }
        
        private class MasterBundleContainer
        {
            private Bundle _master;
            private List<Bundle> _subBundles;

            public MasterBundleContainer(Bundle master)
            {
                _master = master;
                _subBundles = new List<Bundle>();
            }
            public void AddSubBundle(Bundle bundle)
            {
                _subBundles.Add(bundle);
            }
            
            public Bundle GetMasterBundle()
            {
                return _master;
            }
            public List<Bundle> GetSubBundles()
            {
                return _subBundles;
            }
        }
    }
}
