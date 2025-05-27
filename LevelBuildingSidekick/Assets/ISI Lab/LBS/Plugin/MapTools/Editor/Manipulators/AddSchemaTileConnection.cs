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
        private List<Vector2Int> Directions => Commons.Directions.Bidimencional.Edges;

        private SchemaBehaviour schema;
        private Vector2Int first;

        protected override string IconGuid => "b06c784e5d88d1547a40d4fc2f54b485";
        
        public string ToSet
        {
            get => schema.conectionToSet;
            set => schema.conectionToSet = value;
        }

        public AddSchemaTileConnection()
        {
            feedback = new ConnectedLine();
            feedback.fixToTeselation = true;

            name = "Set connection";
            description = "Draw across a zone's border to generate a connection.";
        }

        public override void Init(LBSLayer layer, object behaviour)
        {
            base.Init(layer, behaviour);
            
            schema = behaviour as SchemaBehaviour;
            feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
        }

        protected override void OnMouseDown(VisualElement _target, Vector2Int position, MouseDownEvent e)
        {
            first = schema.OwnerLayer.ToFixedPosition(position);
        }

        protected override void OnMouseUp(VisualElement _target, Vector2Int position, MouseUpEvent e)
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
            Vector2Int lastPos = schema.OwnerLayer.ToFixedPosition(position);

            // Get vector direction
            int dx =  first.x - lastPos.x;
            int dy = first.y - lastPos.y;
            
            
            // Get index of directions
            int frontDirIndex = -1;
            int backDirIndex = -1;
            
            float  dLenght =  Mathf.Sqrt(dx * dx  +  dy * dy);
            
            if (dLenght>= 2)
            {
                int totalConnections = (int)Math.Floor(dLenght);
                List<LBSTile> selectedTiles = new List<LBSTile>();
                
                for (int i = 0; i <= totalConnections; i++)
                {
                    //Get the next tile 
                    selectedTiles.Add(schema.GetTile(first - new Vector2Int(Math.Sign(dx) * i, Math.Sign(dy) * i)));
                }
                
                frontDirIndex = schema.Directions.FindIndex(d => d.Equals(-new Vector2Int(Math.Sign(dx), Math.Sign(dy))));
                backDirIndex = schema.Directions.FindIndex(d => d.Equals(new Vector2Int(Math.Sign(dx), Math.Sign(dy))));

                for(int i = 0; i <= selectedTiles.Count; i++)
                {
                    //TODO - Allow Paint More Thant 1 Tile 
                    LBSTile tile = selectedTiles[i];
                    LBSTile nextTile = selectedTiles[i+1];
                    
                    if (tile != null)
                    {
                        List<string> connections = schema.GetConnections(tile);
                        
                        foreach (var connection in connections)
                        {

                            if (frontDirIndex != -1 || frontDirIndex < schema.Directions.Count)
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
            
            frontDirIndex = schema.Directions.FindIndex(d => d.Equals(-new Vector2Int(dx, dy)));
            backDirIndex = schema.Directions.FindIndex(d => d.Equals(new Vector2Int(dx, dy)));
            
            // Check if index is validate
            if (frontDirIndex < 0 || frontDirIndex >= schema.Directions.Count)
                return;
            
            LBSTile t1 = schema.GetTile(first);
            LBSTile t2 = schema.GetTile(lastPos);
            
            TrySetSingleConnection(t1, t2 ,frontDirIndex, backDirIndex);
            

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(level);
            }
            
        }

        private void TrySetSingleConnection(
            LBSTile _firstTile,
            LBSTile _secondTile,
            int _frontDirIndex,
            int _backDirIndex
            )
        {
            
            if (_firstTile == null)
            {
                if (_secondTile != null)
                {
                    schema.SetConnection(_secondTile, _backDirIndex, ToSet, false);
                    return;
                }
            }
            else
            {
                if (_firstTile.Equals(_secondTile))
                {
                    Debug.Log("Not Valid Tile - Same Tile with lenght 0");
                    return;
                }
            }
            
            if (_secondTile == null)
            {
                if (_firstTile == null)
                {
                    return;
                }
                schema.SetConnection(_firstTile, _frontDirIndex, ToSet, false);
                return;
            }
            
            // set both connections
            schema.SetConnection(_firstTile, _frontDirIndex, ToSet, false);
            schema.SetConnection(_secondTile, _backDirIndex, ToSet, false);
        }
    }
}