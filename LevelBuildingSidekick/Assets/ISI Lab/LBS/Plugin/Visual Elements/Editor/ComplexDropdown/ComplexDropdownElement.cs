using ISILab.Commons.Utility.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;

namespace ISILab.LBS.VisualElements
{
    public class ComplexDropdownElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<ComplexDropdownElement, UxmlTraits> { }

        public readonly Texture2D defaultIcon = new Texture2D(16, 16);

        private VisualElement content;
        private VisualElement icon;
        private Label nameLabel;
        private VisualElement arrow;

        public ComplexDropdownElement()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("ComplexDropdownElement");
            visualTree.CloneTree(this);

            content = this.Q<VisualElement>("Content");
            icon = this.Q<VisualElement>("Icon");
            nameLabel = this.Q<Label>("Name");
            arrow = this.Q<VisualElement>("Arrow");
        }

        public void SetInfo(string name, Texture2D icon = null, bool needArrow = false)
        {
            nameLabel.text = name;
            this.icon.style.backgroundImage = icon == null ? defaultIcon : icon;
            arrow.SetDisplay(needArrow);
        }
    }
}