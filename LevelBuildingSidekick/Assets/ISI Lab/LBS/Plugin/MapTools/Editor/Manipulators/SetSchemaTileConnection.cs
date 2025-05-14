using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using ISILab.LBS.Editor.Windows;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class SetSchemaTileConnection : LBSManipulator
    {
        private List<Vector2Int> Directions => Commons.Directions.Bidimencional.Edges;

        private SchemaBehaviour schema;
        private Vector2Int first;

        public string ToSet
        {
            get => schema.conectionToSet;
            set => schema.conectionToSet = value;
        }

        public SetSchemaTileConnection() : base()
        {
            feedback = new ConnectedLine();
            feedback.fixToTeselation = true;
        }

        public override void Init(LBSLayer layer, object behaviour)
        {
            base.Init(layer, behaviour);
            
            schema = behaviour as SchemaBehaviour;
            feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
        }

        protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
        {
            first = schema.OwnerLayer.ToFixedPosition(position);
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
        {
            if (ToSet == null)
            {
                LBSMainWindow.MessageNotify("Select a connection type in the LBS-inspector panel",LogType.Warning,4);
                return;
            }

            var level = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(level, "Add Connection between tile");

            // Get tile in first position
            var t1 = schema.GetTile(first);

            // Cheack if valid
            if (t1 == null)
                return;

            // Get second fixed position
            var pos = schema.OwnerLayer.ToFixedPosition(position);

            // Get vector direction
            var dx = t1.Position.x - pos.x;
            var dy = t1.Position.y - pos.y;

            // Get index of direction
            var fDir = Directions.FindIndex(d => d.Equals(-new Vector2Int(dx, dy)));

            // Check if index is validate
            if (fDir < 0 || fDir >= schema.Directions.Count)
                return;

            // Get tile in second position
            var t2 = schema.GetTile(pos);

            if (t2 == null)
            {
                schema.SetConnection(t1, fDir, ToSet, false);
                return;
            }

            if (t1.Equals(t2))
                return;

            if (Mathf.Abs(dx) + Mathf.Abs(dy) > 1f)
                return;

            var tDir = schema.Directions.FindIndex(d => d.Equals(new Vector2Int(dx, dy)));

            schema.SetConnection(t1, fDir, ToSet, false);
            schema.SetConnection(t2, tDir, ToSet, false);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(level);
            }
        }
    }
}