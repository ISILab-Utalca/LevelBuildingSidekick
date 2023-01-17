using LBS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
public class ModeSelector : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ModeSelector, VisualElement.UxmlTraits> { }

    private Button button;
    private DropdownField dropdown;

    public ModeSelector()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ModeSelector"); // Editor
        visualTree.CloneTree(this);

        // Button
        button = this.Q<Button>();

        // Dropdown
        dropdown = this.Q<DropdownField>();
    }


}
