using ISILab.Commons.Utility.Editor;
using System;

using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Label = UnityEngine.UIElements.Label;
using ISILab.Extensions;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Manipulators;
using ISILab.LBS.VisualElements.Editor;
using ISILab.LBS.Settings;
using ISILab.Macros;
using LBS.VisualElements;
using UnityEditor.UIElements;

namespace ISILab.LBS.VisualElements
{
    /// <summary>
    /// Represents the visual node element for a <see cref="QuestNode"/> inside the quest graph editor.
    /// 
    /// This class is a UI container (`GraphElement`) that displays node-related information such as:
    /// - Its ID as a label
    /// - Start node icon
    /// - Toolbar menu for node actions (e.g., Set as Start Node)
    /// - Grammar status via colored borders
    /// 
    /// It also supports mouse interaction (dragging, right-click for menu) and syncs its UI position with the logical node data.
    /// </summary>
    public class QuestNodeView : GraphElement
    {
        private readonly QuestNode _node;
        private readonly VisualElement _root;
        private readonly VisualElement _startIcon;
        private static VisualTreeAsset _view;

        private readonly ToolbarMenu _toolbar;
        private readonly Label _label;

        public Action<Rect> OnMoving;

        private readonly Color _defaultBackgroundColor;

        private static readonly Color GrammarWrong = LBSSettings.Instance.view.warningColor;
       // private static Color _mapWrong = LBSSettings.Instance.view.errorColor;
        private static readonly Color UncheckedGrammar = LBSSettings.Instance.view.okColor ;
        private static readonly Color CorrectGrammar = LBSSettings.Instance.view.successColor;

        // Only one instance can be highlighted
        private static QuestNodeView _highligheted;
        
        protected QuestNodeView() { }

        public QuestNodeView(QuestNode node)
        {
            if (_view == null)
            {
                _view = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNodeView");
            }
            _view.CloneTree(this);

            // Label
            _label = this.Q<Label>("Title");
            _root = this.Q<VisualElement>(name: "Root");
            _startIcon = this.Q<VisualElement>(name: "Start");
            _toolbar = this.Q<ToolbarMenu>("ToolBar");
            _toolbar.menu.AppendAction("Set as Start Node", MakeRoot);
            _toolbar.style.display = DisplayStyle.None;
            
            SetText(node.ID);
            SetBorder(node);
            
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
            
            _node = node;
            _startIcon.style.display = DisplayStyle.None;
            _defaultBackgroundColor = new Color(0.19f, 0.19f, 0.19f);
        }

        private void OnMouseLeave(MouseLeaveEvent e)
        {
            OnMouseMove(MouseMoveEvent.GetPooled(e.mousePosition, e.button, e.clickCount, e.mouseDelta ));
        }
        
        public void SetBorder(QuestNode node)
        {
            _root.SetBorder(UncheckedGrammar, 1f);
            if (!node.ValidGrammar)
            {
                _root.SetBorder(GrammarWrong, 1f);
                return;
            }

            _root.SetBorder(CorrectGrammar, 1f);
        }

        public override void SetPosition(Rect newPos)
        {
            // Set new Rect position
            base.SetPosition(newPos);

            // call movement event
            OnMoving?.Invoke(newPos);
            MarkDirtyRepaint();
        }

        private void SetText(string text)
        {
            if (text.Length > 20)
            {
                text = text.Substring(0, 8) + "...";
            }

            // Remove leading spaces and capitalize the first letter
            _label.text = string.IsNullOrWhiteSpace(text) ? text : char.ToUpper(text.TrimStart()[0]) + text.TrimStart().Substring(1);

        }

        private void OnMouseMove(MouseMoveEvent e)
        {
            if (!Equals(LBSMainWindow.Instance._selectedLayer, _node.Graph.OwnerLayer)) return;
            
            // left button pressed
            if (e.pressedButtons != 1) return;
            if (!MainView.Instance.HasManipulator<SelectManipulator>() ) return;
            
            var grabPosition =  GetPosition().position + e.mouseDelta / MainView.Instance.viewTransform.scale;
            grabPosition *= MainView.Instance.viewport.transform.scale;
            Rect newPos = new Rect(grabPosition.x, grabPosition.y, resolvedStyle.width, resolvedStyle.height);
            SetPosition(newPos);
            _node.Position = grabPosition.ToInt();
            _node.NodeViewPosition = newPos;
        }
        
        private void OnMouseDown(MouseDownEvent evt)
        {
            if (!Equals(LBSMainWindow.Instance._selectedLayer, _node.Graph.OwnerLayer)) return;
            
            DrawManager.Instance.RedrawLayer(_node.Graph.OwnerLayer);
            
            // RightClick
            if (evt.button == 1)
            {
                _toolbar.style.display = DisplayStyle.Flex;
                _toolbar.ShowMenu();
            }
            // Assign selected quest node behavior only if the node belongs to the active layer
            else if (evt.button == 0)
            {
                if (ToolKit.Instance.GetActiveManipulatorInstance().GetType() != typeof(SelectManipulator)) return;
                
                QuestNodeBehaviour qnb = LBSLayerHelper.GetObjectFromLayer<QuestNodeBehaviour>(_node.Graph.OwnerLayer);
                if(qnb is null) return;
                LBSInspectorPanel.ActivateBehaviourTab();
                
                if (!qnb.Graph.QuestNodes.Contains(_node)) return;
                qnb.SelectedQuestNode = _node;

            }
        }

        private void MakeRoot(DropdownMenuAction obj = null)
        {
            _startIcon.style.display = DisplayStyle.Flex;
            _node.Graph.SetRoot(_node);
            _toolbar.menu.ClearItems();
            _toolbar.menu.AppendAction("Remove Start Node assignation", RemoveRoot);
        }

        private void RemoveRoot(DropdownMenuAction obj)
        {
            _startIcon.style.display = DisplayStyle.None;
            _node.Graph.SetRoot(null);
            _toolbar.menu.ClearItems();
            _toolbar.menu.AppendAction("Set as Start Node", MakeRoot);
        }

        public void IsSelected(bool isSelected)
        {
            Color color = _defaultBackgroundColor;
            color.a = 1f;
            if (isSelected)
            {
                color = _node.ValidGrammar ? CorrectGrammar : GrammarWrong;
                color.a = 0.33f;
                _highligheted = this;
            }
            
            _root.style.backgroundColor = new StyleColor(color);
        }


        public static void Deselect()
        {
            _highligheted?.IsSelected(false);
        }
    }
}