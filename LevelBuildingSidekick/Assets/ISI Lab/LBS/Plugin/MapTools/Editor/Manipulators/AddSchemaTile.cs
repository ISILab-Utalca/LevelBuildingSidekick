using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.VisualElements;
using LBS.Components;

using System.Collections.Generic;
using ISILab.LBS.Assistants;
using ISILab.LBS.Editor.Windows;
using ISILab.Macros;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class AddSchemaTile : LBSManipulator
    {
        private SchemaBehaviour _schema;
        protected override string IconGuid => "ce4ce3091e6cf864cbbdc1494feb6529";

        private Zone ToSet
        {
            get => _schema.RoomToSet;
            set => _schema.RoomToSet = value;
        }

        public AddSchemaTile()
        {
            Name = "Paint Zone";
            Description = "Add a new zone in the inspector and then paint in the graph. Hold CTRL and select an area to auto-generate a new zone.";
            
            Feedback = new AreaFeedback();
            Feedback.fixToTeselation = true;
        }

        public override void Init(LBSLayer layer, object provider = null)
        {
            base.Init(layer, provider);
            
            _schema = provider as SchemaBehaviour;
            Feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => Feedback.TeselationSize = val;
        }

        protected override void OnKeyDown(KeyDownEvent e)
        {
            base.OnKeyDown(e);
            if (e.ctrlKey) LBSMainWindow.WarningManipulator("(CTRL) Adding New Zone");
        }
        
        protected override void OnKeyUp(KeyUpEvent e)
        {
            LBSMainWindow.WarningManipulator();
        }
        
        protected override void OnMouseUp(VisualElement element, Vector2Int position, MouseUpEvent e)
        {
            var level = LBSController.CurrentLevel;
            Undo.RegisterCompleteObjectUndo(level, "Add Zone");
            EditorGUI.BeginChangeCheck();

            if (e.ctrlKey)
            {
                OnManipulationLeftClickCtrl.Invoke();
            }
            
            if(!_schema.Zones.Contains(ToSet)) { ToSet = null; }

            if (ToSet == null)
            {
                LBSMainWindow.MessageNotify("You don't have any selected area to place. Create a new Zone in the panel or press 'CTRL' when left clicking.", LogType.Error, 8);
                return;
            }
            
            var corners = _schema.OwnerLayer.ToFixedPosition(StartPosition, EndPosition);

            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    var tile = _schema.AddTile(new Vector2Int(i, j), ToSet);
                    _schema.AddConnections(
                        tile,
                        new List<string>() { "", "", "", "" },
                        new List<bool> { true, true, true, true }
                        );
                }
            }
            _schema.RecalculateWalls();

            
            // Try to calculate constraints
            var assistant = LBSLayerHelper.GetObjectFromLayer<HillClimbingAssistant>(_schema.OwnerLayer);
            assistant?.RecalculateConstraint();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(level);
            }
        }
    }
}