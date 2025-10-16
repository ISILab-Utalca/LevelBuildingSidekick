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
using LBS.Components.TileMap;
using System.Linq;

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
            Name = "Paint Selected Zone";
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
            base.OnMouseUp(element, position, e);

            //If esc key was pressed, cancel the operation
            if (ForceCancel)
            {
                ForceCancel = false;
                return;
            }

            LoadedLevel level = LBSController.CurrentLevel;
            Undo.RegisterCompleteObjectUndo(level, "Add Zone");
            EditorGUI.BeginChangeCheck();

            if (e.ctrlKey)
            {
                OnManipulationLeftClickCtrl.Invoke();
            }
            
            if(!_schema.Zones.Contains(ToSet)) { ToSet = null; }

            if (ToSet == null)
            {
                LBSMainWindow.MessageNotify("You don't have any selected Zone to paint with. Create a new Zone in the Behaviours panel or press 'CTRL' when left clicking.", LogType.Error, 8);
                return;
            }
            
            (Vector2Int, Vector2Int) corners = _schema.OwnerLayer.ToFixedPosition(StartPosition, EndPosition);

            var tilesToRecalculate = new HashSet<LBSTile>();
            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    LBSTile tile = _schema.AddTile(new Vector2Int(i, j), ToSet);
                    if (tile == null) continue;
                    _schema.AddConnections(
                        tile,
                        new List<string>() { "", "", "", "" },
                        new List<bool> { true, true, true, true }
                        );
                    tilesToRecalculate.Add(tile);
                    var neighs = _schema.GetTileNeighbors(tile, _schema.Directions);
                    neighs.ForEach(n => { if (n is not null) tilesToRecalculate.Add(n); });
                }
            }
            _schema.RecalculateWalls(tilesToRecalculate.ToList());

            
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