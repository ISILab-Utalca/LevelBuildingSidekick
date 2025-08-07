using ISILab.LBS.Behaviours;
using ISILab.LBS.Editor.Windows;
using LBS.Components;
using System.Collections.Generic;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RotatePopulationTile : LBSManipulator
    {
        private List<Vector2Int> Directions => Commons.Directions.Bidimencional.Edges;

        public TileBundleGroup Selected { get; private set; }
        private PopulationBehaviour _population;
        private Vector2Int _storedPosition;
        protected override string IconGuid => "485afea6f40f10e41a28c3d016a9250b";

        public RotatePopulationTile()
        {
           Name = "Rotate Tile";
           Description = "Left Click to rotate counter-clockwise, Right Click to clockwise. May use Mouse Wheel as well.";
        }

        public override void Init(LBSLayer layer, object provider = null)
        {
            base.Init(layer, provider);
            _population = provider as PopulationBehaviour; 
        }

        protected override void OnWheelEvent(WheelEvent evt)
        {
            if (Selected == null) return;
            
            if (evt.delta.y > 0) RotateLeft();
            else if (evt.delta.y < 0) RotateRight();

        }

        protected override void OnMouseMove(VisualElement element, Vector2Int movePosition, MouseMoveEvent e)
        {
            var position = _population.OwnerLayer.ToFixedPosition(movePosition);
             var tileGroup = _population.GetTileGroup(position);
             if (tileGroup == null ||
                 tileGroup.BundleData == null ||
                 !tileGroup.BundleData.Bundle ||
                !tileGroup.BundleData.Bundle.GetHasTagCharacteristic("NonRotate"))
             {
                 Selected = null;
             }
             
             Selected = tileGroup;
             if(Selected!=null) _storedPosition = position;
             MainView.Instance.SetManipulatorZoom(Selected == null);
             DrawManager.Instance.RedrawLayer(_population.OwnerLayer);
        }

        protected override void OnMouseUp(VisualElement element, Vector2Int endPosition, MouseUpEvent e)
        {
            if (Selected == null) return;
            
            if (e.button == 0)
            {
                RotateRight();
            }
            // rotate clockwise
            else if (e.button == 1)
            {
                RotateLeft();
            }
        }

        private void RotateRight()
        {
            PreRotate();
            
            var rotation = _population.GetTileRotation(_storedPosition);
            if(rotation == default) return;
            var index = Directions.FindIndex(dir => dir == new Vector2Int((int)rotation.x, (int)rotation.y));
            index--;
            if(index < 0) index = Directions.Count-1;
            _population.RotateTile(_storedPosition, Directions[index]);
                        
            PostRotate();
        }

        private void RotateLeft()
        {
            PreRotate();
            
            var rotation = _population.GetTileRotation(_storedPosition);
            if(rotation == default) return;
            var index = Directions.FindIndex(dir => dir == new Vector2Int((int)rotation.x, (int)rotation.y));
            index++;
            if(index >= Directions.Count) index = 0;
            _population.RotateTile(_storedPosition, Directions[index]);
            
            PostRotate();
        }

        private void PreRotate()
        {
            EditorGUI.BeginChangeCheck();
            var level = LBSController.CurrentLevel;
            Undo.RegisterCompleteObjectUndo(level, "Rotate");
        }

        private void PostRotate()
        {
            var level = LBSController.CurrentLevel;
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(level);
            }
            
            DrawManager.Instance.RedrawLayer(_population.OwnerLayer);
        }
    }
}