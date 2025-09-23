using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.LBS.Assistants;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.VisualElements;
using ISILab.LBS.VisualElements.Editor;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.CustomComponents;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
using ISILab.Macros;
using LBS.Components;
using LBS.Bundles;
using LBS.VisualElements;
using Random = UnityEngine.Random;

namespace ISILab.LBS.Editor
{
    #region ACTION SUGGESTION DEFINITIONS
    /// <summary>
    /// Represents an action with a name and the required tags to access it.
    /// </summary>
    public struct ActionInfo
    {
        public readonly string Name;
        public readonly List<string> RequiredTags;

        public ActionInfo(string name, List<string> requiredTags = null)
        {
            Name = name;
            RequiredTags = requiredTags ?? new List<string>();
        }
    }

    /// <summary>
    /// Represents a combination of population types for action mapping.
    /// </summary>
    public readonly struct PopulationTypeCombination : IEquatable<PopulationTypeCombination>
    {
        public readonly List<Bundle.PopulationTypeE> Types;

        public PopulationTypeCombination(IEnumerable<Bundle.PopulationTypeE> types)
        {
            Types = types.OrderBy(t => (int)t).ToList();
        }

        public bool Equals(PopulationTypeCombination other)
        {
            return Types.Count == other.Types.Count && Types.SequenceEqual(other.Types);
        }

        public override int GetHashCode()
        {
            return Types.Aggregate(17, (hash, t) => hash * 31 + t.GetHashCode());
        }
    }

    /// <summary>
    /// Defines valid actions for combinations of population types.
    /// </summary>
    public static class PopulationActions
    {
        public static readonly Dictionary<PopulationTypeCombination, List<ActionInfo>> ActionsByCombination = new()
        {
            {
                new PopulationTypeCombination(new[] { Bundle.PopulationTypeE.Character }),
                new List<ActionInfo>
                {
                    new("kill", new List<string> { /*"LBSTAG"*/ }),
                    new("listen", new List<string> { /*"LBSTAG"*/ })
                }
            },
            {
                new PopulationTypeCombination(new[] { Bundle.PopulationTypeE.Item }),
                new List<ActionInfo>
                {
                    new("gather", new List<string> { /*"LBSTAG"*/ }),
                    new("take", new List<string> { /*"LBSTAG"*/ })
                }
            },
            {
                new PopulationTypeCombination(new[] { Bundle.PopulationTypeE.Character, Bundle.PopulationTypeE.Item }),
                new List<ActionInfo>
                {
                    new("give", new List<string> { /*"LBSTAG"*/ }),
                    new("exchange", new List<string> { /*"LBSTAG"*/ }),
                    new("stealth", new List<string> { /*"LBSTAG"*/ })
                }
            }
        };
    }
    #endregion

    [LBSCustomEditor("QuestAssistant", typeof(QuestAssistant))]
    public class QuestAssistantEditor : LBSCustomEditor, IToolProvider
    {
        
        private const int SuggestionCount = 3;
        
        #region FIELDS
        
        private static class UIElementNames
        {
            public const string VisualTree = "QuestAssistantEditor";
            public const string LockedLayerContainer = "LockedLayerContainer";
            public const string LayerList = "LayerList";
            public const string SuggestionList = "SuggestionList";
            public const string AddLayerButton = "AddLayerButton";
            public const string GenerateSuggestionsButton = "GenerateSuggestions";
            public const string NoSuggestionPanel = "NoSuggestionPanel";
        }

        private QuestAssistant _questAssistant;
        private QuestGraph _questGraph;
        private ListView _layerList;
        private ListView _suggestionList;
        private Button _addLayerButton;
        private Button _generateSuggestionsButton;
        private VisualElement _lockedContextEntryContainer;
        private LBSPanelTextIcon _noSuggestionPanel;
        
        #endregion

        #region PROPERTIES
        private LBSLevelData Data => _questGraph.OwnerLayer.Parent;
        #endregion

        #region CONSTRUCTORS
        public QuestAssistantEditor(QuestAssistant target) : base(target)
        {
            SetInfo(target);
            CreateVisualElement();
        }
        #endregion

        #region METHODS
        public sealed override void SetInfo(object target)
        {
            this.target = target as QuestAssistant;
            _questAssistant = target as QuestAssistant;
            _questGraph = _questAssistant?.OwnerLayer.GetModule<QuestGraph>();
        }

