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
using ISILab.LBS.Settings;
using System.IO;
using Commons.Optimization.Evaluator;
using ISILab.LBS.AI.VisualElements;
using ISILab.AI.Optimization;

namespace ISILab.LBS.VisualElements.Editor
{
    public class PopulationAssistantWindow : EditorWindow
    {
        #region UXMLFACTORY
        // [UxmlElementAttribute]
        // public new class UxmlFactory { }
        #endregion

        #region Utilities
        private Dictionary<String, MAPElitesPreset> presetDictionary = new Dictionary<string, MAPElitesPreset>();
        #endregion

        #region VIEW ELEMENTS
        private DropdownField presetField;
        private ClassDropDown param1Field;
        private ClassDropDown param2Field;
        private ClassDropDown optimizerField;

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
        public Action<string> OnPresetChanged;
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
            //Start by setting up the presets
            

            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("PopulationAssistantWindow");
            visualTree.CloneTree(rootVisualElement);

            presetField = rootVisualElement.Q<DropdownField>("Preset");
            SetPresets();
            presetField.RegisterValueChangedCallback(evt => UpdatePreset(evt.newValue));
            presetField.value = "Select Preset";

            //Set parameters
            param1Field = rootVisualElement.Q<ClassDropDown>("XParamDropdown");
            param1Field.Type = typeof(IRangedEvaluator);
            param1Field.value = "Select...";
            param1Field.SetEnabled(false);

            param2Field = rootVisualElement.Q<ClassDropDown>("YParamDropdown");
            param2Field.Type = typeof(IRangedEvaluator);
            param2Field.value = "Select...";
            param2Field.SetEnabled(false);

            optimizerField = rootVisualElement.Q<ClassDropDown>("ZParamDropdown");
            optimizerField.Type = typeof(BaseOptimizer);
            optimizerField.value = "Select Optimizer";
            optimizerField.SetEnabled(false);

            

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

        private void SetPresets()
        {
            var settings = LBSSettings.Instance;
            var presetPath = settings.paths.assistantPresetFolderPath;
            //Directory making
            var info = new DirectoryInfo(presetPath);
            var fileInfo = info.GetFiles();

            var mapPresets = new List<MAPElitesPreset>();
            foreach (var file in fileInfo)
            {
                var newPreset = AssetDatabase.LoadAssetAtPath<MAPElitesPreset>(presetPath + "\\" + file.Name);
                if (newPreset != null)
                {
                    Debug.Log("loaded: " + newPreset);
                    mapPresets.Add(newPreset);
                }
            }

            presetField.choices.Clear();

            foreach(var preset in mapPresets)
            {
                presetField.choices.Add(preset.PresetName);
                presetDictionary.Add(preset.PresetName, preset);
            }
        }

        private void UpdatePreset(string value)
        {
            if (value == null) return;
            if (!presetDictionary.ContainsKey(value)) return;
            
            //Everything else happens here
            mapEliteBundle = presetDictionary[value];
            Debug.Log("selected map elite: " + mapEliteBundle);

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