using System;
using ISILab.Commons.Utility.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using UnityEditor;

namespace ISILab.LBS.VisualElements
{
    public class LBSQuestEdgeView : GraphElement
    {
        private Vector2Int _pos1, _pos2;
        private const int PosOffset = 16;
        private readonly float _lineWidth;
        private readonly float _stroke;

        private Label _connectionTypeLabel;

        private QuestEdge _edge;

        private QuestGraph _graph;

    private readonly VisualElement _connectionView;

        public LBSQuestEdgeView(QuestGraph questGraph, QuestEdge edge, QuestNodeView node1, QuestNodeView node2, float lineWidth = 5f, float stroke = 3f)
        {
            _graph = questGraph;
            _edge = edge;
            _lineWidth = lineWidth;
            _stroke = stroke;
            
            
            // Load the UXML into this GraphElement
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestEdgeView");
            visualTree.CloneTree(this);

            // Grab the arrow view
            _connectionView = this.Q<VisualElement>("View");
            
            // Handle movement of first node
            node1.OnMoving += (rect) =>
            {
                ActualizePositions(rect.center.ToInt(), _pos2);
            };


            // Handle movement of second node
            node2.OnMoving += (rect) =>
            {
                ActualizePositions(_pos1, rect.center.ToInt());
            };

            // Initial positions
            var sPos1 = new Vector2Int((int)node1.GetPosition().xMax - PosOffset*4, (int)node1.GetPosition().center.y- PosOffset);
            var sPos2 = new Vector2Int((int)node2.GetPosition().xMin, (int)node2.GetPosition().center.y- PosOffset);
            ActualizePositions(sPos1, sPos2);

            // Draw the dotted line on the root
            generateVisualContent += DrawLine;
            
            // to change the edge type
            RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            // Only right-click
            if (evt.button == (int)MouseButton.RightMouse)
            {
                var menu = new GenericMenu();
        
                menu.AddItem(new GUIContent("Set Type/Direct"), false, () => _graph.ChangeConnection(_edge, typeof(QuestNode)));
                menu.AddItem(new GUIContent("Set Type/OR"), false, () => _graph.ChangeConnection(_edge, typeof(OrNode)));
                menu.AddItem(new GUIContent("Set Type/AND"), false, () => _graph.ChangeConnection(_edge, typeof(AndNode)));
                menu.AddSeparator("");
                //menu.AddItem(new GUIContent("Delete Edge"), false, () => DeleteThisEdge());

                menu.ShowAsContext();

                evt.StopPropagation(); // Prevent other handlers from firing
            }
        }

        private void DrawLine(MeshGenerationContext mgc)
        {
            var painter = mgc.painter2D;
            painter.DrawDottedLine(_pos1, _pos2, Color.white, _stroke, _lineWidth);
        }
        

        private void ActualizePositions(Vector2Int newPos1, Vector2Int newPos2)
        {
            _pos1 = newPos1;
            _pos2 = newPos2;

            // Midpoint position
            Vector2 midpoint = new Vector2(_pos1.x + _pos2.x - PosOffset*3f,  _pos1.y + _pos2.y - PosOffset*3f) / 2f;
            _connectionView.transform.position = midpoint;

            // Rotation: angle from right (1,0) to direction vector
            Vector2 direction =  new Vector2(_pos2.x - _pos1.x, _pos2.y - _pos1.y).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Apply rotation to the arrow visual element
            _connectionView.transform.rotation = Quaternion.Euler(0, 0, angle);

            // Repaint both
            MarkDirtyRepaint();
            _connectionView.MarkDirtyRepaint();
        }

    }
}
