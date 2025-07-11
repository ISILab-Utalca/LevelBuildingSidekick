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

            var pos = _schema.OwnerLayer.ToFixedPosition(position);

            var dx = _first.x - pos.x;
            var dy = _first.y - pos.y;

            if (Mathf.Abs(dx) + Mathf.Abs(dy) > 1f)
                return;

            var dir1 = Directions.FindIndex(d => d.Equals(-new Vector2Int(dx, dy)));
            var dir2 = Directions.FindIndex(d => d.Equals(new Vector2Int(dx, dy)));

            if (dir1 < 0 || dir1 >= Directions.Count || dir2 < 0 || dir2 >= Directions.Count)
                return;

            var t1 = _schema.GetTile(_first);
            var t2 = _schema.GetTile(pos);

            bool t1Exists = t1 != null;
            bool t2Exists = t2 != null;

            if (!(t1Exists || t2Exists))
                return;

            //if (!t1Exists)
            //{
            //    _schema.SetConnection(t2, dir2, "", true);
            //    return;
            //}
            //if (!t2Exists)
            //{
            //    _schema.SetConnection(t1, dir1, "", true);
            //    return;
            //}

            if (Equals(t1, t2))
                return;

            if (t1Exists) _schema.SetConnection(t1, dir1, "", true);
            if (t2Exists) _schema.SetConnection(t2, dir2, "", true);
            _schema.RecalculateWalls();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
        }
    }
}