using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.Extensions;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Manipulators;
using ISILab.LBS.VisualElements.Editor;
using ISILab.Macros;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    /// <summary>
    /// Represents a visual element on the quest graph used to indicate a trigger area or region.
    /// 
    /// This element is associated with a <see cref="BaseQuestNodeData"/> and draws a visual box on the graph.
    /// 
    /// Supports interaction such as:
    /// - Dragging to reposition
    /// - Resizing via a handle
    /// - Updating the logical data when moved
    /// - An Icon that represents the node type for easier readability.
    /// 
    /// Also handles custom visual generation through <see cref="MeshGenerationContext"/> to draw lines between this element and its node origin.
    /// </summary>
    public class TriggerElement : GraphElement
    {
        private readonly BaseQuestNodeData _data;
        private readonly Color _currentColor;

        private bool _isDragging;
        private Vector2 _dragStartMouse;
        private Vector2 _dragStartPosition;

        public TriggerElement(BaseQuestNodeData data, Rect area, Color color)
        {
            _data = data;

            VisualTreeAsset visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("TriggerElementArea");
            visualTree.CloneTree(this);

            _currentColor = data.Color;

            // Calculate initial visual position
            var position = LBSMainWindow.Instance._selectedLayer.FixedToPosition(
                new Vector2Int((int)area.x, (int)area.y), true);
            Rect drawArea = new Rect(position, new Vector2(area.width * 100, area.height * 100));

            SetPosition(drawArea);

            VisualElement triggerElementGizmo = this.Q<VisualElement>("TriggerElementSelector");

            // Styling
            Color backgroundColor = _currentColor;
            backgroundColor.a = 0.2f;
            triggerElementGizmo.style.backgroundColor = backgroundColor;
            triggerElementGizmo.style.unityBackgroundImageTintColor = backgroundColor;
            triggerElementGizmo.style.borderBottomColor = _currentColor;
            triggerElementGizmo.style.borderTopColor = _currentColor;
            triggerElementGizmo.style.borderRightColor = _currentColor;
            triggerElementGizmo.style.borderLeftColor = _currentColor;

            // Register mouse callbacks on the whole element
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            RegisterCallback<MouseUpEvent>(OnMouseUp);

            // Handle for resizing (optional)
            VisualElement handle = this.Q<VisualElement>("ScaleHandle");
            handle.RegisterCallback<MouseMoveEvent>(OnHandleRectMove);

            generateVisualContent -= OnGenerateVisualContent;
            generateVisualContent += OnGenerateVisualContent;
        }

        void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            var painter = mgc.painter2D;
            var lbsLayer = _data.Layer;
            
            var nodeElements = MainView.Instance.GetElementsFromLayerContainer(lbsLayer, _data.ID);

            var node = nodeElements?.FirstOrDefault();
            if (node == null) return;

            Vector2 center = new Vector2(GetPosition().width / 2f, GetPosition().height / 2f);
            Rect nodeRect = node.worldBound;
            Vector2 nodeWorldCenter = nodeRect.position + nodeRect.size / 2f;
            Vector2 to = this.WorldToLocal(nodeWorldCenter); // convert world to local space
            
            painter.DrawDottedLine(center, to, _currentColor, 4f, 10f);
        }


        private void OnMouseDown(MouseDownEvent e)
        {
            if (e.button != 0) return;
            _isDragging = true;
            _dragStartMouse = e.mousePosition;
            _dragStartPosition = GetPosition().position;

            e.StopPropagation();
        }

        private void OnMouseMove(MouseMoveEvent e)
        {
            if (!_isDragging || e.pressedButtons != 1) return;
            if (!MainView.Instance.HasManipulator<SelectManipulator>()) return;

            var scale = MainView.Instance.viewTransform.scale;

            Vector2 delta = (e.mousePosition - _dragStartMouse) / scale;
            Vector2 newPos = _dragStartPosition + delta;

            Rect newRect = new Rect(newPos, GetPosition().size);
            SetPosition(newRect);
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            if (!_isDragging) return;
            _isDragging = false;

            if (_data.OwnerNode?.Graph?.OwnerLayer is null) return;

            var finalRect = LBSMainWindow._gridPosition;
            _data.Area = new Rect(finalRect.x, finalRect.y, _data.Area.width, _data.Area.height);
            _data.Graph.DataChanged(_data.OwnerNode);
        }

        private void OnHandleRectMove(MouseMoveEvent e)
        {
            if (e.pressedButtons == 0 || e.button != 0) return;

            Vector2 delta = e.mouseDelta / MainView.Instance.viewTransform.scale;
            delta = delta.normalized;

            Rect currentRect = GetPosition();
            Rect newRect = new Rect(
                currentRect.x,
                currentRect.y,
                currentRect.width + delta.x,
                currentRect.height + delta.y
            );
            SetPosition(newRect);
        }
    }
}
