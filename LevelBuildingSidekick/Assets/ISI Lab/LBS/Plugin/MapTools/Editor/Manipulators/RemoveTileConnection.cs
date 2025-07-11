using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements;
using LBS.Components;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RemoveTileConnection : LBSManipulator
    {
        private static List<Vector2Int> Directions => Commons.Directions.Bidimencional.Edges;

        private SchemaBehaviour _schema;
        private Vector2Int _first;

        protected override string IconGuid => "0ce694377e9e05a478862c63a2ca952d";
        
        public RemoveTileConnection()
        {
            Feedback = new ConnectedLine();
            Feedback.fixToTeselation = true;
            
            Name = "Remove connection";
            Description = "Click on a connection to remove it.";
        }

        public override void Init(LBSLayer layer, object behaviour = null)
        {
            base.Init(layer, behaviour);
            
            _schema = behaviour as SchemaBehaviour;
            Feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => Feedback.TeselationSize = val;
           
        }

        protected override void OnMouseDown(VisualElement element, Vector2Int position, MouseDownEvent e)
        {
            _first = _schema.OwnerLayer.ToFixedPosition(position);
        }

        protected override void OnMouseUp(VisualElement element, Vector2Int position, MouseUpEvent e)
        {
            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Remove Connection between tile");

            var t1 = _schema.GetTile(_first);
            if (t1 == null)
                return;

            var pos = _schema.OwnerLayer.ToFixedPosition(position);

            var dx = t1.Position.x - pos.x;
            var dy = t1.Position.y - pos.y;

            var fDir = Directions.FindIndex(d => d.Equals(-new Vector2Int(dx, dy)));

            if (fDir < 0 || fDir >= _schema.Directions.Count)
                return;

            var t2 = _schema.GetTile(pos);

            if (t2 == null)
            {
                _schema.SetConnection(t1, fDir, "", true);
                return;
            }

            if (t1.Equals(t2))
                return;

            if (Mathf.Abs(dx) + Mathf.Abs(dy) > 1f)
                return;

            var tDir = _schema.Directions.FindIndex(d => d.Equals(new Vector2Int(dx, dy)));

            _schema.SetConnection(t1, fDir, "", true);
            _schema.SetConnection(t2, tDir, "", true);
            _schema.RecalculateWalls();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
        }
    }
}