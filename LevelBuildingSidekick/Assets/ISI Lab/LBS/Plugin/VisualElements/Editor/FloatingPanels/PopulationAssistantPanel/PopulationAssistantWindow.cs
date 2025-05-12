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
using ISILab.LBS.Assistants;
using ISILab.LBS.Editor;
using ISILab.LBS.Manipulators;
using ISILab.LBS.AI.Assistants.Editor;
using ISILab.LBS.Internal;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Drawers;
using ISILab.Extensions;
using LBS.VisualElements;
using UnityEditor.VersionControl;
using UnityEditor.Graphs;
using UnityEngine.PlayerLoop;
using System.Collections.Concurrent;
using static UnityEngine.GraphicsBuffer;

namespace ISILab.LBS.VisualElements.Editor
{
    public class PopulationAssistantWindow : EditorWindow
    {
        #region UXMLFACTORY
        // [UxmlElementAttribute]
        // public new class UxmlFactory { }
        #endregion

        #region Utilities
        private Dictionary<String, MAPElitesPreset> presetDictionary;
        private PopulationAssistantEditor editor;
        private AssistantMapElite assistant;

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

        protected IRangedEvaluator currentXField
        {
            get => mapEliteBundle.XEvaluator;
            set
            {
                if (mapEliteBundle == null) return;
                mapEliteBundle.XEvaluator = value;
            }
        }
        protected IRangedEvaluator currentYField
        {
            get => mapEliteBundle.YEvaluator;
            set
            {
                if (mapEliteBundle == null) return;
                mapEliteBundle.YEvaluator = value;
            }
        }
        protected BaseOptimizer currentOptimizer
        {
            get => mapEliteBundle.Optimizer;
            set {  
                if (mapEliteBundle == null) return;
                mapEliteBundle.Optimizer = value;
            }
        }
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


        public void CreateGUI()
        {
            presetDictionary = new Dictionary<string, MAPElitesPreset>();

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
                var xChoice = param1Field.GetChoiceInstance() as IRangedEvaluator;
                currentXField = xChoice;
                //xParamText.text = xChoice.GetType().Name + " / X Axis";
            });

            param1Field.SetEnabled(false);

            //Param 2
            param2Field = rootVisualElement.Q<ClassDropDown>("YParamDropdown");
            param2Field.Type = typeof(IRangedEvaluator);
            param2Field.value = defaultSelectText;

            param2Field.RegisterValueChangedCallback(evt =>
            {
                if (param2Field.value == null) return;
                var yChoice = param2Field.GetChoiceInstance() as IRangedEvaluator;
                currentYField = yChoice;
                //yParamText.text = yChoice.GetType().Name + " / Y Axis";
            });

            param2Field.SetEnabled(false);

            //Optimizer
            optimizerField = rootVisualElement.Q<ClassDropDown>("ZParamDropdown");
            optimizerField.Type = typeof(BaseOptimizer);
            optimizerField.value = defaultSelectText;

            optimizerField.RegisterValueChangedCallback(evt =>
            {
                if (optimizerField.value == null) return;
                var optimizerChoice = optimizerField.GetChoiceInstance() as BaseOptimizer;
                currentOptimizer = optimizerChoice;
                //zParamText.text = optimizerChoice.GetType().Name + " / Fitness";
            });

            //I set everything false so they can't be manipulated if there's no preset present.
            optimizerField.SetEnabled(false);

            //PRESET SETTINGS
            presetSettingsContainer = rootVisualElement.Q<VisualElement>("PresetSettingsContainer");

            //Preset field (always disabled)
            presetFieldRef = rootVisualElement.Q<ObjectField>("PresetObjectField");
            presetFieldRef.SetEnabled(false);

            //Preset manipulation buttons
            openPresetButton = rootVisualElement.Q<Button>("OpenPresetButton");
            openPresetButton.clicked += () => { UnityEditor.Selection.activeObject = presetFieldRef.value; };

