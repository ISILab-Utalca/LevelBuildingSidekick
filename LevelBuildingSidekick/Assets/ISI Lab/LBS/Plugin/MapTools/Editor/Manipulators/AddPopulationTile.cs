using System.Collections.Generic;
using ISILab.Extensions;
using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements;
using LBS.Bundles;
using LBS.Components;
using ISILab.LBS.Editor.Windows;
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
        }

        public override void Init(LBSLayer layer, object owner)
        {
            base.Init(layer, owner);
            
            population = owner as PopulationBehaviour;
            feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
        {
            if (e.ctrlKey)
            {
                var bundleTile = population.GetTileGroup(population.OwnerLayer.ToFixedPosition(endPosition));
                var bundleName = bundleTile?.BundleData.BundleName;
                LBSMainWindow.MessageNotify("Highlighted Element " + population.OwnerLayer.ToFixedPosition(endPosition).ToString() + " : " + (bundleName!=null ? bundleName : "None "));
                
                return;
            }

            if (ToSet == null)
            {
                LBSMainWindow.MessageNotify("You don't have any selected item to place.", LogType.Error);
                return;
            }

            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Add Element population");

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
                EditorUtility.SetDirty(x);
            }
        }

        // TODO Currently it completely bugs out whenever x or y are 0 in the grid space. why? wish i fucking knew
        protected override void OnMouseMove(VisualElement target, Vector2Int endPosition, MouseMoveEvent e)
        {
            MainView.Instance.RemoveElement(previewFeedback);
            if (ToSet == null) return;
            
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
            var valid = population.ValidNewGroup(-topLeftCorner, ToSet); // undo the negative of topLeftCorner
            previewFeedback.ValidForInput(valid);
            
        }
    }
}