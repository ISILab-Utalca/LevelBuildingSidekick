using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.Macros;

namespace ISILab.LBS.Manipulators
{
    /// <summary>
    /// Allows selecting a population bundle from any layer and assigns it to the selected quest node if compatible.
    /// </summary>
    public class QuestPicker : LBSManipulator
    {
        // Private fields
        private QuestNodeBehaviour _behaviour;

        public bool PickTriggerPosition = false;
        
        // Public properties
        public BaseQuestNodeData ActiveData { get; set; }

        /// <summary>
        /// Callback invoked when a bundle is picked. Only one function is allowed at a time.
        ///- layer
        /// - tilebundleGroup grid positions
        /// - bundleGuid
        /// - grid position
        /// </summary>
        public Action<LBSLayer, TileBundleGroup> OnBundlePicked { get; set; }
        public Action<Vector2Int> OnPositionPicked { get; set; }
        
        /// <summary>
        /// Icon used by this manipulator.
        /// </summary>
        protected override string IconGuid => "f53f51dae7956eb4b99123e868e99d67";

        public QuestPicker()
        {
            Name = "Pick population element";
            Description = "Pick the foremost population element from any layer in the graph. " +
                          "The picked bundle is assigned to the selected behaviour node.";
        }

        public override void Init(LBSLayer layer, object owner = null)
        {
            base.Init(layer, owner);
            _behaviour = layer.GetBehaviour<QuestNodeBehaviour>();
        }

        protected override void OnMouseUp(VisualElement element, Vector2Int endPosition, MouseUpEvent e)
        {
            if (_behaviour.Graph.SelectedGraphNode is not QuestNode node || ActiveData == null) return;
                
            Vector2Int location = LBSMainWindow._gridPosition;
            
            if (PickTriggerPosition)
            {
                // Only sets position on the trigger
                OnPositionPicked?.Invoke(location);
            }
            else
            {
                Tuple<LBSLayer, TileBundleGroup> foundTile = LBSLayerHelper.GetBundleTileByMouse(endPosition, LBS.loadedLevel.data.Layers);
                if (foundTile is not null)
                {
                    OnBundlePicked?.Invoke(foundTile.Item1, foundTile.Item2);
                    // If a new bundle is added try to resize (only implement if using bundleGraph field)
                    ActiveData.Resize();
                }
            }
            
            _behaviour.Graph.NodeDataChanged(node);
            OnManipulationEnd?.Invoke();
        }
    }
}