            resetPresetButton = rootVisualElement.Q<Button>("ResetPresetButton");
            resetPresetButton.clicked += () =>
            {
                if (mapEliteBundle != null) mapEliteBundle = mapEliteBundle.ResetValues();
                UpdatePreset(mapEliteBundle.PresetName);
            };

            //Visualization option buttons
            visualizationOptionsContent = rootVisualElement.Q<VisualElement>("VisualizationOptionsContent");

            rows = rootVisualElement.Q<SliderInt>("RowsSlideInt");
            rows.RegisterValueChangedCallback(evt => UpdateGrid());
            columns = rootVisualElement.Q<SliderInt>("ColumnsSlideInt");
            columns.RegisterValueChangedCallback(evt => UpdateGrid());

            //Recalculate button
            recalculate = rootVisualElement.Q<Button>("ButtonRecalculate");
            recalculate.clicked += RunAlgorithm;

            applySuggestion = rootVisualElement.Q<Button>("ButtonApplySuggestion");
            applySuggestion.clicked += ApplyResult;

            closeWindow = rootVisualElement.Q<Button>("ButtonClose");
            closeWindow.clicked += Close;

            gridContent = rootVisualElement.Q<VisualElement>("GridContent");
            UpdateGrid();

            //PARAMETER'S GRAPH
            //Find container
            graphOfHell = rootVisualElement.Q<VisualElement>("GraphOfHell");
            
            //Create and add VisualElement: PopulationAssistantGraph to the container
            PopulationAssistantGraph graph = new(new[] { 0, 0.2f, 0.4f, 0.6f, 0.8f, 1 }, 2);
            graphOfHell.Add(graph);
            
            //Modify graph's colors (not necessary, it comes with default colors)
            graph.MainColor = Color.green;
            graph.SecondaryColor = Color.cyan;
            
            //Change axes color
            graph.SetAxisColor(Color.magenta, 0);
            graph.SetAxisColor(Color.yellow, 1);
            graph.SetAxisColor(Color.white, 2);
            if (graph.SetAxisColor(Color.white, 6))
            {
                Debug.LogError("Can't SetAxisValue: Index out of range");
            }
            
            //Change axes value
            graph.SetAxisValue(0.5f,0);
            if (graph.SetAxisValue(1, 6))
            {
                Debug.LogError("Can't SetAxisValue: Index out of range");
            }
            graph.RecalculateCorners(); //Important after changing axes' values
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

            param1Field.value = currentXField != null ? currentXField.GetType().Name : defaultSelectText;
            param2Field.value = currentYField != null ? currentYField.GetType().Name : defaultSelectText;
            optimizerField.value = currentOptimizer != null ? currentOptimizer.GetType().Name : defaultSelectText;

