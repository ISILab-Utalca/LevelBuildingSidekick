using ISILab.Commons;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.VisualElements;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RotatePopulationTile : LBSManipulator
    {
        private List<Vector2Int> Directions => Commons.Directions.Bidimencional.Edges;

        public TileBundleGroup Selected { get; set; }
        PopulationBehaviour population;
        private Vector2Int storedPosition;
        protected override string IconGuid { get => "485afea6f40f10e41a28c3d016a9250b"; }

        public RotatePopulationTile():base()
        {
            //feedback = new ConnectedLine();
           // feedback.fixToTeselation = true;
           name = "Rotate Tile";
           description = "Left Click to rotate counter-clockwise, Right Click to clockwise. May use Mouse Wheel as well.";
        }

        public override void Init(LBSLayer layer, object provider)
        {
            base.Init(layer, provider);
            
            population = provider as PopulationBehaviour; 
           // feedback.TeselationSize = layer.TileSize;
            //layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
        }

        protected override void OnWheelEvent(WheelEvent evt)
        {
            if(Selected == null)
            {
                LBSMainWindow.MessageNotify("No tile available to rotate. Click on a tile first, then use the Wheel");
                return;
            }  
    
            if (evt.delta.y > 0)
            {
                RotateLeft();
            }
            else if (evt.delta.y < 0)
            {
                RotateRight();
            }
        }

        protected override void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e)
        {
            var position = population.OwnerLayer.ToFixedPosition(movePosition);
             var tilegroup = population.GetTileGroup(position);
             if (tilegroup == null ||
                 tilegroup.BundleData == null ||
                 !tilegroup.BundleData.Bundle ||
                !tilegroup.BundleData.Bundle.GetHasTagCharacteristic("NonRotate"))
             {
                 Selected = null;
             }
             
             Selected = tilegroup;
             if(Selected!=null) storedPosition = position;
             MainView.Instance.SetManipulatorZoom(Selected == null);
            DrawManager.Instance.RedrawLayer(population.OwnerLayer, MainView.Instance);
        }

        protected override void OnMouseUp(VisualElement paramTarget, Vector2Int endPosition, MouseUpEvent e)
        {
            if(Selected == null) 
            {
                LBSMainWindow.MessageNotify("No tile available to rotate at position.");
            } 
            
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
            
            var rotation = population.GetTileRotation(storedPosition);
            if(rotation == default) return;
            var index = Directions.FindIndex(dir => dir == new Vector2Int((int)rotation.x, (int)rotation.y));
            index--;
            if(index < 0) index = Directions.Count-1;
            population.RotateTile(storedPosition, Directions[index]);
                        
            PostRotate();
        }

        private void RotateLeft()
        {
            PreRotate();
            
            var rotation = population.GetTileRotation(storedPosition);
            if(rotation == default) return;
            var index = Directions.FindIndex(dir => dir == new Vector2Int((int)rotation.x, (int)rotation.y));
            index++;
            if(index >= Directions.Count) index = 0;
            population.RotateTile(storedPosition, Directions[index]);
            
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
            
            DrawManager.Instance.RedrawLayer(population.OwnerLayer, MainView.Instance);
        }
    }
}