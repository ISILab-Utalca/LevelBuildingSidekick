using System;
using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements;
using LBS.Components;
using System.Collections.Generic;
using ISILab.LBS.Editor.Windows;
using LBS.Components.TileMap;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class AddSchemaTileConnection : LBSManipulator
    {
        private SchemaBehaviour _schema;
        private Vector2Int _first;

        protected override string IconGuid => "b06c784e5d88d1547a40d4fc2f54b485";
        
        public string ToSet
        {
            get => _schema.conectionToSet;
            set => _schema.conectionToSet = value;
        }

        public AddSchemaTileConnection()
        {
            Feedback = new ConnectedLine();
            Feedback.fixToTeselation = true;

            Name = "Set connection";
            Description = "Draw across a zone's border to generate a connection.";
        }

        public override void Init(LBSLayer layer, object provider = null)
        {
            base.Init(layer, provider);
            
            _schema = provider as SchemaBehaviour;
            Feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => Feedback.TeselationSize = val;
        }

        protected override void OnMouseDown(VisualElement element, Vector2Int position, MouseDownEvent e)
        {
            _first = _schema.OwnerLayer.ToFixedPosition(position);
        }

        protected override void OnMouseUp(VisualElement element, Vector2Int position, MouseUpEvent e)
        {
            if (ToSet == null)
            {
                LBSMainWindow.MessageNotify("Select a connection type in the LBS-inspector panel",LogType.Warning,4);
                return;
            }

            var level = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(level, "Add Connection between tile");
            
            // Get second fixed position
            Vector2Int lastPos = _schema.OwnerLayer.ToFixedPosition(position);

            // Get vector direction
            int dx =  _first.x - lastPos.x;
            int dy = _first.y - lastPos.y;
            
            
            // Get index of directions
            int frontDirIndex;
            int backDirIndex;
            
            float dLength = Mathf.Sqrt(dx * dx  +  dy * dy);

            if (dLength < 1)
                return;

            // Multi-connection mode
            bool requiresWall = dLength > 1;

            int totalConnections = (int)Math.Floor(dLength);
            List<LBSTile> selectedTiles = new List<LBSTile>();

            for (int i = 0; i <= totalConnections; i++)
            {
                //Get the next tile 
                selectedTiles.Add(_schema.GetTile(_first - new Vector2Int(Math.Sign(dx) * i, Math.Sign(dy) * i)));
            }

            frontDirIndex = _schema.Directions.FindIndex(d => d.Equals(-new Vector2Int(Math.Sign(dx), Math.Sign(dy))));
            backDirIndex = _schema.Directions.FindIndex(d => d.Equals(new Vector2Int(Math.Sign(dx), Math.Sign(dy))));
            if (frontDirIndex < 0 || frontDirIndex >= _schema.Directions.Count)
                return;

            for (int i = 1; i < selectedTiles.Count; i++)
            {
                LBSTile tile1 = selectedTiles[i - 1];
                LBSTile tile2 = selectedTiles[i];

                bool setDoorOrWindow = ToSet.Equals("Door") || ToSet.Equals("Window");
                if (requiresWall && setDoorOrWindow)
                {
                    bool tile1Exists = tile1 != null;
                    bool tile2Exists = tile2 != null;
                    string conn1 = tile1Exists ? _schema.GetConnections(tile1)[frontDirIndex] : "Empty";
                    string conn2 = tile2Exists ? _schema.GetConnections(tile2)[backDirIndex] : "Empty";
                    bool firstHasWall = !conn1.Equals("Empty");
                    bool secondHasWall = !conn2.Equals("Empty");
                    if (!((firstHasWall || secondHasWall) && (tile1Exists || secondHasWall) && (tile2Exists || firstHasWall)))
                        continue;
                }

                TrySetSingleConnection(tile1, tile2, frontDirIndex, backDirIndex);
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(level);
            }
        }

        private void TrySetSingleConnection(
            LBSTile firstTile,
            LBSTile secondTile,
            int frontDirIndex,
            int backDirIndex
            )
        {
            
            if (firstTile == null)
            {
                if (secondTile != null)
                {
                    _schema.SetConnection(secondTile, backDirIndex, ToSet, false);
                    return;
                }
            }
            else
            {
                if (firstTile.Equals(secondTile))
                {
                    Debug.Log("Not Valid Tile - Same Tile with lenght 0");
                    return;
                }
            }
            
            if (secondTile == null)
            {
                if (firstTile == null)
                {
                    return;
                }
                _schema.SetConnection(firstTile, frontDirIndex, ToSet, false);
                return;
            }
            
            // set both connections
            _schema.SetConnection(firstTile, frontDirIndex, ToSet, false);
            _schema.SetConnection(secondTile, backDirIndex, ToSet, false);
        }
    }
}