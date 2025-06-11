using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RemoveTileConnection : LBSManipulator
    {
        private List<Vector2Int> Directions => Commons.Directions.Bidimencional.Edges;

        private SchemaBehaviour schema;
        private Vector2Int first;

        protected override string IconGuid => "0ce694377e9e05a478862c63a2ca952d";
        
        public RemoveTileConnection() : base()
        {
            feedback = new ConnectedLine();
            feedback.fixToTeselation = true;
            
            name = "Remove connection";
            description = "Click on a connection to remove it.";
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

        protected override void OnMouseUp(VisualElement paramTarget, Vector2Int position, MouseUpEvent e)
        {
            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Remove Connection between tile");

            var t1 = schema.GetTile(first);
            if (t1 == null)
                return;

            var pos = schema.OwnerLayer.ToFixedPosition(position);

            var dx = t1.Position.x - pos.x;
            var dy = t1.Position.y - pos.y;

            var fDir = Directions.FindIndex(d => d.Equals(-new Vector2Int(dx, dy)));

            if (fDir < 0 || fDir >= schema.Directions.Count)
                return;

            var t2 = schema.GetTile(pos);

            if (t2 == null)
            {
                schema.SetConnection(t1, fDir, "", true);
                return;
            }

            if (t1.Equals(t2))
                return;

            if (Mathf.Abs(dx) + Mathf.Abs(dy) > 1f)
                return;

            var tDir = schema.Directions.FindIndex(d => d.Equals(new Vector2Int(dx, dy)));

            schema.SetConnection(t1, fDir, "", true);
            schema.SetConnection(t2, tDir, "", true);
            schema.RecalculateWalls();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
        }
    }
}