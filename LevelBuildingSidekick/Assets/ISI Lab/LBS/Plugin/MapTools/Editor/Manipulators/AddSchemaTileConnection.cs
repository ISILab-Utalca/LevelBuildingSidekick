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

                foreach (var tile in selectedTiles)
                {
                    Debug.Log(tile.Position);
                    //TODO - Allow Paint More Thant 1 Tile 
                }
                
            }
            else
            {
                frontDirIndex = schema.Directions.FindIndex(d => d.Equals(-new Vector2Int(dx, dy)));
                backDirIndex = schema.Directions.FindIndex(d => d.Equals(new Vector2Int(dx, dy)));
            }
            
            // Debug.Log(fDirIndex);
            // Debug.Log(tDirIndex);
            
            // Check if index is validate
            if (frontDirIndex < 0 || frontDirIndex >= schema.Directions.Count)
                return;

            // Get tile in first position
            LBSTile t1 = schema.GetTile(first);
            // Get tile in second position
            LBSTile t2 = schema.GetTile(lastPos);

            if (t1 == null)
            {
                if (t2 != null)
                {
                    schema.SetConnection(t2, frontDirIndex, ToSet, false);
                    return;
                }
            }
            else
            {
                if (t1.Equals(t2))
                {
                    Debug.Log("Not Valid Tile - Same Tile with lenght 0");
                    return;
                }
            }
            
            if (t2 == null)
            {
                if (t1 == null)
                {
                    return;
                }
                schema.SetConnection(t1, frontDirIndex, ToSet, false);
                return;
            }
            
            // set both connections
            schema.SetConnection(t1, frontDirIndex, ToSet, false);
            schema.SetConnection(t2, backDirIndex, ToSet, false);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(level);
            }
            
        }
    }
}