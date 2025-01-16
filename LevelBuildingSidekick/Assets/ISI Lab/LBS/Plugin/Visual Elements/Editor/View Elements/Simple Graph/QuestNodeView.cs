using ISILab.Commons.Utility.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Label = UnityEngine.UIElements.Label;
using ISILab.Extensions;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Settings;
using UnityEditor;
using UnityEditor.UIElements;

namespace ISILab.LBS.VisualElements
{
    /**
     * Node that is displayed in the quest graph
     */
    public class QuestNodeView : GraphElement
    {
        private QuestNode _node;
        private VisualElement _root;
        private static VisualTreeAsset _view;

        private ToolbarMenu _toolbar;
        private Label _label;

        public Action<Rect> OnMoving;

        public static Color GrammarWrong = LBSSettings.Instance.view.warningColor;
        public static Color MapWrong = LBSSettings.Instance.view.errorColor;
        public static Color Unchecked = LBSSettings.Instance.view.okColor ;
        public static Color Correct = LBSSettings.Instance.view.successColor;

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
            _toolbar = this.Q<ToolbarMenu>("ToolBar");
            _toolbar.menu.AppendAction("Make Graph Root", MakeRoot);
            _toolbar.style.display = DisplayStyle.None;
            SetText(node.QuestAction);
            SetBorder(node);
            
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            
            this._node = node;
        }

        private void SetBorder(QuestNode node)
        {
            _root.SetBorder(Unchecked);

            if (!node.GrammarCheck)
            {
                _root.SetBorder(GrammarWrong);
                return;
            }

            _root.SetBorder(Correct);
        }

        public override void SetPosition(Rect newPos)
        {
            // Set new Rect position
            base.SetPosition(newPos);

            // call movement event
            OnMoving?.Invoke(newPos);

            MarkDirtyRepaint();
        }

        public void SetText(string text)
        {
            if (text.Length > 11)
            {
                text = text.Substring(0, 8) + "...";
            }

            _label.text = text;
        }
        
        private void OnMouseDown(MouseDownEvent evt)
        {
            // RightClick
            if (evt.button == 1)
            {
                _toolbar.style.display = DisplayStyle.Flex;
                _toolbar.ShowMenu();
            }
        }

        private void MakeRoot(DropdownMenuAction obj)
        {
            _node.Graph.SetRoot(_node);
        }
        
    }
}