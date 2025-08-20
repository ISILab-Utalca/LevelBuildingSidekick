using System.Collections.Generic;
using ISILab.Extensions;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements;
using ISILab.LBS.VisualElements.Editor;
using LBS.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RemovePopulationTile : LBSManipulator
    {
        private PopulationBehaviour _population;
        
        private Feedback _previewFeedback;

        private List<Feedback> previews = new();

        private bool LeftButtonPressed;
        
        protected override string IconGuid => "ce08b36a396edbf4394f7a4e641f253d";

        public RemovePopulationTile()
        {
            Feedback = new AreaFeedback();
            Feedback.fixToTeselation = true;
            Name = "Remove Tile";
            Description = "Click on an item in the graph to remove it.";
        }

        public override void Init(LBSLayer layer, object provider = null)
        {
            base.Init(layer, provider);
            
            _previewFeedback = new DottedAreaFeedback();
            _previewFeedback.preview = true;
            _previewFeedback.fixToTeselation = true;
            
            _population = provider as PopulationBehaviour;
            Feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => Feedback.TeselationSize = val;
        }

        protected override void OnMouseUp(VisualElement element, Vector2Int endPosition, MouseUpEvent e)
        {
            base.OnMouseUp(element, endPosition, e);

            LeftButtonPressed = false;

            //If esc key was pressed, cancel the operation
            if (ForceCancel)
            {
                MainView.Instance.RemoveElement(_previewFeedback);
                ForceCancel = false;
                return;
            }

            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Remove element population");

            var corners = _population.OwnerLayer.ToFixedPosition(StartPosition, EndPosition);

            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    Vector2Int position = new Vector2Int(i, j);
                    _population.RemoveTileGroup(position);
                }
            }

            var level = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(level, "Remove Element population");
            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }

            _population.OwnerLayer.OnChangeUpdate();
            
            CleanPreviews();
        }

        protected override void OnMouseDown(VisualElement element, Vector2Int startPosition, MouseDownEvent e)
        {
            base.OnMouseDown(element, startPosition, e);
            StartPosition = startPosition;

            LeftButtonPressed = e.button == 0;
            
            CleanPreviews();
        }

        protected override void OnMouseMove(VisualElement element, Vector2Int movePosition, MouseMoveEvent e)
        {
            CleanPreviews();

            if (!LeftButtonPressed || ForceCancel) return;
            
            var corners = _population.OwnerLayer.ToFixedPosition(StartPosition, movePosition);
            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    Vector2Int position = new Vector2Int(i, j);
                    var tileGroup = _population.GetTileGroup(position);
                       
                    if (tileGroup is null) continue;
                    var pf = new DottedAreaFeedback
                    {
                        preview = true,
                        fixToTeselation = true
                    };

                    var topLeftCorner = new Vector2Int(-i, -j);
                    var bottomRightCorner = topLeftCorner;
                        
                    pf.ValidForInput(true);
                    if (tileGroup.BundleData.Bundle.TileSize.x > 1 || tileGroup.BundleData.Bundle.TileSize.y > 1 )
                    {
                        var offset = tileGroup.BundleData.Bundle.TileSize - new Vector2Int(1, 1);
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
                
                    pf.ValidForInput(true);
                    pf.ActualizePositions(firstPos.ToInt(), lastPos.ToInt());
                    previews.Add(pf);
                }
            }

            foreach (var feedback in previews)
            {
                MainView.Instance.AddElement(feedback);
            }

        }

        private void CleanPreviews()
        {
            foreach (var feedback in previews)
            {
                if (feedback is null) continue;
                MainView.Instance.RemoveElement(feedback);
                feedback.visible = false;
            }

            previews.Clear();
        }
    }
}