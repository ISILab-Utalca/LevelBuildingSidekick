using System.Collections.Generic;
using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.Extensions;
using ISILab.LBS.Characteristics;
using LBS.Bundles;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISI_Lab.LBS.Plugin.VisualElements.Editor.Windows.BundleManager
{
    public class BundleManagerWindow : EditorWindow
    {
        // References
        private VectorImage _arrowDown;
        private VectorImage _arrowSide;
        
        // Bundle lists
        private readonly List<Bundle> _allBundles = new();
        private readonly List<BundleContainer> _masterBundles = new();
        
        private readonly List<BundleContainer> _interiorBundles = new();
        private readonly List<BundleContainer> _exteriorBundles = new();
        private readonly List<BundleContainer> _populationBundles = new();
        private readonly List<BundleContainer> _unassignedBundles = new();
        
        private readonly List<BundleContainer> _subBundles = new();
        private readonly List<BundleContainer> _orphanBundles = new();
        
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
            
            // Find issues in bundles
            FindWarnings();
            
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
            
            // Setting organize button
            Button organizeButton = rootVisualElement.Q<VisualElement>("BottomBar").Q<Button>("OrganizeButton");
            organizeButton.clickable.clicked += () =>
            {
                SearchAllBundles();
                _interiorList.RefreshItems();
                _exteriorList.RefreshItems();
                _populationList.RefreshItems();
                _unassignedList.RefreshItems();
                _subBundleList.RefreshItems();
                _orphanList.RefreshItems();
            };
            
            Button issuesButton = rootVisualElement.Q<VisualElement>("BottomBar").Q<Button>("IssuesButton");
            issuesButton.clickable.clicked += FindWarnings;
        }
        
        /// <summary>
        /// Finds all bundles in project and sets their reference in the BundleContainer lists.
        /// </summary>
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
                        List<BundleContainer> subBundles = new();
                        foreach (Bundle cb in b.ChildsBundles)
                        {
                            subBundles.Add(new BundleContainer(cb));
                        }
                        
                        BundleContainer mBundle = new BundleContainer(b, subBundles);
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

        void FindWarnings()
        {
            // CLEAR ALL WARNINGS
            foreach (BundleContainer b in _masterBundles)
            {
                b.ClearWarnings();
                foreach (BundleContainer sbc in b.GetSubBundles())
                {
                    sbc.ClearWarnings();
                }
            }

            // Case 0: Unassigned type in master bundles
            foreach (BundleContainer bundle in _unassignedBundles)
            {
                bundle.AddWarning("Layer Content Flag is none.");
            }
            
            foreach (BundleContainer bundleContainer in _masterBundles)  // MASTER BUNDLES
            {
                Bundle masterBundle = bundleContainer.GetMainBundle();
                
                // Case 1: Prefab in master bundle
                if (masterBundle.Assets.Count > 0)
                {
                    bundleContainer.AddWarning("Master bundle contains assets of their own; should have it as subBundle, or be one.");
                }
                
                // Case 2: No characteristics
                if (masterBundle.Characteristics.Count <= 0)
                {
                    bundleContainer.AddWarning("There are no characteristic assigned to this bundle.");
                }
                
                // Case 2.1: Characteristic empty
                for (int i = 0; i < masterBundle.Characteristics.Count; i++)
                {
                    LBSCharacteristic cha = masterBundle.Characteristics[i];
                    if (cha == null)
                    {
                        bundleContainer.AddWarning("Characteristic " + i + " is null");
                    }
                }
                
                foreach (BundleContainer subBundleContainer in bundleContainer.GetSubBundles()) // SUB BUNDLES
                {
                    Bundle subBundle = subBundleContainer.GetMainBundle();
                    // Case 0: Different parent
                    if (!subBundle.Parents().Contains(bundleContainer.GetMainBundle()))
                    {
                        var parents = subBundle.Parents().Aggregate("", (current, p) => current + (" " + p.name));
                        subBundleContainer.AddWarning("SubBundle's parents doesn't contain assigned master bundle. Identified parents: " + parents);
                    }
                    
                    // Case 1: No asset assigned
                    if (subBundle.Assets.Count <= 0)
                    {
                        subBundleContainer.AddWarning("SubBundle has no asset assigned.");
                    }
                    
                    // Case 1.1: No prefab in asset
                    for(int i = 0; i < subBundle.Assets.Count; i++)
                    {
                        Asset a = subBundle.Assets[i];
                        if (a.obj == null)
                        {
                            subBundleContainer.AddWarning("Asset " + i + " has no prefab assigned.");
                        }
                    }
                    
                    // Case 2: No characteristics
                    if (subBundle.Characteristics.Count <= 0)
                    {
                        subBundleContainer.AddWarning("There are no characteristic assigned to this bundle.");
                    }
                    
                    // Case 2.1: Characteristic empty
                    for (int n = 0; n < subBundle.Characteristics.Count; n++)
                    {
                        LBSCharacteristic subCha = subBundle.Characteristics[n];
                        if (subCha == null)
                        {
                            subBundleContainer.AddWarning("Characteristic " + n + " is null");
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Sets a given listView with elements from a list of BundleContainer.
        /// </summary>
        /// <param name="listView">ListView to set.</param>
        /// <param name="columnName">Name of the VisualElement in window that contains the specific ListView.</param>
        /// <param name="itemHeight">Height of each element in the ListView.</param>
        /// <param name="bundles">List that will be used as itemSource for the ListView.</param>
        /// <param name="master">Bundles with subBundles should be set as "master" bundles.</param>
        void SetBundleViewSettings(out ListView listView, string columnName, int itemHeight, List<BundleContainer> bundles, bool master = false)
        {   
            // Get listView
            listView = rootVisualElement.Q<VisualElement>(columnName).Q<ListView>();
            
            // Set listView params
            listView.itemsSource = bundles;
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
                
                BundleContainer bundle = (BundleContainer)view.itemsSource[selections.First()];
                ClearSelectionInOtherLists(columnName);
                
                // Set subBundle list
                if (master)
                {
                    // Set _subBundle list of BundleContainer, using subBundles from selected item
                    List<BundleContainer> subBundles = bundle.GetSubBundles();
                    _subBundles.Clear();
                    foreach (BundleContainer b in subBundles)
                    {
                        _subBundles.Add(b);
                    }
                    
                    SetBundleViewSettings(out _subBundleList, "SubBundles", itemHeight, _subBundles);
                    _subBundleList.RefreshItems();
                    SetExpandButtonSetting("SubBundles", _subBundleList);
                }
                
                // Set validator list
                SetValidatorViewSettings(bundle, itemHeight * 2);
                SetExpandButtonSetting("BundleValidator", _validatorList);
                
                // Display selection on inspector
                Selection.activeObject = bundles[selections.First()].GetMainBundle();
            };

            listView.itemsChosen += _ =>
            {
                Selection.activeObject = bundles[view.selectedIndex].GetMainBundle();
            };
        }
        
        /// <summary>
        /// Sets a fixed ListView to show warning messages for a specified BundleContainer.
        /// </summary>
        /// <param name="selected">The ListView will show warnings related to the selected BundleContainer.</param>
        /// <param name="itemHeight">Height of each element in the ListView.</param>
        private void SetValidatorViewSettings(BundleContainer selected, int itemHeight)
        {   
            // Get listView
            _validatorList = rootVisualElement.Q<VisualElement>("BundleValidator").Q<ListView>();
            
            // Set listView params
            _validatorList.itemsSource = selected.GetWarnings();
            _validatorList.fixedItemHeight = itemHeight;
            
            // Set listView methods
            _validatorList.makeItem = () => new BundleManagerWarning();
            _validatorList.bindItem = (e, i) => ((BundleManagerWarning)e).SetWarningContent((string)_validatorList.itemsSource[i]);

            _validatorList.selectedIndicesChanged += _ =>
            {
                Selection.activeObject = selected.GetMainBundle();
            };

            _validatorList.itemsChosen += _ =>
            {
                Selection.activeObject = selected.GetMainBundle();
            };
        }
        
        /// <summary>
        /// Sets a button to show and hide a related ListView.
        /// </summary>
        /// <param name="columnName">Name of the VisualElement in window that contains the specific Button.</param>
        /// <param name="list">ListView that will show and hide with the button.</param>
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
        
        /// <summary>
        /// Clears selection in all ListView elements, but the specified one.
        /// </summary>
        /// <param name="noClear">Name of the ListView that won't be cleared.</param>
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
            private readonly Bundle _master;
            private readonly List<BundleContainer> _subBundles;
            private readonly List<string> _warnings;

            public BundleContainer(Bundle master, List<BundleContainer> subBundles = null)
            {
                _master = master;
                _subBundles = subBundles;
                _warnings = new List<string>();
            }
            
            public Bundle GetMainBundle()
            {
                return _master;
            }
            public List<BundleContainer> GetSubBundles()
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

            public void ClearWarnings()
            {
                _warnings.Clear();
            }
        }
    }
}
