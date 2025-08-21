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
        private Vector2 _pos1, _pos2; // Use Vector2 for precise positioning
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
            // Convert node local centers -> world space
            var worldPos1 = _node1.worldBound.center;
            var worldPos2 = _node2.worldBound.center;

            // Convert world space -> edge's local space
            var graphSpace1 = this.WorldToLocal(worldPos1);
            var graphSpace2 = this.WorldToLocal(worldPos2);

            _pos1 = graphSpace1;
            _pos2 = graphSpace2;

            MarkDirtyRepaint();
        }

        private void DrawLine(MeshGenerationContext mgc)
        {
            var painter = mgc.painter2D;
            painter.DrawDottedLine(_pos1, _pos2, Color.white, _stroke, _lineWidth);
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