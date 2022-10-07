using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{
    public class DropdownWVariable : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<DropdownWVariable, UxmlTraits> { }

        public DropdownField dropdown;
        public VisualElement varPanel;

        public DropdownWVariable()
        {
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("DropdownWVariableUXML");
            visualTree.CloneTree(this);

            dropdown = this.Q<DropdownField>();
            varPanel = this.Q<VisualElement>("varPanel");
        }

    }
}