        /// <summary>
        /// Creates and configures the visual elements for the editor.
        /// </summary>
        protected sealed override VisualElement CreateVisualElement()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>(UIElementNames.VisualTree);
            visualTree.CloneTree(this);

            _lockedContextEntryContainer = this.Q<VisualElement>(UIElementNames.LockedLayerContainer);
            _layerList = this.Q<ListView>(UIElementNames.LayerList);
            _suggestionList = this.Q<ListView>(UIElementNames.SuggestionList);
            _addLayerButton = this.Q<Button>(UIElementNames.AddLayerButton);
            _generateSuggestionsButton = this.Q<Button>(UIElementNames.GenerateSuggestionsButton);
            _noSuggestionPanel = this.Q<LBSPanelTextIcon>(UIElementNames.NoSuggestionPanel);

            _addLayerButton.clicked += ShowAddLayerMenu;
            _generateSuggestionsButton.clicked += GenerateSuggestions;

            SetupLayerContextList();
            SetupSuggestionList();
            AddLockedLayer();
            return this;
        }

        #region LAYERS
        private void SetupLayerContextList()
        {
            _layerList.reorderable = false;
            _layerList.makeItem = () => new LayerContextEntry();
            _layerList.bindItem = BindLayerContextEntry;
            _layerList.itemsSource = Data.ContextLayers;
            UpdateContextDisplay();
        }
        
        private void UpdateContextDisplay()
        {
            _layerList.Rebuild();
            _layerList.style.display = Data.ContextLayers.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }
        
        private void BindLayerContextEntry(VisualElement element, int index)
        {
            if (element is not LayerContextEntry layerContextEntry) return;

            layerContextEntry.UpdateData(Data.ContextLayers[index]);
            layerContextEntry.EvaluateOverlap(Data.ContextLayers);
            layerContextEntry.OnRemoveButtonClicked = null;
            layerContextEntry.OnRemoveButtonClicked += () =>
            {
                Data.ContextLayers.RemoveAt(index);
                UpdateContextDisplay();
            };
        }
        
        private void AddLockedLayer()
        {
            var lockedLayer = new LayerContextEntry();
            lockedLayer.UpdateData(_questGraph.OwnerLayer);
            lockedLayer.SetEnabled(false);
            _lockedContextEntryContainer.Add(lockedLayer);
        }
        
        private void ShowAddLayerMenu()
        {
            var menu = new GenericMenu();
            foreach (var layer in Data.Layers)
            {
                if (!_questGraph.OwnerLayer.Equals(layer))
                {
                    menu.AddItem(new GUIContent(layer.Name), Data.ContextLayers.Contains(layer), ToggleLayerContext, layer);
                }
            }
            menu.ShowAsContext();
        }
        
        private void ToggleLayerContext(object layer)
        {
            if (layer is not LBSLayer lbsLayer)
            {
                Debug.LogError("Invalid layer object.");
                return;
            }

            if (Data.ContextLayers.Contains(lbsLayer))
                Data.ContextLayers.Remove(lbsLayer);
            else
                Data.ContextLayers.Add(lbsLayer);

            UpdateContextDisplay();
        }
        #endregion

        #region SUGGESTIONS
        private void SetupSuggestionList()
        {
            _suggestionList.reorderable = false;
            _suggestionList.makeItem = () => new QuestNodeSuggestion();
            _suggestionList.bindItem = BindQuestNodeSuggestion;
            _suggestionList.itemsSource = _questGraph.Suggestions;
            UpdateSuggestionsDisplay();
        }
        
        private void UpdateSuggestionsDisplay()
        {
            bool hasSuggestions = _suggestionList.itemsSource.Count > 0;
            _suggestionList.Rebuild();
            _noSuggestionPanel.style.display = hasSuggestions ? DisplayStyle.None : DisplayStyle.Flex;
            _suggestionList.style.display = hasSuggestions ? DisplayStyle.Flex : DisplayStyle.None;
        }
        
        private void BindQuestNodeSuggestion(VisualElement element, int index)
        {
            if (element is not QuestNodeSuggestion suggestionEntry) return;

            suggestionEntry.UpdateData(_questGraph.Suggestions[index]);
            suggestionEntry.OnDiscard = null;
            suggestionEntry.OnDiscard += () =>
            {
                _questGraph.Suggestions.RemoveAt(index);
                UpdateSuggestionsDisplay();
            };
        }
        
