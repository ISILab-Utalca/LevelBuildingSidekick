using ISILab.LBS.Assistants;
using ISILab.LBS.Editor.Windows;
using LBS.Components;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class WaveFunctionCollapseManipulator : ManipulateTeselation
    {
        private Vector2Int _cornerStart;

        private AssistantWFC _assistant;

        protected override string IconGuid => "08c60bd0a76e4bb4dad11ebf18bca46e";

        public WaveFunctionCollapseManipulator()
        {
            Feedback.fixToTeselation = true;
            Name = "Wave Function Collapse";
            Description = "Select an area to generate new connections.";
        }

        public override void Init(LBSLayer layer, object provider)
        {
            base.Init(layer, provider);

            _assistant = provider as AssistantWFC;
            Feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => Feedback.TeselationSize = val;
        }

        protected override void OnMouseDown(VisualElement element, Vector2Int position, MouseDownEvent e)
        {
            _cornerStart = position;
        }

        protected override void OnMouseUp(VisualElement element, Vector2Int position, MouseUpEvent e)
        {
            base.OnMouseUp(element, position, e);

            //If esc key was pressed, cancel the operation
            if (ForceCancel)
            {
                ForceCancel = false;
                return;
            }

            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "WFC");

            var corners = _assistant.OwnerLayer.ToFixedPosition(_cornerStart, position);

            var positions = new List<Vector2Int>();
            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    var selected = new Vector2Int(i, j);
                    positions.Add(selected);
                }
            }

            _assistant.Positions = positions;

            // No longer having empty tiles means overwrite is default
            //
            _assistant.OverrideValues = e.ctrlKey;
            _assistant.TryExecute(out string log, out LogType type);

            LBSMainWindow.MessageNotify(log, type, 5);
            if (type == LogType.Log)
                Debug.Log(log);
            else
                Debug.LogWarning(log);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
        }
    }
}