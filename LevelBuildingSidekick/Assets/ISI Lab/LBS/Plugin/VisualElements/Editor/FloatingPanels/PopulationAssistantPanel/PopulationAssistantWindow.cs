using ISILab.LBS.Editor.Windows;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using UnityEditor;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.AI.Categorization;
using UnityEditor.UIElements;
using ISILab.LBS.Settings;
using System.IO;
using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization;
using System.Linq;
using ISILab.LBS.Assistants;
using ISILab.LBS.Editor;
using ISILab.LBS.AI.Assistants.Editor;
using ISILab.LBS.Internal;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Drawers;
using ISILab.Extensions;
using ISILab.Macros;
using ISILab.AI.Categorization;
using static UnityEngine.Analytics.IAnalytic;
using LBS.Components.TileMap;
using ISILab.LBS.Modules;

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
        [SerializeField]
        private PopulationAssistantEditor editor;
        [SerializeField]
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
        private Button optimizerButton;

        //Optimizer Editor
        private LBSCustomEditor optimizerEditor;

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
        private PopulationAssistantButtonResult selectedMap;

        protected IRangedEvaluator currentXField
        {
            get => mapEliteBundle?.XEvaluator;
            set
            {
                if (mapEliteBundle == null) return;
                mapEliteBundle.XEvaluator = value;
            }
        }
        protected IRangedEvaluator currentYField
        {
            get => mapEliteBundle?.YEvaluator;
            set
            {
                if (mapEliteBundle == null) return;
                mapEliteBundle.YEvaluator = value;
            }
        }
        protected BaseOptimizer currentOptimizer
        {
            get => mapEliteBundle?.Optimizer;
            set {  
                if (mapEliteBundle == null) return;
                mapEliteBundle.Optimizer = value;
            }
        }

        protected PopulationBehaviour LayerPopulation
        {
            get => assistant.OwnerLayer.Behaviours.Find(b => b.GetType().Equals(typeof(PopulationBehaviour))) as PopulationBehaviour;
        }
        #endregion

        #region EVENTS
        // public Action OnExecute;
        public Action<string> OnPresetChanged;
        public Action UpdatePins;
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
                //Failsafe stuff
                if (param1Field.value == null) return;
                if (param1Field.value == currentXField?.GetType().Name) return;

                //Choice change
                var xChoice = param1Field.GetChoiceInstance() as IRangedEvaluator;
                currentXField = xChoice;
                if(xChoice != null) currentXField.InitializeDefault();
                xParamText.text = param1Field.Value;
                
            });
            param1Field.SetEnabled(false);


            //Param 2
            param2Field = rootVisualElement.Q<ClassDropDown>("YParamDropdown");
            param2Field.Type = typeof(IRangedEvaluator);
            param2Field.value = defaultSelectText;
            param2Field.RegisterValueChangedCallback(evt =>
            {
                //Failsafe stuff
                if (param2Field.value == null) return;
                if (param2Field.value == currentYField?.GetType().Name) return;

                //Choice change
                var yChoice = param2Field.GetChoiceInstance() as IRangedEvaluator;
                currentYField = yChoice;
                if (yChoice != null) currentYField.InitializeDefault();
                yParamText.text = param2Field.Value;
            });
            param2Field.SetEnabled(false);

            //Optimizer

            //This button doesn't work anymore lol. Next time!!!
            optimizerField = rootVisualElement.Q<ClassDropDown>("ZParamDropdown");
            optimizerField.Type = typeof(BaseOptimizer);
            optimizerField.value = defaultSelectText;
            optimizerField.RegisterValueChangedCallback(evt =>
            {
                if (optimizerField.value == null) return;
                if (optimizerField.value == currentOptimizer?.GetType().Name) return;

                var optimizerChoice = optimizerField.GetChoiceInstance() as BaseOptimizer;
                currentOptimizer = optimizerChoice;
                if(optimizerChoice != null) currentOptimizer.InitializeDefault();
            });
            optimizerButton = rootVisualElement.Q<Button>("OpenOptimizerButton");
            optimizerButton.SetEnabled(false);
            optimizerButton.visible = false;
            //optimizerButton.clicked += OpenOptimizer;

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
            applySuggestion.clicked += () =>
            {
                // Save history version to revert
                var level = LBSController.CurrentLevel;
                Undo.RegisterCompleteObjectUndo(level, "Select Suggestion");
                EditorGUI.BeginChangeCheck();
                
                
                ApplySuggestion();
                
                
                // Mark as dirty
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(level);
                }

            };

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
            
            ySlider = rootVisualElement.Q<Slider>("YSlider");
            xSlider = rootVisualElement.Q<Slider>("XSlider");
            zProgressBar = rootVisualElement.Q<ProgressBar>("ZProgressBar");
        }

        #region MAIN METHODS
        
        //Add all presets in the preset directory to the preset dropdown
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
                    //Debug.Log("loaded: " + newPreset);
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
        
        //Update preset whenever it's changed with its respective stats
        private void UpdatePreset(string value)
        {
            //Disable parameters unless the preset is valid. Otherwise, enable them since they can be manipulated.
            if ((value == null) || (!presetDictionary.ContainsKey(value)))
            {
                param1Field.SetEnabled(false);
                param2Field.SetEnabled(false);
                //optimizerField.SetEnabled(false);
                return;
            }
            
            //Set the map elite accordingly.
            mapEliteBundle = presetDictionary[value];
            presetFieldRef.value = mapEliteBundle;

            //Enable params set the preset things to the new choice.
            param1Field.SetEnabled(true);
            param2Field.SetEnabled(true);
            //optimizerField.SetEnabled(true);

            param1Field.Value = currentXField != null ? currentXField.GetType().Name : defaultSelectText;
            param2Field.Value = currentYField != null ? currentYField.GetType().Name : defaultSelectText;
            optimizerField.value = currentOptimizer != null ? currentOptimizer.GetType().Name : defaultSelectText;
            
            yParamText.text = param2Field.Value;
            xParamText.text = param1Field.Value;
        }
        
        //Run the algorithm for suggestions
        private void RunAlgorithm()
        {
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

            //TODO: Hay que pasarle el Optimizer a los Map Elites
            LBSMainWindow.OnWindowRepaint += RepaintContent;

            //Update button
            recalculate.text = "Recalculate";
        }
        
        //Apply the suggestion in the world
        private void ApplySuggestion() => ApplySuggestion(selectedMap.Data);
        private void ApplySuggestion(object obj)
        {
            var chrom = obj as BundleTilemapChromosome;
            if (chrom == null)
            {
                if (selectedMap.Data == null) throw new Exception("[ISI Lab] Data " + selectedMap.Data.GetType().Name + " is not LBSChromosome!");
            }

            var rect = chrom.Rect;

            for (int i = 0; i < chrom.Length; i++)
            {
                var pos = chrom.ToMatrixPosition(i) + rect.position.ToInt();
                LayerPopulation.RemoveTileGroup(pos);
                var gene = chrom.GetGene(i);
                if (gene == null)
                    continue;
                LayerPopulation.AddTileGroup(pos, gene as BundleData);
            }
            DrawManager.Instance.RedrawLayer(assistant.OwnerLayer, MainView.Instance);
            LBSMainWindow.MessageNotify("Layer modified by Population Assistant");
        }

        //Pin the suggestion into the assistant tab
        private void PinSuggestion() => PinSuggestion(selectedMap.Data);
        private void PinSuggestion(object obj)
        {
            //Get chromosome
            var objVE = obj as PopulationAssistantButtonResult;
            var suggestionData = objVE.Data as BundleTilemapChromosome;
            if (suggestionData == null)
            {
                throw new Exception("[ISI Lab] Data " + selectedMap.Data.GetType().Name + " is not LBSChromosome!");
            }

            //Create bundle tile map
            var newTileMap = new BundleTileMap();
            for (int i = 0; i < suggestionData.GetGenes().Length; i++)
            {
                if (suggestionData.GetGenes()[i] != null)
                {
                    var geneData = suggestionData.GetGenes()[i] as BundleData;
                    newTileMap.AddGroup(new TileBundleGroup(suggestionData.ToMatrixPosition(i), geneData.Bundle.TileSize, geneData, Vector2.right));
                }
            }

            //Get level data and layer
            var layer = LayerPopulation.OwnerLayer;
            var levelData = LayerPopulation.OwnerLayer.Parent;
            var savedMapList = levelData.GetSavedMaps(layer);
            if(savedMapList!=null)
            {
                //Check for duplicates
                foreach (SavedMap storedMap in savedMapList.Maps)
                {
                    if (suggestionData.Equals(storedMap.Map))
                    {
                        LBSMainWindow.MessageNotify("An equal suggestion already exists.", LogType.Warning);
                        return;
                    }
                }
            }
            var newSavedMap = new SavedMap(suggestionData, "", (float)suggestionData.Fitness);
            newSavedMap.Image = objVE.GetTexture();
            levelData.SaveMapInLayer(newSavedMap, layer);
            LBSMainWindow.MessageNotify("Suggestion pinned.");
            UpdatePins?.Invoke();

            //Backup file setup
            /*var settings = LBSSettings.Instance;
            var savedMapPath = settings.paths.savedMapsPresetPath;
            string savedMapName = "Saved Map";

            //Directory making
            if (!Directory.Exists(savedMapPath))
            {
                Directory.CreateDirectory(savedMapPath);
            }
            else
            {
                var info = new DirectoryInfo(savedMapPath);
                var fileInfo = info.GetFiles();

                //Check if newTileMap is equal to any of the saved maps.
                if (fileInfo.Length > 0)
                {
                    int count = 0;
                    foreach (var file in fileInfo)
                    {
                        var mapToCompare = AssetDatabase.LoadAssetAtPath<SavedMap>(savedMapPath + "\\" + file.Name);
                        if (mapToCompare != null)
                        {
                            if (mapToCompare.Map.Equals(newTileMap))
                            {
                                //Maps are the same, so return
                                LBSMainWindow.MessageNotify("An equal suggestion has already been pinned.", LogType.Warning);
                                return;
                            }
                            count++;
                        }
                    }
                    //Then name it after the count in file info
                    savedMapName = "Saved Map " + count;
                }
            }
            //Finally, save!
            SavedMap newSavedMap = ScriptableObject.CreateInstance<SavedMap>();
            newSavedMap.Map = newTileMap;
            newSavedMap.Name = savedMapName;
            newSavedMap.Score = (float)suggestionData.Fitness;
            AssetDatabase.CreateAsset(newSavedMap, savedMapPath + "\\" + savedMapName + ".asset");

            //LayerPopulation.SaveMap(newTileMap, (float)suggestionData.Fitness);
            Debug.Log("saved map: "+newSavedMap.Name);*/
        }
        #endregion

        #region GRID-RELATED METHODS
        //
        private void RepaintContent()
        {
            UpdateContent();
            if (assistant.Finished)
            {
                LBSMainWindow.OnWindowRepaint -= RepaintContent;
            }
        }
        
        //Update all squares in the grid
        public void UpdateContent()
        {
            var veChildren = GetButtonResults(new List<PopulationAssistantButtonResult>(), gridContent);
            for (int i = 0; i < assistant.toUpdate.Count &&
                            i < veChildren.Count; i++)
            {
                var v = assistant.toUpdate[i];
                var index = (int)(v.y * assistant.SampleWidth + v.x);

                SetBackgroundTexture(veChildren[index], assistant.RawToolRect);

                veChildren[index].Data = assistant.Samples[(int)v.y, (int)v.x];
                veChildren[index].Score = ((decimal)assistant.Samples[(int)v.y, (int)v.x].Fitness).ToString("f4");
                var t = veChildren[index].GetTexture();
                if (veChildren[index].Data != null)
                {
                    veChildren[index].SetTexture(veChildren[index].backgroundTexture.MergeTextures(t).FitSquare());
                }
                else
                {
                    veChildren[index].SetTexture(DirectoryTools.GetAssetByName<Texture2D>("LoadingContent"));
                }
                //veChildren[i].selectButton.clicked += () => ShowResults(veChildren[i-1].Data);
                veChildren[index].UpdateLabel();
            }
            assistant.toUpdate.Clear();
        }
        
        //Redraws the grid
        private void UpdateGrid()
        {
            //assistant.SampleWidth = rows.value;
            //assistant.SampleHeight = columns.value;
            mapEliteBundle.SampleCount = new Vector2Int(rows.value, columns.value);

           // TODO change the population sample size
            
            gridContent.Clear();
            gridContent.style.flexDirection = FlexDirection.ColumnReverse;
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
                    resultVE.selectButton.clicked += () => ShowButtonStats(resultVE);
                    resultVE.OnApplySuggestion += () => ApplySuggestion(resultVE.Data);
                    resultVE.OnSaveSuggestion += () => PinSuggestion(resultVE);
                }
            }
          
            foreach (var rVE in rowsVE)
            {
                gridContent.Add(rVE);
            }
        }
        
        //Change the texture of a specific button
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

            //Debug.Log("texture changed");

            //content.background = texture;
        }

        //Show the stats for any button selected
        public void ShowButtonStats(PopulationAssistantButtonResult buttonResult)
        {
            var mapData = buttonResult.Data as IOptimizable;

            //Shows data if non null
            if (mapData == null) return;

            zProgressBar.value = (float)mapData.Fitness;
            xSlider.value = (float)mapData.xFitness;
            ySlider.value = (float)mapData.yFitness;

            //Takes border off selected map previously selected map
            if (selectedMap != null)
            {
                selectedMap.OnButtonDeselected?.Invoke();
            }
            selectedMap = buttonResult;
            buttonResult.OnButtonSelected?.Invoke();
        }

        #endregion

        #region OTHER METHODS

        //Return a list of all buttons available.
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
        
        //Set assistant for window
        public void SetAssistant(AssistantMapElite target)
        {
            assistant = target;
        }

        //Create the window
        public void ShowWindow()
       {
           titleContent = new GUIContent("Population Assistant");
           minSize = new Vector2(1000, 500); // use the Canvas Size of the uxml
           Show();
       }

        #endregion

        #region OBSOLETE
        /*private void OpenOptimizer()
        {
            if ((currentOptimizer is GeneticAlgorithm) && (currentOptimizer != null))
            {
                //Debug.Log("optimizer is compatible");
                optimizerEditor = new GeneticAlgorithmVE(mapEliteBundle.Optimizer);

                var optimizerWindow = ScriptableObject.CreateInstance<EditorWindow>();
                optimizerWindow.titleContent = new GUIContent("Optimizer Editor");
                optimizerWindow.minSize = new Vector2(350, 500); // use the Canvas Size of the uxml
                optimizerWindow.Show();
                optimizerWindow.rootVisualElement.Add(optimizerEditor);
            }
        }*/

        //To keep the grid updated
        #endregion
    }
}