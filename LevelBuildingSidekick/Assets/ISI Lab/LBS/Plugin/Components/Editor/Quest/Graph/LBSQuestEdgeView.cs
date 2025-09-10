using System;
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
        private const float curveBendStrength = 0.4f;
        
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
            var worldPos1 = _node1.GetSelectVisualElement().worldBound.center;
            var worldPos2 = _node2.GetSelectVisualElement().worldBound.center;
            var dir = (worldPos2 - worldPos1).normalized;
            
            var edge1 = GetRectEdgePoint(_node1.GetSelectVisualElement().worldBound, dir, 5f);   // circle offset
            var edge2 = GetRectEdgePoint(_node2.GetSelectVisualElement().worldBound, -dir, 5f);  // arrow offset

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
                ? (rect.yMax - center.y) / direction.y
                : (rect.yMin - center.y) / direction.y;

            float t = Mathf.Min(tx, ty);

            return center + direction * (t + extraOffset);
        }


        private void DrawLine(MeshGenerationContext mgc)
        {
            var painter = mgc.painter2D;
            painter.strokeColor = Color.white;
            painter.lineWidth = _stroke;

            Vector2 dir = (_endPos - _startPos).normalized;
            float distance = Vector2.Distance(_startPos, _endPos);
            float bend = distance * curveBendStrength; 

            // Move perpendicular to the line
            Vector2 perp1 = new Vector2(-dir.x, dir.y);
            Vector2 perp2 = new Vector2(dir.x, -dir.y);
            
            Vector2 controlPoint1 = _startPos + dir * (distance * 0.4f) + perp1 * bend ;
            Vector2 controlPoint2 = _endPos - dir * (distance * 0.4f) + perp2 * bend ;
            
            // Direction is tangent at the end of the line
            Vector2 endDir = painter.DrawBezierLine(_startPos, controlPoint1, controlPoint2, _endPos, 
                painter.strokeColor, painter.strokeColor);

            // if at similar Y positions of visual elements, use normal direction
            if (MathF.Abs(_startPos.y - _endPos.y) <= _node1.worldBound.height)
            {
                endDir = dir;
            }
            
            
            painter.DrawArrow(_endPos, endDir,16,3f, painter.strokeColor);
            painter.DrawCircle(_startPos, 10, painter.strokeColor);
        }



        private void OnMouseDown(MouseDownEvent evt)
        {
            // Only right-click
            if (evt.button != (int)MouseButton.RightMouse) return;
            
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Set Type/Direct"), false, () => _graph.ChangeConnection(_edge, typeof(QuestNode)));
            menu.AddItem(new GUIContent("Set Type/OR"), false, () => _graph.ChangeConnection(_edge, typeof(OrNode)));
            menu.AddItem(new GUIContent("Set Type/AND"), false, () => _graph.ChangeConnection(_edge, typeof(AndNode)));
            menu.AddSeparator("");

            menu.ShowAsContext();
            evt.StopPropagation();
        }
    }
}