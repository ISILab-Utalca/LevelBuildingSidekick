using ISILab.LBS.VisualElements;
using ISILab.LBS.VisualElements.Editor;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class MapEliteAreaSelector : LBSManipulator
    {
        private readonly Action<Rect> _onSelection;
        protected override string IconGuid => "132787114daf605489a3d20bafcf2844";



        public MapEliteAreaSelector(Action<Rect> action)
        {
            Feedback = new AreaFeedback();
            Feedback.fixToTeselation = true;
            _onSelection = action;
            
            Name = "Assistant Area Selector";
            Description = "Select an area that will be used by Map Elites Assistant.";
        }



        protected override void OnMouseUp(VisualElement element, Vector2Int endPosition, MouseUpEvent e)
        {
            base.OnMouseUp(element, endPosition, e);

            //If esc key was pressed, cancel the operation
            if (ForceCancel)
            {
                ForceCancel = false;
                return;
            }

            var level = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(level, "On Rect");

            var corners = LBSLayer.ToFixedPosition(StartPosition, EndPosition);
            var size = corners.Item2 - corners.Item1 + Vector2.one;
            var rect = new Rect(corners.Item1, size);
            _onSelection?.Invoke(rect);
            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(level);
            }
        }
    }
}