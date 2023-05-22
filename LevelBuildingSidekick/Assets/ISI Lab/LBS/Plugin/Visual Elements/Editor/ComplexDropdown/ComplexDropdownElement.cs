using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ComplexDropdownElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ComplexDropdownElement, VisualElement.UxmlTraits> { }

    public readonly Texture2D defaultIcon = new Texture2D(16,16); // poner una textura default (!!)

    private VisualElement content;
    private VisualElement icon;
    private Label nameLabel;
    private VisualElement arrow;

    public ComplexDropdownElement()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ComplexDropdownElement"); // Editor
        visualTree.CloneTree(this);

        content = this.Q<VisualElement>("Content");
        icon = this.Q<VisualElement>("Icon");
        nameLabel = this.Q<Label>("Name");
        arrow = this.Q<VisualElement>("Arrow");
    }

    public void SetInfo(string name,Texture2D icon = null, bool needArrow = false)
    {
        this.nameLabel.text = name;
        this.icon.style.backgroundImage = (icon == null) ? defaultIcon : icon;
        this.arrow.SetDisplay(needArrow);
    }
}