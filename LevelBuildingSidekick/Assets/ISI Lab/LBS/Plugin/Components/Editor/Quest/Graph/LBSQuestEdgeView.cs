using System;
using ISILab.Commons.Utility.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Components;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Modules;
using ISILab.LBS.Settings;
using ISILab.LBS.VisualElements.Editor;
using UnityEditor;

namespace ISILab.LBS.VisualElements
{
    public class LBSQuestEdgeView : GraphElement
    {
        private Vector2 _startPos, _endPos; // Use Vector2 for precise positioning
        private readonly float _lineWidth;
        private readonly float _stroke;
        private readonly QuestEdge _edge;
        private readonly QuestGraph _graph;
        private readonly VisualElement _connectionView;
        private readonly QuestGraphNodeView _node1;
        private readonly QuestGraphNodeView _node2;

        public LBSQuestEdgeView(QuestGraph questGraph, QuestEdge edge, QuestGraphNodeView node1, QuestGraphNodeView node2, float lineWidth = 5f, float stroke = 3f)
        {
            _graph = questGraph ?? throw new ArgumentNullException(nameof(questGraph));
            _edge = edge ?? throw new ArgumentNullException(nameof(edge));
            _node1 = node1 ?? throw new ArgumentNullException(nameof(node1));
            _node2 = node2 ?? throw new ArgumentNullException(nameof(node2));
            _lineWidth = lineWidth;
            _stroke = stroke;

            // Grab the arrow view
            _connectionView = this.Q<VisualElement>("View");

            // Handle movement of first node
            node1.OnMoving += UpdatePositionFromNode1;

            // Handle movement of second node
            node2.OnMoving += UpdatePositionFromNode2;

            // Initialize positions
            UpdatePositions();

            // Draw the dotted line
            generateVisualContent -= DrawLine;
            generateVisualContent += DrawLine;

            // Register right-click menu for edge type change
            RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void UpdatePositionFromNode1(Rect node1Rect)
        {
            UpdatePositions();
        }

        private void UpdatePositionFromNode2(Rect node2Rect)
        {
            UpdatePositions();
        }

        private void UpdatePositions()
        {
            var worldPos1 = _node1.worldBound.center;
            var worldPos2 = _node2.worldBound.center;
            var dir = (worldPos2 - worldPos1).normalized;

            // circle radius ~10, arrow length ~12 → use as offsets
            var edge1 = GetRectEdgePoint(_node1.worldBound, dir, 5f);   // circle offset
            var edge2 = GetRectEdgePoint(_node2.worldBound, -dir, 5f);  // arrow offset

            _startPos = this.WorldToLocal(edge1);
            _endPos   = this.WorldToLocal(edge2);

            MarkDirtyRepaint();
        }

        
        private Vector2 GetRectEdgePoint(Rect rect, Vector2 direction, float extraOffset = 0f)
        {
            Vector2 center = rect.center;
            if (direction == Vector2.zero)
                return center;

            direction.Normalize();

            float tx = direction.x > 0
                ? (rect.xMax - center.x) / direction.x
                : (rect.xMin - center.x) / direction.x;

            float ty = direction.y > 0
                ? (rect.yMax - center.y + 5) / direction.y
                : (rect.yMin - center.y + 5) / direction.y;

            float t = Mathf.Min(tx, ty);

            return center + direction * (t + extraOffset);
        }

        
        private void DrawLine(MeshGenerationContext mgc)
        {
            var painter = mgc.painter2D;
            painter.strokeColor = Color.white;
            painter.lineWidth = _stroke;

            painter.DrawDottedLine(_startPos, _endPos, painter.strokeColor, _lineWidth);

            Vector2 dir = (_endPos - _startPos).normalized;

            painter.DrawEquilateralArrow(_endPos, dir, 24, painter.strokeColor);
            painter.DrawCircle(_startPos, 10, painter.strokeColor);
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
                evt.StopPropagation();
            }
        }
    }
}