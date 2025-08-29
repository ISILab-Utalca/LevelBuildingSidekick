using System.Collections.Generic;
using UnityEngine;
using LBS.Components;
using LBS.Components.TileMap;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.VisualElements;
using ISILab.LBS.Editor.Windows;
using UnityEditor;

namespace ISILab.LBS.Manipulators
{
    public class SetVertexExteriorTileConnection : LBSManipulator
    {
        private static List<Vector2Int> Directions => Commons.Directions.Bidimencional.Edges;

        private ExteriorBehaviour _exterior;
        private Vector2Int _first;

        private readonly ConnectedConrnerLine _lineFeedback = new();
        private readonly Feedback _areaFeedback = new AreaFeedback();
        protected override string IconGuid => "89403d16440c74442a7260e1a2fe2a40";

        private LBSTag ToSet => _exterior.identifierToSet;

        public SetVertexExteriorTileConnection()
        {
            _lineFeedback.fixToTeselation = true;
            _lineFeedback.useVertices = true;
            _areaFeedback.fixToTeselation = true;
            Feedback = _lineFeedback;

            Name = "Paint roads";
            Description = "Paint line across tile vertices to make connections. Hold CTRL to connect areas.";
        }

        public override void Init(LBSLayer layer, object provider = null)
        {
            base.Init(layer, provider);
            _exterior = provider as ExteriorBehaviour;
            _lineFeedback.TeselationSize = layer.TileSize;
            _areaFeedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) =>
            {
                _lineFeedback.TeselationSize = val;
                _areaFeedback.TeselationSize = val;
            };
        }

        protected override void OnKeyDown(KeyDownEvent e)
        {
            base.OnKeyDown(e);
            //if (e.ctrlKey) LBSMainWindow.WarningManipulator("(CTRL) Adding connections in area");
        }

        protected override void OnKeyUp(KeyUpEvent e)
        {
            LBSMainWindow.WarningManipulator();
        }

        protected override void OnMouseDown(VisualElement element, Vector2Int position, MouseDownEvent e)
        {
            _first = _exterior.OwnerLayer.ToFixedPositionOffset(position, new Vector2(50, -50));
        }

        protected override void OnMouseMove(VisualElement element, Vector2Int movePosition, MouseMoveEvent e)
        {
            if (ForceCancel) return;

            _lineFeedback.LeftSide = e.shiftKey;

            //SetFeedback(!e.ctrlKey ? _lineFeedback : _areaFeedback);
            SetFeedback(_lineFeedback);
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

            if (ToSet == null || ToSet.Label == "")
            {
                Debug.LogWarning("You don't have any connection selected.");
                return;
            }

            LoadedLevel x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Add Connections");

            // Get end position
            Vector2Int end = _exterior.OwnerLayer.ToFixedPositionOffset(position, new Vector2(50, -50));

            //if (!e.ctrlKey)
            if (true)
            {
                LineEffect(end, e);
            }
            else
            {
                AreaEffect();
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }

        }

        private void LineEffect(Vector2Int end, MouseUpEvent e)
        {
            // Get corner position
            Vector2Int corner = e.shiftKey ?
                new Vector2Int(_first.x, end.y) :
                new Vector2Int(end.x, _first.y);

            List<(LBSTile, Vector2Int) > path = new();

            // Get first path
            Vector2Int current = _first;
            while (!current.Equals(corner))
            {
                LBSTile tile = _exterior.GetTile(current);
                Vector2Int dir = ((Vector2)(corner - _first)).normalized.ToInt();

                path.Add((tile, current));
                current += dir;
            }

            // Get second path
            current = corner;
            while (!current.Equals(end))
            {
                LBSTile tile = _exterior.GetTile(current);
                Vector2Int dir = ((Vector2)(end - corner)).normalized.ToInt();

                path.Add((tile, current));
                current += dir;
            }

            path.Add((_exterior.GetTile(current), current));

            for (int i = 0; i < path.Count; i++)
            {
                string connection = ToSet.Label;
                List<Vector2Int> dirs = Directions.Rotate(2);
                for(int j = 0; j < dirs.Count; j++)
                {
                    Vector2Int targetDir = Vector2Int.zero;
                    if(j < dirs.Count - 1)
                    {
                        for(int k = 0; k <= j; k++)
                        {
                            targetDir += dirs[k];
                        }
                    }
                    LBSTile tile = targetDir != Vector2Int.zero ? _exterior.GetTile(path[i].Item2 + targetDir) : path[i].Item1;
                    if(tile != null)
                        _exterior.SetConnection(tile, (j + 3) % 4, connection, false);
                }
            }
        }

        private void AreaEffect()
        {
            var corners = _exterior.OwnerLayer.ToFixedPosition(StartPosition, EndPosition);

            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    var pos = new Vector2Int(i, j);
                    var tile = _exterior.GetTile(pos);

                    if (tile == null)
                    {
                        continue;
                    }

                    for (int k = 0; k < Directions.Count; k++)
                    {
                        _exterior.SetConnection(tile, k, ToSet.Label, false);

                        var dir = Directions[k];
                        var neighbor = _exterior.GetTile(pos + dir);

                        if (neighbor != null)
                        {
                            _exterior.SetConnection(neighbor, (k + 2) % 4, ToSet.Label, false);
                        }
                    }
                }
            }
        }
    }
}