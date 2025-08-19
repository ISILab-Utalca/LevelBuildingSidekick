using System;
using ISILab.Commons.Utility.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Components;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Manipulators;
using ISILab.LBS.VisualElements.Editor;
using LBS.VisualElements;

namespace ISILab.LBS.VisualElements
{
    public abstract class QuestGraphNodeView : GraphElement
    {
        protected GraphNode Node;
        public Action<Rect> OnMoving;
        protected Color DefaultBackgroundColor;

        public abstract void DisplayGrammarState(GraphNode node);
        protected void OnMouseLeave(MouseLeaveEvent e)
        {
            if(Node is null) return;
            OnMouseMove(MouseMoveEvent.GetPooled(e.mousePosition, e.button, e.clickCount, e.mouseDelta ));
        }
        
        protected void OnMouseMove(MouseMoveEvent e)
        {
            if(Node is null) return;
            if (!Equals(LBSMainWindow.Instance._selectedLayer, Node.Graph.OwnerLayer)) return;
            
            // left button pressed
            if (e.pressedButtons != 1) return;
            if (!MainView.Instance.HasManipulator<SelectManipulator>() ) return;
            
            var grabPosition =  GetPosition().position + e.mouseDelta / MainView.Instance.viewTransform.scale;
            grabPosition *= MainView.Instance.viewport.transform.scale;
            Rect newPos = new Rect(grabPosition.x, grabPosition.y, resolvedStyle.width, resolvedStyle.height);
            SetPosition(newPos);
            Node.Position = grabPosition.ToInt();
            Node.NodeViewPosition = newPos;
        }
        
        protected virtual void OnMouseDown(MouseDownEvent evt)
        {
            if(Node is null) return;
            if (!Equals(LBSMainWindow.Instance._selectedLayer, Node.Graph.OwnerLayer)) return;
            
            DrawManager.Instance.RedrawLayer(Node.Graph.OwnerLayer);
        }
        protected void CenterElement(Vector2 clickPosition)
        {
            if (clickPosition != default)
            {
                // Get the size of the element after layout
                float width = layout.width;
                float height = layout.height;

                // Calculate the offset to center the element
                float offsetX = width / 2f;
                float offsetY = height / 2f;

                // Adjust the position to center the element at the click position
                style.left = clickPosition.x - offsetX;
                style.top = clickPosition.y - offsetY;
            }
        }

    }

    public class QuestBranchView : QuestGraphNodeView
    {
        private static VisualTreeAsset _view;

        public QuestBranchView(GraphNode graphNode, Vector2 clickPosition = default)
        {
            if (_view == null)
            {
                _view = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestBranchView");
            }
            _view.CloneTree(this);
            var label = this.Q<Label>("Label");

            // Assign name
            label.text = graphNode switch
            {
                OrNode on => on.ToString(),
                AndNode an => an.ToString(),
                _ => label.text
            };

            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
            
            // Ensure the element is laid out before adjusting position
            RegisterCallback<GeometryChangedEvent>(evt => CenterElement(clickPosition));
            
            Node = graphNode;
        }
        
        public override void DisplayGrammarState(GraphNode node)
        {
        }
    }
}