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
        private List<SavedMap> savedMapList = new ();
        private List<PopulationMapEntry> mapEntries = new ();
    
        #endregion

        #region PROPERTIES

        List<SavedMap> SavedMapList
        {
            get => savedMapList;
            set => savedMapList = value;
        }
        protected LBSLayer TargetLayer
        {
            get => target.OwnerLayer;
        }
        #endregion

        #region CONSTRUCTORS
        public PopulationAssistantTab(AssistantMapElite target)
        {
            this.target = target;
            window = ScriptableObject.CreateInstance<PopulationAssistantWindow>();
            window.SetAssistant(target);

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
            mapElitesList.reorderable = true;

            mapElitesList.makeItem = () => new PopulationMapEntry(); 
            mapElitesList.bindItem = (element, index) =>
            {
                var mapEntryVE = element as PopulationMapEntry;
                if (mapEntryVE == null) return;

                var mapEntry = savedMapList[index];
                mapEntryVE.SetData(mapEntry);

                mapEntryVE.RemoveMapEntry = null;
                mapEntryVE.RemoveMapEntry += () =>
                {
                    Debug.Log("Remove at " +index);
                    mapEntries.RemoveAt(index);
                    RemoveMap(index);
                    mapElitesList.Rebuild();
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

            var data = TargetLayer.Parent;
            var savedMapList = data.GetSavedMaps(TargetLayer);
            if (savedMapList == null) return;

            if (savedMapList.Maps.Count>0)
            {
                foreach(SavedMap map in savedMapList.Maps)
                {
                    //Add map to saved map list
                    SavedMapList.Add(map);
                    //Then make a new visual element to set it up later
                    var mapEntryVE = new PopulationMapEntry();
                    mapEntries.Add(mapEntryVE);
                }
            }
        }

        #endregion

    }
}