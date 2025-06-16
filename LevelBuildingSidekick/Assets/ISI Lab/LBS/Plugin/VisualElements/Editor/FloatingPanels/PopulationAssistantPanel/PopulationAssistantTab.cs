using ISILab.LBS.Editor.Windows;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using ISI_Lab.LBS.Plugin.MapTools.Generators3D;
using UnityEditor;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.AI.Categorization;
using ISILab.LBS.Generators;
using UnityEditor.UIElements;
using Object = UnityEngine.Object;
using ISILab.LBS.AI.Assistants.Editor;
using ISILab.LBS.Assistants;
using ISILab.LBS.Behaviours;
using static UnityEditor.Experimental.GraphView.GraphView;
using ISILab.AI.Categorization;
using LBS.Components.TileMap;
using ISILab.LBS.Modules;
using ISILab.Extensions;

namespace ISILab.LBS.VisualElements.Editor
{
    public class PopulationAssistantTab : VisualElement
    {
        #region UXMLFACTORY
        [UxmlElementAttribute]
        public new class UxmlFactory { }
        #endregion
        private PopulationAssistantWindow window;

        #region VIEW ELEMENTS
        private VisualElement mapEliteContent;
        private VisualElement savedElitesContent;
        
        private Foldout mapEliteFoldout;
        
        private Button buttonMapElitesAssistant;

        private ListView mapElitesList;
        private AssistantMapElite target;
        
        #endregion

        #region FIELDS
        //private List<SavedMap> savedMapList = new ();
        private List<PopulationMapEntry> mapEntries = new ();
    
        #endregion

        #region PROPERTIES

        
        protected LBSLayer TargetLayer
        {
            get => target.OwnerLayer;
        }
        List<SavedMap> SavedMapList
        {
            get => TargetLayer.Parent.GetSavedMaps(TargetLayer)?.Maps;
            set => TargetLayer.Parent.GetSavedMaps(TargetLayer).Maps = value;
        }
        #endregion

        #region CONSTRUCTORS
        public PopulationAssistantTab(AssistantMapElite target)
        {
            this.target = target;
            window = ScriptableObject.CreateInstance<PopulationAssistantWindow>();
            window.SetAssistant(target);

            window.UpdatePins = null;
            window.UpdatePins += () =>
            {
                Debug.Log("pins updated");
                UpdateMapEntries();
                mapElitesList.Rebuild();
            };

            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("PopulationAssistantTab");
            visualTree.CloneTree(this);
            
            //Main thing
            mapEliteFoldout = this.Q<Foldout>("FoldoutMapElites");
            mapEliteContent = this.Q<VisualElement>("MapEliteContent");
            
            //Assistant button
            buttonMapElitesAssistant = this.Q<Button>("ButtonMapElitesAssistant");
            buttonMapElitesAssistant.clicked += ()=>
            {
                if (!window)
                {
                    window = ScriptableObject.CreateInstance<PopulationAssistantWindow>();
                    window.SetAssistant(target);
                }
                window.ShowWindow();
            };

            //savedElitesContent = this.Q<VisualElement>("SavedElitesContent");

            UpdateMapEntries();
            mapElitesList = this.Q<ListView>("MapElitesList");

            //False until I find out why it simply snaps back to the original order when you try to move something.
            //Guess that counts as disable by default anyway...? -Alice
            mapElitesList.reorderable = false;

            mapElitesList.makeItem = () => new PopulationMapEntry(); 
            
            mapElitesList.bindItem = (element, index) =>
            {
                var mapEntryVE = element as PopulationMapEntry;
                if (mapEntryVE == null) return;

                var mapEntry = SavedMapList[index];
                mapEntryVE.SetData(mapEntry);

                mapEntryVE.RemoveMapEntry = null;
                mapEntryVE.RemoveMapEntry += () =>
                {
                    Debug.Log("Remove at " +index);
                    mapEntries.RemoveAt(index);
                    RemoveMap(index);
                    mapElitesList.Rebuild();
                };
                mapEntryVE.ApplyMapEntry = null;
                mapEntryVE.ApplyMapEntry += () =>
                {
                    Debug.Log("applying from " + index);
                    ApplySuggestion(index);
                };
            };
            mapElitesList.itemsSource = mapEntries;
        }
        #endregion

        #region METHODS
        // should pass the preset as parameter
        /*private void AddEntry()
        {
            var mapEntry1 = ScriptableObject.CreateInstance<MAPElitesPreset>(); // not null
            savedMaps.Add(mapEntry1);

            var mapEntryVE = new PopulationMapEntry();
            mapEntries.Add(mapEntryVE);
        }*/

        private void RemoveMap(int index) => RemoveMap(SavedMapList[index].Name);
        private void RemoveMap(string name)
        {
            if (TargetLayer == null) return;
            var data = TargetLayer.Parent;
            var savedMapList = data.GetSavedMaps(TargetLayer);
            if (savedMapList == null) return;

            var maps = savedMapList.Maps;
            maps.Remove(maps.Find(c => c.Name == name));
        }
        private void UpdateMapEntries()
        {
            //Get population behavior = it's now TargetLayer!
            //Get saved maps
            if (TargetLayer == null) return;
            if (SavedMapList == null) return;

            var data = TargetLayer.Parent;
            
            //Clear map entries
            mapEntries.Clear();

            if (SavedMapList.Count>0)
            {
                foreach(SavedMap map in SavedMapList)
                {
                    //Make a new visual element to set it up later
                    var mapEntryVE = new PopulationMapEntry();
                    mapEntries.Add(mapEntryVE);
                }
            }
        }
        private void ApplySuggestion(int index) => ApplySuggestion(SavedMapList[index]);
        private void ApplySuggestion(object obj)
        {
            window.OnTileMapChanged?.Invoke();
            var savedMap = obj as SavedMap;
            var chrom = savedMap.Map;
            if (chrom == null) return;

            var level = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(level, "Add Element population");

            var layerPopulation = TargetLayer.Behaviours.Find(b => b.GetType().Equals(typeof(PopulationBehaviour))) as PopulationBehaviour;
            var rect = chrom.Rect;

            for (int i = 0; i < chrom.Length; i++)
            {
                var pos = chrom.ToMatrixPosition(i) + rect.position.ToInt();
                layerPopulation.RemoveTileGroup(pos);
                var gene = chrom.GetGene(i);
                if (gene == null)
                    continue;
                layerPopulation.AddTileGroup(pos, gene as BundleData);
            }
            DrawManager.Instance.RedrawLayer(TargetLayer, MainView.Instance);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(level);
            }
            LBSMainWindow.MessageNotify("Layer modified by Population Assistant");
        }
        #endregion

    }
}