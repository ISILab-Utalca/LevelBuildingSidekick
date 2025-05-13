using ISILab.AI.Grammar;
using ISILab.AI.Optimization.Populations;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Components;
using LBS.Components.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ISILab.Macros;
using LBS.Bundles;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    /// <summary>
    /// Meant to find the bundle of a population object from any existing
    /// population layer, and assign it to the current QuestNodeBehavior's
    /// selected node if it's data type allows for a bundle. 
    /// </summary>
    public class QuestPicker : LBSManipulator
    {
        QuestGraph quest;
        QuestNodeBehaviour behaviour;
        public Bundle pickedBundle;
        
        public QuestPicker() : base()
        {
        }

        public override void Init(LBSLayer layer, object owner)
        {
            base.Init(layer, owner);
            
            quest = layer.GetModule<QuestGraph>();
            behaviour = layer.GetBehaviour<QuestNodeBehaviour>();
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
        {
            var node = behaviour.SelectedQuestNode;
            if (node == null) return;
            var populationLayers = LBS.loadedLevel.data.Layers
                .Where(l => l.Behaviours.Any(bh => bh.GetType() == typeof(PopulationBehaviour)))
                .ToList();


            TileBundleGroup bundleTile = null;

            // Here search for the bundle ref in the layer graph
            if (populationLayers.Any())
            {
                foreach (var layer in populationLayers)
                {
                    var population = layer.GetBehaviour<PopulationBehaviour>();
                    bundleTile = population.GetTileGroup(population.OwnerLayer.ToFixedPosition(endPosition));
                    if (bundleTile != null) break;
                }
            }

            if (bundleTile == null)
            {
                OnManipulationEnd.Invoke();
                return;
            }

            var bundle = bundleTile.BundleData.Bundle;
            string bundleDataGui = LBSAssetMacro.GetGuidFromAsset(bundle);

            if (node.NodeData.HasBundle())
            {
                node.NodeData.Bundle.bundleGuid = bundleDataGui;
            }
            
            behaviour.DataChanged(node);

            OnManipulationEnd.Invoke();
        }
    }
}