using ISILab.LBS.Behaviours;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.VisualElements;
using LBS.Components;
using LBS.Components.TileMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class AddSchemaTileConnection : LBSManipulator
    {
        private SchemaBehaviour _schema;
        private List<SchemaBehaviour> _others;
        private Vector2Int _first;
        private List<Vector2Int> Dirs => ISILab.Commons.Directions.Bidimencional.Edges;

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

            Name = "Set Manual Connection";
            Description = "Draw across a zone's border to generate a connection.";
        }

        public override void Init(LBSLayer layer, object provider = null)
        {
            //Debug.Log("Tile connection Init");
            base.Init(layer, provider);
            
            _schema = provider as SchemaBehaviour;
            if(_schema.MultiLayerConnections) 
                MultiLayerSetup();

            Feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => Feedback.TeselationSize = val;
        }

        public void MultiLayerSetup()
        {
            _others = LBSController.CurrentLevel.data.Layers
                .Select(l => l.GetBehaviour<SchemaBehaviour>())
                .Where(b => b is not null && b != _schema)
                .ToList();
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

            if (ToSet is null)
            {
                LBSMainWindow.MessageNotify("Select a connection type in the LBS-inspector panel",LogType.Warning,4);
                return;
            }

            LoadedLevel level = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(level, "Add Connection between tile");

            // Get second fixed position
            Vector2Int lastPos = _schema.OwnerLayer.ToFixedPosition(position);

            // Get vector direction
            int dx = _first.x - lastPos.x;
            int dy = _first.y - lastPos.y;
            
            float dLength = Mathf.Sqrt(dx * dx  +  dy * dy);

            if (dLength < 1)
                return;

            // Get index of directions
            int frontDirIndex = Dirs.FindIndex(d => d.Equals(-new Vector2Int(Math.Sign(dx), Math.Sign(dy))));
            if (frontDirIndex < 0 || frontDirIndex >= Dirs.Count) return;
            int backDirIndex = Dirs.FindIndex(d => d.Equals(new Vector2Int(Math.Sign(dx), Math.Sign(dy))));

            // Multi-connection mode
            bool requiresWall = dLength > 1;

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

                bool setDoorOrWindow = ToSet.Equals("Door") || ToSet.Equals("Window");
                if (requiresWall && setDoorOrWindow && !ValidWallReplace(_schema, tile1, tile2)) continue;

                TrySetSingleConnection(_schema, tile1, tile2, frontDirIndex, backDirIndex);
                
                if (_schema.MultiLayerConnections && setDoorOrWindow)
                {
                    foreach (SchemaBehaviour other in _others)
                    {
                        LBSTile t1 = GetTileInLine(other, i - 1);
                        LBSTile t2 = GetTileInLine(other, i);
                        if (ValidWallReplace(other, t1, t2))
                        {
                            TrySetSingleConnection(other, t1, t2, frontDirIndex, backDirIndex);
                            Action redrawCallback = null;
                            redrawCallback = () =>
                            {
                                DrawManager.Instance.RedrawLayer(other.OwnerLayer);
                                OnManipulationEnd -= redrawCallback;
                            };
                            OnManipulationEnd += redrawCallback;
                            break; // The multilayer connections feature was planned for adjacent rooms of two different layers. It is not expected to set the same connection in more than 2 layers.
                        }
                    }
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(level);
            }

            /// END OF METHOD ///

            // Local functions
            LBSTile GetTileInLine(SchemaBehaviour schema, int i) => schema.GetTile(_first - new Vector2Int(Math.Sign(dx) * i, Math.Sign(dy) * i));

            bool ValidWallReplace(SchemaBehaviour schema, LBSTile tile1, LBSTile tile2)
            {
                bool tile1Exists = tile1 is not null;
                bool tile2Exists = tile2 is not null;
                string conn1 = tile1Exists ? schema.GetConnections(tile1)[frontDirIndex] : "Empty";
                string conn2 = tile2Exists ? schema.GetConnections(tile2)[backDirIndex] : "Empty";
                bool firstHasWall = !conn1.Equals("Empty");
                bool secondHasWall = !conn2.Equals("Empty");
                return (firstHasWall || secondHasWall) && (tile1Exists || secondHasWall) && (tile2Exists || firstHasWall);
            }
        }

        private void TrySetSingleConnection(
            SchemaBehaviour schema,
            LBSTile firstTile,
            LBSTile secondTile,
            int frontDirIndex,
            int backDirIndex
            )
        {
            
            if (firstTile is null)
            {
                if (secondTile is not null)
                {
                    schema.SetConnection(secondTile, backDirIndex, ToSet, false);
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
            
            if (secondTile is null)
            {
                if (firstTile is null)
                {
                    return;
                }
                schema.SetConnection(firstTile, frontDirIndex, ToSet, false);
                return;
            }

            // set both connections
            schema.SetConnection(firstTile, frontDirIndex, ToSet, false);
            schema.SetConnection(secondTile, backDirIndex, ToSet, false);
        }
    }
}