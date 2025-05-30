using ISILab.Commons.Utility.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;

namespace ISILab.LBS.VisualElements
{
    public class DataContent : VisualElement
    {
        private Label label;
        private Foldout foldout;
        private VisualElement content;

        public DataContent(VisualElement content, string name)
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("DataContent");
            visualTree.CloneTree(this);

            // Foldout
            foldout = this.Q<Foldout>();
            foldout.RegisterCallback<ChangeEvent<bool>>(OnFoldoutPressed);

            // Label
            label = this.Q<Label>();
            label.text = name;

            // Content
            this.content = this.Q<VisualElement>("Content");
            this.content.Add(content);
        }

        private void OnFoldoutPressed(ChangeEvent<bool> evt)
        {
            content.SetDisplay(evt.newValue);
        }
    }
}