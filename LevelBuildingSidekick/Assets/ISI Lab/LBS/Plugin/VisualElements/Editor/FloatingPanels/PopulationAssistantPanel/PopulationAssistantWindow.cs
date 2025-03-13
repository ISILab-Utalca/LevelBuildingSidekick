using ISILab.LBS.Editor.Windows;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Components;

using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using ISI_Lab.LBS.Plugin.MapTools.Generators3D;
using UnityEditor;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.AI.Categorization;
using ISILab.LBS.Generators;
using UnityEditor.UIElements;
using Object = UnityEngine.Object;

namespace ISILab.LBS.VisualElements.Editor
{
    public class PopulationAssistantWindow : EditorWindow
    {
        #region UXMLFACTORY
       // [UxmlElementAttribute]
       // public new class UxmlFactory { }
        #endregion

        #region VIEW ELEMENTS
        private EnumField presetField;
        private EnumField param1Field;
        private EnumField param2Field;
        
        // their foldout functionalities are set in the constructor
        private VisualElement visualizationOptionsContent;
        private VisualElement presetSettingsContainer;
        private VisualElement gridContent;
        //

        private SliderInt rows;
        private SliderInt columns;
        
        private Button recalculate;
        private Button applySuggestion;
        private Button closeWindow;
        
        private ObjectField presetFieldRef;
        #endregion

        #region FIELDS
        private MAPElitesPreset mapEliteBundle;
    
        #endregion

        #region EVENTS
       // public Action OnExecute;
        #endregion

        #region PROPERTIES

        MAPElitesPreset MapEliteBundle
        {
            get => mapEliteBundle;
            set => mapEliteBundle = value;
        }
        #endregion

        #region CONSTRUCTORS

        public void CreateGUI()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("PopulationAssistantWindow");
            visualTree.CloneTree(rootVisualElement);

            presetField = rootVisualElement.Q<EnumField>("Preset");
            param1Field = rootVisualElement.Q<EnumField>("XPreset");
            param2Field = rootVisualElement.Q<EnumField>("YPreset");
            
            visualizationOptionsContent =  rootVisualElement.Q<VisualElement>("VisualizationOptionsContent");
            presetSettingsContainer = rootVisualElement.Q<VisualElement>("PresetSettingsContainer");

            rows =  rootVisualElement.Q<SliderInt>("RowsSlideInt");
            rows.RegisterValueChangedCallback(evt => UpdateGrid());
            columns = rootVisualElement.Q<SliderInt>("ColumnsSlideInt");
            columns.RegisterValueChangedCallback(evt => UpdateGrid());
            
            recalculate = rootVisualElement.Q<Button>("ButtonRecalculate");
            recalculate.clicked += RunAlgorithm;
            
            applySuggestion =  rootVisualElement.Q<Button>("ButtonApplySuggestion");
            applySuggestion.clicked += ApplyResult;
            
            closeWindow =  rootVisualElement.Q<Button>("ButtonClose");
            closeWindow.clicked += Close;
            
            gridContent = rootVisualElement.Q<VisualElement>("GridContent");
            UpdateGrid();
            
            presetFieldRef = rootVisualElement.Q<ObjectField>("PresetObjectField");
        }

        private void ApplyResult()
        {
     
        }

        private void RunAlgorithm()
        {
         
        }

        private void UpdateGrid()
        {
            gridContent.Clear();
            gridContent.style.flexDirection = FlexDirection.Column;
            List<VisualElement> rowsVE = new();
            for (int i = 0; i < rows.value; i++)
            {
                var newRowVE =  new VisualElement();
                newRowVE.style.flexDirection = FlexDirection.Row;
                newRowVE.style.flexGrow = 1;
                rowsVE.Add(newRowVE);
            }
            foreach (var rVE in rowsVE)
            {
                for (int i = 0; i < columns.value; i++)
                {
                    var resultVE = new PopulationAssistantButtonResult();
                    resultVE.style.flexGrow = 1;
                    resultVE.style.alignSelf = Align.Stretch;
                    rVE.Add(resultVE);
                }
            }
          
            foreach (var rVE in rowsVE)
            {
                gridContent.Add(rVE);
            }
        }

        #endregion

       #region METHODS
       
       public static void ShowWindow()
       {
           var window = GetWindow<PopulationAssistantWindow>();
           window.titleContent = new GUIContent("Population Assistant");
           window.minSize = new Vector2(1000, 500); // use the Canvas Size of the uxml
           window.Show();
       }

       
       #endregion
    }
}