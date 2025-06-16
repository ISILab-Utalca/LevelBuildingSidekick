using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using ISILab.Macros;
using LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;

namespace ISILab.LBS.Manipulators
{
    /// <summary>
    /// Allows selecting a population bundle from any layer and assigns it to the selected quest node if compatible.
    /// </summary>
    public class QuestPicker : LBSManipulator
    {
        // Private fields
        private QuestNodeBehaviour _behaviour;

        // Public properties
        public BaseQuestNodeData ActiveData { get; set; }

        /// <summary>
        /// Callback invoked when a bundle is picked. Only one function is allowed at a time.
        /// </summary>
        public Action<LBSLayer, string, Vector2Int> OnBundlePicked { get; set; }

        /// <summary>
        /// Icon used by this manipulator.
        /// </summary>
        protected override string IconGuid => "f53f51dae7956eb4b99123e868e99d67";

        public QuestPicker()
        {
            name = "Pick population element";
            description = "Pick the foremost population element from any layer in the graph. " +
                          "The picked bundle is assigned to the selected behaviour node.";
        }

        public override void Init(LBSLayer layer, object owner = null)
        {
            base.Init(layer, owner);
            _behaviour = layer.GetBehaviour<QuestNodeBehaviour>();
        }

        protected override void OnMouseUp(VisualElement paramTarget, Vector2Int endPosition, MouseUpEvent e)
        {
            var node = _behaviour.SelectedQuestNode;
            if (node == null || ActiveData == null)
                return;

            Vector2Int location = LBSMainWindow._gridPosition;
            ActiveData.Position = location;

            // Search population layers
            var populationLayers = LBS.loadedLevel.data.Layers
                .Where(l => l.Behaviours.Any(bh => bh is PopulationBehaviour))
                .ToList();

            TileBundleGroup bundleTile = null;
            LBSLayer pickedLayer = null;

            foreach (var layer in populationLayers)
            {
                var population = layer.GetBehaviour<PopulationBehaviour>();
                bundleTile = population.GetTileGroup(population.OwnerLayer.ToFixedPosition(endPosition));
                if (bundleTile != null)
                {
                    pickedLayer = layer;
                    break;
                }
            }

            string bundleGuid = string.Empty;

            if (bundleTile != null)
            {
                var bundle = bundleTile.BundleData.Bundle;
                bundleGuid = LBSAssetMacro.GetGuidFromAsset(bundle);
            }

            OnBundlePicked?.Invoke(pickedLayer, bundleGuid, location);
            _behaviour.DataChanged(node);
            OnManipulationEnd?.Invoke();
        }
    }
}
