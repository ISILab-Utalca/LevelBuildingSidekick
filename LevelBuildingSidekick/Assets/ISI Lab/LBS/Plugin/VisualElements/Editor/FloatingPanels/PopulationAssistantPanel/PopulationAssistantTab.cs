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
        private List<MAPElitesPreset> savedMaps = new ();
        private List<PopulationMapEntry> mapEntries = new ();
    
        #endregion

        #region PROPERTIES

        List<MAPElitesPreset> MapEliteBundle
        {
            get => savedMaps;
            set => savedMaps = value;
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
            
            mapEliteFoldout = this.Q<Foldout>("FoldoutMapElites");
            mapEliteContent = this.Q<VisualElement>("MapEliteContent");
            
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
            
            savedElitesContent = this.Q<VisualElement>("SavedElitesContent");
            
            mapEntries.Clear();
            // replace these calls with reading the actual saved data from the user
            AddEntry();
            AddEntry();
            AddEntry();
            
            mapElitesList = this.Q<ListView>("MapElitesList");
            mapElitesList.reorderable = true;
            
            mapElitesList.makeItem = () => new PopulationMapEntry(); 
            mapElitesList.bindItem = (element, index) =>
            {
                var mapEntryVE = element as PopulationMapEntry;
                if (mapEntryVE == null) return;

                var mapEntry = savedMaps[index]; 
                mapEntryVE.SetData(mapEntry);

                mapEntryVE.RemoveMapEntry = null;
                mapEntryVE.RemoveMapEntry += () =>
                {
                    Debug.Log("Remove at " +index);
                    mapEntries.RemoveAt(index);
                    mapElitesList.Rebuild();
                };
            };

            mapElitesList.itemsSource = mapEntries;
        }
        
        #endregion

        #region METHODS

        // should pass the preset as parameter
        private void AddEntry()
        {
            var mapEntry1 = ScriptableObject.CreateInstance<MAPElitesPreset>(); // not null
            savedMaps.Add(mapEntry1);

            var mapEntryVE = new PopulationMapEntry();
            mapEntries.Add(mapEntryVE);
        }
        
        #endregion
       
    }
}