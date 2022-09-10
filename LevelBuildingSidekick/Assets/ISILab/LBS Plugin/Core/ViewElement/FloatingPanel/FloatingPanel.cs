using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{
    public class FloatingPanel : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<FloatingPanel, VisualElement.UxmlTraits> { }

        private List<Toggle> toggles = new List<Toggle>();
        private List<Button> actions = new List<Button>();

        private Label title;
        private VisualElement toggleContent;
        private VisualElement buttonContent;
        private Button prev, next;

        public FloatingPanel() { }

        public FloatingPanel(string title, List<Tuple <string,Action>> actions, List<IRepController> controllers, Type prev = null, Type next = null)
        {
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("FloatingPanel");
            visualTree.CloneTree(this);

            this.title = this.Q<Label>("Title");
            this.toggleContent = this.Q<VisualElement>("ToggleContent");
            this.buttonContent = this.Q<VisualElement>("ButtonContent");
            this.prev = this.Q<Button>("Prev");
            this.next = this.Q<Button>("Next");

            this.title.text = title;

            foreach (var c in controllers)
            {
                var toggle = new Toggle(c.GetName()); // cambiar a un nombre decvente  en vez de usar toString() (!)
                toggle.RegisterValueChangedCallback((v) => { c.ShowView(!v.newValue);});
                toggleContent.Add(toggle);
            }

            foreach (var action in actions)
            {
                var button = new Button();
                button.text = action.Item1;
                button.clicked += action.Item2;
                buttonContent.Add(button);
            }

            if (prev != null)
            {
                this.prev.style.display = DisplayStyle.Flex;
                this.prev.clicked += () => EditorWindow.GetWindow(prev);
            }
            else
            {
                this.prev.style.display = DisplayStyle.None;
            }

            if (next != null)
            {
                this.next.style.display = DisplayStyle.Flex;
                this.next.clicked += () => EditorWindow.GetWindow(next);
            }
            else
            {
                this.next.style.display = DisplayStyle.None;
            }

        }

    }

}