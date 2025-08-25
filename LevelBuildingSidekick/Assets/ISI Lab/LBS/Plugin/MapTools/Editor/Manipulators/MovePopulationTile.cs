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
using ISILab.LBS.VisualElements.Editor;

namespace ISILab.LBS.Manipulators
{
    public class MovePopulationTile : LBSManipulator
    {
        private PopulationBehaviour _population;

        private readonly Feedback _previewFeedback;
        // Used to access from me draw manager
        public TileBundleGroup Selected { get; private set; }
        
        protected override string IconGuid => "ad3a2ec3b8f589d42a66626c44a3fd17";

        private Bundle ToSet => _population.selectedToSet;

        public MovePopulationTile()
        {
            _previewFeedback = new DottedAreaFeedback();
            _previewFeedback.preview = true;
            _previewFeedback.fixToTeselation = true;

            Name = "Move Item Tile";
            Description =
                "Click on the graph to and drag a population tile to move it.";
        }

        public override void Init(LBSLayer layer, object provider = null)
        {
            base.Init(layer, provider);
            _population = provider as PopulationBehaviour;

        }

        protected override void OnMouseLeave(VisualElement element, MouseLeaveEvent e)
        {
            MainView.Instance.RemoveElement(_previewFeedback);
        }

        protected override void OnMouseUp(VisualElement element, Vector2Int endPosition, MouseUpEvent e)
        {
            base.OnMouseUp(element, endPosition, e);

            //If esc key was pressed, cancel the operation
            if (ForceCancel)
            {
                MainView.Instance.RemoveElement(_previewFeedback);
                ForceCancel = false;
                return;
            }

            var endPos = _population.OwnerLayer.ToFixedPosition(endPosition);

            // Check if the move is valid
            if (!_population.BundleTilemap.ValidMoveGroup(endPos, Selected, Vector2.right)) return;
               

            // Calculate the difference between the new position and the original top-left position of the group
            Vector2Int originalTopLeft = Selected.TileGroup[0].Position;
            Vector2Int offset = endPos - originalTopLeft;

            // Move each tile relative to the offset
            foreach (var lbsTile in Selected.TileGroup)
            {
                lbsTile.Position += offset;
            }

            _population.OwnerLayer.OnChangeUpdate();
        }

        protected override void OnMouseDown(VisualElement element, Vector2Int startPosition, MouseDownEvent e)
        { 
            var position = _population.OwnerLayer.ToFixedPosition(startPosition);
            var tileGroup = _population.GetTileGroup(position);
            if (tileGroup == null ||
                tileGroup.BundleData == null ||
                !tileGroup.BundleData.Bundle)
            {
                Selected = null;
                return;
            }
             
            Selected = tileGroup;
        }
        
        // TODO Currently it completely bugs out whenever x or y are 0 in the grid space. why? wish i fucking knew
        protected override void OnMouseMove(VisualElement element, Vector2Int movePosition, MouseMoveEvent e)
        {
            MainView.Instance.RemoveElement(_previewFeedback);

            if (ForceCancel) return;
 
            var topLeftCorner = -_population.OwnerLayer.ToFixedPosition(movePosition); // use negative value for corner
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
            var firstPos = _population.OwnerLayer.FixedToPosition(topLeftCorner);
            var lastPos = _population.OwnerLayer.FixedToPosition(bottomRightCorner);

            // weird correction on coordinates, hate it but it works
            if(movePosition.y < 0)
            {
                firstPos.y += 99;
                lastPos.y += 99;
            }
            if(movePosition.x < 0)
            {
                firstPos.x -= 99;
                lastPos.x -= 99;
            }
            firstPos.x *= -1;
            lastPos.x *= -1;
            
            _previewFeedback.ActualizePositions(firstPos.ToInt(), lastPos.ToInt());
            MainView.Instance.AddElement(_previewFeedback);

            bool valid;
            // dragging feedback
            if (Selected != null)
            {
                // undo the negative of topLeftCorner
                valid = _population.ValidMoveGroup(-topLeftCorner, Selected); 
                _previewFeedback.ValidForInput(valid);
            }
            // adding feedback
            else
            {
                var position = _population.OwnerLayer.ToFixedPosition(movePosition);
                var tileGroup = _population.GetTileGroup(position);
                if (tileGroup == null ||
                    tileGroup.BundleData == null ||
                    !tileGroup.BundleData.Bundle)
                {
                    _previewFeedback.ValidForInput(false);  
                    return;
                }

                // undo the negative of topLeftCorner
                _previewFeedback.ValidForInput(true);            
            }
        }
    }
}