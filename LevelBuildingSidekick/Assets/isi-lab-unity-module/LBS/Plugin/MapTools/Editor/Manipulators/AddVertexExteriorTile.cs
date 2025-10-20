using ISILab.LBS.Behaviours;
using LBS.Components;
using LBS.Components.TileMap;
using System.Collections.Generic;
using ISILab.LBS.Editor.Windows;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;

namespace ISILab.LBS.Manipulators
{
    public class AddVertexExteriorTile : ManipulateTeselation
    {
        //private static List<Vector2Int> Directions => Commons.Directions.Bidimencional.Edges;
        private List<Vector2Int> NeighbourDirections = Commons.Directions.Bidimencional.All;
        private ExteriorBehaviour _exterior;
        protected override string IconGuid => "ce4ce3091e6cf864cbbdc1494feb6529";

        public AddVertexExteriorTile()
        {
            Name = "Add Tile";
            Description = "Add an Exterior Tile. Hold CTRL to paint neighbors as well.";
        }

        public override void Init(LBSLayer layer, object provider)
        {
            base.Init(layer, provider);

            _exterior = provider as ExteriorBehaviour;
        }

        protected override void OnKeyDown(KeyDownEvent e)
        {
            base.OnKeyDown(e);
            if (e.ctrlKey) LBSMainWindow.WarningManipulator("(CTRL) Preserving neighbour connections");
        }

        protected override void OnKeyUp(KeyUpEvent e)
        {
            LBSMainWindow.WarningManipulator();
        }

        protected override void OnMouseUp(VisualElement element, Vector2Int endPosition, MouseUpEvent e)
        {
            base.OnMouseUp(element, endPosition, e);

            //If esc key was pressed, cancel the operation
            if (ForceCancel)
            {
                ForceCancel = false;
                return;
            }

            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Add empty tiles");

            var paintNeighbors = !e.ctrlKey;

            var corners = _exterior.OwnerLayer.ToFixedPosition(StartPosition, EndPosition);

            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    var pos = new Vector2Int(i, j);
                    var tile = new LBSTile(pos);

                    _exterior.AddTile(tile);


                    if (!_exterior.identifierToSet ||
                        _exterior.identifierToSet.Label == null || _exterior.identifierToSet.Label == "Empty") continue;

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
            for (int i = 0; i < 4; i++)
            {
                Vector2Int edgeNeighbour = NeighbourDirections[i * 2];
                Vector2Int vertexNeighbour = NeighbourDirections[i * 2 + 1];
                LBSTile neighbour = _exterior.GetTile(pos + edgeNeighbour);
                if (neighbour != null && !paintNeighbors)
                {
                    // Conservar conexiones de los vecinos
                    string conn = _exterior.GetConnections(neighbour)[(i + 1) % 4];
                    if (conn.Equals(""))
                    {
                        _exterior.SetConnection(tile, i, _exterior.identifierToSet.Label, true);
                    }
                    else
                    {
                        _exterior.SetConnection(tile, i, conn, true);
                    }
                    continue;
                }

                _exterior.SetConnection(tile, i, _exterior.identifierToSet.Label, true);

                foreach (Vector2Int neighbourDir in new[] { edgeNeighbour, vertexNeighbour })
                {
                    neighbour = _exterior.GetTile(pos + neighbourDir);
                    if (neighbour is { })
                    {
                        List<int> indices = neighbourDir.GetVertices();
                        indices.ForEach(ind => _exterior.SetConnection(neighbour, ind, _exterior.identifierToSet.Label, true));
                    }
                }
            }
        }
    }
}