using ISILab.Commons.Utility.Editor;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Components;
using ISILab.Macros;
using UnityEditor.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class QuestActionView : QuestGraphNodeView
    {
        #region FIELDS
        private const string StartIconGuid = "4bb3ddd9a5b4b7746b055de57781a9e7";
        private const string GoalIconGuid  = "e219993bb5fe0f246b3797a8c2f3b126";
        
        private static VisualTreeAsset _asset;

        #region VIEWS
        private readonly VisualElement _root;
        
        private readonly VisualElement _start;
        private readonly VisualElement _goal;
        private readonly VisualElement _scrollIcon;
        
        private readonly VisualElement _iconGrammarInvalid;
        private readonly VisualElement _iconNodeDataInvalid;
        
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
            _start          = this.Q<VisualElement>("StartVe");
            _goal          = this.Q<VisualElement>("GoalVe");
            _scrollIcon = this.Q<VisualElement>("ScrollIcon");
            InvalidConnectionIcon = this.Q<VisualElement>("InvalidConnectionIcon");
            _iconNodeDataInvalid = this.Q<VisualElement>("InvalidDataIcon");
            _iconGrammarInvalid = this.Q<VisualElement>("InvalidGrammarIcon");
            _toolbar           = this.Q<ToolbarMenu>("ToolBar");

            VisualElement coloredVe = this.Q<VisualElement>("Root");
            coloredVe.style.backgroundColor = DefaultBackgroundColor;
            
            SetupToolbar();
            SetupNode(graphNode);
            SetupCallbacks();

            style.marginBottom = style.marginLeft = style.marginRight = style.marginTop = 0;

            InvalidConnectionIcon.style.unityBackgroundImageTintColor = InvalidGrammarColor;
            _iconNodeDataInvalid.style.unityBackgroundImageTintColor = InvalidGrammarColor;
            _iconGrammarInvalid.style.unityBackgroundImageTintColor = InvalidGrammarColor;
            
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

            _start.style.display = DisplayStyle.None;
            _goal.style.display = DisplayStyle.None;
            
            switch (graphNode.NodeType)
            {
                case QuestNode.ENodeType.Start:
                   _start.style.display = DisplayStyle.Flex; 
                    break;
                case QuestNode.ENodeType.Goal:
                    _goal.style.display = DisplayStyle.Flex;
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
                schedule.Execute(UpdateWidth).ExecuteLater(1);
                return;
            }

            float padding = 72f;
            float minWidth = 100f;
            float typeIconWidth = GetElementWidthIfVisible(_scrollIcon, 24f);
            float grammarIconWidth = GetElementWidthIfVisible(_iconGrammarInvalid, 24f);
            float dataIconWidth = GetElementWidthIfVisible(_iconNodeDataInvalid, 24f);
            float connectionIconWidth = GetElementWidthIfVisible(InvalidConnectionIcon, 24f);
            
            var textSize = _label.MeasureTextSize(_label.text,0, VisualElement.MeasureMode.Undefined, 0, VisualElement.MeasureMode.Undefined);
            float newWidth = Mathf.Max(minWidth, textSize.x + padding + grammarIconWidth + connectionIconWidth + typeIconWidth + dataIconWidth);

            _root.style.width = new StyleLength(newWidth);
            _label.style.width = new StyleLength(StyleKeyword.Auto);
            _label.style.whiteSpace = WhiteSpace.NoWrap;

            MarkDirtyRepaint();
        }
        #endregion

        #region Grammar State
        public sealed override void DisplayGrammarState(GraphNode node)
        {
            if(node is not QuestNode qn) return;
         
            base.DisplayGrammarState(node);
            
            _iconNodeDataInvalid.style.display = qn.NodeData.IsValid() ? DisplayStyle.None : DisplayStyle.Flex;
            _iconGrammarInvalid.style.display = node.ValidGrammar ? DisplayStyle.None : DisplayStyle.Flex;
            _root.SetBorder(node.isValid() ? ValidGrammarColor : InvalidGrammarColor, 1f);
        }
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
        }
        #endregion

        #region Helpers

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
