using ISILab.Commons.Utility.Editor;
using System;

using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Label = UnityEngine.UIElements.Label;
using ISILab.Extensions;
using ISILab.LBS.Components;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements.Editor;
using LBS.Settings;
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
        private VisualElement _startIcon;
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
            _startIcon = this.Q<VisualElement>(name: "Start");
            _toolbar = this.Q<ToolbarMenu>("ToolBar");
            _toolbar.menu.AppendAction("Set as Start Node", MakeRoot);
            _toolbar.style.display = DisplayStyle.None;
            
            SetText(node.QuestAction);
            SetBorder(node);
            
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
            this._node = node;
            _startIcon.style.display = DisplayStyle.None;
        }

        private void OnMouseLeave(MouseLeaveEvent e)
        {
            OnMouseMove(MouseMoveEvent.GetPooled(e.mousePosition, e.button, e.clickCount, e.mouseDelta, EventModifiers.None ));
        }

        private void SetBorder(QuestNode node)
        {
            _root.SetBorder(Unchecked, 1f);

            if (!node.GrammarCheck)
            {
                _root.SetBorder(GrammarWrong, 1f);
                return;
            }

            _root.SetBorder(Correct, 1f);
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

        private void OnMouseMove(MouseMoveEvent e)
        {
            // left button pressed
            if (e.pressedButtons != 1) return;
            if (!MainView.Instance.HasManipulator<Select>() ) return;
            
            var grabPosition =  GetPosition().position + e.mouseDelta / MainView.Instance.viewTransform.scale;
            grabPosition *= MainView.Instance.viewport.transform.scale;
            Rect newPos = new Rect(grabPosition.x, grabPosition.y, resolvedStyle.width, resolvedStyle.height);
            SetPosition(newPos);
            _node.Position = grabPosition.ToInt();
            /*

            var mousPos = MainView.Instance.FixPos(e.mouseDelta);
            var vect = mousPos + GetPosition().position;
            Rect newPos = new Rect(vect.x, vect.y, resolvedStyle.width, resolvedStyle.height);
            SetPosition(newPos);


            */
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

        public void MakeRoot(DropdownMenuAction obj = null)
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
    }
}