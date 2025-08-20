using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements;
using LBS.Components;
using LBS.Components.TileMap;
using System;
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
            base.OnMouseUp(element, position, e);

            //If esc key was pressed, cancel the operation
            if (ForceCancel)
            {
                ForceCancel = false;
                return;
            }

            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Remove Connection between tile");

            var pos = _schema.OwnerLayer.ToFixedPosition(position);

            var dx = _first.x - pos.x;
            var dy = _first.y - pos.y;

            float dLength = Mathf.Sqrt(dx * dx + dy * dy);

            int totalConnections = (int)Math.Floor(dLength);
            List<LBSTile> selectedTiles = new List<LBSTile>();

            for (int i = 0; i <= totalConnections; i++)
            {
                //Get the next tile 
                selectedTiles.Add(_schema.GetTile(_first - new Vector2Int(Math.Sign(dx) * i, Math.Sign(dy) * i)));
            }

            var dir1 = Directions.FindIndex(d => d.Equals(-new Vector2Int(Math.Sign(dx), Math.Sign(dy))));
            var dir2 = Directions.FindIndex(d => d.Equals(new Vector2Int(Math.Sign(dx), Math.Sign(dy))));

            if (dir1 < 0 || dir1 >= Directions.Count || dir2 < 0 || dir2 >= Directions.Count)
                return;

            for (int i = 1; i < selectedTiles.Count; i++) 
            {
                LBSTile tile1 = selectedTiles[i - 1];
                LBSTile tile2 = selectedTiles[i];

                bool t1Exists = tile1 != null;
                bool t2Exists = tile2 != null;

                if (!(t1Exists || t2Exists))
                    continue;

                if (Equals(tile1, tile2))
                    continue;

                if (t1Exists) _schema.SetConnection(tile1, dir1, "", true);
                if (t2Exists) _schema.SetConnection(tile2, dir2, "", true);
            }
            
            _schema.RecalculateWalls();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
        }
    }
}