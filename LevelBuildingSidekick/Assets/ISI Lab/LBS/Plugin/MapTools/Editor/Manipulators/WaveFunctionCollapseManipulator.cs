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
        private struct Candidate
        {
            public string[] array;
            public float weight;
        }

        private Vector2Int cornerStart;

        private AssistantWFC assistant;

        protected override string IconGuid { get => "08c60bd0a76e4bb4dad11ebf18bca46e"; }
        
        public WaveFunctionCollapseManipulator() : base()
        {
            feedback.fixToTeselation = true;
            name = "Wave Function Collapse";
            description = "Select an area to generate new connections.";
        }

        public override void Init(LBSLayer layer, object provider)
        {
            base.Init(layer, provider);

            assistant = provider as AssistantWFC;
            feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
        }

        protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
        {
            cornerStart = position;
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
        {
            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "WFC");
            
            var corners = assistant.OwnerLayer.ToFixedPosition(cornerStart, position);
            
            var positions = new List<Vector2Int>();
            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    var selected = new Vector2Int(i, j);
                    positions.Add(selected);
                }
            }
            
            assistant.Positions = positions;

            // No longer having empty tiles means overwrite is default
            //
            assistant.OverrideValues = e.ctrlKey;
            assistant.TryExecute(out string log, out LogType type);

            LBSMainWindow.MessageNotify(log, type);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
        }
    }
}