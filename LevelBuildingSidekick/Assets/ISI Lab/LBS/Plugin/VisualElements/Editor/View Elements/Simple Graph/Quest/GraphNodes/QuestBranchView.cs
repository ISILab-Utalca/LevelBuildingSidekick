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
        public GraphNode Node;
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
        }
        
        protected virtual void OnMouseDown(MouseDownEvent evt)
        {
            if(Node is null) return;
            if (!Equals(LBSMainWindow.Instance._selectedLayer, Node.Graph.OwnerLayer)) return;
            
            DrawManager.Instance.RedrawLayer(Node.Graph.OwnerLayer);
        }
    }

    public class QuestBranchView : QuestGraphNodeView
    {
        private static VisualTreeAsset _root;
        private VisualElement _view;

        public QuestBranchView(GraphNode graphNode, Vector2 clickPosition = default)
        {
            if (_root == null)
            {
                _root = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestBranchView");
            }
            _root.CloneTree(this);
            _view = this.Q<VisualElement>("View");
            var label = this.Q<Label>("Label");

            Node = graphNode ?? throw new ArgumentNullException(nameof(graphNode));
            // Assign name
            label.text = graphNode switch
            {
                OrNode on => on.ToString(),
                AndNode an => an.ToString(),
                _ => label.text
            };
            SetPosition(new Rect(Node.NodeViewPosition.position, Vector2.one));
            
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            
            OnMoving += rect1 =>
            {
                Node.NodeViewPosition = rect1;
            };

            Update();
        }
        
        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            Update();
        }
        
        private void Update()
        {
            SetPosition(
                new Rect(GetPosition().position, 
                    new Vector2(_view.resolvedStyle.width, _view.resolvedStyle.height))
            );
            
            // Notify movement for edge updates
            OnMoving?.Invoke(GetPosition());
        }
        
        public override void DisplayGrammarState(GraphNode node)
        {
        }
    }
}