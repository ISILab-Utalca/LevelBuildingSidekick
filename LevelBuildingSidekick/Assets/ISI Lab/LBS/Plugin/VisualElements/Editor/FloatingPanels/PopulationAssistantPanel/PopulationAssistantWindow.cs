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
using UnityEditor.Graphs;
using LBS.Components;
using System.Xml.Serialization;
using static UnityEditor.Experimental.GraphView.GraphView;

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
        private MAPElitesPreset mapEliteBundle;

        private BundleTileMap originalTileMap;
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
        private ProgressBar xProgressBar;
        private ProgressBar yProgressBar;
        private ProgressBar zProgressBar;

        // their foldout functionalities are set in the constructor
        private VisualElement visualizationOptionsContent;
        private VisualElement presetSettingsContainer;
        private VisualElement gridContent;
        
        //Visualization Information
        private SliderInt rows;
        private SliderInt columns;
        private PopulationAssistantButtonResult selectedMap;

        //Functionality buttons
        private Button recalculate;
        private Button applySuggestion;
        private Button resetButton;
        private Button closeWindow;
        
        //Scriptable Object Manipulation
        private ObjectField presetFieldRef;
        private Button openPresetButton;
        private Button resetPresetButton;
        
        //Parameters' graphic
        private VisualElement graphOfHell;
        private PopulationAssistantGraph hellGraph;

        //Layer Context
        private ListView layerList;
        private Button addLayerButton;
        private VisualElement lockedContextEntryContainer;

        //Context Evaluator (the hardest part lol)
        #endregion

        #region FIELDS
        MAPElitesPreset MapEliteBundle
        {
            get => mapEliteBundle;
            set => mapEliteBundle = value;
        }
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
        protected PopulationAssistantGraph CurrentGraph
        {
            get => hellGraph;
            set => hellGraph = value;
        }
        protected PopulationBehaviour LayerPopulation
        {
            get => assistant.OwnerLayer.Behaviours.Find(b => b.GetType().Equals(typeof(PopulationBehaviour))) as PopulationBehaviour;
        }
        protected LBSLevelData Data
        {
            get => LayerPopulation.OwnerLayer.Parent;
        }
        #endregion

        #region EVENTS
        // public Action OnExecute;
        public Action<string> OnPresetChanged;
        public Action OnTileMapChanged;
        public Action OnTileMapReset;
        public Action UpdatePins;
        public Action<IOptimizable> OnValuesUpdated;
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
            presetField.RegisterValueChangedCallback(evt =>
            {
                UpdatePreset(evt.newValue);
                LBSMainWindow.MessageNotify($"Selected MAP Elite preset: {evt.newValue}");
            });

            //Progress Bar and Sliders
            xParamText = rootVisualElement.Q<Label>("XParamText");
            yParamText = rootVisualElement.Q<Label>("YParamText");
            zParamText = rootVisualElement.Q<Label>("ZParamText");
            xProgressBar = rootVisualElement.Q<ProgressBar>("XProgressBar");
            yProgressBar = rootVisualElement.Q<ProgressBar>("YProgressBar");
            zProgressBar = rootVisualElement.Q<ProgressBar>("ZProgressBar");

            //var interiorLayer = assistant.OwnerLayer.Parent.Layers.Where(l => l.ID.Equals("Interior") && l.IsVisible).ToList();
            //interiorLayer.ForEach(l => l.GetModule<SectorizedTileMapModule>()?.RecalculateZonesProximity()); // Buscar un mejor lugar para esto despues

            //Set parameters. Make everyone a ranged evaluator, make the value a default, add the listener to change the chosen elite bundle and then disable it.
            //I set everything false so they can't be manipulated if there's no preset present.
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
                InitializeEvaluator(xChoice);
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
                InitializeEvaluator(yChoice);
                yParamText.text = param2Field.Value;
            });
            param2Field.SetEnabled(false);

            //Optimizer

            optimizerField = rootVisualElement.Q<ClassDropDown>("ZParamDropdown");
            optimizerField.Type = typeof(IRangedEvaluator);
            optimizerField.value = defaultSelectText;
            optimizerField.RegisterValueChangedCallback(evt =>
            {
                if (optimizerField.value == null) return;
                if (optimizerField.value == currentOptimizer?.Evaluator?.GetType().Name) return;
                if (currentOptimizer == null) return;

                var optimizerChoice = optimizerField.GetChoiceInstance() as IRangedEvaluator;
                currentOptimizer.Evaluator = optimizerChoice;
                InitializeEvaluator(optimizerChoice);
                zParamText.text = new string("Fitness ("+optimizerField.Value+")");

            });
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

            //Grid
            rows = rootVisualElement.Q<SliderInt>("RowsSlideInt");
            rows.RegisterValueChangedCallback(evt => UpdateGrid());
            columns = rootVisualElement.Q<SliderInt>("ColumnsSlideInt");
            columns.RegisterValueChangedCallback(evt => UpdateGrid());

            gridContent = rootVisualElement.Q<VisualElement>("GridContent");
            UpdateGrid();

            //Recalculate button
            recalculate = rootVisualElement.Q<Button>("ButtonRecalculate");
            recalculate.clicked += RunAlgorithm;

            //Suggestion button
            applySuggestion = rootVisualElement.Q<Button>("ButtonApplySuggestion");
            applySuggestion.clicked += () => ApplySuggestion();

            //Reset button
            originalTileMap = LayerPopulation.BundleTilemap.Clone() as BundleTileMap;
            resetButton = rootVisualElement.Q<Button>("ButtonReset");
            resetButton.clicked += ResetSuggestion;
            resetButton.SetEnabled(false);

            OnTileMapChanged += () => {
                originalTileMap = LayerPopulation.BundleTilemap.Clone() as BundleTileMap;
                resetButton.SetEnabled((originalTileMap!=null));
            };
            OnTileMapReset += () => 
            {
                originalTileMap = null;
                resetButton.SetEnabled(false);
            };

            //Close button...?
            closeWindow = rootVisualElement.Q<Button>("ButtonClose");
            closeWindow.clicked += Close;

            //PARAMETER'S GRAPH
            graphOfHell = rootVisualElement.Q<VisualElement>("GraphOfHell");
            
            //Create and add VisualElement: PopulationAssistantGraph to the container
            hellGraph = new(new[] { 0f, 0f, 0f }, 2);
            SetGraph();
            graphOfHell.Add(hellGraph);

            //LAYER CONTEXT
            lockedContextEntryContainer = rootVisualElement.Q<VisualElement>("LockedLayerContainer");
            AddLockedLayer();

            layerList = rootVisualElement.Q<ListView>("LayerList");

            layerList.reorderable = false;
            layerList.makeItem += () => new LayerContextEntry();
            layerList.bindItem = (element, index) =>
            {
                var layerContextVE = element as LayerContextEntry;
                if (layerContextVE == null) return;

                layerContextVE.UpdateData(Data.ContextLayers[index]);
                layerContextVE.EvaluateOverlap(Data.ContextLayers);
                layerContextVE.OnRemoveButtonClicked = null;
                layerContextVE.OnRemoveButtonClicked += () =>
                {
                    Data.ContextLayers.RemoveAt(index);
                    layerList.Remove(element);
                    layerList.Rebuild();
                };
            };

            layerList.itemsSource = Data.ContextLayers;

            addLayerButton = rootVisualElement.Q<Button>("AddLayerButton");
            addLayerButton.clicked += AddLayerMenu;
        }

        private void OnDestroy()
        {
            assistant.RequestOptimizerStop();
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
                optimizerField.SetEnabled(false);
                return;
            }
            
            //Set the map elite accordingly.
            mapEliteBundle = presetDictionary[value];
            presetFieldRef.value = mapEliteBundle;
            rows.value = mapEliteBundle.SampleCount.x;
            columns.value = mapEliteBundle.SampleCount.y;

            //Enable params set the preset things to the new choice.
            param1Field.SetEnabled(true);
            param2Field.SetEnabled(true);
            optimizerField.SetEnabled(true);

            param1Field.Value = currentXField != null ? currentXField.GetType().Name : defaultSelectText;
            param2Field.Value = currentYField != null ? currentYField.GetType().Name : defaultSelectText;
            optimizerField.value = currentOptimizer?.Evaluator != null ? currentOptimizer.Evaluator.GetType().Name : defaultSelectText;
            
            yParamText.text = param2Field.Value;
            xParamText.text = param1Field.Value;
            zParamText.text = new string("Fitness (" + optimizerField.Value + ")");

            //param1Field.tooltip = currentXField.Tooltip;
            //param2Field.tooltip = currentYField.Tooltip;
            //optimizerField.tooltip = currentOptimizer?.Evaluator.Tooltip;

            InitializeAllCurrentEvaluators();
        }

        private void UpdateTooltips()
        {
            param1Field.tooltip = currentXField?.Tooltip;
            param2Field.tooltip = currentYField?.Tooltip;
            optimizerField.tooltip = currentOptimizer?.Evaluator?.Tooltip;
        }

        private void InitializeAllCurrentEvaluators()
        {
            var evalList = new List<IEvaluator>();
            if (currentXField != null) { evalList.Add(currentXField); }
            if (currentYField != null) { evalList.Add(currentYField); }
            if (currentOptimizer?.Evaluator != null) { evalList.Add(currentOptimizer.Evaluator); }
            if (evalList.Count == 0) return;

            InitializeEvaluator(evalList.ToArray());
        }

        private void InitializeEvaluator(params IEvaluator[] evaluators)
        {
            foreach (var evaluator in evaluators)
            {
                InitializeEvaluator(evaluator);
            }
        }

        private void InitializeEvaluator(IEvaluator evaluator)
        {
            if(evaluator != null)
            {
                var contextualChoice = evaluator as IContextualEvaluator;
                if (contextualChoice != null)
                    contextualChoice.InitializeDefaultWithContext(Data.ContextLayers, assistant.RawToolRect);
                else
                    evaluator.InitializeDefault();
            }

            UpdateTooltips();
        }

        //Run the algorithm for suggestions
        private void RunAlgorithm()
        {
            if(mapEliteBundle == null)
            {
                LBSMainWindow.MessageNotify("MAP Elite Preset not selected or null.", LogType.Error, 5);
                Debug.LogError("[ISI Lab]: MAP Elite Preset not selected or null.");
                return;
            }

            //Check if there's a place to optimize
            if (assistant.RawToolRect.width == 0 || assistant.RawToolRect.height == 0)
            {
                LBSMainWindow.MessageNotify("Use the Area Selector tool to select an area to optimize before starting MAP Elites.", LogType.Error, 5);
                Debug.LogError("[ISI Lab]: Selected evolution area height or width < 0");
                return;
            }

            //Check how many of these there are, and get the optimizer!
            var veChildren = GetButtonResults(new List<PopulationAssistantButtonResult>(), gridContent);

            UpdateGrid();

            InitializeAllCurrentEvaluators();

            //This resets the algorithm all the time, so nothing to worry about regarding whether it's running or not. /// Not sure about that...
            assistant.LoadPresset(mapEliteBundle);

            //SetBackgroundTexture(square, assistant.RawToolRect);
            assistant.SetAdam(assistant.RawToolRect, Data.ContextLayers);
            assistant.Execute();

            //TODO: Hay que pasarle el Optimizer a los Map Elites
            LBSMainWindow.OnWindowRepaint += RepaintContent;

            //Update button
            recalculate.text = "Recalculate";
            
            LBSMainWindow.MessageNotify("Calculating.");
        }

        //Apply the suggestion in the world
        private void ApplySuggestion() => ApplySuggestion(selectedMap.Data);
        private void ApplySuggestion(object obj)
        {
            //This MUST go first since it'll save the original tilemap
            OnTileMapChanged?.Invoke();

            var chrom = obj as BundleTilemapChromosome;
            if (chrom == null)
            {
                if (selectedMap.Data == null) throw new Exception("[ISI Lab] Data " + selectedMap.Data.GetType().Name + " is not LBSChromosome!");
            }

            var level = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(level, "Add Element population");

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
            DrawManager.Instance.RedrawLayer(assistant.OwnerLayer);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(level);
            }
            LBSMainWindow.MessageNotify("Layer modified by Population Assistant");

            LayerPopulation.OwnerLayer.OnChangeUpdate();
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

        }

        //Reset the suggestion to its original form
        private void ResetSuggestion()
        {
            if (originalTileMap == null) return;
            if (LayerPopulation.Tilemap == null)
            {
                LBSMainWindow.MessageNotify("Layer tile map not found."); return;
            }
            var level = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(level, "Add Element population");

            LayerPopulation.ReplaceTileMap(originalTileMap);
            DrawManager.Instance.RedrawLayer(assistant.OwnerLayer);
            LBSMainWindow.MessageNotify("Layer reset.");

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(level);
            }
            LayerPopulation.OwnerLayer.OnChangeUpdate();
            OnTileMapReset?.Invoke();
        }
        #endregion

        #region GRID-RELATED METHODS
        
        private void RepaintContent()
        {
            try
            {
                UpdateContent();
            }
            finally
            {
                if (assistant.Finished || !assistant.Running) // La segunda condicion evita los logs de error de Map Elites, pero obviamente no es una solucion, y no he probado si afecta a los resultados renderizados
                {
                    LBSMainWindow.OnWindowRepaint -= RepaintContent;
                }
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
            if(mapEliteBundle!=null)
            {
                mapEliteBundle.SampleCount = new Vector2Int(rows.value, columns.value);
            }
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
                for (int i = 0; i < texture.width; i++)
                {
                    texture.SetPixel(i, j, new UnityEngine.Color(0.25f, 0.25f, 0.25f, 1));
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

            xProgressBar.value = (float)mapData.xFitness;
            yProgressBar.value = (float)mapData.yFitness;
            zProgressBar.value = (float)mapData.Fitness;
            xProgressBar.title = (Mathf.FloorToInt((float)mapData.xFitness * 100)).ToString() + "%";
            yProgressBar.title = (Mathf.FloorToInt((float)mapData.yFitness * 100)).ToString() + "%";
            zProgressBar.title = (Mathf.FloorToInt((float)mapData.Fitness * 100)).ToString() + "%";

            //Takes border off selected map previously selected map
            if (selectedMap != null)
            {
                selectedMap.OnButtonDeselected?.Invoke();
            }
            selectedMap = buttonResult;
            buttonResult.OnButtonSelected?.Invoke();
            OnValuesUpdated?.Invoke(mapData);
        }

        void SetGraph()
        {
            //Modify graph's colors (not necessary, it comes with default colors)
            CurrentGraph.MainColor = Color.green;
            CurrentGraph.SecondaryColor = Color.cyan;

            CurrentGraph.SetAxisColor(Color.blue, 0);
            CurrentGraph.SetAxisColor(Color.red, 1);
            CurrentGraph.SetAxisColor(Color.green, 2);

            OnValuesUpdated = null;
            OnValuesUpdated += (optimizable) => {
                var opt = optimizable as IOptimizable;
                CurrentGraph.SetAxisValue((float)opt.Fitness, 0);
                CurrentGraph.SetAxisValue((float)opt.xFitness, 1);
                CurrentGraph.SetAxisValue((float)opt.yFitness, 2);
                CurrentGraph.RecalculateCorners();
                CurrentGraph.MarkDirtyRepaint();
            };
        }
        #endregion

        #region LAYER CONTEXT METHODS
        public void AddLockedLayer()
        {
            //Add the layer to Layer context
            var lockedLayer = new LayerContextEntry();
            lockedLayer.UpdateData(assistant.OwnerLayer);
            lockedLayer.SetEnabled(false);
            lockedContextEntryContainer.Add(lockedLayer);
        }

        public void AddLayerMenu()
        {
            GenericMenu menu = new GenericMenu();
            foreach(LBSLayer layer in Data.Layers)
            {
                //The layer the assistant is working on can't be used as context, since its content is overwritten.
                if (!assistant.OwnerLayer.Equals(layer))
                { 
                    menu.AddItem(new GUIContent(layer.Name), Data.ContextLayers.Contains(layer), ToggleLayerContext, layer); 
                }
            }
            menu.ShowAsContext();
        }

        private void ToggleLayerContext(object layer)
        {
            LBSLayer objectLayer = layer as LBSLayer;
            if (objectLayer == null)
            {
                Debug.LogError("Object Layer was null.");
                return;
            }
            switch(Data.ContextLayers.Contains(layer))
            {
                case true: Data.ContextLayers.Remove(objectLayer); break;
                case false: Data.ContextLayers.Add(objectLayer); break;
            }
            layerList.Rebuild();

            InitializeAllCurrentEvaluators();
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