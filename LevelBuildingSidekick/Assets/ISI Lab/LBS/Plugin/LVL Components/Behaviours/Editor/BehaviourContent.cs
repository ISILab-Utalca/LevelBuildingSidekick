using ISILab.Commons.Utility.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Editor;

namespace ISILab.LBS.VisualElements
{
    public class BehaviourContent : VisualElement
    {
        // View
        private Button menu;
        private Label label;
        private VisualElement icon;
        private Foldout foldout;
        private VisualElement content;

        public BehaviourContent(LBSCustomEditor content, string name, Texture2D icon, Color color)
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("BehaviourContent");
            visualTree.CloneTree(this);

            // Foldout
            foldout = this.Q<Foldout>();
            foldout.RegisterCallback<ChangeEvent<bool>>(FoldoutPressed);

            // Icon
            this.icon = this.Q<VisualElement>("Icon");
            this.icon.style.backgroundImage = icon;
            this.icon.style.color = color;

            // Label
            label = this.Q<Label>();
            label.text = name;

            // Menu
            menu = this.Q<Button>();
            var cmm = new ContextualMenuManipulator(content.ContextMenu);
            cmm.target = menu;

            // Content
            this.content = this.Q<VisualElement>("Content");
            this.content.Add(content);
        }

        private void FoldoutPressed(ChangeEvent<bool> evt)
        {
            content.SetDisplay(evt.newValue);
        }
    }
}