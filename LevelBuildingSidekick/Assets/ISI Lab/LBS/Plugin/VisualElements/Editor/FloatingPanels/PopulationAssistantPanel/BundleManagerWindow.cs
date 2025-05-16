using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.Extensions;
using LBS.Bundles;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements.Editor
{
    public class BundleManagerWindow : EditorWindow
    {
        // References
        public VectorImage _arrowDown;
        public VectorImage _arrowSide;
        
        // Bundle lists
        private readonly List<Bundle> _allBundles = new List<Bundle>();
        private readonly List<MasterBundleContainer> _masterBundles = new List<MasterBundleContainer>();
        
        private readonly List<MasterBundleContainer> _interiorBundles = new List<MasterBundleContainer>();
        private readonly List<MasterBundleContainer> _exteriorBundles = new List<MasterBundleContainer>();
        private readonly List<MasterBundleContainer> _populationBundles = new List<MasterBundleContainer>();
        private readonly List<MasterBundleContainer> _unassignedBundles = new List<MasterBundleContainer>();
        
        private List<Bundle> _subBundles = new List<Bundle>();
        private readonly List<Bundle> _orphanBundles = new List<Bundle>();
        private List<Bundle> _validatorBundles = new List<Bundle>();
        
        // ListViews
        private ListView _interiorList;
        private ListView _exteriorList;
        private ListView _populationList;
        private ListView _unassignedList;

        private ListView _subBundleList;
        private ListView _orphanList;
        private ListView _validatorList;
        
        // Is ListView displaying?
        private bool _interiorDisplay = true;
        private bool _exteriorDisplay = true;
        private bool _populationDisplay = true;
        private bool _unassignedDisplay = true;
        private bool _subBundleDisplay = true;
        private bool _orphanDisplay = true;
        private bool _validatorDisplay = true;
        
        // Buttons
        private Button _interiorExpandButton;
        private Button _exteriorExpandButton;
        private Button _populationExpandButton;
        private Button _unassignedExpandButton;
        private Button _subBundleExpandButton;
        private Button _orphanExpandButton;
        private Button _validatorExpandButton;

        [MenuItem("Window/ISILab/Bundle Manager")]
        public static void ShowWindow()
        {
            GetWindow<BundleManagerWindow>("Bundle Manager");
        }

        private void CreateGUI()
        {
            //Set references
            _arrowDown = AssetDatabase.LoadAssetAtPath<VectorImage>(AssetDatabase.GUIDToAssetPath("b570a25de51f01c41bd82dbe5372bb3f")); //GUIDs
            _arrowSide = AssetDatabase.LoadAssetAtPath<VectorImage>(AssetDatabase.GUIDToAssetPath("83eafacbab9ab554299bc4d0f124d980"));
            
            // Collect all bundles in project
            SearchAllBundles();
            
            // Create window
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("BundleManagerWindow");
            visualTree.CloneTree(rootVisualElement);
            
            // Explicit height for every row so ListView can calculate how many items to actually display
            const int itemHeight = 50;
            
            // Setting MasterBundle lists
            SetMasterBundleViewSettings(out _interiorList, "Interior", itemHeight, _interiorBundles);
            SetMasterBundleViewSettings(out _exteriorList, "Exterior", itemHeight, _exteriorBundles);
            SetMasterBundleViewSettings(out _populationList, "Population", itemHeight, _populationBundles);
            SetMasterBundleViewSettings(out _unassignedList, "Unassigned", itemHeight, _unassignedBundles);
            
            // Setting Bundle lists
            SetBundleViewSettings(out _subBundleList, "SubBundles", itemHeight, _subBundles);
            SetBundleViewSettings(out _orphanList, "OrphanBundles", itemHeight, _orphanBundles);
            SetBundleViewSettings(out _validatorList, "BundleValidator", itemHeight, _validatorBundles);
            
            // Setting Buttons
            SetExpandButtonSetting(out _interiorExpandButton, "Interior", _interiorDisplay, _interiorList);
            SetExpandButtonSetting(out _exteriorExpandButton, "Exterior", _interiorDisplay, _exteriorList);
            SetExpandButtonSetting(out _populationExpandButton, "Population", _interiorDisplay, _populationList);
            SetExpandButtonSetting(out _unassignedExpandButton, "Unassigned", _interiorDisplay, _unassignedList);
            SetExpandButtonSetting(out _subBundleExpandButton, "SubBundles", _interiorDisplay, _subBundleList);
            SetExpandButtonSetting(out _orphanExpandButton, "OrphanBundles", _interiorDisplay, _orphanList);
            SetExpandButtonSetting(out _validatorExpandButton, "BundleValidator", _interiorDisplay, _validatorList);
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
            listView.bindItem = (e, i) => ((BundleManagerElement)e).SetBundleRef(masterBundles[i].GetMasterBundle());

            listView.selectedIndicesChanged += objects =>
            {
                //Skip when there's no selection
                var enumerable = objects as int[] ?? objects.ToArray();
                if (enumerable.Length <= 0)
                {
                    return;
                }
                
                ClearSelectionInOtherLists(columnName);
                
                _subBundles = masterBundles[enumerable.First()].GetSubBundles();
                SetBundleViewSettings(out _subBundleList, "SubBundles", itemHeight, _subBundles);
                _subBundleList.RefreshItems();
                
                Selection.activeObject = masterBundles[enumerable.First()].GetMasterBundle();
            };

            var view = listView;
            listView.itemsChosen += objects =>
            {
                Selection.activeObject = masterBundles[view.selectedIndex].GetMasterBundle();
            };
        }

        void SetBundleViewSettings(out ListView listView, string columnName, int itemHeight, List<Bundle> bundles)
        {
            listView = rootVisualElement.Q<VisualElement>(columnName).Q<ListView>();
            
            listView.itemsSource = bundles;
            listView.fixedItemHeight = itemHeight;
            listView.makeItem = () => new BundleManagerElement();
            
            ListView view = listView;
            listView.bindItem = (e, i) => ((BundleManagerElement)e).SetBundleRef((Bundle)view.itemsSource[i]);
            listView.selectedIndicesChanged += objects =>
            {
                var enumerable = objects as int[] ?? objects.ToArray();
                if (enumerable.Length <= 0)
                {
                    return;
                }
                
                ClearSelectionInOtherLists(columnName);
                Selection.activeObject = (Bundle)view.itemsSource[enumerable.First()];
            };
        }

        void SetExpandButtonSetting(out Button button, string columnName, bool display, ListView list)
        {
            button = rootVisualElement.Q<VisualElement>(columnName).Q<Button>("ExpandButton");
            button.iconImage = Background.FromVectorImage(display ? _arrowDown : _arrowSide);

            var auxButton = button;
            button.clickable.clicked += () =>
            {
                display = !display;
                
                auxButton.iconImage = Background.FromVectorImage(display ? _arrowDown : _arrowSide);
                list.SetDisplay(display);
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
