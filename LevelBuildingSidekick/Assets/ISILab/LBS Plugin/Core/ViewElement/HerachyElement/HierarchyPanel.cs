using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{
    public class HierarchyPanel : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<HierarchyPanel, VisualElement.UxmlTraits> { }

        public VisualElement content;

        public HierarchyPanel()
        {
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("HierarchyPanel");
            visualTree.CloneTree(this);

            content = this.Q<VisualElement>("Content");
        }
    }
}