        private void GenerateSuggestions()
        {
            foreach (var contextLayer in Data.ContextLayers)
            {
                var populationLayer = LBSLayerHelper.GetObjectFromLayer<PopulationBehaviour>(contextLayer);
                if (populationLayer != null)
                {
                    for (int i = 0; i < SuggestionCount; i++)
                    {
                        SuggestActionFromPopulation(populationLayer);
                    }
                }
            }
            UpdateSuggestionsDisplay();
            MarkDirtyRepaint();
        }

        /// <summary>
        /// Suggests a single action based on population layer data.
        /// </summary>
        /// <returns>True if an action was suggested, false otherwise.</returns>
        private bool SuggestActionFromPopulation(PopulationBehaviour populationLayer)
        {
            var groups = GroupTilesByPopulationType(populationLayer);

            // Keep only groups with at least one tile
            var validGroups = new List<KeyValuePair<PopulationTypeCombination, HashSet<TileBundleGroup>>>();
            foreach (var group in groups)
            {
                if (group.Value.Any())
                    validGroups.Add(group);
            }

            if (validGroups.Count == 0) return false;

            // Pick a random group
            var randomIndex = Random.Range(0, validGroups.Count);
            var chosenGroup = validGroups[randomIndex];
            var chosenCombo = chosenGroup.Key;
            var chosenTiles = chosenGroup.Value;

            // Collect all valid actions from the tiles in the chosen group
            var validActions = new HashSet<string>();
            foreach (var tile in chosenTiles)
            {
                var actionsForTile = GetValidActionsForTile(tile, chosenCombo);
                foreach (var action in actionsForTile)
                {
                    validActions.Add(action); // HashSet ensures uniqueness
                }
            }

            if (validActions.Count == 0) return false;

            // Pick a random action from the valid actions
            var actionList = validActions.ToList();
            var chosenAction = actionList[Random.Range(0, actionList.Count)];

            Debug.Log($"Suggested Action: {chosenAction}");
            return true;
        }

        /// <summary>
        /// Groups tiles by population type combinations defined in PopulationActions.
        /// </summary>
        /// <param name="populationLayer">The population layer containing tiles.</param>
        /// <returns>A dictionary mapping population type combinations to their corresponding tiles.</returns>
        private Dictionary<PopulationTypeCombination, HashSet<TileBundleGroup>> GroupTilesByPopulationType(PopulationBehaviour populationLayer)
        {
            // Initialize result dictionary with all possible combinations
            var groups = PopulationActions.ActionsByCombination.Keys
                .ToDictionary(combo => combo, _ => new HashSet<TileBundleGroup>());

            // Group tiles by their population type
            foreach (var tile in populationLayer.Tilemap)
            {
                var tileType = tile.BundleData.Bundle.PopulationType;
                foreach (var combo in groups.Keys)
                {
                    if (combo.Types.Contains(tileType))
                    {
                        groups[combo].Add(tile);
                    }
                }
            }

            return groups;
        }

        /// <summary>
        /// Gets valid actions for a tile based on the population type combination.
        /// </summary>
        /// <param name="tile">The tile to evaluate actions for.</param>
        /// <param name="combo">The population type combination.</param>
        /// <returns>A list of valid action names.</returns>
        private List<string> GetValidActionsForTile(TileBundleGroup tile, PopulationTypeCombination combo)
        {
            if (!PopulationActions.ActionsByCombination.TryGetValue(combo, out var actions))
                return new List<string>();

            var validActionNames = new List<string>();
            foreach (var action in actions)
            {
                bool hasAllTags = action.RequiredTags.All(tag => tile.BundleData.Bundle.GetHasTagCharacteristic(tag));
                if (action.RequiredTags.Count == 0 || hasAllTags)
                    validActionNames.Add(action.Name);
            }

            return validActionNames;
        }
        #endregion
        
        public override void OnFocus()
        {
            _questGraph.displaySuggestions = true;
            DrawManager.Instance.RedrawLayer(_questGraph.OwnerLayer);
        }
        
        public override void OnUnfocus()
        {
            _questGraph.displaySuggestions = false;
            DrawManager.Instance.RedrawLayer(_questGraph.OwnerLayer);
        }
        
        public void SetTools(ToolKit toolkit) { }
        #endregion
    }
}