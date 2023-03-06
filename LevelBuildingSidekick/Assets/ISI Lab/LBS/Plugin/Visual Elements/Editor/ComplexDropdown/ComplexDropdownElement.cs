using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ComplexDropdownElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ComplexDropdownElement, VisualElement.UxmlTraits> { }

    private Button button;
    private VisualElement icon;
    private Label nameLabel;
    private VisualElement arrow;

    public ComplexDropdownElement()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ComplexDropdownElement"); // Editor
        visualTree.CloneTree(this);

        button = this.Q<Button>();
        icon = this.Q<VisualElement>("Icon");
        nameLabel = this.Q<Label>("Name");
        arrow = this.Q<VisualElement>("Arrow");
    }

    public void SetAction(Action action)
    {
        button.clicked += action;
    }

    public void SetInfo(string name,Texture2D icon = null, bool needArrow = false)
    {
        this.nameLabel.text = name;
        this.icon.style.backgroundImage = icon;
        this.arrow.style.display = (needArrow) ? DisplayStyle.Flex : DisplayStyle.None;
    }
}