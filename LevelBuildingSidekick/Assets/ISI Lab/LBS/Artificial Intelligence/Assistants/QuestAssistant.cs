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
            for (int i = 0; i < amount; i++)
            {
                var randomIndex = Random.Range(0, _questGraph.Grammar.TerminalActions.Count);
                var randomAction = _questGraph.Grammar.TerminalActions[randomIndex];
                _questGraph.AddNewQuestNode(randomAction, Vector2.zero);
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
            var chosenTiles = chosenGroup.Value;

            // gather valid actions
            var validActions = new HashSet<string>();
            foreach (var tile in chosenTiles)
            {
                foreach (var action in GetValidActionsForTile(tile, chosenCombo))
                {
                    validActions.Add(action);
                }
            }

            if (validActions.Count == 0) return false;

            var chosenAction = validActions.ElementAt(Random.Range(0, validActions.Count));
            Debug.Log($"Suggested Action: {chosenAction}");
            return true;
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
                bool hasAllTags = action.RequiredTags.All(tag => tile.BundleData.Bundle.GetHasTagCharacteristic(tag));
                if (action.RequiredTags.Count == 0 || hasAllTags)
                    validActionNames.Add(action.Name);
            }

            return validActionNames;
        }

        #endregion
        
        #endregion
    }
}
