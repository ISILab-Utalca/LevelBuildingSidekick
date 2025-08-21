using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.LBS.Components;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Settings;
using ISILab.LBS.VisualElements.Editor;

namespace ISILab.LBS.VisualElements
{
    public abstract class QuestGraphNodeView : GraphElement
    {
        #region Static Colors
        protected static readonly Color GrammarWrong     = LBSSettings.Instance.view.warningColor;
        protected static readonly Color UncheckedGrammar = LBSSettings.Instance.view.okColor;
        protected static readonly Color CorrectGrammar   = LBSSettings.Instance.view.successColor;
        #endregion

        #region Fields
        protected GraphNode Node;
        protected Color DefaultBackgroundColor;
        protected VisualElement InvalidConnectionIcon;
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

            DrawManager.Instance.RedrawLayer(Node.Graph.OwnerLayer);
        }

        protected void OnMouseMove(MouseMoveEvent e)
        {
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
    }
}
