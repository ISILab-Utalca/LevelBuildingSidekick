using ISILab.LBS.VisualElements;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class ActOnRect : LBSManipulator
    {
        Action<Rect> OnSelection;

        public ActOnRect(Action<Rect> action)
        {
            feedback = new AreaFeedback();
            feedback.fixToTeselation = true;
            OnSelection = action;
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
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