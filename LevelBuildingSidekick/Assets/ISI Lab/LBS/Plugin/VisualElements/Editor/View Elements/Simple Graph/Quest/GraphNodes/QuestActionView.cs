using ISILab.Commons.Utility.Editor;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Components;
using ISILab.LBS.Manipulators;
using ISILab.Macros;
using LBS.VisualElements;
using UnityEditor.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class QuestActionView : QuestGraphNodeView
    {
        #region FIELDS
        private const string StartIconGuid = "4bb3ddd9a5b4b7746b055de57781a9e7";
        private const string GoalIconGuid  = "e219993bb5fe0f246b3797a8c2f3b126";
        
        private static VisualTreeAsset _asset;
        private static QuestActionView _highlighted;

        #region VIEWS
        private readonly VisualElement _root;
        private readonly VisualElement _iconType;
        private readonly VisualElement _iconGrammarInvalid;
        private readonly ToolbarMenu _toolbar;
        private readonly Label _label;
        #endregion
        
        #endregion

        public QuestActionView(QuestNode graphNode)
        {
            if (_asset == null)
                _asset = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestActionView");

            _asset.CloneTree(this);

            _label             = this.Q<Label>("Title");
            _root              = this.Q<VisualElement>("Root");
            _iconType          = this.Q<VisualElement>("TypeIcon");
            InvalidConnectionIcon = this.Q<VisualElement>("InvalidConnectionIcon");
            _iconGrammarInvalid = this.Q<VisualElement>("InvalidGrammarIcon");
            _toolbar           = this.Q<ToolbarMenu>("ToolBar");

            SetupToolbar();
            SetupNode(graphNode);
            SetupCallbacks();

            style.marginBottom = style.marginLeft = style.marginRight = style.marginTop = 0;
            DefaultBackgroundColor = new Color(0.19f, 0.19f, 0.19f);

            Update();
        }

        #region Setup
        private void SetupToolbar()
        {
            _toolbar.style.display = DisplayStyle.None;
            _toolbar.menu.AppendAction("Set as Start Node", MakeRoot);
        }

        private void SetupNode(QuestNode graphNode)
        {
            Node = graphNode ?? throw new ArgumentNullException(nameof(graphNode));
            SetText(graphNode.ID);
            DisplayGrammarState(graphNode);
            SetPosition(new Rect(Node.NodeViewPosition.position, Vector2.one));

            switch (graphNode.NodeType)
            {
                case QuestNode.ENodeType.Start:
                    ShowIcon(_iconType, StartIconGuid);
                    break;
                case QuestNode.ENodeType.Goal:
                    ShowIcon(_iconType, GoalIconGuid);
                    break;
                case QuestNode.ENodeType.Middle:
                    _iconType.style.display = DisplayStyle.None;
                    break;
            }
        }

        private void SetupCallbacks()
        {
            OnMoving += rect => Node.NodeViewPosition = rect;

            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
            RegisterCallback<GeometryChangedEvent>(_ => Update());
        }
        #endregion

        #region Updates
        private void Update()
        {
            UpdateWidth();
            SetPosition(new Rect(GetPosition().position, new Vector2(_root.resolvedStyle.width, _root.resolvedStyle.height)));
            OnMoving?.Invoke(GetPosition());
        }

        private void UpdateWidth()
        {
            if (_label.resolvedStyle.width == 0 || float.IsNaN(_label.resolvedStyle.width))
            {
                this.schedule.Execute(UpdateWidth).ExecuteLater(1);
                return;
            }

            float padding = 20f;
            float minWidth = 100f;
            float typeIconWidth = GetElementWidthIfVisible(_iconType, 24f);
            float grammarIconWidth = GetElementWidthIfVisible(_iconGrammarInvalid, 24f);
            float connectionIconWidth = GetElementWidthIfVisible(InvalidConnectionIcon, 24f);
            
            var textSize = _label.MeasureTextSize(_label.text, 0, VisualElement.MeasureMode.Undefined, 0, VisualElement.MeasureMode.Undefined);
            float newWidth = Mathf.Max(minWidth, textSize.x + padding + grammarIconWidth + connectionIconWidth + typeIconWidth);

            _root.style.width = new StyleLength(newWidth);
            _label.style.width = new StyleLength(StyleKeyword.Auto);
            _label.style.whiteSpace = WhiteSpace.NoWrap;

            MarkDirtyRepaint();
        }
        #endregion

        #region Grammar State
        public sealed override void DisplayGrammarState(GraphNode node)
        {
            base.DisplayGrammarState(node);
            _iconGrammarInvalid.style.display = node.ValidGrammar ? DisplayStyle.None : DisplayStyle.Flex;
            _root.SetBorder(!node.ValidGrammar || !node.ValidConnections ? GrammarWrong : CorrectGrammar, 1f);
        }
        #endregion

        #region Selection
        public void IsSelected(bool isSelected)
        {
            var color = DefaultBackgroundColor;
            if (isSelected)
            {
                color = Node.ValidGrammar ? CorrectGrammar : GrammarWrong;
                color.a = 0.33f;
                _highlighted = this;
            }
            _root.style.backgroundColor = new StyleColor(color);
        }

        public static void Deselect() => _highlighted?.IsSelected(false);
        #endregion

        #region Toolbar Actions
        private void MakeRoot(DropdownMenuAction _)
        {
            Node.Graph.SetRoot(Node as QuestNode);
            _toolbar.menu.ClearItems();
            _toolbar.menu.AppendAction("Remove Start Node assignation", RemoveRoot);
            UpdateWidth();
        }

        private void RemoveRoot(DropdownMenuAction _)
        {
            Node.Graph.SetRoot(null);
            _toolbar.menu.ClearItems();
            _toolbar.menu.AppendAction("Set as Start Node", MakeRoot);
            UpdateWidth();
        }
        #endregion

        #region Mouse Events
        protected override void OnMouseDown(MouseDownEvent evt)
        {
            base.OnMouseDown(evt);

            if (evt.button == 1)
            {
                _toolbar.style.display = DisplayStyle.Flex;
                _toolbar.ShowMenu();
            }
            else if (evt.button == 0 && ToolKit.Instance.GetActiveManipulatorInstance() is SelectManipulator)
            {
                LBSInspectorPanel.ActivateBehaviourTab();
                if (Node.Graph.GraphNodes.Contains(Node))
                    Node.Graph.SelectedQuestNode = Node as QuestNode;
            }
        }
        #endregion

        #region Helpers
        private void ShowIcon(VisualElement iconElement, string guid)
        {
            iconElement.style.display = DisplayStyle.Flex;
            iconElement.style.backgroundImage = new StyleBackground(LBSAssetMacro.LoadAssetByGuid<VectorImage>(guid));
        }

        private float GetElementWidthIfVisible(VisualElement element, float fallback)
        {
            if (element.style.display != DisplayStyle.Flex) return 0f;
            var width = element.resolvedStyle.width;
            return (float.IsNaN(width) || width == 0) ? fallback : width;
        }

        private void SetText(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
                text = char.ToUpper(text.TrimStart()[0]) + text.TrimStart().Substring(1);

            _label.text = text;
            UpdateWidth();
        }
        #endregion
    }
}
