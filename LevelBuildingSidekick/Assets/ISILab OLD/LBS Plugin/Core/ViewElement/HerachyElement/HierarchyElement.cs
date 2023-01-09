using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{
    public class HierarchyElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<HierarchyElement, VisualElement.UxmlTraits> { }

        private Label nameLabel;
        private Button visibleButton;
        private Button lockButton;

        public HierarchyElement()
        {
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("HierarchyElement");
            visualTree.CloneTree(this);

            nameLabel = this.Q<Label>("NameLabel");
            visibleButton = this.Q<Button>("VisibleButton");
            lockButton = this.Q<Button>("LockButton");
        }
    }
}