using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements;
using LBS.Components;
using LBS.Components.TileMap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RemoveTileConnection : LBSManipulator
    {
        private static List<Vector2Int> Directions => Commons.Directions.Bidimencional.Edges;

        private SchemaBehaviour _schema;
        private List<SchemaBehaviour> _others;
        private Vector2Int _first;

        protected override string IconGuid => "0ce694377e9e05a478862c63a2ca952d";
        
        public RemoveTileConnection()
        {
            Feedback = new ConnectedLine();
            Feedback.fixToTeselation = true;
            
            Name = "Remove Manual Connection";
            Description = "Click on a connection to remove it.";
        }

        public override void Init(LBSLayer layer, object behaviour = null)
        {
            base.Init(layer, behaviour);
            
            _schema = behaviour as SchemaBehaviour;
            if (_schema.MultiLayerConnections)
                _others = LBSController.CurrentLevel.data.Layers
                    .Select(l => l.GetBehaviour<SchemaBehaviour>())
                    .Where(b => b is not null && b != _schema)
                    .ToList();

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

            LoadedLevel x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Remove Connection between tile");

            Vector2Int pos = _schema.OwnerLayer.ToFixedPosition(position);

            int dx = _first.x - pos.x;
            int dy = _first.y - pos.y;

            int dir1 = Directions.FindIndex(d => d.Equals(-new Vector2Int(Math.Sign(dx), Math.Sign(dy))));
            int dir2 = Directions.FindIndex(d => d.Equals(new Vector2Int(Math.Sign(dx), Math.Sign(dy))));

            if (dir1 < 0 || dir1 >= Directions.Count || dir2 < 0 || dir2 >= Directions.Count)
                return;

            float dLength = Mathf.Sqrt(dx * dx + dy * dy);

            int totalConnections = (int)Math.Floor(dLength);
            List<LBSTile> selectedTiles = new List<LBSTile>();

            for (int i = 0; i <= totalConnections; i++)
            {
                //Get the next tile 
                selectedTiles.Add(GetTileInLine(_schema, i));
            }

            for (int i = 1; i < selectedTiles.Count; i++) 
            {
                LBSTile tile1 = selectedTiles[i - 1];
                LBSTile tile2 = selectedTiles[i];

                TryRemoveSingleConnection(_schema, tile1, tile2, dir1, dir2 );

                if (_schema.MultiLayerConnections)
                {
                    //TODO: Multi-layer remove
                }
            }
            
            _schema.RecalculateWalls(selectedTiles.Where(t => t is not null).ToList());

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }

            /// END OF METHOD ///

            // Local functions
            LBSTile GetTileInLine(SchemaBehaviour schema, int i) => schema.GetTile(_first - new Vector2Int(Math.Sign(dx) * i, Math.Sign(dy) * i));
        }

        private void TryRemoveSingleConnection(SchemaBehaviour schema, LBSTile tile1, LBSTile tile2, int dir1, int dir2)
        {
            bool t1Exists = tile1 is not null;
            bool t2Exists = tile2 is not null;

            if (!(t1Exists || t2Exists))
                return;

            if (Equals(tile1, tile2))
                return;

            if (t1Exists) _schema.SetConnection(tile1, dir1, "", true);
            if (t2Exists) _schema.SetConnection(tile2, dir2, "", true);
        }
    }
}