using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.LBS.Components;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Settings;
using ISILab.LBS.VisualElements.Editor;
using LBS.VisualElements;

namespace ISILab.LBS.VisualElements
{
    public abstract class QuestGraphNodeView : GraphElement
    {
        #region Static Colors
        protected static readonly Color InvalidGrammarColor     = LBSSettings.Instance.view.errorColor;
        protected static readonly Color DefaultBackgroundColor = LBSSettings.Instance.view.toolkitNormalDark;
        protected static readonly Color ValidGrammarColor   = LBSSettings.Instance.view.successColor;
        #endregion

        #region Fields
        protected GraphNode Node;
        protected VisualElement InvalidConnectionIcon;

        private static QuestGraphNodeView _selectedGraph;

        private const float Alpha = 0.33f;

        #endregion

        #region Events
        public Action<Rect> OnMoving;
        #endregion

        #region Grammar State
        public virtual void DisplayGrammarState(GraphNode node)
        {
            InvalidConnectionIcon.style.display = node.ValidConnections ? DisplayStyle.None : DisplayStyle.Flex;
        }
        #endregion

        #region Mouse Events
        protected virtual void OnMouseDown(MouseDownEvent evt)
        {
            if (Node == null) return;
            if (!Equals(LBSMainWindow.Instance._selectedLayer, Node.Graph.OwnerLayer)) return;
            
            if (evt.button == 0 && ToolKit.Instance.GetActiveManipulatorInstance() is SelectManipulator)
            {
                LBSInspectorPanel.ActivateBehaviourTab();
                if (Node.Graph.GraphNodes.Contains(Node))
                    Node.Graph.SelectedQuestNode = Node;
            }
            
            DrawManager.Instance.RedrawLayer(Node.Graph.OwnerLayer);
        }

        protected void OnMouseMove(MouseMoveEvent e)
        {
            if(this != _selectedGraph) return;
            
            // only move the selected node
            if (Node == null) return;
            if (!Equals(LBSMainWindow.Instance._selectedLayer, Node.Graph.OwnerLayer)) return;
            if (e.pressedButtons != 1) return; // only while dragging
            if (!MainView.Instance.HasManipulator<SelectManipulator>()) return;

            var grabPosition = GetPosition().position + e.mouseDelta / MainView.Instance.viewTransform.scale;
            grabPosition *= MainView.Instance.viewport.transform.scale;

            var newPos = new Rect(grabPosition.x, grabPosition.y, resolvedStyle.width, resolvedStyle.height);
            SetPosition(newPos);
        }

        protected void OnMouseLeave(MouseLeaveEvent e)
        {
            if (Node == null) return;
            OnMouseMove(MouseMoveEvent.GetPooled(e.mousePosition, e.button, e.clickCount, e.mouseDelta));
        }
        #endregion
        
        #region Selection
        public void IsSelected(bool isSelected)
        {
            var color = DefaultBackgroundColor;
            if (isSelected)
            {
                color = Node.isValid() ? ValidGrammarColor : InvalidGrammarColor;
        
                // Blend color to simulate the alpha effect
                float r = color.r * Alpha; 
                float g = color.g * Alpha;
                float b = color.b * Alpha;
                color = new Color(r, g, b, 1f); 

                _selectedGraph = this;
            }

            VisualElement coloredVe = this.Q<VisualElement>("Root");
            coloredVe.style.backgroundColor = new StyleColor(color);
        }

        public static void Deselect()
        {
            _selectedGraph?.IsSelected(false);
            _selectedGraph = null;
        }

        public bool IsSelectedView()
        {
            return _selectedGraph == this;
        } 
        
        #endregion
    }
}
