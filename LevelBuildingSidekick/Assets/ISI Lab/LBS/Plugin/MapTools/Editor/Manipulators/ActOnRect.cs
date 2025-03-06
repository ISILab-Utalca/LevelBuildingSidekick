using ISILab.AI.Optimization.Populations;
using ISILab.LBS.VisualElements;
using LBS.Components;
using LBS.Components.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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

        public override void Init(LBSLayer layer, object provider)
        {
            base.Init(layer, provider);
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
        {
            var xx = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(xx, "On Rect");

            var corners = lbsLayer.ToFixedPosition(StartPosition, EndPosition);

            var x = StartPosition.x < EndPosition.x ? StartPosition.x : EndPosition.x;
            var y = StartPosition.y < EndPosition.y ? StartPosition.y : EndPosition.y;
            var x2 = StartPosition.x > EndPosition.x ? StartPosition.x : EndPosition.x;
            var y2 = StartPosition.y > EndPosition.y ? StartPosition.y : EndPosition.y;

            var size = corners.Item2 - corners.Item1 + Vector2.one;
            var r = new Rect(corners.Item1, size);
            OnSelection?.Invoke(r);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(xx);
            }
        }
    }
}