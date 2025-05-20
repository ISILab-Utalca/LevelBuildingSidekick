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
        private readonly List<BundleContainer> _masterBundles = new();
        
        private readonly List<BundleContainer> _interiorBundles = new ();
        private readonly List<BundleContainer> _exteriorBundles = new ();
        private readonly List<BundleContainer> _populationBundles = new ();
        private readonly List<BundleContainer> _unassignedBundles = new ();
        
        private readonly List<BundleContainer> _subBundles = new ();
        private readonly List<BundleContainer> _orphanBundles = new ();
        
        // ListViews
        private ListView _interiorList;
        private ListView _exteriorList;
        private ListView _populationList;
        private ListView _unassignedList;

        private ListView _subBundleList;
        private ListView _orphanList;
        private ListView _validatorList;

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
            SetBundleViewSettings(out _interiorList, "Interior", itemHeight, _interiorBundles, true);
            SetBundleViewSettings(out _exteriorList, "Exterior", itemHeight, _exteriorBundles, true);
            SetBundleViewSettings(out _populationList, "Population", itemHeight, _populationBundles, true);
            SetBundleViewSettings(out _unassignedList, "Unassigned", itemHeight, _unassignedBundles, true);
            
            // Setting Bundle lists
            SetBundleViewSettings(out _subBundleList, "SubBundles", itemHeight, _subBundles);
            SetBundleViewSettings(out _orphanList, "OrphanBundles", itemHeight, _orphanBundles);
            SetBundleViewSettings(out _validatorList, "BundleValidator", itemHeight, new List<BundleContainer>());
            
            // Setting Expand List Buttons
            SetExpandButtonSetting("Interior", _interiorList);
            SetExpandButtonSetting("Exterior", _exteriorList);
            SetExpandButtonSetting("Population", _populationList);
            SetExpandButtonSetting("Unassigned", _unassignedList);
            SetExpandButtonSetting("SubBundles", _subBundleList);
            SetExpandButtonSetting("OrphanBundles", _orphanList);
            SetExpandButtonSetting("BundleValidator", _validatorList);
            
            // Setting other Buttons
            Button button = rootVisualElement.Q<VisualElement>("BottomBar").Q<Button>("OrganizeButton");
            button.clickable.clicked += () =>
            {
                SearchAllBundles();
                _interiorList.RefreshItems();
                _exteriorList.RefreshItems();
                _populationList.RefreshItems();
                _unassignedList.RefreshItems();
                _subBundleList.RefreshItems();
                _orphanList.RefreshItems();
            };
            
            button = rootVisualElement.Q<VisualElement>("BottomBar").Q<Button>("IssuesButton"); 
        }
        
        void SearchAllBundles()
        {
            //Clear lists
            _allBundles.Clear();
            _masterBundles.Clear();
            _orphanBundles.Clear();
            _exteriorBundles.Clear();
            _interiorBundles.Clear();
            _populationBundles.Clear();
            _unassignedBundles.Clear();
            
            //Find all bundles in database
            string[] getGUIDs = AssetDatabase.FindAssets("t:Bundle");
            foreach (string guid in getGUIDs)
            {
                _allBundles.Add((Bundle)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Bundle)));
            }
            
            foreach (var b in _allBundles)
            {
                switch (b.ChildsBundles.Count)
                {
                    case > 0:   //Bundle has children = MasterBundle
                    {
                        BundleContainer mBundle = new BundleContainer(b, b.ChildsBundles);
                        _masterBundles.Add(mBundle);
                        break;
                    }
                    case <= 0 when b.Parent() == null:  //Bundle has no children and no parent = OrphanBundle
                        BundleContainer oBundle = new BundleContainer(b);
                        _orphanBundles.Add(oBundle);
                        break;
                }
            }
            
            //Divide MasterBundles by content
            foreach (var mBundle in _masterBundles)
            {
                switch (mBundle.GetMainBundle().LayerContentFlags)
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
            
            Debug.Log("BundleManagerWindow updated");
        }
        
        void SetBundleViewSettings(out ListView listView, string columnName, int itemHeight, List<BundleContainer> masterBundles, bool master = false)
        {   
            // Get listView
            listView = rootVisualElement.Q<VisualElement>(columnName).Q<ListView>();
            
            // Set listView params
            listView.itemsSource = masterBundles;
            listView.fixedItemHeight = itemHeight;
            
            // Set listView methods
            var view = listView;
            listView.makeItem = () => new BundleManagerElement();
            listView.bindItem = (e, i) => ((BundleManagerElement)e).SetRefs(((BundleContainer)view.itemsSource[i]).GetMainBundle(), view, true);

            listView.selectedIndicesChanged += objects =>
            {
                // Omit empty selections
                var selections = objects as int[] ?? objects.ToArray();
                if (selections.Length <= 0)
                {
                    return;
                }
                
                ClearSelectionInOtherLists(columnName);
                
                // Set subBundle list
                if (master)
                {
                    // Set _subBundle list of BundleContainer, using subBundles from selected item
                    List<Bundle> subBundles = ((BundleContainer)view.itemsSource[selections.First()]).GetSubBundles();
                    _subBundles.Clear();
                    foreach (Bundle b in subBundles)
                    {
                        _subBundles.Add(new BundleContainer(b));
                    }
                    
                    SetBundleViewSettings(out _subBundleList, "SubBundles", itemHeight, _subBundles);
                    _subBundleList.RefreshItems();
                    SetExpandButtonSetting("SubBundles", _subBundleList);
                }
                
                // Set validator list
                SetValidatorViewSettings((BundleContainer)view.itemsSource[selections.First()], master, itemHeight);
                
                // Display selection on inspector
                Selection.activeObject = masterBundles[selections.First()].GetMainBundle();
            };

            listView.itemsChosen += objects =>
            {
                Selection.activeObject = masterBundles[view.selectedIndex].GetMainBundle();
            };
        }
         void SetValidatorViewSettings(BundleContainer selected , bool master, int itemHeight)
        {   
            // Get listView
            _validatorList = rootVisualElement.Q<VisualElement>("BundleValidator").Q<ListView>();
            
            // Set listView params
            _validatorList.itemsSource = selected.GetWarnings();
            _validatorList.fixedItemHeight = itemHeight;
            
            // Set listView methods
            var view = _validatorList;
            _validatorList.makeItem = () => new BundleManagerWarning();
            _validatorList.bindItem = (e, i) => ((BundleManagerWarning)e).SetWarningContent((string)_validatorList.itemsSource[i]);

            _validatorList.selectedIndicesChanged += objects =>
            {
                Selection.activeObject = selected.GetMainBundle();
            };

            _validatorList.itemsChosen += objects =>
            {
                Selection.activeObject = selected.GetMainBundle();
            };
        }
        void SetExpandButtonSetting(string columnName, ListView list)
        {
            Button button = rootVisualElement.Q<VisualElement>(columnName).Q<Button>("ExpandButton");
            button.iconImage = Background.FromVectorImage(list.GetDisplay() ? _arrowDown : _arrowSide);

            var auxButton = button;
            button.clickable.clicked += () =>
            {
                list.SetDisplay(!list.GetDisplay());
                auxButton.iconImage = Background.FromVectorImage(list.GetDisplay() ? _arrowDown : _arrowSide);
            };

            list.SetDisplay(list.itemsSource.Count > 0);
            auxButton.iconImage = Background.FromVectorImage(list.GetDisplay() ? _arrowDown : _arrowSide);
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
            if (!noClear.Equals("BundleValidator"))
            {
                _validatorList.ClearSelection();
            }
        }
        public class BundleContainer
        {
            private Bundle _master;
            private List<Bundle> _subBundles;
            private List<string> _warnings;

            public BundleContainer(Bundle master, List<Bundle> subBundles = null)
            {
                _master = master;
                _subBundles = subBundles;   
            }
            
            public Bundle GetMainBundle()
            {
                return _master;
            }
            public List<Bundle> GetSubBundles()
            {
                return _subBundles;
            }

            public List<string> GetWarnings()
            {
                return _warnings;
            }

            public void AddWarning(string warning)
            {
                _warnings.Add(warning);
            }
        }
    }
}
