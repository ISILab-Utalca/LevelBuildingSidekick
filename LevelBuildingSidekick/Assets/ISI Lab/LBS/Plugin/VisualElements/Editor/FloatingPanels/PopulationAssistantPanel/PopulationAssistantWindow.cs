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
using System.Linq;
using System.Speech.Recognition;
using UnityEngine.PlayerLoop;

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

        //Default text for unchosen elements
        private string defaultSelectText = "Select...";
        #endregion

        #region VIEW ELEMENTS
        //Preset Parameters
        private DropdownField presetField;
        private ClassDropDown param1Field;
        private ClassDropDown param2Field;
        private ClassDropDown optimizerField;

        //Parameter Information
        private Label xParamText;
        private Label yParamText;
        private Label zParamText;
        private Slider xSlider;
        private Slider ySlider;
        private ProgressBar zProgressBar;

        // their foldout functionalities are set in the constructor
        private VisualElement visualizationOptionsContent;
        private VisualElement presetSettingsContainer;
        private VisualElement gridContent;
        
        //Visualization Information
        private SliderInt rows;
        private SliderInt columns;
        
        private Button recalculate;
        private Button applySuggestion;
        private Button closeWindow;
        
        //Scriptable Object Manipulation
        private ObjectField presetFieldRef;
        private Button openPresetButton;
        private Button resetPresetButton;
        
        //Parameters' graphic
        private VisualElement graphOfHell;

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
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("PopulationAssistantWindow");
            visualTree.CloneTree(rootVisualElement);
            
            //Set up preset
            presetField = rootVisualElement.Q<DropdownField>("Preset");
            SetPresets();
            presetField.value = "Select Preset";
            presetField.RegisterValueChangedCallback(evt => UpdatePreset(evt.newValue));

            //Progress Bar and Sliders
            xParamText = rootVisualElement.Q<Label>("XParamText");
            yParamText = rootVisualElement.Q<Label>("YParamText");
            zParamText = rootVisualElement.Q<Label>("ZParamText");

            //Set parameters. Make everyone a ranged evaluator, make the value a default, add the listener to change the chosen elite bundle and then disable it.
            param1Field = rootVisualElement.Q<ClassDropDown>("XParamDropdown");
            param1Field.Type = typeof(IRangedEvaluator);
            param1Field.value = defaultSelectText;

            param1Field.RegisterValueChangedCallback(evt =>
            {
                if (param1Field.value == null) return;

                var xChoice = param1Field.GetChoiceInstance();
                if (mapEliteBundle == null) return;
                mapEliteBundle.XEvaluator = xChoice as IRangedEvaluator;
                xParamText.text = xChoice.GetType().Name + " / X Axis";
            });

            param1Field.SetEnabled(false);

            //Param 2
            param2Field = rootVisualElement.Q<ClassDropDown>("YParamDropdown");
            param2Field.Type = typeof(IRangedEvaluator);
            param2Field.value = defaultSelectText;

            param2Field.RegisterValueChangedCallback(evt =>
            {
                if (param2Field.value == null) return;

                var yChoice = param2Field.GetChoiceInstance();
                if (mapEliteBundle == null) return;
                mapEliteBundle.YEvaluator = yChoice as IRangedEvaluator;
                yParamText.text = yChoice.GetType().Name + " / Y Axis";
            });

            param2Field.SetEnabled(false);

            //Optimizer
            optimizerField = rootVisualElement.Q<ClassDropDown>("ZParamDropdown");
            optimizerField.Type = typeof(BaseOptimizer);
            optimizerField.value = defaultSelectText;

            optimizerField.RegisterValueChangedCallback(evt =>
            {
                if (optimizerField.value == null) return;

                var optimizerChoice = optimizerField.GetChoiceInstance();
                if (mapEliteBundle == null) return;
                mapEliteBundle.Optimizer = optimizerChoice as BaseOptimizer;
                zParamText.text = optimizerChoice.GetType().Name + " / Fitness";
            });

            optimizerField.SetEnabled(false);
            //I set everything false so they can't be manipulated if there's no preset present.

            //PRESET SETTINGS
            presetSettingsContainer = rootVisualElement.Q<VisualElement>("PresetSettingsContainer");

            //Preset field (always disabled)
            presetFieldRef = rootVisualElement.Q<ObjectField>("PresetObjectField");
            presetFieldRef.SetEnabled(false);

            //Preset manipulation buttons
            openPresetButton = rootVisualElement.Q<Button>("OpenPresetButton");
            openPresetButton.clicked += () => { UnityEditor.Selection.activeObject = presetFieldRef.value; };

            resetPresetButton = rootVisualElement.Q<Button>("ResetPresetButton");
            resetPresetButton.clicked += () => { if (mapEliteBundle != null) mapEliteBundle = mapEliteBundle.ResetValues(); UpdatePreset(mapEliteBundle.PresetName); };

            //Visualization option buttons
            visualizationOptionsContent =  rootVisualElement.Q<VisualElement>("VisualizationOptionsContent");

            rows =  rootVisualElement.Q<SliderInt>("RowsSlideInt");
            rows.RegisterValueChangedCallback(evt => UpdateGrid());
            columns = rootVisualElement.Q<SliderInt>("ColumnsSlideInt");
            columns.RegisterValueChangedCallback(evt => UpdateGrid());
            
            //Recalculate button
            recalculate = rootVisualElement.Q<Button>("ButtonRecalculate");
            recalculate.clicked += RunAlgorithm;
            
            applySuggestion =  rootVisualElement.Q<Button>("ButtonApplySuggestion");
            applySuggestion.clicked += ApplyResult;
            
            closeWindow =  rootVisualElement.Q<Button>("ButtonClose");
            closeWindow.clicked += Close;
            
            gridContent = rootVisualElement.Q<VisualElement>("GridContent");
            UpdateGrid();
            
            //Parameters' graph
            graphOfHell = rootVisualElement.Q<VisualElement>("GraphOfHell");

            float[] axes = new[] { 0.5f, 1, 0.7f };
            graphOfHell.Add(new PopulationParamsGraph(axes, Color.yellow, 1));

        }

        private void SetPresets()
        {
            var settings = LBSSettings.Instance;
            var presetPath = settings.paths.assistantPresetFolderPath;
            
            //Directory making
            var info = new DirectoryInfo(presetPath);
            var fileInfo = info.GetFiles();

            //Find all presets in the directory
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

            //Just in case
            presetField.choices.Clear();

            //Add presets found in the dictionary
            foreach(var preset in mapPresets)
            {
                presetField.choices.Add(preset.PresetName);
                presetDictionary.Add(preset.PresetName, preset);
            }
        }

        private void UpdatePreset(string value)
        {
            //Disable parameters unless the preset is valid. Otherwise, enable them since they can be manipulated.
            if ((value == null) || (!presetDictionary.ContainsKey(value)))
            {
                param1Field.SetEnabled(false);
                param2Field.SetEnabled(false);
                optimizerField.SetEnabled(false);
                return;
            }
            
            //Set the map elite accordingly.
            mapEliteBundle = presetDictionary[value];
            presetFieldRef.value = mapEliteBundle;

            //Enable params set the preset things to the new choice.
            param1Field.SetEnabled(true);
            param2Field.SetEnabled(true);
            optimizerField.SetEnabled(true);

            param1Field.value = mapEliteBundle.XEvaluator != null ? mapEliteBundle.XEvaluator.GetType().Name : defaultSelectText;
            param2Field.value = mapEliteBundle.YEvaluator != null ? mapEliteBundle.YEvaluator.GetType().Name : defaultSelectText;
            optimizerField.value = mapEliteBundle.Optimizer != null ? mapEliteBundle.Optimizer.GetType().Name : defaultSelectText;

            //Debug stuff!
            //Debug.Log("Preset Updated: " + mapEliteBundle.name + " / PARAM 1: " + param1Field.GetChoiceInstance() + " / PARAM 2: " + param2Field.GetChoiceInstance() + " / OPTIMIZER: " + optimizerField.GetChoiceInstance());
        }


        private void ApplyResult()
        {
     
        }

        private void RunAlgorithm()
        {
            Debug.Log("calculating for " + (rows.value * columns.value) + " windows");
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