            //Debug stuff!
            //Debug.Log("Preset Updated: " + mapEliteBundle.name + " / PARAM 1: " + param1Field.GetChoiceInstance() + " / PARAM 2: " + param2Field.GetChoiceInstance() + " / OPTIMIZER: " + optimizerField.GetChoiceInstance());
        }


        private void ApplyResult()
        {
     
        }

        private void RunAlgorithm()
        {
            currentOptimizer.Evaluator = currentXField;

            Debug.Log("running algorithm");

            //Check how many of these there are, and get the optimizer!
            var veChildren = GetButtonResults(new List<PopulationAssistantButtonResult>(), gridContent);

            UpdateGrid();
            //This resets the algorithm all the time, so nothing to worry about regarding whether it's running or not.
            assistant.LoadPresset(mapEliteBundle);
            
            //Check if there's a place to optimize
            if (assistant.RawToolRect.width == 0 || assistant.RawToolRect.height == 0)
                {
                    Debug.LogError("[ISI Lab]: Selected evolution area height or with < 0");
                    return;
            }
            //SetBackgroundTexture(square, assistant.RawToolRect);
            assistant.SetAdam(assistant.RawToolRect);
            assistant.Execute();

            Debug.Log("executed");
            //TODO: Hay que pasarle el Optimizer a los Map Elites
            LBSMainWindow.OnWindowRepaint += RepaintContent;

            //Update button
            recalculate.text = "Recalculate";
        }

        private void RepaintContent()
        {
            UpdateContent();
            if (assistant.Finished)
                LBSMainWindow.OnWindowRepaint -= RepaintContent;
        }

        private void UpdateGrid()
        {
            assistant.SampleWidth = rows.value;
            assistant.SampleHeight = columns.value;

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

        #region METHODS

        public void UpdateContent()
        {
            var veChildren = GetButtonResults(new List<PopulationAssistantButtonResult>(), gridContent);
            for (int i = 0; i < assistant.toUpdate.Count; i++)
            {
                var v = assistant.toUpdate[i];
                //var index = (int)(v.y * assistant.SampleWidth + v.x);

                SetBackgroundTexture(veChildren[i], assistant.RawToolRect);

                veChildren[i].Data = assistant.Samples[(int)v.y, (int)v.x];
                veChildren[i].Score = ((decimal)assistant.Samples[(int)v.y, (int)v.x].Fitness).ToString("f4");
                var t = veChildren[i].GetTexture();
                if (veChildren[i].Data != null)
                {
                    veChildren[i].SetTexture(veChildren[i].backgroundTexture.MergeTextures(t).FitSquare());
                }
                else
                {
                    veChildren[i].SetTexture(DirectoryTools.GetAssetByName<Texture2D>("LoadingContent"));
                }
                veChildren[i].UpdateLabel();
            }
            assistant.toUpdate.Clear();
        }

        public MAPElitesPreset GetPresset()
        {
            return LBSAssetsStorage.Instance.Get<MAPElitesPreset>().Find(p => p.name == mapEliteBundle.name);
        }

        public void SetBackgroundTexture(PopulationAssistantButtonResult gridSquare, Rect rect)
        {
            var behaviours = assistant.OwnerLayer.Parent.Layers.SelectMany(l => l.Behaviours);
            var bh = assistant.OwnerLayer.Behaviours.Find(b => b is PopulationBehaviour);

            var size = 16;

            var textures = new List<Texture2D>();

            foreach (var b in behaviours)
            {
                if (b == null)
                    continue;

                if (bh != null && b.Equals(bh))
                    continue;

                var drawerT = LBS_Editor.GetDrawer(b.GetType());
                var drawer = Activator.CreateInstance(drawerT) as Drawer;
                textures.Add(drawer.GetTexture(b, rect, Vector2Int.one * size));
            }

            var texture = new Texture2D((int)(rect.width * size), (int)(rect.height * size));

            for (int j = 0; j < texture.height; j++)
            {
                for (int i = 0; i < texture.height; i++)
                {
                    texture.SetPixel(i, j, new UnityEngine.Color(0.1f, 0.1f, 0.1f, 1));
                }
            }

            for (int i = textures.Count - 1; i >= 0; i--)
            {
                if (textures[i] == null)
                    continue;

                texture = texture.MergeTextures(textures[i]);
            }

            texture.Apply();

            //Update texture on the chosen square
            gridSquare.SetTexture(texture);
            //veChildren.First().SetColor(new Color(0, 1, 0, 1));

            Debug.Log("texture changed");
            
            //content.background = texture;
        }

        private List<PopulationAssistantButtonResult> GetButtonResults(List<PopulationAssistantButtonResult> buttons, VisualElement parent)
        {
            foreach (var ve in parent.Children())
            {
                if (ve is PopulationAssistantButtonResult buttonResult)
                {
                    buttons.Add(buttonResult);
                }

                // Recurse on children
                GetButtonResults(buttons, ve);
            }
            return buttons;
        }

        public void SetAssistant(AssistantMapElite target)
        {
            assistant = target;
        }

        public void ShowWindow()
       {
           titleContent = new GUIContent("Population Assistant");
           minSize = new Vector2(1000, 500); // use the Canvas Size of the uxml
           Show();
       }

       #endregion
    }
}