using ISILab.LBS.VisualElements;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class MapEliteAreaSelector : LBSManipulator
    {
        Action<Rect> OnSelection;
        protected override string IconGuid { get => "132787114daf605489a3d20bafcf2844"; }
        
        public MapEliteAreaSelector(Action<Rect> action)
        {
            feedback = new AreaFeedback();
            feedback.fixToTeselation = true;
            OnSelection = action;
            
            name = "Assistant Area Selector";
            description = "Select an area that will be used by Map Elites Assistant.";
        }



        protected override void OnMouseUp(VisualElement paramTarget, Vector2Int endPosition, MouseUpEvent e)
        {
            var Level = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(Level, "On Rect");

            var corners = lbsLayer.ToFixedPosition(StartPosition, EndPosition);
            var size = corners.Item2 - corners.Item1 + Vector2.one;
            var rect = new Rect(corners.Item1, size);
            OnSelection?.Invoke(rect);
            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(Level);
            }
        }
    }
}