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
            
            float  dLenght =  Mathf.Sqrt(dx * dx  +  dy * dy);
            
            if (dLenght>= 2)
            {
                int totalConnections = (int)Math.Floor(dLenght);
                List<LBSTile> selectedTiles = new List<LBSTile>();
                
                for (int i = 0; i <= totalConnections; i++)
                {
                    //Get the next tile 
                    selectedTiles.Add(_schema.GetTile(_first - new Vector2Int(Math.Sign(dx) * i, Math.Sign(dy) * i)));
                }
                
                frontDirIndex = _schema.Directions.FindIndex(d => d.Equals(-new Vector2Int(Math.Sign(dx), Math.Sign(dy))));
                backDirIndex = _schema.Directions.FindIndex(d => d.Equals(new Vector2Int(Math.Sign(dx), Math.Sign(dy))));

                for(int i = 0; i <= selectedTiles.Count; i++)
                {
                    //TODO - Allow Paint More Thant 1 Tile 
                    LBSTile tile = selectedTiles[i];

                    if (tile != null)
                    {
                        List<string> connections = _schema.GetConnections(tile);
                        
                        foreach (var connection in connections)
                        {

                            if (frontDirIndex != -1 || frontDirIndex < _schema.Directions.Count)
                            {
                                
                                Debug.Log(frontDirIndex + ", " + backDirIndex);
                                //TrySetSingleConnection(tile, nextTile ,frontDirIndex, backDirIndex);
                            }
                            
                            if (connection == "Wall" && ToSet == "Door")
                            {
                                Debug.Log("Place a Door");
                                
                            }
                        }
                    }
                }
                return;
                
            }
            
            frontDirIndex = _schema.Directions.FindIndex(d => d.Equals(-new Vector2Int(dx, dy)));
            backDirIndex = _schema.Directions.FindIndex(d => d.Equals(new Vector2Int(dx, dy)));
            
            // Check if index is validate
            if (frontDirIndex < 0 || frontDirIndex >= _schema.Directions.Count)
                return;
            
            LBSTile t1 = _schema.GetTile(_first);
            LBSTile t2 = _schema.GetTile(lastPos);
            
            TrySetSingleConnection(t1, t2 ,frontDirIndex, backDirIndex);
            

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