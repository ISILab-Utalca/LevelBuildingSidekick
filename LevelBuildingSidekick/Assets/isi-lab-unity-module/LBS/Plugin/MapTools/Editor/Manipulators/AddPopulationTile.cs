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
    public class AddPopulationTile : LBSManipulator
    {
        private PopulationBehaviour _population;

        private readonly Feedback _previewFeedback;
        private TileBundleGroup _selectedTile;
        protected override string IconGuid => "ce4ce3091e6cf864cbbdc1494feb6529";

        private Bundle ToSet => _population.selectedToSet;

        public AddPopulationTile()
        {
            Feedback = new AreaFeedback();
            Feedback.fixToTeselation = true;
            
            _previewFeedback = new DottedAreaFeedback();
            _previewFeedback.preview = true;
            _previewFeedback.fixToTeselation = true;

            Name = "Paint Tile with Item";
            Description =
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

        public override void Init(LBSLayer layer, object provider = null)
        {
            base.Init(layer, provider);
            
            _population = provider as PopulationBehaviour;
            Feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => Feedback.TeselationSize = val;
        }

        protected override void OnMouseLeave(VisualElement element, MouseLeaveEvent e)
        {
            MainView.Instance.RemoveElement(_previewFeedback);
        }

        protected override void OnMouseUp(VisualElement element, Vector2Int endPosition, MouseUpEvent e)
        {
            base.OnMouseUp(element, endPosition, e);

            var endPos = _population.OwnerLayer.ToFixedPosition(endPosition);

            // Dragging selected tile
            if (e.ctrlKey)
            {
                if (_selectedTile == null) return;

                // Check if the move is valid
                if (!_population.BundleTilemap.ValidMoveGroup(endPos, _selectedTile, Vector2.right))
                    return;

                // Calculate the difference between the new position and the original top-left position of the group
                Vector2Int originalTopLeft = _selectedTile.TileGroup[0].Position;
                Vector2Int offset = endPos - originalTopLeft;

                // Move each tile relative to the offset
                foreach (var lbsTile in _selectedTile.TileGroup)
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

            //If esc key was pressed, cancel the operation
            if (ForceCancel)
            {
                MainView.Instance.RemoveElement(_previewFeedback);
                ForceCancel = false;
                return;
            }

            var level = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(level, "Add Element population");

            var corners = _population.OwnerLayer.ToFixedPosition(StartPosition, EndPosition);

            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    _population.AddTileGroup(new Vector2Int(i, j), ToSet);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(level);
            }

            _population.OwnerLayer.OnChangeUpdate();
        }

        protected override void OnMouseDown(VisualElement element, Vector2Int startPosition, MouseDownEvent e)
        {
            // get tile to drag
            if (!e.ctrlKey) return;
            var tile = _population.GetTile(_population.OwnerLayer.ToFixedPosition(startPosition));
            if (tile == null) return;
            _selectedTile = _population.GetTileGroup(tile.Position);
        }

        // TODO Currently it completely bugs out whenever x or y are 0 in the grid space. why? wish i fucking knew
        protected override void OnMouseMove(VisualElement element, Vector2Int endPosition, MouseMoveEvent e)
        {
            MainView.Instance.RemoveElement(_previewFeedback);
            if (!ToSet || ForceCancel) return;

            base.OnMouseMove(element, endPosition, e);

            // when dragging by using CTRL, do not display the feedback area
            Feedback.SetDisplay(!e.ctrlKey);

            var topLeftCorner = -_population.OwnerLayer.ToFixedPosition(endPosition); // use negative value for corner
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
            
            _previewFeedback.ActualizePositions(firstPos.ToInt(), lastPos.ToInt());
            MainView.Instance.AddElement(_previewFeedback);

            bool valid;
            // dragging feedback
            if (e.ctrlKey && _selectedTile != null)
            {
                // undo the negative of topLeftCorner
                valid = _population.ValidMoveGroup(-topLeftCorner, _selectedTile); 
            }
            // adding feedback
            else
            {
                // undo the negative of topLeftCorner
                valid = _population.ValidNewGroup(-topLeftCorner, ToSet); 
            }
            _previewFeedback.ValidForInput(valid);
            
        }
    }
}