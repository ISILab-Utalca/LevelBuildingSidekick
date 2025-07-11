using LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.LBS.Assistants;
using ISILab.LBS.VisualElements;
using ISILab.LBS.Editor.Windows;
using UnityEditor;

namespace ISILab.LBS.Manipulators
{
    public class SetZoneConnection : LBSManipulator
    {
        private HillClimbingAssistant _assistant;
        private Vector2Int _first;

        protected override string IconGuid => "9205ce0b509ff9442963e8161b25d8a2";

        public SetZoneConnection()
        {
            Feedback = new ConnectedLine();
            Feedback.fixToTeselation = false;

            Name = "Add Assistant zone connection";
            Description = "Select an start and end point between zones to create a connection.";
        }

        public override void Init(LBSLayer layer, object provider = null)
        {
            base.Init(layer, provider);
            
            _assistant = provider as HillClimbingAssistant;

            Feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => Feedback.TeselationSize = val;
        }

        protected override void OnMouseDown(VisualElement element, Vector2Int position, MouseDownEvent e)
        {
            _first = LBSLayer.ToFixedPosition(position);
        }

        protected override void OnMouseUp(VisualElement element, Vector2Int position, MouseUpEvent e)
        {
            LBSLayer.ToFixedPosition(position);

            var z1 = _assistant.GetZone(_first);

            if (z1 == null)
            {
                LBSMainWindow.MessageNotify("No origin selected!", LogType.Error, 2);
                return;
            }

            var pos = LBSLayer.ToFixedPosition(position);
            var z2 = _assistant.GetZone(pos);
            if (z2 == null)
            {
                LBSMainWindow.MessageNotify("No destination selected!", LogType.Error, 2);
                return;
            }

            if (z1.Equals(z2))
            {
                LBSMainWindow.MessageNotify("Can't connect a zone with itself!", LogType.Error, 2);
                return;
            }

            if (_assistant.CheckEdges(z1, z2))
            {
                LBSMainWindow.MessageNotify("This connection already exists.");
                return;
            }

            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Add Zone Connections");
            _assistant.ConnectZones(z1, z2);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
        }
    }
}