using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ISILab.Extensions;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Bundles;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace ISILab.LBS.Assistants
{
    #region TILE BUNDLE GROUP TO ACTION WRAPPER
    /// <summary>
    /// Stores a tile of bundles to a string that can be applied on them
    /// </summary>
    public readonly struct TileBundleToAction : IEquatable<TileBundleToAction>
    {
        public List<TileBundleGroup> Tiles { get; }
        public string Action { get; }

        public TileBundleToAction(List<TileBundleGroup> tiles, string action)
        {
            Tiles = tiles ?? new List<TileBundleGroup>();
            Action = action ?? string.Empty;
        }

        public bool Equals(TileBundleToAction other)
        {
            if (Action == other.Action) return true;
            if (Tiles == null && other.Tiles == null) return true;
            
            return false;
        }

        public override bool Equals(object obj) => obj is TileBundleToAction other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Tiles, Action);
    }
    #endregion
    
    #region ACTION INFO (Action and RequiredTags)
    /// <summary>
    /// Represents an action with a name and required tags.
    /// </summary>
    public struct ActionInfo
    {
        public string Action { get; }
        public List<string> RequiredLBSTags { get; }

        public ActionInfo(string action, List<string> requiredTags = null)
        {
            Action = action;
            RequiredLBSTags = requiredTags ?? new List<string>();
        }
    }

    /// <summary>
    /// Represents a combination of population types for action mapping.
    /// </summary>
    public readonly struct ElementFlagToAction : IEquatable<ElementFlagToAction>
    {
        public readonly List<Bundle.EElementFlag> Types;

        public ElementFlagToAction(IEnumerable<Bundle.EElementFlag> types)
        {
            Types = types.OrderBy(t => (int)t).ToList();
        }

        public bool Equals(ElementFlagToAction other) =>
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
    public static class ElementActionDictionary
    {
        public static readonly Dictionary<ElementFlagToAction, List<ActionInfo>> Definitions = new()
        {
            {
                new ElementFlagToAction(new[] { Bundle.EElementFlag.Character }),
                new List<ActionInfo>
                {
                    new ActionInfo("stealth"),
                    new ActionInfo("spy")
                }
            },
            {
                new ElementFlagToAction(new[] { Bundle.EElementFlag.Enemy }),
                new List<ActionInfo>
                {
                    new ActionInfo("kill"),
                    new ActionInfo("capture")
                }
            },
            {
                new ElementFlagToAction(new[] { Bundle.EElementFlag.Ally }),
                new List<ActionInfo>
                {
                    new ActionInfo("listen"),
                    new ActionInfo("report")
                }
            },
            {
                new ElementFlagToAction(new[] { Bundle.EElementFlag.Item }),
                new List<ActionInfo>
                {
                    new ActionInfo("gather"), 
                    new ActionInfo("take"),
                    new ActionInfo("read")
                }
            },
            {
                new ElementFlagToAction(new[] { Bundle.EElementFlag.Ally, Bundle.EElementFlag.Item }),
                new List<ActionInfo>
                {
                    new ActionInfo("give"), 
                    new ActionInfo("exchange")
                }
            }
        };
    }
    #endregion

    [Serializable]
    [RequieredModule(typeof(QuestGraph))]
    public class QuestAssistant : LBSAssistant
    {
        #region FIELDS
        
        [SerializeField]
        private uint suggestionAmount = 3;
        private Vector2Int _positionOverlapOffset = new(25, 50);
        #endregion
        
        #region PROPERTIES
        [JsonIgnore]
        private QuestGraph QuestGraph => OwnerLayer.GetModule<QuestGraph>();

        [JsonIgnore] public uint SuggestionAmount
        {
            get => suggestionAmount;
            set => suggestionAmount = value;
        }

        public LBSLevelData Data => QuestGraph.OwnerLayer.Parent;

        #endregion

        #region CONSTRUCTORS
        public QuestAssistant() : base(null, null, Color.black) { }

        public QuestAssistant(VectorImage icon, string name, Color colorTint)
            : base(icon, name, colorTint) { }
        #endregion

        #region PUBLIC METHODS
        public override object Clone() => new QuestAssistant(Icon, Name, ColorTint);

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
        public  List<TileBundleToAction> GenerateSuggestions(int suggestionsCount, Action<float> onProgress = null, CancellationToken token = default)
        {
            return GenerateSuggestionList(suggestionsCount, onProgress, token);
        }
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Generates a list of suggestions from population layers.
        /// </summary>
        private List<TileBundleToAction> GenerateSuggestionList(int suggestionsCount,  Action<float> onProgress,  CancellationToken token = default)
        {
            HashSet<TileBundleToAction> suggestionList = new HashSet<TileBundleToAction>();
            foreach (var contextLayer in Data.ContextLayers)
            {
                if (token.IsCancellationRequested) return suggestionList.ToList();
                var populationLayer = contextLayer.GetBehaviour<PopulationBehaviour>();
                if (populationLayer == null) continue;
                for (int index = 0; index < suggestionsCount; index++)
                {
                    if (token.IsCancellationRequested) return suggestionList.ToList();
                    var newSuggestion = SuggestActionFromPopulation(populationLayer);
                    suggestionList.Add(newSuggestion);
                    onProgress?.Invoke((float)index/suggestionList.Count);
                }
     
            }
            
            return suggestionList.ToList();
        }

        /// <summary>
        /// Creates suggestion nodes in the quest graph using the suggestion list.
        /// </summary>
        public  List<QuestNode> CreateNewSuggestions(List<TileBundleToAction> suggestions, Action<float> onProgress = null, CancellationToken token = default)
        {
            List<QuestNode> Suggestions = new List<QuestNode>();
            QuestGraph.Suggestions.Clear();
            if(!suggestions.Any()) return Suggestions;
            
            List<Vector2> existingPositions = new();
            suggestions = suggestions.Distinct().ToList();
            for (var index = 0; index < suggestions.Count; index++)
            {
                if(token.IsCancellationRequested) return Suggestions;
                var entry = suggestions[index];
                var newNode = QuestGraph.CreateSuggestionNode(entry.Action, Suggestions);
                var nodeData = newNode.NodeData;

                entry.Tiles.Shuffle();
                nodeData.SetDataByTiles(Data.ContextLayers, entry.Tiles);
                nodeData.Resize();

                // trigger position is used to draw the suggestion element area
                var triggerPos = nodeData.Area.position;
                // to move the capsule within the suggestion element area
                Vector2Int offsetPosition = Vector2Int.zero;

                while (existingPositions.Contains(triggerPos))
                {
                    triggerPos += _positionOverlapOffset;
                    offsetPosition += _positionOverlapOffset;
                }

                existingPositions.Add(triggerPos);
                newNode.Position = offsetPosition;
                Suggestions.Add(newNode);
                
                onProgress?.Invoke((float)index/suggestions.Count);
            }
            
            return Suggestions;
        }

        private List<KeyValuePair<ElementFlagToAction, HashSet<TileBundleGroup>>> GetValidGroups(PopulationBehaviour populationLayer)
        {
            var groups = GroupTilesByPopulationType(populationLayer);
            return groups.Where(g => g.Value.Any()).ToList();
        }

        /// <summary>
        /// Maps tileGroups to their valid actions based on population type combination.
        /// </summary>
        private Dictionary<TileBundleGroup, HashSet<string>> MapTilesToActions(HashSet<TileBundleGroup> tileGroups, ElementFlagToAction combo)
        {
            var tilesToActions = new Dictionary<TileBundleGroup, HashSet<string>>();
            foreach (var tileGroup in tileGroups)
            {
                var validActions = GetActionsByTileGroup(tileGroup, combo);
                if (validActions.Any())
                {
                    tilesToActions[tileGroup] = new HashSet<string>(validActions);
                }
            }
            return tilesToActions;
        }

        /// <summary>
        /// Groups tiles by population type combinations.
        /// </summary>
        private Dictionary<ElementFlagToAction, HashSet<TileBundleGroup>> GroupTilesByPopulationType(PopulationBehaviour populationLayer)
        {
            var dictionary = ElementActionDictionary.Definitions.Keys
                .ToDictionary(entryDef => entryDef, _ => new HashSet<TileBundleGroup>());

            foreach (var tile in populationLayer.Tilemap)
            {
                var flag = tile.BundleData.Bundle.ElementFlag;
                foreach (ElementFlagToAction flagToAction in dictionary.Keys)
                {
                    if (flagToAction.Types.Contains(flag))
                        dictionary[flagToAction].Add(tile);
                }
            }
            return dictionary;
        }

        /// <summary>
        /// Gets valid actions for a tile based on the population type combination.
        /// </summary>
        private List<string> GetActionsByTileGroup(TileBundleGroup tile, ElementFlagToAction combo)
        {
            if (!ElementActionDictionary.Definitions.TryGetValue(combo, out var actions))
                return new List<string>();

            var validActionNames = new List<string>();
            foreach (var action in actions)
            {
                if (action.RequiredLBSTags.All(tag => tile.BundleData.Bundle.GetHasTagCharacteristic(tag)))
                {
                    validActionNames.Add(action.Action);
                }
            }
            return validActionNames;
        }

        private TileBundleToAction SuggestActionFromPopulation(PopulationBehaviour populationLayer)
        {
            var validGroups = GetValidGroups(populationLayer);
            if (!validGroups.Any())
                return new TileBundleToAction(new List<TileBundleGroup>(), string.Empty);

            var rnd = new System.Random();

            // Pick a random index safely in background thread
            var chosenGroup = validGroups[rnd.Next(0, validGroups.Count)];
            var chosenTiles = chosenGroup.Value;

            // Map tiles to valid actions
            var tilesToActions = MapTilesToActions(chosenTiles, chosenGroup.Key);
            if (!tilesToActions.Any())
                return new TileBundleToAction(new List<TileBundleGroup>(), string.Empty);

            return GetActionByTileGroup(tilesToActions, rnd);
}

        private TileBundleToAction GetActionByTileGroup(Dictionary<TileBundleGroup, HashSet<string>> tilesToActions, System.Random rnd)
        {
            if (!tilesToActions.Any())
                return new TileBundleToAction(new List<TileBundleGroup>(), string.Empty);

            // Shuffle dictionary keys to avoid consistent order
            var shuffledKeys = tilesToActions.Keys.OrderBy(_ => rnd.Next()).ToList();

            var commonActions = new HashSet<string>(tilesToActions[shuffledKeys[rnd.Next(shuffledKeys.Count)]]);
            foreach (var key in shuffledKeys.Skip(1))
            {
                commonActions.IntersectWith(tilesToActions[key]);
                if (!commonActions.Any())
                    break;
            }

            if (commonActions.Any())
            {
                // Shuffle commonActions
                var shuffledActions = commonActions.OrderBy(_ => rnd.Next()).ToList();
                var action = shuffledActions[rnd.Next(shuffledActions.Count)];
                return new TileBundleToAction(tilesToActions.Keys.ToList(), action);
            }

            // Fallback to a random tile and action
            var randomEntry = tilesToActions.ElementAt(rnd.Next(0, tilesToActions.Count));
            var shuffledEntryActions = randomEntry.Value.OrderBy(_ => rnd.Next()).ToList();
            var fallbackAction = shuffledEntryActions.Count > 0 ? shuffledEntryActions[rnd.Next(shuffledEntryActions.Count)] : string.Empty;

            return new TileBundleToAction(
                new List<TileBundleGroup> { randomEntry.Key },
                fallbackAction
            );
        }

        #endregion
    }
}