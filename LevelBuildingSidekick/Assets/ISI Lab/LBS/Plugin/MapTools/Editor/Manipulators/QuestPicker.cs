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
        QuestNodeBehaviour behaviour;

        public BaseQuestNodeData activeData;
        
        private Action<string> _onBundlePicked;

        public Action<string> OnBundlePicked
        {
            get => _onBundlePicked;
            set
            {
                // only one function set at a time
                _onBundlePicked = value; 
            }
        }
        
        protected override string IconGuid { get => "f53f51dae7956eb4b99123e868e99d67"; }
        
        public QuestPicker() : base()
        {
            name = "Pick population element";
            description = "Pick the foremost population element from any layer within the graph." +
                          " The picked bundle is assigned to the selected behaviour node";
        }
        
        public override void Init(LBSLayer layer, object owner)
        {
            base.Init(layer, owner);
            behaviour = layer.GetBehaviour<QuestNodeBehaviour>();
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
        {
            var node = behaviour.SelectedQuestNode;
            if (node == null) return;


            if (activeData is not null)
            {
                var location = LBSMainWindow._gridPosition;
                activeData.position = location;


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
                OnBundlePicked?.Invoke(bundleDataGui);
            }

            behaviour.DataChanged(node);
            OnManipulationEnd.Invoke();
        }
        
    }
}