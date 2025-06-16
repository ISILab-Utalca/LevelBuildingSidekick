using System.Collections.Generic;
using ISILab.Extensions;
using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements;
using LBS.Bundles;
using LBS.Components;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Modules;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.LBS.Settings;
using ISILab.LBS.VisualElements.Editor;

namespace ISILab.LBS.Manipulators
{
    public class AddPopulationTile : LBSManipulator
    {
        PopulationBehaviour population;

        Feedback previewFeedback;
        private TileBundleGroup selectedTile;
        protected override string IconGuid { get => "ce4ce3091e6cf864cbbdc1494feb6529"; }
        
        public Bundle ToSet
        {
            get => population.selectedToSet;
        }

        public AddPopulationTile() : base()
        {
            feedback = new AreaFeedback();
            feedback.fixToTeselation = true;
            
            previewFeedback = new DottedAreaFeedback();
            previewFeedback.preview = true;
            previewFeedback.fixToTeselation = true;

            name = "Paint Tile with Item";
            description =
                "Select an item in Behaviour panel and Click on the graph to add a population tile. Hold CTRL to drag it.";
        }
        
        protected override void OnKeyDown(KeyDownEvent e)
        {
            base.OnKeyDown(e);
            if (e.ctrlKey) LBSMainWindow.WarningManipulator("(CTRL) Dragging selected tile");
        }
        
        protected override void OnKeyUp(KeyUpEvent e)
        {
            LBSMainWindow.WarningManipulator();
        }

        public override void Init(LBSLayer layer, object owner)
        {
            base.Init(layer, owner);
            
            population = owner as PopulationBehaviour;
            feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
        }

        protected override void OnMouseLeave(VisualElement _target, MouseLeaveEvent e)
        {
            MainView.Instance.RemoveElement(previewFeedback);
        }

        protected override void OnMouseUp(VisualElement paramTarget, Vector2Int endPosition, MouseUpEvent e)
        {
            var endPos = population.OwnerLayer.ToFixedPosition(endPosition);

            // Dragging selected tile
            if (e.ctrlKey)
            {
                if (selectedTile == null) return;

                // Check if the move is valid
                if (!population.BundleTilemap.ValidMoveGroup(endPos, selectedTile, Vector2.right))
                    return;

                // Calculate the difference between the new position and the original top-left position of the group
                Vector2Int originalTopLeft = selectedTile.TileGroup[0].Position;
                Vector2Int offset = endPos - originalTopLeft;

                // Move each tile relative to the offset
                foreach (var lbsTile in selectedTile.TileGroup)
                {
                    lbsTile.Position += offset;
                }
                return;
            }

            // Default Add Tile
            if (ToSet == null)
            {
                LBSMainWindow.MessageNotify("You don't have any selected item to place.", LogType.Error);
                return;
            }

            var level = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(level, "Add Element population");

            var corners = population.OwnerLayer.ToFixedPosition(StartPosition, EndPosition);

            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    population.AddTileGroup(new Vector2Int(i, j), ToSet);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(level);
            }
        }

        protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
        {
            // get tile to drag
            if (!e.ctrlKey) return;
            var tile = population.GetTile(population.OwnerLayer.ToFixedPosition(startPosition));
            if (tile == null) return;
            selectedTile = population.GetTileGroup(tile.Position);
            if (selectedTile == null) return;
            Debug.Log(selectedTile.BundleData.BundleName);
        }

        // TODO Currently it completely bugs out whenever x or y are 0 in the grid space. why? wish i fucking knew
        protected override void OnMouseMove(VisualElement target, Vector2Int endPosition, MouseMoveEvent e)
        {
            MainView.Instance.RemoveElement(previewFeedback);
            if (!ToSet) return;
           
            // when dragging by using CTRL, do not display the feedback area
            feedback.SetDisplay(!e.ctrlKey);


            var topLeftCorner = -population.OwnerLayer.ToFixedPosition(endPosition); // use negative value for corner
            var bottomRightCorner = topLeftCorner;

            // Set corner by tile size
            if (ToSet.TileSize.x > 1 || ToSet.TileSize.y > 1 )
            {
                var offset = ToSet.TileSize - new Vector2Int(1, 1);
                offset.x = -Mathf.Abs(offset.x);
                offset.y = Mathf.Abs(offset.y);
                bottomRightCorner += offset;
            }

            // grid to local position
            var firstPos = population.OwnerLayer.FixedToPosition(topLeftCorner);
            var lastPos = population.OwnerLayer.FixedToPosition(bottomRightCorner);

            // weird correction on coordinates, hate it but it works
            if(endPosition.y < 0)
            {
                firstPos.y += 99;
                lastPos.y += 99;
            }
            if(endPosition.x < 0)
            {
                firstPos.x -= 99;
                lastPos.x -= 99;
            }
            firstPos.x *= -1;
            lastPos.x *= -1;
            
            previewFeedback.ActualizePositions(firstPos.ToInt(), lastPos.ToInt());
            MainView.Instance.AddElement(previewFeedback);

            bool valid;
            // dragging feedback
            if (e.ctrlKey && selectedTile != null)
            {
                // undo the negative of topLeftCorner
                valid = population.ValidMoveGroup(-topLeftCorner, selectedTile); 
            }
            // adding feedback
            else
            {
                // undo the negative of topLeftCorner
                valid = population.ValidNewGroup(-topLeftCorner, ToSet); 
            }
            previewFeedback.ValidForInput(valid);
            
        }
    }
}