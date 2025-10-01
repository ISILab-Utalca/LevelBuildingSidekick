using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Modules;
using ISILab.Macros;
using LBS.Bundles;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Components;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace ISILab.LBS.Assistants
{
    #region ACTION SUGGESTION DICTIONARY
    /// <summary>
    /// Represents an action with a name and required tags.
    /// </summary>
    public struct ActionInfo
    {
        public string Name { get; }
        public List<string> RequiredTags { get; }

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

        public bool Equals(PopulationTypeCombination other) =>
            Types.Count == other.Types.Count && Types.SequenceEqual(other.Types);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 0;
                foreach (var t in Types)
                    hash += t.GetHashCode();
                return hash;
            }
        }
    }
    #endregion

    #region ACTION DEFINITIONS
    /// <summary>
    /// Defines valid actions for combinations of population types.
    /// </summary>
    public static class PopulationActions
    {
        public static readonly Dictionary<PopulationTypeCombination, List<ActionInfo>> ActionsByCombination = new()
        {
            {
                new PopulationTypeCombination(new[] { Bundle.PopulationTypeE.Character }),
                new List<ActionInfo> { new("kill"), new("listen") }
            },
            {
                new PopulationTypeCombination(new[] { Bundle.PopulationTypeE.Item }),
                new List<ActionInfo> { new("gather"), new("take") }
            },
            {
                new PopulationTypeCombination(new[] { Bundle.PopulationTypeE.Character, Bundle.PopulationTypeE.Item }),
                new List<ActionInfo> { new("give"), new("exchange"), new("stealth") }
            }
        };
    }
    #endregion

    [Serializable]
    [RequieredModule(typeof(QuestGraph))]
    public class QuestAssistant : LBSAssistant
    {
        #region PROPERTIES
        [JsonIgnore]
        private QuestGraph QuestGraph => OwnerLayer.GetModule<QuestGraph>();

        public LBSLevelData Data => QuestGraph.OwnerLayer.Parent;
        #endregion

        #region CONSTRUCTORS
        public QuestAssistant() : base(null, null, Color.black) { }

        public QuestAssistant(VectorImage icon, string name, Color colorTint)
            : base(icon, name, colorTint) { }
        #endregion

        #region PUBLIC METHODS
        public override object Clone() => new QuestAssistant(Icon, Name, ColorTint);

        public override void OnAttachLayer(LBSLayer layer) => base.OnAttachLayer(layer);

        public override void OnGUI() { }

        /// <summary>
        /// Generates a specified number of random quest nodes starting from a random root action.
        /// </summary>
        public void GenerateRandomNodes(int count)
        {
            var grammarAssistant = QuestGraph.OwnerLayer.GetAssistant<GrammarAssistant>();
            Assert.IsNotNull(grammarAssistant, "GrammarAssistant should not be null.");

            if (QuestGraph.Grammar.TerminalActions.Count == 0) return;

            // Set random root node
            var randomIndex = Random.Range(0, QuestGraph.Grammar.TerminalActions.Count);
            var currentNode = QuestGraph.AddNewQuestNode(QuestGraph.Grammar.TerminalActions[randomIndex], Vector2.zero);
            QuestGraph.SetRoot(currentNode);

            // Add subsequent nodes
            for (int i = 1; i < count - 1; i++)
            {
                var nextActions = grammarAssistant.GetAllValidNextActionsInsert(currentNode.QuestAction, QuestGraph);
                if (!nextActions.Any()) break;

                var newAction = nextActions[Random.Range(0, nextActions.Count)];
                currentNode = QuestGraph.AddNewQuestNode(newAction, Vector2.zero);
            }
        }

        /// <summary>
        /// Connects all quest nodes in sequence.
        /// </summary>
        public void ConnectAllNodes()
        {
            for (int i = 0; i < QuestGraph.GraphNodes.Count - 1; i++)
            {
                QuestGraph.AddEdge(QuestGraph.GraphNodes[i], QuestGraph.GraphNodes[i + 1]);
            }
        }

        /// <summary>
        /// Generates suggestion nodes based on population data from context layers.
        /// </summary>
        public void GenerateSuggestions(int suggestionsCount)
        {
            var suggestionList = GenerateSuggestionList(suggestionsCount);
            CreateSuggestionNodes(suggestionList);
        }
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Generates a list of suggestions from population layers.
        /// </summary>
        private List<KeyValuePair<List<TileBundleGroup>, string>> GenerateSuggestionList(int suggestionsCount)
        {
            var suggestionList = new List<KeyValuePair<List<TileBundleGroup>, string>>();
            foreach (var contextLayer in Data.ContextLayers)
            {
                var populationLayer = contextLayer.GetBehaviour<PopulationBehaviour>();
                if (populationLayer == null) continue;

                for (int i = 0; i < suggestionsCount; i++)
                {
                    suggestionList.Add(SuggestActionFromPopulation(populationLayer));
                }
            }
            return suggestionList;
        }

        /// <summary>
        /// Creates suggestion nodes in the quest graph using the suggestion list.
        /// </summary>
        private void CreateSuggestionNodes(List<KeyValuePair<List<TileBundleGroup>, string>> suggestionList)
        {
            foreach (var suggestion in suggestionList)
            {
                var middlePosition = CalculateMiddlePosition(suggestion.Key);
                var suggestionNode = QuestGraph.AddSuggestion(suggestion.Value, middlePosition);
                var nodeData = suggestionNode.NodeData;
                ListHelper.Shuffle(suggestion.Key);
                nodeData.SetDataByTiles(Data.ContextLayers,suggestion.Key);
            }
        }

        /// <summary>
        /// Calculates the average position of a list of TileBundleGroups.
        /// </summary>
        private Vector2 CalculateMiddlePosition(List<TileBundleGroup> tileBundleGroups)
        {
            if (tileBundleGroups == null || tileBundleGroups.Count == 0)
                return Vector2.zero;

            Vector2 middlePosition = Vector2.zero;
            foreach (var tileBundleGroup in tileBundleGroups)
            {
                middlePosition += tileBundleGroup.AreaRect.center;
            }
            return middlePosition / tileBundleGroups.Count;
        }

        /// <summary>
        /// Suggests a single action based on population layer data.
        /// </summary>
        private KeyValuePair<List<TileBundleGroup>, string> SuggestActionFromPopulation(PopulationBehaviour populationLayer)
        {
            var groups = GroupTilesByPopulationType(populationLayer);
            var validGroups = groups.Where(g => g.Value.Any()).ToList();
            if (!validGroups.Any())
                return new KeyValuePair<List<TileBundleGroup>, string>(new List<TileBundleGroup>(), string.Empty);

            // Pick a random group
            var chosenGroup = validGroups[Random.Range(0, validGroups.Count)];
            var chosenTiles = chosenGroup.Value;

            // Map tiles to valid actions
            var tilesToActions = MapTilesToActions(chosenTiles, chosenGroup.Key);
            if (!tilesToActions.Any())
                return new KeyValuePair<List<TileBundleGroup>, string>(new List<TileBundleGroup>(), string.Empty);

            return GetActionByTileGroup(tilesToActions);
        }

        /// <summary>
        /// Maps tiles to their valid actions based on population type combination.
        /// </summary>
        private Dictionary<TileBundleGroup, HashSet<string>> MapTilesToActions(HashSet<TileBundleGroup> tiles, PopulationTypeCombination combo)
        {
            var tilesToActions = new Dictionary<TileBundleGroup, HashSet<string>>();
            foreach (var tile in tiles)
            {
                var validActions = GetValidActionsForTile(tile, combo);
                if (validActions.Any())
                {
                    tilesToActions[tile] = new HashSet<string>(validActions);
                }
            }
            return tilesToActions;
        }

        /// <summary>
        /// Groups tiles by population type combinations.
        /// </summary>
        private Dictionary<PopulationTypeCombination, HashSet<TileBundleGroup>> GroupTilesByPopulationType(PopulationBehaviour populationLayer)
        {
            var groups = PopulationActions.ActionsByCombination.Keys
                .ToDictionary(combo => combo, _ => new HashSet<TileBundleGroup>());

            foreach (var tile in populationLayer.Tilemap)
            {
                var tileType = tile.BundleData.Bundle.PopulationType;
                foreach (var combo in groups.Keys)
                {
                    if (combo.Types.Contains(tileType))
                        groups[combo].Add(tile);
                }
            }
            return groups;
        }

        /// <summary>
        /// Gets valid actions for a tile based on the population type combination.
        /// </summary>
        private List<string> GetValidActionsForTile(TileBundleGroup tile, PopulationTypeCombination combo)
        {
            if (!PopulationActions.ActionsByCombination.TryGetValue(combo, out var actions))
                return new List<string>();

            var validActionNames = new List<string>();
            foreach (var action in actions)
            {
                if (action.RequiredTags.All(tag => tile.BundleData.Bundle.GetHasTagCharacteristic(tag)))
                {
                    validActionNames.Add(action.Name);
                }
            }
            return validActionNames;
        }

        /// <summary>
        /// Selects a common action for a group of tiles or a random action if no common action exists.
        /// </summary>
        private KeyValuePair<List<TileBundleGroup>, string> GetActionByTileGroup(Dictionary<TileBundleGroup, HashSet<string>> tilesToActions)
        {
            if (!tilesToActions.Any())
                return new KeyValuePair<List<TileBundleGroup>, string>(new List<TileBundleGroup>(), string.Empty);

            // Find common actions across all tiles
            var commonActions = new HashSet<string>(tilesToActions.First().Value);
            foreach (var kvp in tilesToActions.Skip(1))
            {
                commonActions.IntersectWith(kvp.Value);
                if (!commonActions.Any())
                    break;
            }

            if (commonActions.Any())
            {
                return new KeyValuePair<List<TileBundleGroup>, string>(
                    tilesToActions.Keys.ToList(),
                    commonActions.First()
                );
            }

            // Fallback to a random tile and action
            var randomEntry = tilesToActions.ElementAt(Random.Range(0, tilesToActions.Count));
            return new KeyValuePair<List<TileBundleGroup>, string>(
                new List<TileBundleGroup> { randomEntry.Key },
                randomEntry.Value.FirstOrDefault() ?? string.Empty
            );
        }
        #endregion
    }
}