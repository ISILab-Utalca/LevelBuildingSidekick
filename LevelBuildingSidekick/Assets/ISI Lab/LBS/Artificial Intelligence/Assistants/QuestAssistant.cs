using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Modules;
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
            // normalize order so {A,B} == {B,A}
            Types = types.OrderBy(t => (int)t).ToList();
        }

        public bool Equals(PopulationTypeCombination other) =>
            Types.Count == other.Types.Count && Types.SequenceEqual(other.Types);

        public override int GetHashCode()
        {
            // order-independent hashing (sum)
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
                new List<ActionInfo>
                {
                    new("kill"),
                    new("listen")
                }
            },
            {
                new PopulationTypeCombination(new[] { Bundle.PopulationTypeE.Item }),
                new List<ActionInfo>
                {
                    new("gather"),
                    new("take")
                }
            },
            {
                new PopulationTypeCombination(new[] { Bundle.PopulationTypeE.Character, Bundle.PopulationTypeE.Item }),
                new List<ActionInfo>
                {
                    new("give"),
                    new("exchange"),
                    new("stealth")
                }
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
        public QuestGraph _questGraph => OwnerLayer.GetModule<QuestGraph>();

        public LBSLevelData Data => _questGraph.OwnerLayer.Parent;

        #endregion
        
        #region CONSTRUCTORS
        public QuestAssistant() : base(null, null, Color.black) { }

        public QuestAssistant(VectorImage icon, string name, Color colorTint)
            : base(icon, name, colorTint) { }

        #endregion
        
        #region METHODS
        public override object Clone() => new QuestAssistant(Icon, Name, ColorTint);

        public override void OnAttachLayer(LBSLayer layer) =>
            base.OnAttachLayer(layer);

        public override void OnGUI() { }

        public void GenerateRandomNodes(int amount)
        {
            var grammarAssistant = _questGraph.OwnerLayer.GetAssistant<GrammarAssistant>();
            Assert.IsNotNull(grammarAssistant, "GrammarAssistant should not be null.");

            // Pick random root
            var randomIndex = Random.Range(0, _questGraph.Grammar.TerminalActions.Count);
            var randomAction = _questGraph.Grammar.TerminalActions[randomIndex];
            var currentNode = _questGraph.AddNewQuestNode(randomAction, Vector2.zero);
            _questGraph.SetRoot(currentNode);

            // Sequentially add nodes from grammar
            for (int i = 1; i < amount-1; i++)
            {
                var nextActions = grammarAssistant.GetAllValidNextActionsInsert(currentNode.QuestAction, _questGraph);

                if (nextActions.Any())
                {
                    var newAction = nextActions[Random.Range(0, nextActions.Count)];
                    currentNode = _questGraph.AddNewQuestNode(newAction, Vector2.zero);
                }
                else
                {
                    // No valid continuation, stop early
                    break;
                }
            }
        }


        public void ConnectAllNodes()
        {
            for (int i = 0; i < _questGraph.GraphNodes.Count - 1; i++)
            {
                _questGraph.AddEdge(_questGraph.GraphNodes[i], _questGraph.GraphNodes[i + 1]);
            }
        }

        #region SUGGESTION GENERATION

        public void GenerateSuggestions(int suggestionsCount)
        {
            foreach (var contextLayer in Data.ContextLayers)
            {
                var populationLayer = contextLayer.GetBehaviour<PopulationBehaviour>();
                if (populationLayer == null) continue;

                for (int i = 0; i < suggestionsCount; i++)
                {
                    SuggestActionFromPopulation(populationLayer);
                }
            }
        }

        /// <summary>
        /// Suggests a single action based on population layer data.
        /// </summary>
        private bool SuggestActionFromPopulation(PopulationBehaviour populationLayer)
        {
            var groups = GroupTilesByPopulationType(populationLayer);
            var validGroups = groups.Where(g => g.Value.Any()).ToList();

            if (validGroups.Count == 0) return false;

            // pick random group
            var chosenGroup = validGroups[Random.Range(0, validGroups.Count)];
            var chosenCombo = chosenGroup.Key;
            // the tiles that will be pertinent to the generated action
            var chosenTiles = chosenGroup.Value;

            // gather valid actions to their corresponding tiles
            Dictionary<TileBundleGroup, HashSet<string>> tilesToActions = new();
            foreach (var tile in chosenTiles)
            {
                foreach (var action in GetValidActionsForTile(tile, chosenCombo))
                {
                    if (!tilesToActions.ContainsKey(tile))
                    {
                        tilesToActions[tile] = new HashSet<string>();
                    }
                    
                    tilesToActions[tile].Add(action);
                }
            }

            if (!tilesToActions.Any()) return false;

            var tilesToAction = GetActionByTileGroup(tilesToActions);
            List<Vector2> tilePositions = new();
            foreach (var tileBundleGroup in tilesToActions.Keys)
            {
                tilePositions.Add(tileBundleGroup.AreaRect.position);
            }
            Debug.Log($"The tiles {tilePositions} have the suggested Action: {tilesToAction.Value}");
            return true;
        }

        /// <summary>
        /// Returns an action that is shared by many tiles in a tile-group, depending
        /// on their LBSTags and distance (as per Max distance relation value)
        /// </summary>
        /// <param name="tilesToActions">dictionary of all the tilebundlegroups and their valid actions</param>
        /// <returns></returns>
        private KeyValuePair<List<TileBundleGroup>, string> GetActionByTileGroup(
            Dictionary<TileBundleGroup, HashSet<string>> tilesToActions)
        {
            if (!tilesToActions.Any())
            {
                return new KeyValuePair<List<TileBundleGroup>, string>(
                    new List<TileBundleGroup>(),
                    string.Empty
                );
            }

            // Take the first entry as a baseline
            var first = tilesToActions.First();
            var commonActions = new HashSet<string>(first.Value);

            foreach (var kvp in tilesToActions)
            {
                if (Equals(kvp.Key, first.Key)) continue;
                // find shared actions
                commonActions.IntersectWith(kvp.Value);

                if (!commonActions.Any())
                    break;
            }

            if (commonActions.Any())
            {
                string chosenAction = commonActions.First();
                var tiles = tilesToActions.Keys.ToList();
                return new KeyValuePair<List<TileBundleGroup>, string>(tiles, chosenAction);
            }

            // If no consensus, return a random entry
            var randomEntry = tilesToActions.ElementAt(UnityEngine.Random.Range(0, tilesToActions.Count));
            return new KeyValuePair<List<TileBundleGroup>, string>(
                new List<TileBundleGroup> { randomEntry.Key },
                randomEntry.Value.FirstOrDefault() ?? string.Empty
            );
        }

        
        /// <summary>
        /// Groups tiles by population type combinations defined in PopulationActions.
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
                bool hasRequiredTags = true;

                foreach (var tag in action.RequiredTags)
                {
                    if (tile.BundleData.Bundle.GetHasTagCharacteristic(tag)) continue;
                    hasRequiredTags = false;
                    break;
                }

                if (hasRequiredTags) validActionNames.Add(action.Name);
            }


            return validActionNames;
        }

        #endregion
        
        #endregion
    }
}
