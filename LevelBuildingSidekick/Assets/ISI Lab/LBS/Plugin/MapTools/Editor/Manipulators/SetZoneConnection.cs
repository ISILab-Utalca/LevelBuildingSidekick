using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
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
        private HillClimbingAssistant assistant;
        private Vector2Int first;

        protected override string IconGuid { get => "9205ce0b509ff9442963e8161b25d8a2"; }
        
        public SetZoneConnection() : base()
        {
            feedback = new ConnectedLine();
            feedback.fixToTeselation = false;

            name = "Add Assistant zone connection";
            description = "Select an start and end point between zones to create a connection.";
        }

        public override void Init(LBSLayer layer, object provider)
        {
            base.Init(layer, provider);
            
            assistant = provider as HillClimbingAssistant;

            feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
        }

        protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
        {
            first = lbsLayer.ToFixedPosition(position);
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
        {
            var second = lbsLayer.ToFixedPosition(position);

            var z1 = assistant.GetZone(first);

            if (z1 == null)
            {
                LBSMainWindow.MessageNotify("No origin selected!", LogType.Error, 2);
                return;
            }

            var pos = lbsLayer.ToFixedPosition(position);
            var z2 = assistant.GetZone(pos);
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

            if (assistant.CheckEdges(z1, z2))
            {
                LBSMainWindow.MessageNotify("This connection already exists.");
                return;
            }

            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Add Zone Connections");
            assistant.ConnectZones(z1, z2);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
        }
    }
}