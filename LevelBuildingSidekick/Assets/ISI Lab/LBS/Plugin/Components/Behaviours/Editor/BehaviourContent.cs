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
    /// <summary>
    /// Visual Element Class that displays the different behaviors within the behavior's panel
    /// </summary>
    public class BehaviourContent : VisualElement
    {
        // View
        private Button menu;
        private Label label;
        private VisualElement icon;
        private Foldout foldout;
        private VisualElement content;

        public BehaviourContent(LBSCustomEditor content, string name, VectorImage icon, Color color)
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("BehaviourContent");
            visualTree.CloneTree(this);
            
            foldout = this.Q<Foldout>();
            foldout.RegisterCallback<ChangeEvent<bool>>(FoldoutPressed);
            
            this.icon = this.Q<VisualElement>("Icon");
            this.icon.style.backgroundImage = new StyleBackground(icon);
            this.icon.style.unityBackgroundImageTintColor = new StyleColor(color);
            
            label = this.Q<Label>();
            label.text = name;
            
            menu = this.Q<Button>();
            var cmm = new ContextualMenuManipulator(content.ContextMenu);
            cmm.target = menu;
            
            this.content = this.Q<VisualElement>("Content");
            this.content.Add(content);
        }

        private void FoldoutPressed(ChangeEvent<bool> evt)
        {
            content.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}