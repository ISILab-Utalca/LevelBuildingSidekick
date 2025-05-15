using System;
using ISILab.LBS.Behaviours;
using LBS.Components;
using LBS.Components.TileMap;
using System.Collections.Generic;
using ISILab.LBS.Editor.Windows;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class AddExteriorTile : ManipulateTeselation
    {
        private List<Vector2Int> Directions => Commons.Directions.Bidimencional.Edges;
        private ExteriorBehaviour exterior;
        protected override string IconGuid => "ce4ce3091e6cf864cbbdc1494feb6529";

        public AddExteriorTile() : base()
        {
            name = "Add Tile";
            description = "Add an Exterior Tile. Hold CTRL to paint neighbors as well.";
        }
        
        public override void Init(LBSLayer layer, object owner)
        {
            base.Init(layer, owner);

            exterior = owner as ExteriorBehaviour;
        }

        protected override void OnKeyDown(KeyDownEvent e)
        {
            base.OnKeyDown(e);
            if (e.ctrlKey) LBSMainWindow.WarningManipulator("(CTRL) Adding Tile with neighbour connections");
        }
        
        protected override void OnKeyUp(KeyUpEvent e)
        {
            LBSMainWindow.WarningManipulator();
        }
        
        protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
        {
            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Add empty tiles");

            var paintNeighbors = e.ctrlKey;
            
            var corners = exterior.OwnerLayer.ToFixedPosition(StartPosition, EndPosition);

            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    var pos = new Vector2Int(i, j);
                    var tile = new LBSTile(pos);
                    exterior.AddTile(tile);
                    
                    if (!exterior.identifierToSet || 
                        exterior.identifierToSet.Label == null) continue;
            
                    SetConnections(tile, pos, paintNeighbors);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
        }

        private void SetConnections(LBSTile tile, Vector2Int pos, bool paintNeighbors)
        {
            // Paint all connections
            for (int k = 0; k < Directions.Count; k++)
            {
                exterior.SetConnection(tile, k, exterior.identifierToSet.Label, true);

                if(!paintNeighbors) continue;
                
                var dir = Directions[k];
                var neig = exterior.GetTile(pos + dir);
                if (neig != null)
                {
                    exterior.SetConnection(neig, (k + 2) % 4, exterior.identifierToSet.Label, true);
                }
            }
        }
    }
}