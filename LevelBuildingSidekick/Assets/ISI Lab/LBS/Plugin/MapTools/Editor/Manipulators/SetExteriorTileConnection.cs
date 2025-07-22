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
    public class SetExteriorTileConnection : LBSManipulator
    {
        private static List<Vector2Int> Directions => Commons.Directions.Bidimencional.Edges;

        private ExteriorBehaviour _exterior;
        private Vector2Int _first;

        private readonly ConnectedConrnerLine _lineFeedback = new();
        private readonly Feedback _areaFeedback = new AreaFeedback();
        protected override string IconGuid => "89403d16440c74442a7260e1a2fe2a40";

        private LBSTag ToSet => _exterior.identifierToSet;

        public SetExteriorTileConnection()
        {
            _lineFeedback.fixToTeselation = true;
            _areaFeedback.fixToTeselation = true;
            Feedback = _lineFeedback;

            Name = "Set connection";
            Description = "Paint line across tiles to make connections. Hold CTRL to connect areas.";
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
            if (e.ctrlKey) LBSMainWindow.WarningManipulator("(CTRL) Adding connections in area");
        }
        
        protected override void OnKeyUp(KeyUpEvent e)
        {
            LBSMainWindow.WarningManipulator();
        }
        
        protected override void OnMouseDown(VisualElement element, Vector2Int position, MouseDownEvent e)
        {
            _first = _exterior.OwnerLayer.ToFixedPosition(position);
        }

        protected override void OnMouseMove(VisualElement element, Vector2Int movePosition, MouseMoveEvent e)
        {
            _lineFeedback.LeftSide = e.shiftKey;

            SetFeedback(!e.ctrlKey ? _lineFeedback : _areaFeedback);
        }

        protected override void OnMouseUp(VisualElement element, Vector2Int position, MouseUpEvent e)
        {
            if (ToSet == null || ToSet.Label == "")
            {
                Debug.LogWarning("You don't have any connection selected.");
                return;
            }

            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Add Connections");

            // Get end position
            var end = _exterior.OwnerLayer.ToFixedPosition(position);

            if (!e.ctrlKey)
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
            var corner = e.shiftKey ?
                new Vector2Int(_first.x, end.y) :
                new Vector2Int(end.x, _first.y);

            List<(LBSTile, Vector2Int, Vector2Int)> path = new();

            // Get first path
            Vector2Int current = _first;
            while (!current.Equals(corner))
            {
                var tile = _exterior.GetTile(current);
                var dir = ((Vector2)(corner - _first)).normalized.ToInt();

                path.Add((tile, new Vector2Int(current.x, current.y), dir));
                current += dir;
            }

            // Get second path
            current = corner;
            while (!current.Equals(end))
            {
                var tile = _exterior.GetTile(current);
                var dir = ((Vector2)(end - corner)).normalized.ToInt();

                path.Add((tile, new Vector2Int(current.x, current.y), dir));
                current += dir;
            }

            for (int i = 0; i < path.Count; i++)
            {
                var t1 = path[i].Item1;
                var fDir = Directions.FindIndex(d => d.Equals(path[i].Item3));

                if (t1 != null)
                {
                    _exterior.SetConnection(t1, fDir, ToSet.Label, false);
                }

                var t2 = _exterior.GetTile(path[i].Item2 + path[i].Item3);
                var dDir = Directions.FindIndex(d => d.Equals(-path[i].Item3));

                if (t2 != null)
                {
                    _exterior.SetConnection(t2, dDir, ToSet.Label, false);
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