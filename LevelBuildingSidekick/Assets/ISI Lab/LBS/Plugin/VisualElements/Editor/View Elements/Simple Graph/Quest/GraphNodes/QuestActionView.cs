using ISILab.Commons.Utility.Editor;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
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
    public class QuestActionView : QuestGraphNodeView
    {
        private readonly VisualElement _root;
        
        private readonly VisualElement _typeIcon;
        private readonly VisualElement _statusIcon;
        
        private static VisualTreeAsset _view;
        private readonly ToolbarMenu _toolbar;
        private readonly Label _label;
        
        private static readonly Color GrammarWrong = LBSSettings.Instance.view.warningColor;
        private static readonly Color UncheckedGrammar = LBSSettings.Instance.view.okColor;
        private static readonly Color CorrectGrammar = LBSSettings.Instance.view.successColor;

        private const string StartIcon = "4bb3ddd9a5b4b7746b055de57781a9e7";
        private const string GoalIcon = "e219993bb5fe0f246b3797a8c2f3b126";

        // Only one instance can be highlighted
        private static QuestActionView _highligheted;
        
        protected QuestActionView() { }

        public QuestActionView(QuestNode node, Vector2 clickPosition = default)
        {
            if (_view == null)
            {
                _view = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestActionView");
            }
            _view.CloneTree(this);

            // Initialize UI elements
            _label = this.Q<Label>("Title");
            _root = this.Q<VisualElement>("Root");
            _typeIcon = this.Q<VisualElement>("TypeIcon");
            _statusIcon = this.Q<VisualElement>("StatusIcon");
            _toolbar = this.Q<ToolbarMenu>("ToolBar");
            _toolbar.menu.AppendAction("Set as Start Node", MakeRoot);
            _toolbar.style.display = DisplayStyle.None;
            
            // Set node data
            Node = node ?? throw new ArgumentNullException(nameof(node));
            SetText(node.ID);
            DisplayGrammarState(node);

            switch (node.NodeType)
            {
                case QuestNode.ENodeType.Start: 
                    _typeIcon.style.display = DisplayStyle.Flex;
                    _typeIcon.style.backgroundImage = new StyleBackground(LBSAssetMacro.LoadAssetByGuid<VectorImage>(StartIcon));
                    break;
                case QuestNode.ENodeType.Middle:
                    _typeIcon.style.display = DisplayStyle.None;
                    break;
                case QuestNode.ENodeType.Goal:
                    _typeIcon.style.display = DisplayStyle.Flex;
                    _typeIcon.style.backgroundImage = new StyleBackground(LBSAssetMacro.LoadAssetByGuid<VectorImage>(GoalIcon));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            // Register callbacks
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
            RegisterCallback<GeometryChangedEvent>(evt => OnGeometryChanged(evt, clickPosition));
            
             // only display the error icon if grammar is not correct
            _statusIcon.style.display = node.ValidGrammar ? DisplayStyle.None : DisplayStyle.Flex;
            
            DefaultBackgroundColor = new Color(0.19f, 0.19f, 0.19f);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt, Vector2 clickPosition)
        {
            // Center the node at the click position
            CenterElement(clickPosition);
            // Update width based on text and icon
            UpdateWidth();
            // Notify movement for edge updates
            OnMoving?.Invoke(GetPosition());
        }

        private void UpdateWidth()
        {
            // Ensure the label has resolved its style
            if (_label.resolvedStyle.width == 0 || float.IsNaN(_label.resolvedStyle.width))
            {
                // Schedule a retry if style isn’t resolved yet
                this.schedule.Execute(() => UpdateWidth()).ExecuteLater(1);
                return;
            }

            // Measure the text size
            var textSize = _label.MeasureTextSize(
                _label.text,
                0, // Auto-width
                VisualElement.MeasureMode.Undefined,
                0, // Auto-height
                VisualElement.MeasureMode.Undefined
            );

            // Add padding for visual appeal
            float padding = 20f; // Matches typical USS padding
            float minWidth = 100f; // Minimum width to prevent overly narrow nodes

            // Add space for _startIcon if visible
            float typeIconWidth = 0f;
            if (_typeIcon.style.display == DisplayStyle.Flex)
            {
                typeIconWidth = _statusIcon.resolvedStyle.width;
                if (float.IsNaN(typeIconWidth) || typeIconWidth == 0)
                {
                    typeIconWidth = 24f; // Fallback width (adjust based on USS or icon size)
                }
            }

            // Add space for _startIcon if visible
            float statusIconWidth = 0f;
            if (_statusIcon.style.display == DisplayStyle.Flex)
            {
                statusIconWidth = _statusIcon.resolvedStyle.width;
                if (float.IsNaN(statusIconWidth) || statusIconWidth == 0)
                {
                    statusIconWidth = 24f; // Fallback width (adjust based on USS or icon size)
                }
            }
            
            // Calculate total width: text + padding + icon
            float newWidth = Mathf.Max(minWidth, textSize.x + padding + statusIconWidth + typeIconWidth);

            // Set the width of the _root element
            _root.style.width = new StyleLength(newWidth);
            // Ensure the label fits within the root
            _label.style.width = new StyleLength(StyleKeyword.Auto);
            _label.style.whiteSpace = WhiteSpace.NoWrap;

            // Mark for repaint and notify movement for edge updates
            MarkDirtyRepaint();
        }

        public sealed override void DisplayGrammarState(GraphNode node)
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
            base.SetPosition(newPos);
            OnMoving?.Invoke(newPos);
            MarkDirtyRepaint();
        }

        private void SetText(string text)
        {
            // Capitalize first letter and trim leading spaces
            text = string.IsNullOrWhiteSpace(text) ? text : char.ToUpper(text.TrimStart()[0]) + text.TrimStart().Substring(1);
            _label.text = text;
            // Update width to fit the text and icon
            UpdateWidth();
        }

        private void MakeRoot(DropdownMenuAction obj = null)
        {
            Node.Graph.SetRoot(Node as QuestNode);
            _toolbar.menu.ClearItems();
            _toolbar.menu.AppendAction("Remove Start Node assignation", RemoveRoot);
            // Update width to account for icon
            UpdateWidth();
        }

        private void RemoveRoot(DropdownMenuAction obj)
        {
            Node.Graph.SetRoot(null);
            _toolbar.menu.ClearItems();
            _toolbar.menu.AppendAction("Set as Start Node", MakeRoot);
            // Update width to remove icon space
            UpdateWidth();
        }

        public void IsSelected(bool isSelected)
        {
            Color color = DefaultBackgroundColor;
            color.a = 1f;
            if (isSelected)
            {
                color = Node.ValidGrammar ? CorrectGrammar : GrammarWrong;
                color.a = 0.33f;
                _highligheted = this;
            }
            _root.style.backgroundColor = new StyleColor(color);
        }

        public static void Deselect()
        {
            _highligheted?.IsSelected(false);
        }

        protected override void OnMouseDown(MouseDownEvent evt)
        {
            base.OnMouseDown(evt);
            if (evt.button == 1)
            {
                _toolbar.style.display = DisplayStyle.Flex;
                _toolbar.ShowMenu();
            }
            else if (evt.button == 0)
            {
                var currentMani = ToolKit.Instance.GetActiveManipulatorInstance();
                if (currentMani is null || currentMani.GetType() != typeof(SelectManipulator)) return;
                LBSInspectorPanel.ActivateBehaviourTab();
                if (!Node.Graph.GraphNodes.Contains(Node)) return;
                Node.Graph.SelectedQuestNode = Node as QuestNode;
            }
            DrawManager.Instance.RedrawLayer(Node.Graph.OwnerLayer);
        }
    }
}