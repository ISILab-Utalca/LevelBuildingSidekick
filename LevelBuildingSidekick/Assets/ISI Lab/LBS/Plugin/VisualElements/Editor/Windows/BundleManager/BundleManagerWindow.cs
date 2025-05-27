using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.Extensions;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Internal;
using LBS.Bundles;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISI_Lab.LBS.Plugin.VisualElements.Editor.Windows.BundleManager
{
    public class BundleManagerWindow : EditorWindow
    {
        // Explicit height for every row so ListView can calculate how many items to actually display
        private const int ItemHeight = 32;
        private const int ItemGap = 2;
        
        // References
        private VectorImage _arrowDown;
        private VectorImage _arrowSide;
        
        // Bundle lists
        private List<Bundle> _allBundles = new();
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
            
            //Find all bundles in database
            SearchAllBundles();
            
            // Find issues in bundles
            FindWarnings();
            
            // Create window
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("BundleManagerWindow");
            visualTree.CloneTree(rootVisualElement);
            
            // Setting MasterBundle lists
            SetBundleViewSettings(out _interiorList, "Interior", _interiorBundles, true);
            SetBundleViewSettings(out _exteriorList, "Exterior", _exteriorBundles, true);
            SetBundleViewSettings(out _populationList, "Population", _populationBundles, true);
            SetBundleViewSettings(out _unassignedList, "Unassigned", _unassignedBundles, true);
            
            // Setting Bundle lists
            SetBundleViewSettings(out _subBundleList, "SubBundles", _subBundles);
            SetBundleViewSettings(out _orphanList, "OrphanBundles", _orphanBundles);
            SetBundleViewSettings(out _validatorList, "BundleValidator", new List<BundleContainer>());
            
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
                rootVisualElement.Q<VisualElement>("SubBundles").Q<Label>().text = "Sub Bundles - Layer";
                ClearSelectionInOtherLists();
                SearchAllBundles();
                FindWarnings();
                
                _interiorList.RefreshItems();
                _exteriorList.RefreshItems();
                _populationList.RefreshItems();
                _unassignedList.RefreshItems();
                _subBundleList.RefreshItems();
                _orphanList.RefreshItems();
                
                SetExpandButtonSetting("SubBundles", _subBundleList);
                SetBundleViewSettings(out _validatorList, "BundleValidator", new List<BundleContainer>());
                SetExpandButtonSetting("BundleValidator", _validatorList);
            };
            
            // Setting findIssues button
            Button issuesButton = rootVisualElement.Q<VisualElement>("BottomBar").Q<Button>("IssuesButton");
            issuesButton.clickable.clicked += () =>
            {
                rootVisualElement.Q<VisualElement>("SubBundles").Q<Label>().text = "Sub Bundles - Layer";
                ClearSelectionInOtherLists();
                _subBundles.Clear();
                FindWarnings();
                
                _interiorList.RefreshItems();
                _exteriorList.RefreshItems();
                _populationList.RefreshItems();
                _unassignedList.RefreshItems();
                _subBundleList.RefreshItems();
                _orphanList.RefreshItems();
                
                SetExpandButtonSetting("SubBundles", _subBundleList);
                SetBundleViewSettings(out _validatorList, "BundleValidator", new List<BundleContainer>());
                SetExpandButtonSetting("BundleValidator", _validatorList);
            };
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
            _subBundles.Clear();
            
            _allBundles =  LBSAssetsStorage.Instance.Get<Bundle>();
            
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

        /// <summary>
        /// Searches through the BundleContainer lists to find dangerous or invalid settings and records them as warning in each corresponding BundleContainer.
        /// </summary>
        void FindWarnings()
        {
            // ----------------------------------- CLEAR ALL WARNINGS -----------------------------------
            foreach (BundleContainer bundleContainer in _masterBundles)
            {
                bundleContainer.ClearWarnings();
                foreach (BundleContainer subBundleContainer in bundleContainer.GetSubBundles())
                {
                    subBundleContainer.ClearWarnings();
                }
            }
            foreach (BundleContainer orphanContainer in _orphanBundles)
            {
                orphanContainer.ClearWarnings();
            }
            
            // ----------------------------------- GLOBAL CASES -----------------------------------
            List<BundleContainer> allContainers = new();
            allContainers.AddRange(_masterBundles);
            allContainers.AddRange(_orphanBundles);
            foreach (BundleContainer bundleContainer in _masterBundles)
            {
                allContainers.AddRange(bundleContainer.GetSubBundles());
            }

            foreach (BundleContainer bundleContainer in allContainers)
            {
                Bundle bundle = bundleContainer.GetMainBundle();
                
                // Case 0: Null bundle
                if (bundle == null)
                {
                    bundleContainer.AddWarning("Bundle is null. Use Organize Folder button to clear empty bundles.");
                    continue;
                }
                
                // Case 1: No characteristics
                if (bundle.Characteristics.Count <= 0)
                {
                    bundleContainer.AddWarning("There are no characteristic assigned to this bundle.");
                }
                
                for (var i = 0; i < bundle.Characteristics.Count; i++)
                {
                    var cha = bundle.Characteristics[i];
                    
                    // Case 1.1: Characteristics empty
                    if (cha == null)
                    {
                        bundleContainer.AddWarning("Characteristic " + i + " is null.");
                        continue;
                    }
                    
                    // Particular cases (it depends and is defined on the type of the characteristic)
                    List<string> warnings = cha.Validate();
                    foreach (var w in warnings)
                    {
                        bundleContainer.AddWarning(w);
                    }
                }
            }


            // ----------------------------------- MASTER BUNDLES -----------------------------------
            // Case 0: Unassigned type in master bundles
            foreach (BundleContainer bundle in _unassignedBundles)
            {
                bundle.AddWarning("Layer Content Flag is none.");
            }
            
            foreach (BundleContainer bundleContainer in _masterBundles) 
            {
                Bundle masterBundle = bundleContainer.GetMainBundle();
                
                // Case 1: Prefab in master bundle
                if (masterBundle.Assets.Count > 0)
                {
                    bundleContainer.AddWarning("Master bundle contains assets of their own; should have it as subBundle, or be one.");
                }
                
                // ----------------------------------- SUB BUNDLES -----------------------------------
                foreach (BundleContainer subBundleContainer in bundleContainer.GetSubBundles()) 
                {
                    Bundle subBundle = subBundleContainer.GetMainBundle();
                    
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
                }
            }
        }
        
        /// <summary>
        /// Sets a given listView with elements from a list of BundleContainer.
        /// </summary>
        /// <param name="listView">ListView to set.</param>
        /// <param name="columnName">Name of the VisualElement in window that contains the specific ListView.</param>
        /// <param name="bundles">List that will be used as itemSource for the ListView.</param>
        /// <param name="master">Bundles with subBundles should be set as "master" bundles.</param>
        void SetBundleViewSettings(out ListView listView, string columnName, List<BundleContainer> bundles, bool master = false)
        {   
            // Get listView
            listView = rootVisualElement.Q<VisualElement>(columnName).Q<ListView>();
            
            // Set listView params
            listView.itemsSource = bundles;
            listView.fixedItemHeight = ItemHeight;
            
            // Set listView methods
            var view = listView;
            listView.makeItem = () => new BundleManagerElement(ItemHeight, ItemGap);
            listView.bindItem = (e, i) =>
            {
                BundleManagerElement element = (BundleManagerElement)e;
                BundleContainer container = (BundleContainer)view.itemsSource[i];
                
                element.SetRefs(container.GetMainBundle(), view, true);
                element.SetIconDisplay(BundleManagerElement.Icons.Master, master);
                element.SetIconDisplay(BundleManagerElement.Icons.Warning, container.GetWarnings().Count > 0);
            };

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
                    _subBundles.AddRange(subBundles);
                    
                    rootVisualElement.Q<VisualElement>("SubBundles").Q<Label>().text = "Sub Bundles - " + bundle.GetMainBundle().name;
                    
                    SetBundleViewSettings(out _subBundleList, "SubBundles", _subBundles);
                    _subBundleList.RefreshItems();
                    SetExpandButtonSetting("SubBundles", _subBundleList);
                }
                
                // Set validator list
                SetValidatorViewSettings(bundle);
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
        private void SetValidatorViewSettings(BundleContainer selected)
        {   
            // Get listView
            _validatorList = rootVisualElement.Q<VisualElement>("BundleValidator").Q<ListView>();
            
            // Set listView params
            _validatorList.itemsSource = selected.GetWarnings();
            _validatorList.fixedItemHeight = ItemHeight * 2;
            
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
        void ClearSelectionInOtherLists(string noClear = null)
        {
            noClear ??= "NoMatch";
            
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
